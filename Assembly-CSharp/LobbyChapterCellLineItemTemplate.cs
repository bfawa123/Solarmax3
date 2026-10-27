using System;
using UnityEngine;

public class LobbyChapterCellLineItemTemplate : MonoBehaviour
{
	public void SetSpriteLine(Vector3 startPos, Vector3 endPos, string spriteName, string windowType)
	{
		UISprite component = base.gameObject.GetComponent<UISprite>();
		if (component == null)
		{
			return;
		}
		base.transform.localScale = new Vector3(1f, 0.3f, 1f);
		Vector3 localPosition = (startPos + endPos) * 0.5f;
		base.transform.localPosition = localPosition;
		float num = Vector3.Distance(startPos, endPos);
		component.width = (int)num;
		component.spriteName = spriteName;
		Vector3 vector = endPos - startPos;
		this.eulerAngle.z = Mathf.Atan2(vector.y, vector.x) * 180f / 3.1415927f;
		base.transform.eulerAngles = this.eulerAngle;
		if (windowType == "LobbyWindow")
		{
			component.alpha = 0.3f;
		}
	}

	public void SetSprite(string spriteName)
	{
		base.gameObject.GetComponent<UISprite>().spriteName = spriteName;
	}

	private Vector3 eulerAngle = Vector3.zero;
}
