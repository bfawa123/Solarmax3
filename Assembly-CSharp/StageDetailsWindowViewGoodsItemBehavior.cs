using System;
using UnityEngine;

public class StageDetailsWindowViewGoodsItemBehavior : MonoBehaviour
{
	public void Show(bool selected)
	{
		this.selectedBg.SetActive(selected);
		this.bg.SetActive(!selected);
	}

	public GameObject selectedBg;

	public GameObject bg;
}
