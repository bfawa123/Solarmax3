using System;
using System.Collections.Generic;

[Serializable]
public class SyncMessageStruct
{
	public string version;

	public string PlayerName = string.Empty;

	public string PlayerIcon = string.Empty;

	public long PlayerFrist;

	public long regtimeSaveFile;

	public Dictionary<string, string> listChapters = new Dictionary<string, string>();
}
