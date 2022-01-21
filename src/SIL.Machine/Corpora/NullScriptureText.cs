﻿using System.Collections.Generic;
using SIL.Machine.Tokenization;
using SIL.Scripture;

namespace SIL.Machine.Corpora
{
	public class NullScriptureText : ScriptureText
	{
		public NullScriptureText(ITokenizer<string, int, string> wordTokenizer, string id, ScrVers versification)
			: base(wordTokenizer, id, versification)
		{
		}

		protected override IEnumerable<TextSegment> GetSegmentsInDocOrder(bool includeText = true)
		{
			yield break;
		}
	}
}