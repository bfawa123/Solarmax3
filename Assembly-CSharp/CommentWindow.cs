using System;
using System.Text.RegularExpressions;
using Solarmax;
using UnityEngine;

public class CommentWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		this.model = Solarmax.Singleton<CommentModel>.Get();
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.commentEvalution.EnsureInit();
		this.PullComents();
	}

	public override void OnHide()
	{
		this.scrollView.onMomentumMove = null;
		this.commentEvalution.EnsureDestroy();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommentWindow");
	}

	private bool IsEmptyOrError(string str)
	{
		Regex regex = new Regex("[a-zA-z]+://[^s]*");
		Regex regex2 = new Regex("[/\\\\^%&',;=?$\\x22]+");
		Regex regex3 = new Regex("[()（）a-zA-z0-9\\u4e00-\\u9fa5]+");
		return string.IsNullOrEmpty(str) || regex.IsMatch(str) || regex2.IsMatch(str) || !regex3.IsMatch(str);
	}

	public void OnSendClick()
	{
		if (this.IsEmptyOrError(this.inputText))
		{
			this.inputField.value = string.Empty;
			return;
		}
		base.StartCoroutine(this.model.PostCommentRequest(this.inputText, delegate(bool result)
		{
			if (result)
			{
				if (this.wrapContent.enabled && this.model.comments.Count > 10)
				{
					this.wrapContent.ResetMax(this.table.transform.childCount);
				}
				else
				{
					this.Add();
				}
			}
			else
			{
				Solarmax.Singleton<LoggerSystem>.Get().Error("---发送{0}评论失败！---", new object[]
				{
					this.model.levelId
				});
			}
		}));
		this.inputField.value = string.Empty;
	}

	public void OnInputValueChanged()
	{
		this.inputText = this.inputField.value;
	}

	private void PullComents()
	{
		base.StartCoroutine(this.model.PullCommentsRequest(delegate(bool result)
		{
			if (result)
			{
				if (this.model.comments.Count > 0 && this.model.comments.Count < 11)
				{
					this.UpdateUI();
				}
				else if (this.model.comments.Count > 10)
				{
					this.UpdateReuseUI();
				}
			}
			else
			{
				Solarmax.Singleton<LoggerSystem>.Get().Error("---拉取关卡{0}评论失败！---", new object[]
				{
					this.model.levelId
				});
			}
		}));
	}

	private void UpdateUI()
	{
		base.InvokeRepeating("Add", 0f, 0.01f);
		base.Invoke("CancelInvokeRepeated", 0.015f * (float)this.model.comments.Count);
	}

	private void UpdateReuseUI()
	{
		this.resueUI = true;
		base.InvokeRepeating("Add", 0f, 0.01f);
	}

	private void ClearTable()
	{
		foreach (Transform transform in this.table.GetChildList())
		{
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		this.table.repositionNow = true;
	}

	private void Add()
	{
		if (this.resueUI && this.invokeCount == 10)
		{
			this.resueUI = false;
			this.table.enabled = false;
			this.wrapContent.enabled = true;
			this.wrapContent.EnsureInit();
			this.CancelInvokeRepeated();
			return;
		}
		Comment comment = this.model.GetComment(++this.model.currentEndIndex);
		if (comment == null)
		{
			return;
		}
		GameObject gameObject = this.table.gameObject.AddChild(this.commentTempalte);
		CommentTemplate component = gameObject.GetComponent<CommentTemplate>();
		if (component != null)
		{
			component.EnsureInit(comment);
		}
		gameObject.SetActive(true);
		this.table.Reposition();
		this.invokeCount++;
	}

	private void CancelInvokeRepeated()
	{
		base.CancelInvoke("Add");
		this.table.repositionNow = true;
	}

	public GameObject closeBtn;

	public GameObject background;

	public GameObject sendBtn;

	public UITable table;

	public UICustomWrapContent wrapContent;

	public UIInput inputField;

	public UIScrollView scrollView;

	public GameObject commentTempalte;

	public CommentEvaluation commentEvalution;

	private string inputText;

	private CommentModel model;

	private Vector3 offset = Vector3.zero;

	private bool resueUI;

	private int invokeCount;
}
