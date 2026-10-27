using System;

public class RewardTipsModel
{
	public RewardTipsModel(int count, RewardType type, bool monthCard, int id = 0)
	{
		this.itemID = id;
		this.count = count;
		this.type = type;
		if (type != RewardType.Money)
		{
			if (type != RewardType.Prop)
			{
			}
		}
		else
		{
			this.reward = "icon_currency";
		}
		this.showMonthCardBtn = monthCard;
	}

	public const string MONEY_ICON = "icon_currency";

	public int itemID;

	public int count;

	public string reward;

	public RewardType type;

	public bool showMonthCardBtn;
}
