using System.Collections.Generic;
using System.Text;
using SIL.Machine.Fsa;

namespace SIL.Machine.Matching
{
	public class Alternation<TData, TOffset> : PatternNode<TData, TOffset> where TData : IData<TOffset>
	{
		public Alternation()
		{
		}

		public Alternation(IEnumerable<PatternNode<TData, TOffset>> nodes)
			: base(nodes)
		{
		}

		public Alternation(Alternation<TData, TOffset> alternation)
			: base(alternation)
		{
		}

		internal override State<TData, TOffset> GenerateNfa(FiniteStateAutomaton<TData, TOffset> fsa, State<TData, TOffset> startState)
		{
			if (this.IsLeaf())
				return startState;

			State<TData, TOffset> endState = fsa.CreateState();
			foreach (PatternNode<TData, TOffset> node in Children)
			{
				State<TData, TOffset> nodeEndState = node.GenerateNfa(fsa, startState);
				nodeEndState.AddArc(endState);
			}
			return endState;
		}

		public override PatternNode<TData, TOffset> Clone()
		{
			return new Alternation<TData, TOffset>(this);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			bool first = true;
			foreach (PatternNode<TData, TOffset> node in Children)
			{
				if (!first)
					sb.Append("|");
				sb.Append(node);
				first = false;
			}
			return sb.ToString();
		}
	}
}
