using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class CommentModel : Solarmax.Singleton<CommentModel>
{
	public void EnsureInit(string levelId)
	{
		if (levelId == this.levelId)
		{
			return;
		}
		this.EnsureClear();
		this.canEvaluate = false;
		this.levelId = levelId;
		this.levelScore = PlayerPrefs.GetInt(string.Format("LEVEL_SCORE_{0}", levelId), -1);
		this.averageScore = PlayerPrefs.GetFloat(string.Format("LEVEL_AVERAGE_SCORE_{0}", levelId), 0f);
		if (this.levelScore == -1)
		{
			this.levelScore = 0;
			this.canEvaluate = true;
		}
	}

	private void EnsureClear()
	{
		this.levelId = string.Empty;
		this.currentEndIndex = -1;
		this.comments.Clear();
	}

	public string PacketJson(string json)
	{
		json = "{\"data\":{data}}".Replace("{data}", json);
		return json;
	}

	public IEnumerator PullCommentsRequest(CommentModel.RequestResult result)
	{
		string url = string.Format("{0}/comments?tag=level:{1}&openId={2}", "http://120.92.133.64/comment/v1/app/n3n1rcpq", this.levelId, Solarmax.Singleton<LocalAccountStorage>.Get().account);
		using (UnityWebRequest www = UnityWebRequest.Get(url))
		{
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.LogError("PullCommentsRequest request failed: " + www.error);
				result(false);
			}
			else if (www.responseCode == 200L)
			{
				string json = this.PacketJson(www.downloadHandler.text);
				JsonListData jsonListData = JsonUtility.FromJson<JsonListData>(json);
				this.currentEndIndex = -1;
				this.comments.Clear();
				foreach (PullCommentJson model in jsonListData.data)
				{
					this.JsonModel2Comment(model);
				}
				result(true);
			}
			else
			{
				result(false);
			}
		}
		yield break;
	}

	public IEnumerator PostCommentRequest(string comment, CommentModel.RequestResult result)
	{
		PostCommentJson json = new PostCommentJson
		{
			openId = Solarmax.Singleton<LocalAccountStorage>.Get().account,
			content = comment
		};
		json.tags = new string[3];
		json.tags[0] = string.Format("level:{0}", this.levelId);
		json.tags[1] = string.Format("icon:{0}", Solarmax.Singleton<LocalPlayer>.Get().playerData.icon);
		json.tags[2] = string.Format("name:{0}", Solarmax.Singleton<LocalPlayer>.Get().playerData.name);
		using (UnityWebRequest www = new UnityWebRequest(string.Format("{0}/comments", "http://120.92.133.64/comment/v1/app/n3n1rcpq"), "POST"))
		{
			byte[] bodyRaw = Json.EnCodeBytes(json);
			www.uploadHandler = new UploadHandlerRaw(bodyRaw);
			www.downloadHandler = new DownloadHandlerBuffer();
			www.SetRequestHeader("Content-Type", "application/json");
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.LogError("PostCommentRequest request failed: " + www.error);
				result(false);
			}
			else
			{
				Debug.Log("PostCommentRequest request complete: " + www.downloadHandler.text);
				if (www.responseCode == 201L)
				{
					if (this.comments.Count < 100)
					{
						Comment comment2 = new Comment();
						comment2.isLike = false;
						comment2.commentId = 0;
						comment2.icon = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
						comment2.name = Solarmax.Singleton<LocalPlayer>.Get().playerData.name;
						comment2.comment = comment;
						comment2.likeCount = 0;
						this.comments.Add(comment2);
					}
					this.currentEndIndex = this.comments.Count - 2;
					result(true);
				}
			}
		}
		yield break;
	}

	public IEnumerator LikeCommentRequest(int commentId, CommentModel.RequestResult result)
	{
		string url = string.Format("{0}/comments/{1}/likes?openId={2}", "http://120.92.133.64/comment/v1/app/n3n1rcpq", commentId, Solarmax.Singleton<LocalAccountStorage>.Get().account);
		using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
		{
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log("LikeCommentRequest request failed: " + www.error);
				result(false);
			}
			else if (www.responseCode == 200L)
			{
				result(true);
			}
		}
		yield break;
	}

	public IEnumerator UnLikeCommentRequest(int commentId, CommentModel.RequestResult result)
	{
		string url = string.Format("{0}/comments/{1}/likes?openId={2}", "http://120.92.133.64/comment/v1/app/n3n1rcpq", commentId, Solarmax.Singleton<LocalAccountStorage>.Get().account);
		using (UnityWebRequest www = UnityWebRequest.Delete(url))
		{
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log("LikeCommentRequest request failed: " + www.error);
				result(false);
			}
			else if (www.responseCode == 200L)
			{
				result(true);
			}
		}
		yield break;
	}

	public IEnumerator PullLevelScore(CommentModel.RequestResult result)
	{
		string url = string.Format("{0}/scores/{1}/openId/{2}", "http://120.92.133.64/comment/v1/app/n3n1rcpq", this.levelId, Solarmax.Singleton<LocalAccountStorage>.Get().account);
		using (UnityWebRequest www = UnityWebRequest.Get(url))
		{
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.LogError("PullLevelScore request failed: " + www.error);
				result(false);
			}
			else
			{
				Debug.Log("PullCommentsRequest request complete: " + www.downloadHandler.text);
				if (www.responseCode == 200L)
				{
					LevelScoreJson levelScoreJson = Json.DeCode<LevelScoreJson>(Encoding.UTF8.GetBytes(www.downloadHandler.text));
					this.levelScore = levelScoreJson.score;
					result(true);
				}
				else
				{
					result(false);
				}
			}
		}
		yield break;
	}

	public IEnumerator PullAverageLevelScore(CommentModel.RequestResult result)
	{
		string url = string.Format("{0}/scores/{1}", "http://120.92.133.64/comment/v1/app/n3n1rcpq", this.levelId);
		using (UnityWebRequest www = UnityWebRequest.Get(url))
		{
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.LogError("PullAverageLevelScore request failed: " + www.error);
				result(false);
			}
			else
			{
				Debug.Log("PullCommentsRequest request complete: " + www.downloadHandler.text);
				if (www.responseCode == 200L)
				{
					AverageScoreJson averageScoreJson = Json.DeCode<AverageScoreJson>(Encoding.UTF8.GetBytes(www.downloadHandler.text));
					this.averageScore = averageScoreJson.average;
					result(true);
				}
				else
				{
					result(false);
				}
			}
		}
		yield break;
	}

	public IEnumerator SendLevelEvalution(int score, CommentModel.RequestResult result)
	{
		string url = string.Format("{0}/{1}/openIds/{2}?score={3}", new object[]
		{
			"http://120.92.133.64/comment/v1/app/n3n1rcpq",
			this.levelId,
            Solarmax.Singleton<LocalAccountStorage>.Get().account,
			score
		});
		using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
		{
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log("SendLevelEvalution request failed: " + www.error);
				this.canEvaluate = true;
				result(false);
			}
			else if (www.responseCode == 200L)
			{
				this.canEvaluate = false;
				result(true);
			}
		}
		yield break;
	}

	public Comment GetComment(int index)
	{
		if (index < 0 || index >= this.comments.Count)
		{
			return null;
		}
		return this.comments[index];
	}

	public void JsonModel2Comment(PullCommentJson model)
	{
		if (model == null)
		{
			return;
		}
		Comment comment = new Comment();
		comment.isLike = model.liked;
		comment.commentId = model.id;
		foreach (string text in model.tags)
		{
			string[] array = text.Split(new char[]
			{
				':'
			});
			if (array[0] == "icon")
			{
				comment.icon = array[1];
			}
			if (array[0] == "name")
			{
				comment.name = array[1];
			}
		}
		comment.comment = model.content;
		comment.likeCount = model.totalLikes;
		this.comments.Add(comment);
	}

	public string levelId;

	public List<Comment> comments = new List<Comment>();

	public int currentEndIndex;

	public int levelScore;

	public float averageScore;

	public bool canEvaluate;

	private const string URL = "http://120.92.133.64/comment/v1/app/n3n1rcpq";

	private const string LEVEL_SCORE = "LEVEL_SCORE_{0}";

	private const string LEVEL_AVERAGE_SCORE = "LEVEL_AVERAGE_SCORE_{0}";

	private static int index;

	public delegate void RequestResult(bool success);
}
