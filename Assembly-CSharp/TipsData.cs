using System;

public class TipsData
{
	public TipsData()
	{
		this.mType = Tips.TipsType.Default;
		this.mMessage = string.Empty;
		this.mSeconds = 0f;
		this.mStatus = TipsData.Status._NEW_;
	}

	public TipsData(Tips.TipsType type, string message, float time)
	{
		this.mType = type;
		this.mMessage = message;
		this.mSeconds = time;
		this.mStatus = TipsData.Status._NEW_;
	}

	public void Init(Tips.TipsType type, string message, float time)
	{
		this.mType = type;
		this.mMessage = message;
		this.mSeconds = time;
		this.mStatus = TipsData.Status._NEW_;
	}

	public bool IsImportantNotice()
	{
		return this.mType == Tips.TipsType.FlowLeft;
	}

	public bool IsNew()
	{
		return this.mStatus == TipsData.Status._NEW_;
	}

	public bool IsShow()
	{
		return this.mStatus == TipsData.Status._SHOW_;
	}

	public bool IsEnd()
	{
		return this.mStatus == TipsData.Status._END_ || this.mStatus == TipsData.Status._TIMEOUT_;
	}

	public bool UnUse()
	{
		return this.mStatus == TipsData.Status._RECYCLE_;
	}

	public void OnShow()
	{
		this.mStatus = TipsData.Status._SHOW_;
	}

	public void OnTimeout()
	{
		this.mStatus = TipsData.Status._TIMEOUT_;
	}

	public void OnEnd()
	{
		this.mStatus = TipsData.Status._END_;
	}

	public void OnRecycle()
	{
		this.mStatus = TipsData.Status._RECYCLE_;
	}

	public Tips.TipsType mType;

	public string mMessage;

	public float mSeconds;

	private TipsData.Status mStatus;

	public enum Status
	{
		_NEW_,
		_SHOW_,
		_TIMEOUT_,
		_END_,
		_RECYCLE_
	}
}
