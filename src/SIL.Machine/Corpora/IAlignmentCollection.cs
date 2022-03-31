﻿using System.Collections.Generic;

namespace SIL.Machine.Corpora
{
	public interface IAlignmentCollection
	{
		string Id { get; }

		string SortKey { get; }

		bool MissingRowsAllowed { get; }

		int Count(bool includeEmpty = true);

		IEnumerable<AlignmentRow> GetRows();
	}
}
