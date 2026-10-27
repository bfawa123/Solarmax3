using System;
using NetMessage;

public class MailModel : Solarmax.Singleton<MailModel>
{
	public void DeleteMail(int mailId)
	{
		Mail item = null;
		for (int i = 0; i < this.mailList.mail.Count; i++)
		{
			if (this.mailList.mail[i].mailId == mailId)
			{
				item = this.mailList.mail[i];
			}
		}
		this.mailList.mail.Remove(item);
	}

	public bool HasUnreadMails()
	{
		if (this.mailList == null || this.mailList.mail.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < this.mailList.mail.Count; i++)
		{
			if (!this.mailList.mail[i].Read)
			{
				return true;
			}
		}
		return false;
	}

	public SCMailList mailList;
}
