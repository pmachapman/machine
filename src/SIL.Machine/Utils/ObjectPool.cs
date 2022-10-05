﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using SIL.Machine.Threading;
using SIL.ObjectModel;

namespace SIL.Machine.Utils
{
    public class ObjectPool<T> : DisposableBase
    {
        private readonly BufferBlock<T> _bufferBlock;
        private readonly Func<Task<T>> _factory;
        private readonly AsyncLock _lock;
        private readonly List<T> _objs;

        public ObjectPool(int maxCount, Func<T> factory) : this(maxCount, () => Task.FromResult(factory())) { }

        public ObjectPool(int maxCount, Func<Task<T>> factory)
        {
            _lock = new AsyncLock();
            MaxCount = maxCount;
            _factory = factory;
            _bufferBlock = new BufferBlock<T>();
            _objs = new List<T>();
        }

        public int MaxCount { get; }
        public int Count { get; private set; }
        public int AvailableCount => _bufferBlock.Count;

        public async Task<ObjectPoolItem<T>> GetAsync(CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (_bufferBlock.TryReceive(out T obj))
                return new ObjectPoolItem<T>(this, obj);

            if (Count < MaxCount)
            {
                using (await _lock.LockAsync(cancellationToken))
                {
                    if (Count < MaxCount)
                    {
                        Count++;
                        obj = await _factory();
                        _objs.Add(obj);
                        _bufferBlock.Post(obj);
                    }
                }
            }

            return new ObjectPoolItem<T>(this, await _bufferBlock.ReceiveAsync(cancellationToken));
        }

        public async Task ResetAsync(CancellationToken cancellationToken = default)
        {
            using (await _lock.LockAsync(cancellationToken))
            {
                Reset();
            }
        }

        internal void Put(T item)
        {
            CheckDisposed();

            _bufferBlock.Post(item);
        }

        private void Reset()
        {
            _bufferBlock.TryReceiveAll(out _);
            foreach (T obj in _objs)
            {
                if (obj is IDisposable disposable)
                    disposable.Dispose();
            }
            _objs.Clear();
            Count = 0;
        }

        protected override void DisposeManagedResources()
        {
            Reset();
            _bufferBlock.Complete();
        }
    }
}