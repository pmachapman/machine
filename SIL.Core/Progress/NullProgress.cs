using System;
using System.Threading;

namespace SIL.Progress
{
	public class NullProgress : IProgress
	{
		public NullProgress()
		{
			ProgressIndicator = new NullProgressIndicator();
		}
		public void WriteStatus(string message, params object[] args)
		{

		}

		public void WriteMessage(string message, params object[] args)
		{
		}

		public void WriteMessageWithColor(string colorName, string message, params object[] args)
		{

		}

		public void WriteWarning(string message, params object[] args)
		{
		}

		public void WriteException(Exception error)
		{

		}

		public void WriteError(string message, params object[] args)
		{
			ErrorEncountered = true;
		}

		public void WriteVerbose(string message, params object[] args)
		{

		}

		public bool ShowVerbose
		{
			get { return false; }
			set {  }
		}

		public virtual bool CancelRequested { get; set; }

		public virtual bool ErrorEncountered {get;set;}

		public IProgressIndicator ProgressIndicator { get; set; }

		public SynchronizationContext SyncContext { get; set; }
	}
}