﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using SIL.Machine.Tokenization;
using SIL.Scripture;

namespace SIL.Machine.Corpora
{
	public class UsxFileTextAlignmentCorpus : DictionaryTextAlignmentCorpus
	{
		public UsxFileTextAlignmentCorpus(IRangeTokenizer<string, int, string> srcWordTokenizer,
			IRangeTokenizer<string, int, string> trgWordTokenizer, string srcProjectPath, string trgProjectPath,
			ScrVers srcVersification = null, ScrVers trgVersification = null)
		{
			srcVersification = GetVersification(srcProjectPath, srcVersification);
			trgVersification = GetVersification(trgProjectPath, trgVersification);

			Dictionary<string, string> srcFileNames = GetFileNames(srcProjectPath);
			Dictionary<string, string> trgFileNames = GetFileNames(trgProjectPath);

			foreach (string id in srcFileNames.Keys.Intersect(trgFileNames.Keys))
			{
				string srcFileName = srcFileNames[id];
				string trgFileName = trgFileNames[id];
				AddTextAlignmentCollection(new UsxFileTextAlignmentCollection(srcWordTokenizer, trgWordTokenizer,
					srcFileName, trgFileName, srcVersification, trgVersification));
			}
		}

		public override ITextAlignmentCollection CreateNullTextAlignmentCollection(string id)
		{
			return new NullTextAlignmentCollection(id, CorporaHelpers.GetScriptureTextSortKey(id));
		}

		private static Dictionary<string, string> GetFileNames(string projectPath)
		{
			return Directory.EnumerateFiles(projectPath, "*.usx").ToDictionary(f => CorporaHelpers.GetUsxId(f));
		}

		private static ScrVers GetVersification(string projectPath, ScrVers versification)
		{
			string versificationFileName = Path.Combine(projectPath, "versification.vrs");
			if (versification == null && File.Exists(versificationFileName))
			{
				string vrsName = Path.GetFileName(projectPath);
				versification = Versification.Table.Implementation.Load(versificationFileName, vrsName);
			}
			return versification ?? ScrVers.English;
		}
	}
}
