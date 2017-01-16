﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Options;
using SIL.Machine.Tokenization;
using SIL.Machine.Translation;
using SIL.ObjectModel;

namespace SIL.Machine.WebApi.Models
{
	public class EngineService : DisposableBase, IEngineService
	{
		private readonly EngineOptions _options;
		private readonly Dictionary<Tuple<string, string>, EngineContext> _engines;
		private readonly Timer _cleanupTimer;
		private bool _isTimerStopped;

		public EngineService(IOptions<EngineOptions> options)
		{
			_options = options.Value;
			_engines = new Dictionary<Tuple<string, string>, EngineContext>();
			foreach (string configDir in Directory.EnumerateDirectories(_options.RootDir))
			{
				string dirName = Path.GetFileName(configDir);
				string[] parts = dirName.Split('_');
				string sourceLanguageTag = parts[0];
				string targetLanguageTag = parts[1];
				Tuple<string, string> key = Tuple.Create(sourceLanguageTag, targetLanguageTag);
				_engines[key] = new EngineContext(sourceLanguageTag, targetLanguageTag);
			}
			_cleanupTimer = new Timer(CleanupUnusedEngines, null, _options.UnusedEngineCleanupFrequency, _options.UnusedEngineCleanupFrequency);
		}

		private void CleanupUnusedEngines(object state)
		{
			if (_isTimerStopped)
				return;

			foreach (EngineContext engineContext in _engines.Values)
			{
				lock (engineContext)
				{
					if (engineContext.IsLoaded && DateTime.Now - engineContext.LastUsedTime > _options.EngineTimeout)
						engineContext.Unload();
				}
			}
		}

		public IEnumerable<EngineDto> GetAll()
		{
			return _engines.Values.Select(ec => ec.CreateDto());
		}

		public bool TryGet(string sourceLanguageTag, string targetLanguageTag, out EngineDto engine)
		{
			EngineContext engineContext;
			if (!_engines.TryGetValue(Tuple.Create(sourceLanguageTag, targetLanguageTag), out engineContext))
			{
				engine = null;
				return false;
			}

			engine = engineContext.CreateDto();
			return true;
		}

		public bool TryTranslate(string sourceLanguageTag, string targetLanguageTag, string segment, out string result)
		{
			EngineContext engineContext;
			if (!_engines.TryGetValue(Tuple.Create(sourceLanguageTag, targetLanguageTag), out engineContext))
			{
				result = null;
				return false;
			}

			lock (engineContext)
			{
				if (!engineContext.IsLoaded)
					engineContext.Load(_options.RootDir);
				string[] sourceSegment = engineContext.Tokenizer.TokenizeToStrings(segment).ToArray();
				TranslationResult translationResult = engineContext.Engine.Translate(sourceSegment.Select(w => w.ToLowerInvariant()));
				result = engineContext.Detokenizer.Detokenize(Enumerable.Range(0, translationResult.TargetSegment.Count)
					.Select(j => translationResult.RecaseTargetWord(sourceSegment, j)));
				engineContext.LastUsedTime = DateTime.Now;
				return true;
			}
		}

		public bool TryInteractiveTranslate(string sourceLanguageTag, string targetLanguageTag, IReadOnlyList<string> segment, out InteractiveTranslationResultDto result)
		{
			EngineContext engineContext;
			if (!_engines.TryGetValue(Tuple.Create(sourceLanguageTag, targetLanguageTag), out engineContext))
			{
				result = null;
				return false;
			}

			lock (engineContext)
			{
				if (!engineContext.IsLoaded)
					engineContext.Load(_options.RootDir);
				string[] sourceSegment = segment.Select(s => s.ToLowerInvariant()).ToArray();

				WordGraph smtWordGraph = engineContext.Engine.SmtEngine.GetWordGraph(sourceSegment);
				TranslationResult ruleResult = engineContext.Engine.RuleEngine?.Translate(sourceSegment);

				result = new InteractiveTranslationResultDto
				{
					WordGraph = smtWordGraph.CreateDto(segment),
					RuleResult = ruleResult?.CreateDto(segment)
				};
				engineContext.LastUsedTime = DateTime.Now;
				return true;
			}
		}

		public bool TryTrainSegment(string sourceLanguageTag, string targetLanguageTag, SegmentPairDto segmentPair)
		{
			EngineContext engineContext;
			if (!_engines.TryGetValue(Tuple.Create(sourceLanguageTag, targetLanguageTag), out engineContext))
				return false;

			lock (engineContext)
			{
				if (!engineContext.IsLoaded)
					engineContext.Load(_options.RootDir);

				engineContext.Engine.TrainSegment(segmentPair.SourceSegment.Select(s => s.ToLowerInvariant()), segmentPair.TargetSegment.Select(s => s.ToLowerInvariant()));
				engineContext.LastUsedTime = DateTime.Now;
				return true;
			}
		}

		protected override void DisposeManagedResources()
		{
			_isTimerStopped = true;
			_cleanupTimer.Dispose();

			foreach (EngineContext engineContext in _engines.Values)
			{
				lock (engineContext)
				{
					if (engineContext.IsLoaded)
						engineContext.Unload();
				}
			}
		}
	}
}
