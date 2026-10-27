using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class StartWindowChampionshipCell : MonoBehaviour
{
	public void SetInfo(LeagueInfo info)
	{
	}

	private void UpdateTime()
	{
	}

	public void OnSignUpClick()
	{
	}

	public void OnSignUpResult(ErrCode code)
	{
		if (code == ErrCode.EC_Ok)
		{
			this.signUpBtnLabel.text = LanguageDataProvider.GetValue(408);
			this.signUpBtn.enabled = false;
		}
	}

	public UILabel activityName;

	public UILabel battleType;

	public UILabel vsType;

	public UILabel startTime;

	public UILabel userNum;

	public UILabel totalTime;

	public UILabel activityType;

	public UILabel award;

	public UIButton signUpBtn;

	public UILabel signUpBtnLabel;

	private LeagueInfo data;

	private DateTime beginTime;

	private DateTime start;

	private DateTime end;
}
