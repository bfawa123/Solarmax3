using System;
using UnityEngine;

public class CheckBox : MonoBehaviour
{
	private void Awake()
	{
		this.mChecked = !this.InitChecked;
		this.SetChecked(this.InitChecked);
	}

	public void SetChecked(bool b)
	{
		if (this.mChecked == b)
		{
			return;
		}
		this.mChecked = b;
		if (null != this.CheckedGo)
		{
			this.CheckedGo.SetActive(this.mChecked);
		}
		if (null != this.UnCheckedGo)
		{
			this.UnCheckedGo.SetActive(!this.mChecked);
		}
	}

	public GameObject CheckedGo;

	public GameObject UnCheckedGo;

	public bool InitChecked;

	private bool mChecked;
}
