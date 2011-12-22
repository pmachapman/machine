﻿using System.Collections.Generic;

namespace SIL.Machine
{
	public class SkipList<T> : BidirList<SkipListNode<T>>, ICollection<T>
	{
		public SkipList()
			: this(Comparer<T>.Default)
		{
		}

		public SkipList(IComparer<T> comparer)
			: base(new ProjectionComparer<SkipListNode<T>, T>(node => node.Value, comparer))
		{
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (SkipListNode<T> node in (IEnumerable<SkipListNode<T>>) this)
				yield return node.Value;
		}

		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		public SkipListNode<T> Add(T item)
		{
			var node = new SkipListNode<T>(item);
			Add(node);
			return node;
		}

		public bool Find(T item, out SkipListNode<T> result)
		{
			var node = new SkipListNode<T>(item);
			return Find(node, out result);
		}

		public bool Contains(T item)
		{
			SkipListNode<T> result;
			return Find(item, out result);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			foreach (T value in this)
				array[arrayIndex++] = value;
		}

		public bool Remove(T item)
		{
			SkipListNode<T> result;
			if (Find(item, out result))
				return Remove(result);
			return false;
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return false; }
		}
	}
}