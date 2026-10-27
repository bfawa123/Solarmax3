using System;
using Solarmax;
using UnityEngine;

public class JJCPreviewWindowCell : MonoBehaviour
{
	public void OnItemClick(GameObject go)
	{
		string[] array = go.transform.parent.name.Split(new char[]
		{
			'-'
		});
		int num = int.Parse(array[1]);
		Debug.Log("click on item : " + num);
	}

	public void OnRewardButtonClick()
	{
		Debug.Log("click on reward button");
	}

	public UITexture icon;

	public UILabel nameLabel;

	private LadderConfig data;
}
