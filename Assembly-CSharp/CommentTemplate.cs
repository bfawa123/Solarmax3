using System;
using UnityEngine;

public class CommentTemplate : MonoBehaviour
{
	public void EnsureInit(Comment comment)
	{
		if (comment == null)
		{
			return;
		}
		this.comment = comment;
		this.userName.text = comment.name;
		if (comment.icon != string.Empty)
		{
			this.icon.spriteName = comment.icon;
		}
		this.isLike = comment.isLike;
		this.likeBtn.spriteName = ((!this.isLike) ? "Btn_comment_like" : "Btn_comment_likeC");
		this.context.text = comment.comment;
		this.likeNumber.text = comment.likeCount.ToString();
	}

	public void OnLikeBtnClick()
	{
		if (this.doLike)
		{
			return;
		}
		this.doLike = true;
		if (this.isLike)
		{
			base.StartCoroutine(Solarmax.Singleton<CommentModel>.Get().UnLikeCommentRequest(this.comment.commentId, delegate(bool success)
			{
				this.doLike = false;
				if (success)
				{
					Debug.Log("-----取消点赞成功------");
					this.isLike = !this.isLike;
					this.likeBtn.spriteName = ((!this.isLike) ? "Btn_comment_like" : "Btn_comment_likeC");
					this.comment.likeCount--;
					this.likeNumber.text = this.comment.likeCount.ToString();
				}
				else
				{
					Debug.Log("-----取消点赞失败------");
				}
			}));
		}
		else
		{
			base.StartCoroutine(Solarmax.Singleton<CommentModel>.Get().LikeCommentRequest(this.comment.commentId, delegate(bool success)
			{
				this.doLike = false;
				if (success)
				{
					Debug.Log("-----点赞成功------");
					this.isLike = !this.isLike;
					this.likeBtn.spriteName = ((!this.isLike) ? "Btn_comment_like" : "Btn_comment_likeC");
					this.comment.likeCount++;
					this.likeNumber.text = this.comment.likeCount.ToString();
				}
				else
				{
					Debug.Log("-----点赞失败------");
				}
			}));
		}
	}

	public int GetHeigt()
	{
		return this.bg.height + 10;
	}

	public float GetOffsetY()
	{
		return this.bg.gameObject.transform.localPosition.y;
	}

	public void SetComment(string txt)
	{
		this.context.text = txt;
	}

	public int userId;

	public UISprite icon;

	public UILabel context;

	public UILabel likeNumber;

	public UISprite likeBtn;

	public UILabel userName;

	public UIWidget bg;

	private bool isLike;

	private int index;

	private Comment comment;

	private const string LIKE_ICON = "Btn_comment_likeC";

	private const string UNLIKE_ICON = "Btn_comment_like";

	private bool doLike;
}
