﻿namespace SIL.Machine
{
	public abstract class SpanFactory<TOffset>
	{
		private readonly bool _includeEndpoint;

		protected SpanFactory(bool includeEndpoint)
		{
			_includeEndpoint = includeEndpoint;
		}

		public abstract Span<TOffset> Empty { get; }

		public bool IncludeEndpoint
		{
			get { return _includeEndpoint; }
		}

		public abstract int Compare(TOffset x, TOffset y);

		public abstract int CalcLength(TOffset start, TOffset end);

		public bool IsValidSpan(TOffset start, TOffset end)
		{
			return IsValidSpan(start, end, Direction.LeftToRight);
		}

		public bool IsValidSpan(TOffset start, TOffset end, Direction dir)
		{
			TOffset actualStart;
			TOffset actualEnd;
			if (dir == Direction.LeftToRight)
			{
				actualStart = start;
				actualEnd = end;
			}
			else
			{
				actualStart = end;
				actualEnd = start;
			}

			return Compare(actualStart, actualEnd) <= 0;
		}

		public Span<TOffset> Create(TOffset start, TOffset end)
		{
			return Create(start, end, Direction.LeftToRight);
		}

		public Span<TOffset> Create(TOffset start, TOffset end, Direction dir)
		{
			TOffset actualStart;
			TOffset actualEnd;
			if (dir == Direction.LeftToRight)
			{
				actualStart = start;
				actualEnd = end;
			}
			else
			{
				actualStart = end;
				actualEnd = start;
			}

			return new Span<TOffset>(this, actualStart, actualEnd);
		}

		public Span<TOffset> Create(TOffset offset)
		{
			return new Span<TOffset>(this, offset, offset);
		}
	}
}