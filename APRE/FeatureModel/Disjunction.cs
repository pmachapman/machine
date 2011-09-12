﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SIL.APRE.FeatureModel
{
	public class Disjunction : IEnumerable<FeatureStruct>, ICloneable
	{
		private readonly List<FeatureStruct> _disjuncts;

		public Disjunction(IEnumerable<FeatureStruct> disjuncts)
		{
			_disjuncts = new List<FeatureStruct>(disjuncts);
			if (_disjuncts.Count < 2)
				throw new ArgumentException("At least two disjuncts must be specified.", "disjuncts");
		}

		public Disjunction(Disjunction disjunction)
		{
			_disjuncts = new List<FeatureStruct>(disjunction._disjuncts.Select(disj => (FeatureStruct) disj.Clone()));
		}

		public IEnumerator<FeatureStruct> GetEnumerator()
		{
			return _disjuncts.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Negation(out FeatureStruct output)
		{
			output = null;
			foreach (FeatureStruct disjunct in _disjuncts)
			{
				FeatureStruct negation;
				if (!disjunct.Negation(out negation))
				{
					output = null;
					return false;
				}

				if (output == null)
				{
					output = negation;
				}
				else
				{
					if (!output.Unify(negation, out output))
					{
						output = null;
						return false;
					}
				}
			}
			return true;
		}

		internal Disjunction Clone(IDictionary<FeatureValue, FeatureValue> copies)
		{
			return new Disjunction(_disjuncts.Select(disj => (FeatureStruct) disj.Clone(copies)));
		}

		public Disjunction Clone()
		{
			return new Disjunction(this);
		}

		object ICloneable.Clone()
		{
			return Clone();
		}
	}
}