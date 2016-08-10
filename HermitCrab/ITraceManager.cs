﻿namespace SIL.HermitCrab
{
	public enum FailureReason
	{
		None,
		ObligatorySyntacticFeatures,
		AllomorphCoOccurrenceRules,
		Environments,
		MorphemeCoOccurrenceRules,
		DisjunctiveAllomorph,
		SurfaceFormMismatch,
		Pattern,
		HeadPattern,
		NonHeadPattern,
		RequiredSyntacticFeatureStruct,
		HeadRequiredSyntacticFeatureStruct,
		NonHeadRequiredSyntacticFeatureStruct,
		RequiredMprFeatures,
		ExcludedMprFeatures,
		RequiredStemName,
		ExcludedStemName,
		PartialParse,
		BoundRoot,
		NonPartialRuleProhibitedAfterFinalTemplate,
		NonPartialRuleRequiredAfterNonFinalTemplate,
		MaxApplicationCount
	}

	public interface ITraceManager
	{
		bool IsTracing { get; set; }

		void AnalyzeWord(Language lang, Word input);

		void BeginUnapplyStratum(Stratum stratum, Word input);
		void EndUnapplyStratum(Stratum stratum, Word output);

		void PhonologicalRuleUnapplied(IPhonologicalRule rule, int subruleIndex, Word input, Word output);
		void PhonologicalRuleNotUnapplied(IPhonologicalRule rule, int subruleIndex, Word input);

		void BeginUnapplyTemplate(AffixTemplate template, Word input);
		void EndUnapplyTemplate(AffixTemplate template, Word output, bool unapplied);

		void MorphologicalRuleUnapplied(IMorphologicalRule rule, int subruleIndex, Word input, Word output);
		void MorphologicalRuleNotUnapplied(IMorphologicalRule rule, int subruleIndex, Word input);

		void LexicalLookup(Stratum stratum, Word input);

		void SynthesizeWord(Language lang, Word input);

		void BeginApplyStratum(Stratum stratum, Word input);
		void NonFinalTemplateAppliedLast(Stratum stratum, Word word);
		void ApplicableTemplatesNotApplied(Stratum stratum, Word word);
		void EndApplyStratum(Stratum stratum, Word output);

		void PhonologicalRuleApplied(IPhonologicalRule rule, int subruleIndex, Word input, Word output);
		void PhonologicalRuleNotApplied(IPhonologicalRule rule, int subruleIndex, Word input, FailureReason reason, object failureObj);

		void BeginApplyTemplate(AffixTemplate template, Word input);
		void EndApplyTemplate(AffixTemplate template, Word output, bool applied);

		void MorphologicalRuleApplied(IMorphologicalRule rule, int subruleIndex, Word input, Word output);
		void MorphologicalRuleNotApplied(IMorphologicalRule rule, int subruleIndex, Word input, FailureReason reason, object failureObj);

		void ParseBlocked(IHCRule rule, Word output);

		void ParseSuccessful(Language lang, Word word);
		void ParseFailed(Language lang, Word word, FailureReason reason, Allomorph allomorph, object failureObj);
	}
}
