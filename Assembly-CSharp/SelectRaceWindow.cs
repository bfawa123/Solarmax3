using System;
using Solarmax;
using UnityEngine;

public class SelectRaceWindow : BaseWindow
{
	public override void OnShow()
	{
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	private void OnIconClicked(GameObject obj)
	{
		this.collider.enabled = true;
		this.NameLabel.gameObject.SetActive(true);
		this.PicSelect.SetActive(true);
		string name = obj.transform.parent.name;
		string state = string.Empty;
		this.spriteBtn.color = Color.white;
		if (name != null)
		{
			if (!(name == "race1"))
			{
				if (!(name == "race2"))
				{
					if (!(name == "race3"))
					{
						if (!(name == "race4"))
						{
							if (name == "race5")
							{
								this.NameLabel.text = this.raceName[4];
								state = "SelectRaceWindow_race5";
							}
						}
						else
						{
							this.NameLabel.text = this.raceName[3];
							state = "SelectRaceWindow_race4";
						}
					}
					else
					{
						this.NameLabel.text = this.raceName[2];
						state = "SelectRaceWindow_race3";
					}
				}
				else
				{
					this.NameLabel.text = this.raceName[1];
					state = "SelectRaceWindow_race2";
				}
			}
			else
			{
				this.NameLabel.text = this.raceName[0];
				state = "SelectRaceWindow_race1";
			}
		}
		this.NameLabel.transform.parent = obj.transform.parent;
		Vector3 localPosition = this.NameLabel.transform.localPosition;
		localPosition.x = 0f;
		localPosition.y = -235f;
		this.NameLabel.transform.localPosition = localPosition;
		this.NameLabel.transform.localScale = Vector3.one;
		this.PicSelect.transform.parent = obj.transform.parent;
		this.PicSelect.transform.localPosition = Vector3.zero;
		this.PicSelect.transform.localScale = Vector3.one;
		this.PlayAnimation(state);
	}

	public void OnStartGame()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.MatchGame2();
	}

	private void PlayAnimation(string state)
	{
		this.aniPlayer.clipName = state;
		this.aniPlayer.resetOnPlay = true;
		this.aniPlayer.Play(true, false, 1f, false);
	}

	public UITexture[] PicList;

	public UIEventListener[] iconList;

	public GameObject PicSelect;

	public UIPlayAnimation aniPlayer;

	public BoxCollider collider;

	public UISprite spriteBtn;

	public UILabel NameLabel;

	private string[] raceName = new string[]
	{
		"归零者",
		"歌者",
		"瓦肯人",
		"博格人",
		"克林贡人",
		"罗姆兰人",
		"可汗"
	};
}
