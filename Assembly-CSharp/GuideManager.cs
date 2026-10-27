using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
	private void Start()
	{
		GuideManager.Guide = this;
		base.Invoke("RegistEvent", 0.5f);
	}

	public void OnClickCloseGuide()
	{
		GuideManager.Guide.CompletedGuide(GuildEndEvent.clicked);
	}

	private void RegistEvent()
	{
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.NetworkStatus, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.ReconnectResult, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
	}

	private void OnEventHandler(int eventId, object data, params object[] args)
	{
		if (eventId == (int)EventId.NetworkStatus || eventId == (int)EventId.ReconnectResult)
		{
			GuideManager.ClearGuideData();
		}
	}

	private void Update()
	{
		bool flag = false;
		int count = this.mGuidingList.Count;
		for (int i = 0; i < count; i++)
		{
			GuideNode guideNode = this.mGuidingList[i];
			this.mGuidingList[i].LogicNode();
			if (guideNode.mCurGuideConfig.endCondition1 == GuildEndEvent.animend || guideNode.mCurGuideConfig.endCondition2 == GuildEndEvent.animend)
			{
				flag = true;
			}
		}
		if (flag)
		{
			this.CompletedGuide(GuildEndEvent.animend);
		}
	}

	public static void StartGuide(GuildCondition eC, string strC, GameObject objPanel = null)
	{
		GuideManager.Guide.GetGuideConfigByCondition(eC, strC, objPanel);
	}

	public static void TriggerGuidecompleted(GuildEndEvent eEvent)
	{
		GuideManager.Guide.CompletedGuide(eEvent);
	}

	public static void ClearGuideData()
	{
		GuideManager.Guide.ClearGuide();
	}

	public void InitCompletedGuide(string strComplte)
	{
		this.mCompleteGuideList.Clear();
		if (!string.IsNullOrEmpty(strComplte))
		{
			string[] array = strComplte.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				this.mCompleteGuideList.Add(int.Parse(array[i]));
			}
		}
	}

	private void ClearGuide()
	{
		for (int i = 0; i < this.mGuidingList.Count; i++)
		{
			this.mGuidingList[i].EndGuideNode();
			this.mGuidingList[i] = null;
		}
		this.mGuidingList.Clear();
	}

	private void CompletedGuide(GuildEndEvent endCondition)
	{
		List<GuideNode> list = new List<GuideNode>();
		List<GuideNode> list2 = new List<GuideNode>();
		for (int i = 0; i < this.mGuidingList.Count; i++)
		{
			if (this.mGuidingList[i].Completed(endCondition))
			{
				int configID = this.mGuidingList[i].GetConfigID();
				this.mGuidingList[i].EndGuideNode();
				this.mCompleteGuideList.Add(configID);
				list2.Add(this.mGuidingList[i]);
			}
		}
		if (this.mGuidingList.Count >= 1)
		{
			for (int j = 0; j < list2.Count; j++)
			{
				int endGuide = list2[j].mCurGuideConfig.endGuide;
				if (list2[j].mCurGuideConfig != null && endGuide > 0)
				{
					GuideNode guidle = this.GetGuidle(endGuide);
					if (guidle != null)
					{
						list.Add(guidle);
					}
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			this.mGuidingList.Remove(list[k]);
			list[k].EndGuideNode();
			this.mCompleteGuideList.Add(list[k].GetConfigID());
			list2.Add(list[k]);
		}
		for (int l = 0; l < list2.Count; l++)
		{
			int count = list2[l].mCurGuideConfig.nextID.Count;
			if (count > 0)
			{
				for (int m = 0; m < count; m++)
				{
					CTagGuideConfig value = Solarmax.Singleton<GuideDataProvider>.Get().GetValue(list2[l].mCurGuideConfig.nextID[m]);
					this.StartNewGuide(value);
				}
			}
			this.mGuidingList.Remove(list2[l]);
		}
		list2.Clear();
		Solarmax.Singleton<LocalAccountStorage>.Get().guideFightLevel = this.GuideToString();
		Solarmax.Singleton<LocalStorageSystem>.Instance.SaveLocalAccount(false);
	}

	private GuideNode GetGuidle(int id)
	{
		for (int i = 0; i < this.mGuidingList.Count; i++)
		{
			if (this.mGuidingList[i].GetConfigID() == id)
			{
				return this.mGuidingList[i];
			}
		}
		return null;
	}

	private void StartNewGuide(CTagGuideConfig config)
	{
		if (this.IsFinished(config.id))
		{
			return;
		}
		if (config.prevID > 0 && !this.IsFinished(config.prevID))
		{
			return;
		}
		if (this.IsGuidePlaying(config.id))
		{
			return;
		}
		GuideNode guideNode = new GuideNode();
		if (guideNode != null)
		{
			guideNode.mRootObject = base.gameObject;
			if (config.type == GuideType.Ani)
			{
				guideNode.animator = this.animator;
				guideNode.InitGuideNode(config, this.mGuideTool[config.windowType]);
			}
			else if (config.type == GuideType.Tip)
			{
				guideNode.InitGuideNode(config, this.mTip[config.windowType]);
			}
			else if (config.type == GuideType.Window)
			{
				guideNode.InitGuideNode(config, this.mWindow[config.windowType]);
			}
			this.mGuidingList.Add(guideNode);
		}
	}

	private void GetGuideConfigByCondition(GuildCondition eC, string strC, GameObject objPanel)
	{
		int nCurCompltedID = -1;
		List<CTagGuideConfig> guideConfigByCondition = Solarmax.Singleton<GuideDataProvider>.Get().GetGuideConfigByCondition(eC, strC, nCurCompltedID);
		if (guideConfigByCondition != null)
		{
			for (int i = 0; i < guideConfigByCondition.Count; i++)
			{
				this.StartNewGuide(guideConfigByCondition[i]);
			}
		}
	}

	private bool IsFinished(int Id)
	{
		int count = this.mCompleteGuideList.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.mCompleteGuideList[i] == Id)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsGuidePlaying(int id)
	{
		int count = this.mGuidingList.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.mGuidingList[i].GetConfigID() == id)
			{
				return true;
			}
		}
		return false;
	}

	public string GuideToString()
	{
		string text = string.Empty;
		int count = this.mCompleteGuideList.Count;
		for (int i = 0; i < count; i++)
		{
			if (i == 0)
			{
				text += this.mCompleteGuideList[i].ToString();
			}
			else
			{
				text = text + "," + this.mCompleteGuideList[i].ToString();
			}
		}
		return text;
	}

	public static GuideManager Guide;

	public Animator animator;

	public GameObject mBG;

	public GameObject[] mGuideTool;

	public GameObject[] mTip;

	public UILabel mDesc;

	public List<GameObject> mWindow;

	private List<int> mCompleteGuideList = new List<int>();

	private List<GuideNode> mGuidingList = new List<GuideNode>();
}
