using System;
using Solarmax;
using UnityEngine;

public class TouchHandler : MonoBehaviour
{
	public static void Clean()
	{
		TouchHandler.currentNode = null;
		TouchHandler.currentSelect = null;
		if (TouchHandler.beginHalo != null)
		{
			UnityEngine.Object.Destroy(TouchHandler.beginHalo.gameObject);
		}
		TouchHandler.beginHalo = null;
		if (TouchHandler.endHalo != null)
		{
			UnityEngine.Object.Destroy(TouchHandler.endHalo.gameObject);
		}
		TouchHandler.endHalo = null;
		if (TouchHandler.line != null)
		{
			UnityEngine.Object.Destroy(TouchHandler.line.gameObject);
		}
		TouchHandler.line = null;
		if (TouchHandler.groundBegin != null)
		{
			UnityEngine.Object.Destroy(TouchHandler.groundBegin.transform.parent.gameObject);
		}
		TouchHandler.groundBegin = null;
		if (TouchHandler.groundEnd != null)
		{
			UnityEngine.Object.Destroy(TouchHandler.groundEnd.transform.parent.gameObject);
		}
		TouchHandler.groundEnd = null;
	}

	public static void HideOperater()
	{
		TouchHandler.currentNode = null;
		TouchHandler.currentSelect = null;
		TouchHandler.beginHalo.gameObject.SetActive(false);
		TouchHandler.endHalo.gameObject.SetActive(false);
		TouchHandler.line.gameObject.SetActive(false);
		TouchHandler.groundBegin.gameObject.SetActive(false);
		TouchHandler.groundEnd.gameObject.SetActive(false);
	}

	public static void SetWarning(int warning)
	{
		TouchHandler.isWarning = warning;
		if (TouchHandler.isWarning == 1)
		{
			TouchHandler.beginHalo.color = Color.red;
			TouchHandler.endHalo.color = Color.red;
			TouchHandler.line.color = Color.red;
		}
		else if (TouchHandler.isWarning == 0)
		{
			TouchHandler.beginHalo.color = Color.white;
			TouchHandler.endHalo.color = Color.white;
			TouchHandler.line.color = Color.white;
		}
		else if (TouchHandler.isWarning == 2)
		{
			TouchHandler.beginHalo.color = Color.green;
			TouchHandler.endHalo.color = Color.green;
			TouchHandler.line.color = Color.green;
		}
	}

	public void SetNode(Node node)
	{
		this.m_Node = node;
		if (TouchHandler.beginHalo == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Selected");
			TouchHandler.beginHalo = (UnityEngine.Object.Instantiate(resources) as GameObject).GetComponent<UISprite>();
			TouchHandler.beginHalo.transform.localScale = Vector3.one * 0.0168f;
			TouchHandler.beginHalo.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
			TouchHandler.beginHalo.transform.position = Vector3.zero;
			TouchHandler.beginHalo.gameObject.SetActive(false);
		}
		if (TouchHandler.endHalo == null)
		{
			UnityEngine.Object resources2 = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Selected");
			TouchHandler.endHalo = (UnityEngine.Object.Instantiate(resources2) as GameObject).GetComponent<UISprite>();
			TouchHandler.endHalo.transform.localScale = Vector3.one * 0.0168f;
			TouchHandler.endHalo.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
			TouchHandler.endHalo.transform.position = Vector3.zero;
			TouchHandler.endHalo.gameObject.SetActive(false);
		}
		if (TouchHandler.groundBegin == null)
		{
			UnityEngine.Object resources3 = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Select");
			GameObject gameObject = UnityEngine.Object.Instantiate(resources3) as GameObject;
			gameObject.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			TouchHandler.groundBegin = gameObject.GetComponentInChildren<SpriteRenderer>();
			TouchHandler.groundBegin.enabled = false;
		}
		if (TouchHandler.groundEnd == null)
		{
			UnityEngine.Object resources4 = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Select");
			GameObject gameObject2 = UnityEngine.Object.Instantiate(resources4) as GameObject;
			gameObject2.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
			gameObject2.transform.position = Vector3.zero;
			gameObject2.transform.localScale = Vector3.one;
			TouchHandler.groundEnd = gameObject2.GetComponentInChildren<SpriteRenderer>();
			TouchHandler.groundEnd.enabled = false;
		}
		if (TouchHandler.line == null)
		{
			UnityEngine.Object resources5 = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Line");
			TouchHandler.line = (UnityEngine.Object.Instantiate(resources5) as GameObject).GetComponent<UISprite>();
			TouchHandler.line.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
			TouchHandler.line.transform.position = Vector3.zero;
			TouchHandler.line.transform.localScale = Vector3.one * 0.025f;
			TouchHandler.line.width = 0;
			TouchHandler.line.gameObject.SetActive(false);
		}
	}

	private void OnPress(bool isDown)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.IsPause())
		{
			return;
		}
		if (!Solarmax.Singleton<BattleSystem>.Instance.canOperation)
		{
			return;
		}
		if (isDown)
		{
			TouchHandler.IsPressGuid = true;
			if (this.m_Node == null)
			{
				return;
			}
			if (this.m_Node.nodeType == NodeType.Clouds)
			{
				return;
			}
			if (this.m_Node.nodeType == NodeType.FixedWarpDoor)
			{
				return;
			}
			if ((this.m_Node.nodeIsHide || this.m_Node.forceHideNode) && this.m_Node.currentTeam.team != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
			{
				return;
			}
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay)
			{
				return;
			}
			if (TouchHandler.ShowingGuidEffect)
			{
				Solarmax.Singleton<EffectManager>.Get().HideGuideEffect();
				TouchHandler.ShowingGuidEffect = false;
			}
			if (TouchHandler.currentNode != null)
			{
				return;
			}
			if (TouchHandler.lastPressTime > 0.1f)
			{
				if (Time.unscaledTime - TouchHandler.lastPressTime < 0.5f)
				{
					Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.DoubleTouch, new object[]
					{
						this.m_Node
					});
					TouchHandler.lastPressTime = 0f;
				}
				else
				{
					TouchHandler.lastPressTime = Time.unscaledTime;
				}
			}
			else
			{
				TouchHandler.lastPressTime = Time.unscaledTime;
			}
			TouchHandler.beginHalo.spriteName = string.Format("BattleAtlas-{0}", this.m_Node.GetScale());
			TouchHandler.beginHalo.MakePixelPerfect();
			TouchHandler.beginHalo.gameObject.transform.position = this.m_Node.GetPosition();
			TouchHandler.beginHalo.gameObject.transform.localScale = TouchHandler.haloScaleV;
			TouchHandler.beginHalo.gameObject.SetActive(true);
			TouchHandler.groundBegin.transform.position = this.m_Node.GetPosition();
			TouchHandler.groundBegin.transform.localScale = TouchHandler.groundScale * this.m_Node.GetScale() * Vector3.one;
			TouchHandler.groundBegin.enabled = true;
			Color color = this.m_Node.currentTeam.color;
			color.a = 1f;
			TouchHandler.groundBegin.color = color;
			TouchHandler.line.width = 0;
			this.m_Node.ShowRange(true);
			TouchHandler.currentNode = this.m_Node;
			TouchHandler.currentSelect = null;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnTouchBegin, new object[]
			{
				TouchHandler.currentNode
			});
		}
		else
		{
			TouchHandler.IsPressGuid = false;
			if (this.m_Node == null)
			{
				return;
			}
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay)
			{
				return;
			}
			if (this.m_Node != TouchHandler.currentNode)
			{
				return;
			}
			if (TouchHandler.currentNode != TouchHandler.currentSelect && TouchHandler.currentNode.GetAttributeInt(NodeAttr.Ice) <= 0 && (TouchHandler.isWarning == 0 || TouchHandler.isWarning == 2) && TouchHandler.currentNode != null && TouchHandler.currentSelect != null)
			{
				TouchHandler.currentNode = TouchHandler.currentNode.nodeManager.GetNode(TouchHandler.currentNode.tag);
				TouchHandler.currentSelect = TouchHandler.currentNode.nodeManager.GetNode(TouchHandler.currentSelect.tag);
				if (TouchHandler.currentNode != null && TouchHandler.currentSelect != null)
				{
					Solarmax.Singleton<BattleSystem>.Instance.OnPlayerMove(TouchHandler.currentNode, TouchHandler.currentSelect);
					GuideManager.TriggerGuidecompleted(GuildEndEvent.Slide);
				}
			}
			this.m_Node.ShowRange(false);
			if (TouchHandler.currentSelect != null)
			{
				TouchHandler.currentSelect.ShowRange(false);
			}
			if (TouchHandler.currentSelect != null)
			{
				Solarmax.Singleton<EffectManager>.Get().HideSelectEffect(TouchHandler.currentSelect);
			}
			if (TouchHandler.currentNode != null)
			{
				Solarmax.Singleton<EffectManager>.Get().HideSelectEffect(TouchHandler.currentNode);
			}
			this.OnFadeOutHalo();
			if (TouchHandler.line != null)
			{
				TouchHandler.line.gameObject.SetActive(false);
			}
			TouchHandler.currentNode = null;
			TouchHandler.currentSelect = null;
			TouchHandler.SetWarning(0);
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnTouchEnd, new object[0]);
		}
	}

	private void OnFadeOutHalo()
	{
		if (TouchHandler.beginHalo != null && TouchHandler.beginHalo.gameObject.activeSelf)
		{
			bool flag = true;
			if (TouchHandler.endHalo == null || !TouchHandler.endHalo.gameObject.activeSelf)
			{
				flag = false;
			}
			TweenScale ts = TouchHandler.beginHalo.gameObject.GetComponent<TweenScale>();
			if (ts == null)
			{
				ts = TouchHandler.beginHalo.gameObject.AddComponent<TweenScale>();
			}
			ts.ResetToBeginning();
			ts.from = Vector3.one * 0.0168f;
			ts.to = ((!flag) ? (Vector3.one * 0.9f * 0.0168f) : (Vector3.one * 1.1f * 0.0168f));
			ts.duration = 0.2f;
			ts.SetOnFinished(delegate()
			{
				if (!TouchHandler.IsPressGuid)
				{
					TouchHandler.beginHalo.gameObject.SetActive(false);
				}
				ts.to = Vector3.one * 0.0168f;
			});
			ts.Play(true);
		}
		if (TouchHandler.groundBegin != null)
		{
			this.OnFadeOutScriptRender(TouchHandler.groundBegin);
		}
		if (TouchHandler.endHalo != null && TouchHandler.endHalo.gameObject.activeSelf)
		{
			TweenScale ts = TouchHandler.endHalo.gameObject.GetComponent<TweenScale>();
			if (ts == null)
			{
				ts = TouchHandler.endHalo.gameObject.AddComponent<TweenScale>();
			}
			ts.ResetToBeginning();
			ts.from = TouchHandler.haloScaleV;
			ts.to = TouchHandler.haloScaleV * 0.9f;
			ts.duration = 0.2f;
			ts.SetOnFinished(delegate()
			{
				if (!TouchHandler.IsPressGuid)
				{
					TouchHandler.endHalo.gameObject.SetActive(false);
				}
				ts.to = TouchHandler.haloScaleV;
			});
			ts.Play(true);
		}
		if (TouchHandler.groundEnd != null)
		{
			this.OnFadeOutScriptRender(TouchHandler.groundEnd);
		}
	}

	private void OnDrag(Vector2 pos)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.IsPause())
		{
			return;
		}
		if (this.m_Node == null)
		{
			return;
		}
		if (this.m_Node.nodeType == NodeType.Clouds)
		{
			return;
		}
		if (this.m_Node.nodeType == NodeType.FixedWarpDoor)
		{
			return;
		}
		if (TouchHandler.currentNode != null && (TouchHandler.currentNode.nodeType == NodeType.FixedWarpDoor || TouchHandler.currentNode.nodeType == NodeType.Clouds))
		{
			return;
		}
		if (this.m_Node != null && this.m_Node.nodeIsHide && this.m_Node.currentTeam.team != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
		{
			return;
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay)
		{
			return;
		}
		if (TouchHandler.currentSelect == TouchHandler.currentNode)
		{
			if (TouchHandler.line == null)
			{
				return;
			}
			TouchHandler.line.width = 0;
			TouchHandler.line.gameObject.SetActive(false);
			return;
		}
		else
		{
			Vector3 position = TouchHandler.currentNode.GetPosition();
			Vector3 vector = (TouchHandler.currentSelect != null) ? TouchHandler.currentSelect.GetPosition() : Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10f));
			vector.z = 0f;
			if (TouchHandler.currentSelect != null && !TouchHandler.currentSelect.CanBeTarget())
			{
				return;
			}
			if (TouchHandler.currentSelect != null && (TouchHandler.currentSelect.nodeIsHide || TouchHandler.currentSelect.forceHideNode) && TouchHandler.currentSelect.currentTeam.team != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
			{
				return;
			}
			float num = Vector3.Distance(position, vector);
			float dist = TouchHandler.currentNode.GetDist();
			if (num < dist)
			{
				TouchHandler.line.width = 0;
				TouchHandler.line.gameObject.SetActive(false);
				return;
			}
			float num2 = (TouchHandler.currentSelect != null) ? TouchHandler.currentSelect.GetHalfNodeSize() : 0f;
			float halfNodeSize = TouchHandler.currentNode.GetHalfNodeSize();
			TouchHandler.line.transform.position = Vector3.MoveTowards(position, vector, halfNodeSize);
			int num3 = (int)(40f * (num - halfNodeSize - num2));
			if (num3 <= 0)
			{
				return;
			}
			if (TouchHandler.currentNode.GetAttributeInt(NodeAttr.Ice) > 0)
			{
				TouchHandler.SetWarning(1);
			}
			else if (TouchHandler.currentNode.CanWarp())
			{
				TouchHandler.SetWarning(0);
			}
			else if (TouchHandler.currentNode.team != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam && TouchHandler.currentNode.GetShipCount((int)Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam) == 0)
			{
				TouchHandler.SetWarning(1);
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam).GetAttributeInt(TeamAttr.QuickMove) > 0)
			{
				TouchHandler.SetWarning(0);
			}
			else
			{
				bool flag = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.IsFixedPortal(position, vector);
				bool intersection = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetIntersection(position, vector);
				if (intersection)
				{
					TouchHandler.SetWarning(1);
				}
				else
				{
					TouchHandler.SetWarning(0);
				}
				if (flag)
				{
					TouchHandler.SetWarning(2);
				}
			}
			TouchHandler.line.width = num3;
			Vector3 vector2 = vector - position;
			TouchHandler.eulerAngle.z = Mathf.Atan2(vector2.y, vector2.x) * 180f / 3.1415927f;
			TouchHandler.line.transform.eulerAngles = TouchHandler.eulerAngle;
			TouchHandler.line.gameObject.SetActive(true);
			TouchHandler.beginHalo.gameObject.transform.position = position;
			TouchHandler.groundBegin.transform.position = position;
			if (TouchHandler.currentSelect != null)
			{
				TouchHandler.endHalo.gameObject.transform.position = vector;
				TouchHandler.groundEnd.transform.position = vector;
			}
			return;
		}
	}

	private void OnDragOver(GameObject go)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.IsPause())
		{
			return;
		}
		if (this.m_Node == null || TouchHandler.currentNode == null)
		{
			return;
		}
		if (TouchHandler.currentNode == this.m_Node)
		{
			return;
		}
		if (this.m_Node.nodeType == NodeType.Clouds || TouchHandler.currentNode.nodeType == NodeType.Clouds)
		{
			return;
		}
		if (this.m_Node.nodeType == NodeType.FixedWarpDoor || TouchHandler.currentNode.nodeType == NodeType.FixedWarpDoor)
		{
			return;
		}
		if (this.m_Node != null && (this.m_Node.nodeIsHide || this.m_Node.forceHideNode) && this.m_Node.currentTeam.team != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
		{
			return;
		}
		if (TouchHandler.currentNode != null && (TouchHandler.currentNode.nodeIsHide || TouchHandler.currentNode.forceHideNode) && TouchHandler.currentNode.currentTeam.team != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
		{
			return;
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay)
		{
			return;
		}
		this.m_Node.ShowRange(true);
		TouchHandler.currentSelect = this.m_Node;
		TouchHandler.endHalo.spriteName = string.Format("BattleAtlas-{0}", TouchHandler.currentSelect.GetScale());
		TouchHandler.endHalo.MakePixelPerfect();
		TouchHandler.endHalo.gameObject.transform.position = TouchHandler.currentSelect.GetPosition();
		TouchHandler.endHalo.gameObject.transform.localScale = TouchHandler.haloScaleV;
		TouchHandler.endHalo.gameObject.SetActive(true);
		TouchHandler.groundEnd.transform.position = TouchHandler.currentSelect.GetPosition();
		TouchHandler.groundEnd.transform.localScale = TouchHandler.groundScale * this.m_Node.GetScale() * Vector3.one;
		TouchHandler.groundEnd.enabled = true;
		Color color = TouchHandler.currentSelect.currentTeam.color;
		color.a = 1f;
		TouchHandler.groundEnd.color = color;
	}

	private void OnDragOut(GameObject go)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.IsPause())
		{
			return;
		}
		if (this.m_Node == null)
		{
			return;
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay)
		{
			return;
		}
		if (TouchHandler.endHalo != null && TouchHandler.IsPressGuid)
		{
			TouchHandler.endHalo.gameObject.SetActive(false);
		}
		if (TouchHandler.groundEnd != null)
		{
			TouchHandler.groundEnd.enabled = false;
		}
		this.m_Node.ShowRange(false);
		if (TouchHandler.currentSelect != null)
		{
			TouchHandler.currentSelect.ShowRange(false);
		}
		TouchHandler.currentSelect = null;
		if (this.m_Node == TouchHandler.currentNode)
		{
			TouchHandler.groundBegin.enabled = false;
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnTouchPause, new object[]
		{
			this.m_Node
		});
	}

	private void OnFadeOutScriptRender(SpriteRenderer sRender)
	{
		if (sRender == null || sRender.gameObject == null)
		{
			return;
		}
		TweenAlpha tweenAlpha = sRender.gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = sRender.gameObject.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = 0.2f;
		tweenAlpha.SetOnFinished(delegate()
		{
			sRender.enabled = false;
		});
		tweenAlpha.Play(true);
	}

	private const float DoubleClickInterval = 0.5f;

	public static Node currentNode;

	public static Node currentSelect;

	private Node m_Node;

	public static UISprite beginHalo;

	public static UISprite endHalo;

	public static SpriteRenderer groundBegin;

	public static SpriteRenderer groundEnd;

	public static UISprite line;

	public static bool ShowingGuidEffect = false;

	public static bool IsPressGuid = false;

	private static Vector3 eulerAngle = Vector3.zero;

	private static int isWarning = 0;

	private static float haloScale = 1f;

	private static float groundScale = TouchHandler.haloScale - 0.25f;

	public static Vector3 haloScaleV = Vector3.one * 0.0168f;

	private static float lastPressTime;
}
