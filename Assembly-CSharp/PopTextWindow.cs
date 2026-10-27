using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class PopTextWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnSkillPop);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		for (int i = 0; i < 20; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.popTemp);
			gameObject.transform.parent = this.popTemp.transform.parent;
			gameObject.transform.localScale = Vector3.one;
			this.objList.Enqueue(gameObject);
		}
	}

	public override void OnHide()
	{
	}

	private void Update()
	{
		int i = 0;
		while (i < this.popMap.Count)
		{
			PopItem popItem = this.popMap[i];
			popItem.poptime -= Time.deltaTime;
			if (popItem.poptime < 0f)
			{
				popItem.popobj.SetActive(false);
				this.objList.Enqueue(popItem.popobj);
				this.popMap.Remove(popItem);
			}
			else
			{
				POP_TYPE type = popItem.type;
				if (type != POP_TYPE.POP_UP)
				{
					if (type == POP_TYPE.POP_DOWN)
					{
						Vector3 localPosition = popItem.popobj.transform.localPosition;
						localPosition.y -= Time.deltaTime * 50f;
						popItem.popobj.transform.localPosition = localPosition;
					}
				}
				else
				{
					Vector3 localPosition2 = popItem.popobj.transform.localPosition;
					localPosition2.y += Time.deltaTime * 50f;
					popItem.popobj.transform.localPosition = localPosition2;
				}
				i++;
			}
		}
		if (this.centerTipTime > 0f)
		{
			this.centerTipTime -= Time.deltaTime;
			Vector3 localPosition3 = this.popCenter.transform.localPosition;
			localPosition3.y += Time.deltaTime * 50f;
			this.popCenter.transform.localPosition = localPosition3;
			if (this.centerTipTime < 0f)
			{
				this.popCenter.SetActive(false);
			}
		}
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnSkillPop)
		{
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
			{
				return;
			}
			POP_TYPE pop_TYPE = (POP_TYPE)args[0];
			string text = (string)args[1];
			Vector3 position = Vector3.zero;
			if (pop_TYPE != POP_TYPE.POP_NORMAL)
			{
				position = (Vector3)args[2];
				PopItem popItem = new PopItem();
				popItem.poptext = text;
				popItem.poptime = 2f;
				popItem.type = pop_TYPE;
				if (this.objList.Count > 0)
				{
					popItem.popobj = this.objList.Dequeue();
				}
				else
				{
					popItem.popobj = UnityEngine.Object.Instantiate<GameObject>(this.popTemp);
				}
				popItem.popobj.transform.parent = this.popTemp.transform.parent;
				popItem.popobj.transform.localScale = Vector3.one;
				Vector3 position2 = Camera.main.WorldToScreenPoint(position);
				Vector3 position3 = UICamera.mainCamera.ScreenToWorldPoint(position2);
				position3.z = 0f;
				popItem.popobj.transform.position = position3;
				popItem.popobj.SetActive(true);
				UILabel componentInChildren = popItem.popobj.GetComponentInChildren<UILabel>();
				componentInChildren.text = popItem.poptext;
				this.popMap.Add(popItem);
			}
			else
			{
				this.popCenter.SetActive(true);
				this.popCenterText.text = text;
				this.centerTipTime = 2f;
				this.popCenter.transform.localPosition = new Vector3(0f, 362f, 0f);
			}
		}
	}

	public GameObject popTemp;

	public GameObject popCenter;

	public UILabel popCenterText;

	public List<PopItem> popMap = new List<PopItem>();

	private Queue<GameObject> objList = new Queue<GameObject>();

	private float centerTipTime;
}
