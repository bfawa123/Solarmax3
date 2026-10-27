using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UICustomWrapContent : MonoBehaviour
{
	private void Start()
	{
	}

	public void EnsureInit()
	{
		this.MAX_REUSE = 9;
		this.maxIndex = Solarmax.Singleton<CommentModel>.Get().comments.Count - 1;
		this.currentTail = this.MAX_REUSE;
		this.SortBasedOnScrollMovement();
		if (this.mScroll != null)
		{
			this.mScroll.GetComponent<UIPanel>().onClipMove = new UIPanel.OnClippingMoved(this.OnMove);
		}
		this.mFirstTime = false;
		for (int i = this.MAX_REUSE; i >= 0; i--)
		{
			this.queue.AddFirst(i);
		}
	}

	public void ResetMax(int maxTemplate)
	{
		this.maxIndex = Solarmax.Singleton<CommentModel>.Get().comments.Count - 1;
	}

	public void EnsureClear()
	{
	}

	protected virtual void OnMove(UIPanel panel)
	{
		this.WrapContent();
	}

	public virtual void SortBasedOnScrollMovement()
	{
		if (!this.CacheScrollView())
		{
			return;
		}
		this.mChildren.Clear();
		for (int i = 0; i < this.mTrans.childCount; i++)
		{
			Transform child = this.mTrans.GetChild(i);
			if (!this.hideInactive || child.gameObject.activeInHierarchy)
			{
				this.mChildren.Add(child);
			}
		}
	}

	public virtual void SortAlphabetically()
	{
		if (!this.CacheScrollView())
		{
			return;
		}
		this.mChildren.Clear();
		for (int i = 0; i < this.mTrans.childCount; i++)
		{
			Transform child = this.mTrans.GetChild(i);
			if (!this.hideInactive || child.gameObject.activeInHierarchy)
			{
				this.mChildren.Add(child);
			}
		}
		List<Transform> list = this.mChildren;
		if (UICustomWrapContent.cache0 == null)
		{
			UICustomWrapContent.cache0 = new Comparison<Transform>(UIGrid.SortByName);
		}
		list.Sort(UICustomWrapContent.cache0);
	}

	protected bool CacheScrollView()
	{
		this.mTrans = base.transform;
		this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		this.mScroll = this.mPanel.GetComponent<UIScrollView>();
		if (this.mScroll == null)
		{
			return false;
		}
		if (this.mScroll.movement == UIScrollView.Movement.Horizontal)
		{
			this.mHorizontal = true;
		}
		else
		{
			if (this.mScroll.movement != UIScrollView.Movement.Vertical)
			{
				return false;
			}
			this.mHorizontal = false;
		}
		return true;
	}

	protected virtual float GetItemsHeight()
	{
		float num = 0f;
		int i = 0;
		int count = this.mChildren.Count;
		while (i < count)
		{
			CommentTemplate component = this.mChildren[i].gameObject.GetComponent<CommentTemplate>();
			num += (float)component.GetHeigt();
			i++;
		}
		return num + (float)(this.MAX_REUSE * 10);
	}

	public virtual void WrapContent()
	{
		Vector3[] worldCorners = this.mPanel.worldCorners;
		for (int i = 0; i < 4; i++)
		{
			Vector3 vector = worldCorners[i];
			vector = this.mTrans.InverseTransformPoint(vector);
			worldCorners[i] = vector;
		}
		Vector3 vector2 = Vector3.Lerp(worldCorners[0], worldCorners[2], 0.5f);
		this.mScroll.restrictWithinPanel = false;
		UICustomWrapContent.index++;
		if (!this.mHorizontal)
		{
			float num = this.GetItemsHeight() * 0.5f;
			float num2 = num * 2f + 10f;
			Transform transform = this.mChildren[this.queue.First.Value];
			if (this.moveDirection - this.mScroll.panel.clipOffset.y < 0f)
			{
				transform = this.mChildren[this.queue.Last.Value];
				this.moveDirection = this.mScroll.panel.clipOffset.y;
				float num3 = transform.localPosition.y - vector2.y;
				if (num3 < -num)
				{
					if (this.currentHead <= this.minIndex)
					{
						this.currentHead = this.minIndex;
						this.currentTail = this.minIndex + this.MAX_REUSE;
						this.mScroll.restrictWithinPanel = true;
						return;
					}
					this.UpdateItem(transform, this.queue.Last.Value, --this.currentHead);
					base.StartCoroutine(this.ToTop(this.queue.Last.Value));
					this.currentTail--;
					this.queue.AddFirst(this.queue.Last.Value);
					this.queue.RemoveLast();
				}
			}
			else
			{
				this.moveDirection = this.mScroll.panel.clipOffset.y;
				float num4 = transform.localPosition.y - vector2.y;
				if (num4 > num)
				{
					if (this.currentTail >= this.maxIndex)
					{
						this.currentHead = this.maxIndex - this.MAX_REUSE;
						this.currentTail = this.maxIndex;
						this.mScroll.restrictWithinPanel = true;
						return;
					}
					this.UpdateItem(transform, this.queue.First.Value, ++this.currentTail);
					Vector3 localPosition = transform.localPosition;
					localPosition.y -= num2;
					transform.localPosition = localPosition;
					this.currentHead++;
					this.queue.AddLast(this.queue.First.Value);
					this.queue.RemoveFirst();
				}
			}
		}
	}

	protected void UpdateItem(Transform item, int index, int realIndex = 0)
	{
		CommentTemplate component = this.mChildren[index].gameObject.GetComponent<CommentTemplate>();
		component.EnsureInit(Solarmax.Singleton<CommentModel>.Get().comments[realIndex]);
	}

	private IEnumerator ToTop(int child)
	{
		yield return null;
		float extents = this.GetItemsHeight() * 0.5f;
		float ext2 = extents * 2f + 10f;
		Transform t = this.mChildren[child];
		Vector3 pos = t.localPosition;
		pos.y += ext2;
		t.localPosition = pos;
		yield break;
	}

	public bool hideInactive;

	public UICustomWrapContent.OnInitializeItem onInitializeItem;

	protected Transform mTrans;

	protected UIPanel mPanel;

	protected UIScrollView mScroll;

	protected bool mHorizontal;

	protected bool mFirstTime = true;

	protected List<Transform> mChildren = new List<Transform>();

	private int MAX_REUSE = 9;

	private int minIndex;

	private int maxIndex;

	private int currentHead;

	private int currentTail;

	private List<string> tests = new List<string>();

	private LinkedList<int> queue = new LinkedList<int>();

	private static int index;

	private float moveDirection;

	[CompilerGenerated]
	private static Comparison<Transform> cache0;

	public delegate void OnInitializeItem(GameObject go, int wrapIndex, int realIndex);
}
