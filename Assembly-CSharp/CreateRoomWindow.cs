using System;
using System.Collections.Generic;
using System.Text;
using NetMessage;
using Solarmax;
using UnityEngine;

public class CreateRoomWindow : BaseWindow
{
	private static bool IsBoundsInside(Bounds outer, Bounds inner, float padding)
	{
		return inner.min.x >= outer.min.x - padding && inner.min.y >= outer.min.y - padding && inner.max.x <= outer.max.x + padding && inner.max.y <= outer.max.y + padding;
	}

	private static Transform PromoteToTopmostWithinBounds(Transform root, Transform t, Bounds outerBounds, float padding)
	{
		if (root == null || t == null)
		{
			return t;
		}
		Transform transform = t;
		Transform parent = transform.parent;
		while (parent != null && parent != root)
		{
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(root, parent, true, true);
			if (!CreateRoomWindow.IsBoundsInside(outerBounds, bounds, padding))
			{
				break;
			}
			transform = parent;
			parent = transform.parent;
		}
		return transform;
	}

	private void Ensure3v1RoomPattern(Transform scanRoot, Transform referenceItemRoot, Transform newItemRoot)
	{
		if (scanRoot == null || referenceItemRoot == null || newItemRoot == null)
		{
			return;
		}
		Bounds outerBounds = NGUIMath.CalculateRelativeWidgetBounds(scanRoot, referenceItemRoot, true, true);
		Vector3 min = outerBounds.min;
		Vector3 max = outerBounds.max;
		float num = outerBounds.size.x;
		float num2 = outerBounds.size.y;
		min.x += num * 0.08f;
		max.x -= num * 0.08f;
		min.y += num2 * 0.12f;
		max.y -= num2 * 0.28f;
		Bounds outerBounds2 = outerBounds;
		outerBounds2.SetMinMax(min, max);
		float num3 = outerBounds2.size.x * outerBounds2.size.y;
		if (num3 <= 0.01f)
		{
			return;
		}
		Vector3 vector = newItemRoot.position - referenceItemRoot.position;
		if (vector.sqrMagnitude < 0.01f)
		{
			return;
		}
		UISprite[] componentsInChildren = scanRoot.GetComponentsInChildren<UISprite>(true);
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UISprite uisprite = componentsInChildren[i];
			if (!(uisprite == null))
			{
				Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(scanRoot, uisprite.transform, true, true);
				if (CreateRoomWindow.IsBoundsInside(outerBounds2, bounds, 2f))
				{
					float num5 = bounds.size.x * bounds.size.y;
					if (num5 >= 5f && num5 <= num3 * 0.95f)
					{
						Transform transform = CreateRoomWindow.PromoteToTopmostWithinBounds(scanRoot, uisprite.transform, outerBounds2, 2f);
						if (!(transform == null))
						{
							bool flag = false;
							for (int j = 0; j < list.Count; j++)
							{
								if (list[j] == transform)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								list.Add(transform);
							}
						}
					}
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			Transform transform2 = list[k];
			if (!(transform2 == null))
			{
				string text = "Ptn_3v1_" + transform2.name;
				if (transform2.IsChildOf(referenceItemRoot))
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(transform2.gameObject);
					gameObject.name = text;
					gameObject.transform.parent = newItemRoot;
					Vector3 position = newItemRoot.TransformPoint(referenceItemRoot.InverseTransformPoint(transform2.position));
					gameObject.transform.position = position;
					gameObject.transform.rotation = newItemRoot.rotation * (Quaternion.Inverse(referenceItemRoot.rotation) * transform2.rotation);
					gameObject.transform.localScale = transform2.localScale;
					gameObject.SetActive(true);
				}
				else
				{
					Transform parent = transform2.parent;
					if (!(parent == null) && parent.Find(text) == null)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(transform2.gameObject);
						gameObject2.name = text;
						gameObject2.transform.parent = parent;
						gameObject2.transform.position = transform2.position + vector;
						gameObject2.transform.rotation = transform2.rotation;
						gameObject2.transform.localScale = transform2.localScale;
						gameObject2.SetActive(true);
					}
				}
			}
		}
	}

	private static string GetRelativePath(Transform root, Transform target)
	{
		if (root == null || target == null)
		{
			return string.Empty;
		}
		if (target == root)
		{
			return string.Empty;
		}
		List<string> list = new List<string>();
		Transform transform = target;
		while (transform != null && transform != root)
		{
			list.Add(transform.name);
			transform = transform.parent;
		}
		if (transform != root)
		{
			return string.Empty;
		}
		list.Reverse();
		return string.Join("/", list.ToArray());
	}

	private static Transform FindTransformByPath(Transform root, string path)
	{
		if (root == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(path))
		{
			return root;
		}
		string[] array = path.Split(new char[]
		{
			'/'
		}, StringSplitOptions.RemoveEmptyEntries);
		Transform transform = root;
		for (int i = 0; i < array.Length; i++)
		{
			if (transform == null)
			{
				return null;
			}
			transform = transform.Find(array[i]);
		}
		return transform;
	}

	private static string NormalizeForMatch(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return string.Empty;
		}
		char[] array = s.ToCharArray();
		StringBuilder stringBuilder = new StringBuilder(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			if (!char.IsWhiteSpace(c) && c != '-' && c != '—' && c != '·' && c != '。' && c != '、' && c != '（' && c != '）' && c != '(' && c != ')' && c != '【' && c != '】' && c != '[' && c != ']' && c != '|' && c != '丨')
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	private UIButton FindModeButtonByLabel(string containsText)
	{
		string text = CreateRoomWindow.NormalizeForMatch(containsText);
		GameObject gameObject = (this.centerGo != null) ? this.centerGo : base.gameObject;
		UILabel[] componentsInChildren = gameObject.GetComponentsInChildren<UILabel>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UILabel uilabel = componentsInChildren[i];
			if (!(uilabel == null) && !string.IsNullOrEmpty(uilabel.text) && CreateRoomWindow.NormalizeForMatch(uilabel.text).Contains(text))
			{
				UIButton componentInParent = uilabel.GetComponentInParent<UIButton>();
				if (componentInParent != null)
				{
					return componentInParent;
				}
			}
		}
		if (this.centerGo != null && gameObject != base.gameObject)
		{
			UILabel[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<UILabel>(true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				UILabel uilabel2 = componentsInChildren2[j];
				if (!(uilabel2 == null) && !string.IsNullOrEmpty(uilabel2.text) && CreateRoomWindow.NormalizeForMatch(uilabel2.text).Contains(text))
				{
					UIButton componentInParent2 = uilabel2.GetComponentInParent<UIButton>();
					if (componentInParent2 != null)
					{
						return componentInParent2;
					}
				}
			}
		}
		return null;
	}

	private UILabel FindModeLabelByText(string containsText)
	{
		string text = CreateRoomWindow.NormalizeForMatch(containsText);
		UILabel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UILabel>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UILabel uilabel = componentsInChildren[i];
			if (!(uilabel == null) && !string.IsNullOrEmpty(uilabel.text) && CreateRoomWindow.NormalizeForMatch(uilabel.text).Contains(text))
			{
				return uilabel;
			}
		}
		return null;
	}

	private UIButton FindModeButtonByOnClick(string methodName)
	{
		if (string.IsNullOrEmpty(methodName))
		{
			return null;
		}
		UIButton[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIButton>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UIButton uibutton = componentsInChildren[i];
			if (!(uibutton == null))
			{
				List<EventDelegate> onClick = uibutton.onClick;
				if (onClick != null)
				{
					for (int j = 0; j < onClick.Count; j++)
					{
						EventDelegate eventDelegate = onClick[j];
						if (eventDelegate != null && eventDelegate.target == this && eventDelegate.methodName == methodName)
						{
							return uibutton;
						}
					}
				}
			}
		}
		return null;
	}

	private Transform FindLayoutItemRoot(UIButton anyButtonInItem)
	{
		if (anyButtonInItem == null)
		{
			return null;
		}
		UIGrid uigrid = NGUITools.FindInParents<UIGrid>(anyButtonInItem.gameObject);
		if (uigrid != null)
		{
			Transform transform = uigrid.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (!(child == null) && anyButtonInItem.transform.IsChildOf(child))
				{
					return child;
				}
			}
		}
		UITable uitable = NGUITools.FindInParents<UITable>(anyButtonInItem.gameObject);
		if (uitable != null)
		{
			Transform transform2 = uitable.transform;
			for (int j = 0; j < transform2.childCount; j++)
			{
				Transform child2 = transform2.GetChild(j);
				if (!(child2 == null) && anyButtonInItem.transform.IsChildOf(child2))
				{
					return child2;
				}
			}
		}
		return anyButtonInItem.transform;
	}

	private void Remove3v1RoomUI()
	{
		Transform[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			Transform transform2 = componentsInChildren2[i];
			if (!(transform2 == null) && (transform2.name == "Btn_3v1" || transform2.name == "Lbl_3v1" || transform2.name == "Ptn_3v1_Draw" || transform2.name.StartsWith("Ptn_3v1_")))
			{
				UnityEngine.Object.Destroy(transform2.gameObject);
			}
		}
	}

	private float ComputeSiblingGap(Transform parent)
	{
		if (parent == null)
		{
			return 30f;
		}
		List<Bounds> list = new List<Bounds>();
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if (!(child == null) && child.gameObject.activeInHierarchy && child.GetComponent<UIButton>() != null)
			{
				Bounds item = NGUIMath.CalculateRelativeWidgetBounds(parent, child, true, true);
				list.Add(item);
			}
		}
		if (list.Count < 2)
		{
			return 30f;
		}
		list.Sort((Bounds a, Bounds b) => a.center.x.CompareTo(b.center.x));
		float num = 0f;
		int num2 = 0;
		for (int j = 1; j < list.Count; j++)
		{
			float num3 = list[j].min.x - list[j - 1].max.x;
			if (num3 > 1f && num3 < 500f)
			{
				num += num3;
				num2++;
			}
		}
		if (num2 <= 0)
		{
			return 30f;
		}
		return Mathf.Clamp(num / (float)num2, 10f, 120f);
	}

	private void Ensure3v1RoomUI()
	{
		this.Remove3v1RoomUI();
		UIButton uibutton = this.FindModeButtonByLabel("队伍对战");
		if (uibutton == null)
		{
			uibutton = this.FindModeButtonByOnClick("OnTwoClick");
		}
		if (uibutton == null)
		{
			return;
		}
		Transform transform = this.FindLayoutItemRoot(uibutton);
		if (transform == null)
		{
			return;
		}
		Transform parent = transform.parent;
		if (parent == null)
		{
			return;
		}
		string relativePath = CreateRoomWindow.GetRelativePath(transform, uibutton.transform);
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(transform.gameObject);
		gameObject.name = "Btn_3v1";
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = transform.localPosition;
		gameObject.transform.localRotation = transform.localRotation;
		gameObject.transform.localScale = transform.localScale;
		Transform transform2 = (!string.IsNullOrEmpty(relativePath) ? CreateRoomWindow.FindTransformByPath(gameObject.transform, relativePath) : null);
		UIButton uibutton2 = (transform2 != null) ? transform2.GetComponent<UIButton>() : null;
		if (uibutton2 == null)
		{
			uibutton2 = gameObject.GetComponentInChildren<UIButton>(true);
		}
		if (uibutton2 != null)
		{
			uibutton2.onClick.Clear();
			uibutton2.onClick.Add(new EventDelegate(new EventDelegate.Callback(this.OnThreeVOneClick)));
			uibutton2.enabled = true;
		}
		UILabel[] componentsInChildren = gameObject.GetComponentsInChildren<UILabel>(true);
		string text = CreateRoomWindow.NormalizeForMatch(LanguageDataProvider.GetValue(2144));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UILabel uilabel = componentsInChildren[i];
			if (!(uilabel == null) && !string.IsNullOrEmpty(uilabel.text))
			{
				string text2 = CreateRoomWindow.NormalizeForMatch(uilabel.text);
				if (text2.Contains(CreateRoomWindow.NormalizeForMatch("队伍对战")) || text2.Contains(text))
				{
					uilabel.mKey = -1;
					uilabel.text = LanguageDataProvider.GetValue(7022);
				}
			}
		}
		int siblingIndex = transform.GetSiblingIndex();
		int num = Mathf.Clamp(siblingIndex + 1, 0, parent.childCount);
		gameObject.transform.SetSiblingIndex(num);
		UIGrid component2 = NGUITools.FindInParents<UIGrid>(transform.gameObject);
		if (component2 != null && component2.maxPerLine > 0 && component2.maxPerLine <= 4)
		{
			component2.maxPerLine = 0;
		}
		UITable component3 = NGUITools.FindInParents<UITable>(transform.gameObject);
		if (component2 != null)
		{
			component2.Reposition();
		}
		if (component3 != null)
		{
			component3.Reposition();
		}
		if (component2 == null && component3 == null)
		{
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(parent, transform, true, true);
			Bounds bounds2 = NGUIMath.CalculateRelativeWidgetBounds(parent, gameObject.transform, true, true);
			float num2 = this.ComputeSiblingGap(parent);
			float num3 = bounds.max.x + num2;
			float num4 = num3 - bounds2.min.x;
			gameObject.transform.localPosition = gameObject.transform.localPosition + new Vector3(num4, 0f, 0f);
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, transform.localPosition.y, gameObject.transform.localPosition.z);
		}
		gameObject.SetActive(true);
		NGUITools.UpdateWidgetCollider(gameObject);
		
		UIButton uibuttonFour = this.FindModeButtonByLabel("4人混战");
		if (uibuttonFour == null)
		{
			uibuttonFour = this.FindModeButtonByOnClick("OnFourClick");
		}
		UIButton uibuttonThree = this.FindModeButtonByLabel("3人混战");
		if (uibuttonThree == null)
		{
			uibuttonThree = this.FindModeButtonByOnClick("OnThreeClick");
		}
		Transform patternRefRoot = transform;
		if (uibuttonFour != null)
		{
			Transform rootFour = this.FindLayoutItemRoot(uibuttonFour);
			if (rootFour != null)
			{
				patternRefRoot = rootFour;
			}
		}
		else if (uibuttonThree != null)
		{
			Transform rootThree = this.FindLayoutItemRoot(uibuttonThree);
			if (rootThree != null)
			{
				patternRefRoot = rootThree;
			}
		}

		Transform transform3 = (this.centerGo != null) ? this.centerGo.transform : base.transform;
		this.Ensure3v1RoomPattern(transform3, patternRefRoot, gameObject.transform);

		UILabel uilabel2 = this.FindModeLabelByText("队伍对战");
		if (uilabel2 == null)
		{
			uilabel2 = this.FindModeLabelByText(LanguageDataProvider.GetValue(2144));
		}
		if (uilabel2 != null && !uilabel2.transform.IsChildOf(transform))
		{
			Transform parent2 = uilabel2.transform.parent;
			if (parent2 != null && parent2.Find("Lbl_3v1") == null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(uilabel2.gameObject);
				gameObject2.name = "Lbl_3v1";
				gameObject2.transform.parent = parent2;
				float num5 = gameObject.transform.localPosition.x - transform.localPosition.x;
				gameObject2.transform.localPosition = uilabel2.transform.localPosition + new Vector3(num5, 0f, 0f);
				gameObject2.transform.localRotation = uilabel2.transform.localRotation;
				gameObject2.transform.localScale = uilabel2.transform.localScale;
				UILabel component4 = gameObject2.GetComponent<UILabel>();
				if (component4 != null)
				{
					component4.mKey = -1;
					component4.text = LanguageDataProvider.GetValue(7022);
				}
				gameObject2.SetActive(true);
			}
		}
	}

	private void ApplyCenterOffset(float offsetX)
	{
		if (this.centerGo == null)
		{
			return;
		}
		if (!this.centerOffsetInitialized)
		{
			this.centerBasePos = this.centerGo.transform.localPosition;
			this.centerOffsetInitialized = true;
		}
		Vector3 vector = this.centerBasePos;
		this.centerGo.transform.localPosition = new Vector3(vector.x + offsetX, vector.y, vector.z);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnStartMatchResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		this.upGo.SetActive(true);
		this.centerGo.SetActive(true);
		this.numPage.SetActive(false);
		this.ApplyCenterOffset(-200f);
		this.Ensure3v1RoomUI();
	}

	public void OnOneClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, string.Empty, CooperationType.CT_1v1, 2, false, string.Empty, -1, string.Empty, false);
	}

	public void OnThreeVOneClick()
	{
		LevelConfigConfigProvider levelConfigConfigProvider = Solarmax.Singleton<LevelConfigConfigProvider>.Get();
		LevelConfig levelConfig = null;
		if (levelConfigConfigProvider != null)
		{
			List<LevelConfig> levelsByPrefix = levelConfigConfigProvider.GetLevelsByPrefix("44");
			if (levelsByPrefix != null && levelsByPrefix.Count > 0)
			{
				levelConfig = levelsByPrefix[UnityEngine.Random.Range(0, levelsByPrefix.Count)];
			}
		}
		string text = "440101";
		string text2 = string.Empty;
		if (levelConfig != null)
		{
			text = levelConfig.id;
			text2 = levelConfig.map;
		}
		if (string.IsNullOrEmpty(text2))
		{
			text2 = text;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, text2, CooperationType.CT_3v1, 4, false, string.Empty, -1, text, false);
	}

	public override void OnHide()
	{
		this.ApplyCenterOffset(0f);
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnStartMatchResult)
		{
			ErrCode errCode = (ErrCode)args[0];
			if (errCode == ErrCode.EC_MatchIsFull)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(902), 1f);
			}
			else if (errCode == ErrCode.EC_MatchIsMember)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(903), 1f);
			}
			else if (errCode == ErrCode.EC_NotInMatch)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1142), 1f);
			}
			else if (errCode == ErrCode.EC_NotMaster)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(905), 1f);
			}
			else if (errCode == ErrCode.EC_RoomNotExist)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(906), 1f);
			}
			else
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.Format(901, new object[]
				{
					errCode
				}), 1f);
			}
		}
	}

	public void OnFourClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, string.Empty, CooperationType.CT_1v1v1v1, 4, false, string.Empty, -1, string.Empty, false);
	}

	public void OnThreeClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, string.Empty, CooperationType.CT_1v1v1, 3, false, string.Empty, -1, string.Empty, false);
	}

	public void OnTwoClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, string.Empty, CooperationType.CT_2v2, 4, false, string.Empty, -1, string.Empty, false);
	}

	public void OnAddClick()
	{
		for (int i = 0; i < this.numPageValues.Length; i++)
		{
			this.numPageValues[i].text = string.Empty;
		}
		this.numPage.SetActive(true);
		this.numPageEnterIndex = 0;
	}

	public void OnNumPadClick()
	{
		string name = UIButton.current.gameObject.name;
		int num = 0;
		if (!int.TryParse(name, out num))
		{
			return;
		}
		if (num >= 0)
		{
			if (this.numPageEnterIndex < 4)
			{
				this.numPageValues[this.numPageEnterIndex].text = name;
				this.numPageEnterIndex++;
			}
		}
		else if (this.numPageEnterIndex > 0)
		{
			this.numPageEnterIndex--;
			this.numPageValues[this.numPageEnterIndex].text = string.Empty;
		}
		if (this.numPageEnterIndex >= 4)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.numPageValues.Length; i++)
			{
				stringBuilder.Append(this.numPageValues[i].text);
			}
			string text = stringBuilder.ToString();
			if (this.lastEnterRoomId.Equals(text))
			{
				return;
			}
			this.lastEnterRoomId = text;
			Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, text, string.Empty, CooperationType.CT_Null, 0, false, string.Empty, -1, string.Empty, false);
		}
	}

	public void OnNumPageClose()
	{
		this.numPage.SetActive(false);
		this.upGo.SetActive(true);
		this.centerGo.SetActive(true);
	}

	public void OnBackClick()
	{
		Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HallWindow");
	}

	public void OnBnSettingsClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	public void OnClickAddMoney()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}

	public GameObject upGo;

	public GameObject centerGo;

	public GameObject numPage;

	public UILabel[] numPageValues;

	public UILabel playerMoney;

	private int numPageEnterIndex;

	private string lastEnterRoomId = string.Empty;

	private bool centerOffsetInitialized;

	private Vector3 centerBasePos;
}
