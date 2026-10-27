using System;
using UnityEngine;

[Serializable]
public class FunctionOpenTips
{
	public void Refresh()
	{
		this.pnlFunctionOpenTips.SetActive(false);
	}

	public GameObject pnlFunctionOpenTips;

	public UILabel lblFunctionOpenTips;
}
