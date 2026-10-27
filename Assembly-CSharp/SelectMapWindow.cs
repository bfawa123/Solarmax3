using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class SelectMapWindow : BaseWindow
{
	public override void OnShow()
	{
		base.OnShow();
		this.changePageLength = (float)Screen.currentResolution.width / 8f;
		this.sensitive *= Solarmax.Singleton<UISystem>.Get().GetNGUIRoot().pixelSizeAdjustment;
		this.moveTrigger.onDrag = new UIEventListener.VectorDelegate(this.OnMoveTriggerDrag);
		this.moveTrigger.onDragEnd = new UIEventListener.VoidDelegate(this.OnMoveTriggerDragEnd);
		this.moveTrigger.onClick = new UIEventListener.VoidDelegate(this.OnMoveTriggerClick);
		this.mapList.Clear();
		Dictionary<string, MapConfig> allData = Solarmax.Singleton<MapConfigProvider>.Instance.GetAllData();
		foreach (KeyValuePair<string, MapConfig> keyValuePair in allData)
		{
			if (keyValuePair.Value.vertical)
			{
				this.mapList.Add(keyValuePair.Key);
			}
		}
		if (this.mapList.Count > 0)
		{
			this.ShowMap(0);
		}
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	private void OnMoveTriggerClick(GameObject go)
	{
		this.mapShowCenter.SelectMap();
	}

	private void OnMoveTriggerDrag(GameObject go, Vector2 delta)
	{
		this.dragTotal += delta * this.sensitive;
		float num = Math.Abs(this.dragTotal.x);
		float num2;
		if (num > this.changePageLength)
		{
			num2 = (num - this.changePageLength) / this.changePageLength;
			if (!this.changedPage)
			{
				this.changedPage = true;
				this.topOrEnd = this.ChangeShowMapPage(this.dragTotal);
			}
		}
		else
		{
			num2 = 1f - num / this.changePageLength;
			if (this.changedPage)
			{
				this.changedPage = false;
				this.topOrEnd = this.ChangeShowMapPage(this.dragTotal);
			}
		}
		if (num2 > 1f)
		{
			num2 = 1f;
		}
		if (this.topOrEnd)
		{
			num2 = 0f;
		}
		this.mapShowCenter.ManualFade(num2);
	}

	private void OnMoveTriggerDragEnd(GameObject go)
	{
		this.ShowMap(this.selectIndex);
		this.changedPage = false;
		this.topOrEnd = false;
		this.dragTotal = Vector2.zero;
	}

	private void ShowMap(int index)
	{
		this.selectIndex = index;
		this.mapShowCenter.Switch(this.mapList[index], false);
		if (this.selectIndex - 1 >= 0)
		{
			this.mapShowLeft.Switch(this.mapList[index - 1], true);
		}
		this.mapShowLeft.ManualFade(0f);
		if (this.selectIndex + 1 < this.mapList.Count)
		{
			this.mapShowRight.Switch(this.mapList[index + 1], true);
		}
		this.mapShowRight.ManualFade(0f);
		this.nameLabel.text = string.Format("Map: {0}", this.mapList[index]);
		this.indexLabel.text = string.Format("Index: {0}/{1}", index + 1, this.mapList.Count);
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.mapList[index]);
		this.playerLabel.text = string.Format("Player: {0}", data.player_count);
	}

	private bool ChangeShowMapPage(Vector2 total)
	{
		bool result = false;
		if (this.dragTotal.x < -this.changePageLength || (this.dragTotal.x > 0f && this.dragTotal.x < this.changePageLength))
		{
			if (this.selectIndex + 1 < this.mapList.Count)
			{
				MapShow mapShow = this.mapShowRight;
				this.mapShowRight = this.mapShowLeft;
				this.mapShowLeft = this.mapShowCenter;
				this.mapShowCenter = mapShow;
				this.selectIndex++;
			}
			else
			{
				result = true;
			}
		}
		else if ((this.dragTotal.x > -this.changePageLength && this.dragTotal.x < 0f) || this.dragTotal.x > this.changePageLength)
		{
			if (this.selectIndex - 1 >= 0)
			{
				MapShow mapShow2 = this.mapShowLeft;
				this.mapShowLeft = this.mapShowRight;
				this.mapShowRight = this.mapShowCenter;
				this.mapShowCenter = mapShow2;
				this.selectIndex--;
			}
			else
			{
				result = true;
			}
		}
		this.mapShowLeft.ManualFade(0f);
		this.mapShowRight.ManualFade(0f);
		return result;
	}

	private void OnMapShowFadeOut()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("SelectMapWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("MatchWindow");
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("SelectMapWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StartWindow");
	}

	public UITexture background;

	public UILabel nameLabel;

	public UILabel indexLabel;

	public UILabel playerLabel;

	public MapShow mapShowLeft;

	public MapShow mapShowCenter;

	public MapShow mapShowRight;

	public UIEventListener moveTrigger;

	private List<string> mapList = new List<string>();

	private float sensitive = 1f;

	private Vector2 dragTotal = Vector2.zero;

	private int selectIndex = -1;

	private float changePageLength;

	private bool changedPage;

	private bool topOrEnd;
}
