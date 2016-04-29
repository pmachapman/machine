﻿using NUnit.Framework;

namespace SIL.Machine.Translation.Tests
{
	[TestFixture]
	public class ThotSmtSessionTests
	{
		[Test]
		public void Translate_TranslationCorrect()
		{
			using (var engine = new ThotSmtEngine(TestHelpers.ToyCorpusConfigFileName))
			using (ISmtSession session = engine.StartSession())
			{
				TranslationResult result = session.Translate("voy a marcharme hoy por la tarde .".Split());
				Assert.That(result.TargetSegment, Is.EqualTo("i am leaving today in the afternoon .".Split()));
			}
		}

		[Test]
		public void TranslateInteractively_TranslationCorrect()
		{
			using (var engine = new ThotSmtEngine(TestHelpers.ToyCorpusConfigFileName))
			using (ISmtSession session = engine.StartSession())
			{
				TranslationResult result = session.TranslateInteractively("me marcho hoy por la tarde .".Split());
				Assert.That(result.TargetSegment, Is.EqualTo("i leave today in the afternoon .".Split()));
			}
		}

		[Test]
		public void AddStringToPrefix_TranslationCorrect()
		{
			using (var engine = new ThotSmtEngine(TestHelpers.ToyCorpusConfigFileName))
			using (ISmtSession session = engine.StartSession())
			{
				TranslationResult result = session.TranslateInteractively("me marcho hoy por la tarde .".Split());
				Assert.That(result.TargetSegment, Is.EqualTo("i leave today in the afternoon .".Split()));
				result = session.AddToPrefix("I am".Split(), false);
				Assert.That(result.TargetSegment, Is.EqualTo("I am leave today in the afternoon .".Split()));
				result = session.AddToPrefix("leaving".Split(), false);
				Assert.That(result.TargetSegment, Is.EqualTo("I am leaving today in the afternoon .".Split()));
			}
		}

		[Test]
		public void SetPrefix_TranslationCorrect()
		{
			using (var engine = new ThotSmtEngine(TestHelpers.ToyCorpusConfigFileName))
			using (ISmtSession session = engine.StartSession())
			{
				TranslationResult result = session.TranslateInteractively("me marcho hoy por la tarde .".Split());
				Assert.That(result.TargetSegment, Is.EqualTo("i leave today in the afternoon .".Split()));
				result = session.AddToPrefix("I am".Split(), false);
				Assert.That(result.TargetSegment, Is.EqualTo("I am leave today in the afternoon .".Split()));
				result = session.SetPrefix("I".Split(), false);
				Assert.That(result.TargetSegment, Is.EqualTo("I leave today in the afternoon .".Split()));
			}
		}

		[Test]
		public void Train_TranslationCorrect()
		{
			using (var engine = new ThotSmtEngine(TestHelpers.ToyCorpusConfigFileName))
			using (ISmtSession session = engine.StartSession())
			{
				TranslationResult result = session.Translate("esto es una prueba .".Split());
				Assert.That(result.TargetSegment, Is.EqualTo("esto is a prueba .".Split()));
				session.Train("esto es una prueba .".Split(), "this is a test .".Split());
				result = session.Translate("esto es una prueba .".Split());
				Assert.That(result.TargetSegment, Is.EqualTo("this is a test .".Split()));
			}
		}
	}
}
