using System;
using UnityEngine;

[RequireComponent(typeof(UIWidget))]
public class SetColorPickerColor : MonoBehaviour
{
	public void SetToCurrent()
	{
		if (this.mWidget == null)
		{
			this.mWidget = base.GetComponent<UIWidget>();
		}
		if (UIColorPicker.current != null)
		{
			this.mWidget.color = UIColorPicker.current.value;
		}
	}

	[NonSerialized]
	private UIWidget mWidget;
}
