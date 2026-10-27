using System;
using System.IO;

namespace Solarmax
{
	public class FileSaver
	{
		public FileSaver()
		{
			this.mStreamWriter = null;
		}

		public void Init(string filePathName)
		{
			if (string.IsNullOrEmpty(filePathName))
			{
				return;
			}
			this.mStreamWriter = new StreamWriter(filePathName, true);
		}

		public void WriteLine(string line)
		{
			if (this.mStreamWriter != null)
			{
				this.mStreamWriter.WriteLine(line);
			}
		}

		public void Flush()
		{
			if (this.mStreamWriter != null)
			{
				this.mStreamWriter.Flush();
			}
		}

		public void Close()
		{
			if (this.mStreamWriter != null)
			{
				this.mStreamWriter.Close();
				this.mStreamWriter.Dispose();
			}
		}

		private StreamWriter mStreamWriter;
	}
}
