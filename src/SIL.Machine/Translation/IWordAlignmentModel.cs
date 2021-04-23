﻿using System;
using System.Collections.Generic;
using SIL.Machine.Corpora;
using SIL.ObjectModel;

namespace SIL.Machine.Translation
{
	public interface IWordAlignmentModel : IWordAligner, IDisposable
	{
		IReadOnlyList<string> SourceWords { get; }
		IReadOnlyList<string> TargetWords { get; }
		IReadOnlySet<int> SpecialSymbolIndices { get; }

		ITrainer CreateTrainer(ITokenProcessor sourcePreprocessor, ITokenProcessor targetPreprocessor,
			ParallelTextCorpus corpus, int maxCorpusCount = int.MaxValue);

		IEnumerable<(string TargetWord, double Score)> GetTranslations(string sourceWord, double threshold = 0);
		IEnumerable<(int TargetWordIndex, double Score)> GetTranslations(int sourceWordIndex, double threshold = 0);

		double GetTranslationScore(string sourceWord, string targetWord);
		double GetTranslationScore(int sourceWordIndex, int targetWordIndex);

		double GetAlignmentScore(int sourceLen, int prevSourceIndex, int sourceIndex, int targetLen,
			int prevTargetIndex, int targetIndex);
	}
}
