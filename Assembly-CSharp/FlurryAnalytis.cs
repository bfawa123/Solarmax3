using System;
using System.Collections.Generic;
using FlurrySDK;
using Solarmax;
using UnityEngine;

public class FlurryAnalytis : MonoSingleton<FlurryAnalytis>
{
	protected override void OnDestroy()
	{
		this.mEnabled = true;
		base.OnDestroy();
	}

	public void FlurryInit()
	{
		this.mEnabled = true;
		ChannelConfig channelConfig = UpgradeUtil.GetChannelConfig();
		string flurryApiKey = channelConfig.FlurryApiKey;
		if (string.IsNullOrEmpty(flurryApiKey))
		{
			return;
		}
		this.channel = channelConfig.ChannelId;
		new Flurry.Builder().WithCrashReporting(true).WithLogEnabled(true).WithLogLevel(Flurry.LogLevel.LogERROR).Build(flurryApiKey);
		this.mEnabled = true;
	}

	public void FlurryNoticeEffect()
	{
		this.LogEvent("NoticeEvent", new Dictionary<string, string>
		{
			{
				"Account",
				SystemInfo.deviceUniqueIdentifier.ToString()
			},
			{
				"channel",
				this.channel
			}
		});
	}

	public void FlurrySelectServer(string strRet)
	{
		this.LogEvent("SelectServer", new Dictionary<string, string>
		{
			{
				"Account",
				SystemInfo.deviceUniqueIdentifier.ToString()
			},
			{
				"SelectServer",
				strRet
			},
			{
				"channel",
				this.channel
			}
		});
	}

	public void FlurrySDKLoginStart(bool isVisitor)
	{
		this.LogEvent("SDKLoginStart", new Dictionary<string, string>
		{
			{
				"account",
				SystemInfo.deviceUniqueIdentifier.ToString()
			},
			{
				"visitor",
				isVisitor.ToString()
			},
			{
				"channel",
				this.channel
			}
		});
	}

	public void FlurryLoginSDKSuccess(string account, bool isVisitor)
	{
		this.LogEvent("SDKLoginSuccess", new Dictionary<string, string>
		{
			{
				"account",
				account
			},
			{
				"visitor",
				isVisitor.ToString()
			},
			{
				"channel",
				this.channel
			}
		});
	}

	public void FlurryLoginSDKFailed(string info)
	{
		this.LogEvent("SDKLoginFailed", new Dictionary<string, string>
		{
			{
				"channel",
				this.channel
			},
			{
				"info",
				info
			}
		});
	}

	public void FlurryRequestUserSuccess(string code)
	{
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		this.LogEvent("RequestUserSuccess", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"UserId",
				playerData.userId.ToString()
			},
			{
				"Version",
				UpgradeUtil.GetAppVersion()
			},
			{
				"info",
				code
			}
		});
	}

	public void FlurryCreateUserSuccess()
	{
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		this.LogEvent("RequestUserSuccess", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"UserId",
				playerData.userId.ToString()
			},
			{
				"Version",
				UpgradeUtil.GetAppVersion()
			}
		});
	}

	public void FlurryUserDataInit()
	{
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		this.LogEvent("UserDataInit", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"UserId",
				playerData.userId.ToString()
			},
			{
				"UserName",
				playerData.name
			},
			{
				"Money",
				playerData.money.ToString()
			},
			{
				"DeviceModel",
				SystemInfo.deviceModel.ToString()
			},
			{
				"DeviceUniqueIdentifier",
				SystemInfo.deviceUniqueIdentifier.ToString()
			},
			{
				"OperatingSystem",
				SystemInfo.operatingSystem
			},
			{
				"ProcessorType",
				SystemInfo.processorType
			},
			{
				"channel",
				this.channel
			}
		});
	}

	public void FlurryMoneyCostEvent(string costType, string szReason, string strValue)
	{
		this.LogEvent("MoneyCostEvent", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"CostType",
				costType
			},
			{
				"CostReason",
				szReason
			},
			{
				"CostValue",
				strValue
			},
			{
				"channel",
				this.channel
			}
		});
	}

	public void FlurryBattleEndEvent(string strLevel, string szRet, string score, string star, string destroy, string lost, string totalTime)
	{
		this.LogEvent("EndbattleEvent", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"Level",
				strLevel
			},
			{
				"Type",
				szRet
			},
			{
				"Score",
				score
			},
			{
				"Star",
				star
			},
			{
				"Destroy",
				destroy
			},
			{
				"Lost",
				lost
			},
			{
				"TotalTime",
				totalTime
			},
			{
				"channel",
				this.channel
			}
		});
	}

	public void FlurryPVPBattleEndEvent(string matchType, string strLevel, string score, string destroy, string lost, string totalTime)
	{
		this.LogEvent("EndbattlePvpEvent", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"MatchType",
				matchType
			},
			{
				"Level",
				strLevel
			},
			{
				"Score",
				score
			},
			{
				"Destroy",
				destroy
			},
			{
				"Lost",
				lost
			},
			{
				"TotalTime",
				totalTime
			},
			{
				"channel",
				this.channel
			}
		});
	}

	public void FlurryPVPBattleMatchEvent(string matchType, string strLevel, string matchState, string matchTime, string roomID)
	{
		this.LogEvent("MatchBattlePvpEvent", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"MatchType",
				matchType
			},
			{
				"Level",
				strLevel
			},
			{
				"MatchState",
				matchState
			},
			{
				"MatchTime",
				matchTime
			},
			{
				"RoomID",
				roomID
			},
			{
				"channel",
				this.channel
			}
		});
	}

	public void FlurryPaymentEvent(string productName, string productId, int num, string currency, double price, string transactionId)
	{
		Flurry.LogPayment(productName, productId, num, price, currency, transactionId, new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"channel",
				this.channel
			}
		});
	}

	public void FlurryAdsClickEvent()
	{
		this.LogEvent("AdsClick", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"Channel",
				this.channel
			}
		});
	}

	public void FlurryAdsEndEvent()
	{
		this.LogEvent("AdsEnd", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"Channel",
				this.channel
			}
		});
	}

	public void FlurryOpenADSEvent(string rewardCoin)
	{
		this.LogEvent("AdsOpen", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"Channel",
				this.channel
			},
			{
				"RewardCoin",
				rewardCoin
			}
		});
	}

	public void FlurryRewardCoinADSEvent(string rewardCoin)
	{
		this.LogEvent("AdsReward", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"Channel",
				this.channel
			},
			{
				"RewardCoin",
				rewardCoin
			}
		});
	}

	public void FlurryFinishTaskEvent(string taskId)
	{
		this.LogEvent("finishTask", new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"channel",
				this.channel
			},
			{
				"taskId",
				taskId
			}
		});
	}

	public void FlurryCreateOrderIDEvent(string productName, string productId, int num, string currency, double price, string transactionId)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"ProductId",
				productId
			},
			{
				"ProductName",
				productName
			},
			{
				"ProductNum",
				num.ToString()
			},
			{
				"Currency",
				currency
			},
			{
				"Price",
				price.ToString()
			},
			{
				"OrderId",
				transactionId
			},
			{
				"Channel",
				this.channel
			}
		};
		this.LogEvent("CreateOrder", parameters);
	}

	public void LogPayStart(string productName, string productId, int num, string currency, double price, string orderId, string notifyUrl)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"ProductId",
				productId
			},
			{
				"ProductName",
				productName
			},
			{
				"ProductNum",
				num.ToString()
			},
			{
				"Currency",
				currency
			},
			{
				"Price",
				price.ToString()
			},
			{
				"OrderId",
				orderId
			},
			{
				"NotifyUrl",
				notifyUrl
			},
			{
				"Channel",
				this.channel
			}
		};
		this.LogEvent("PayStart", parameters);
	}

	public void LogPaySuccess(string info)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"Info",
				info
			},
			{
				"Channel",
				this.channel
			}
		};
		this.LogEvent("PaySuccess", parameters);
	}

	public void LogPayFailed(string info)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"Info",
				info
			},
			{
				"Channel",
				this.channel
			}
		};
		this.LogEvent("PayFailed", parameters);
	}

	public void LogOrderComplete(string productName, string productId, int num, string currency, double price, string orderId)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"ProductId",
				productId
			},
			{
				"ProductName",
				productName
			},
			{
				"ProductNum",
				num.ToString()
			},
			{
				"Currency",
				currency
			},
			{
				"Price",
				price.ToString()
			},
			{
				"OrderId",
				orderId
			},
			{
				"Channel",
				this.channel
			}
		};
		this.LogEvent("OrderComplete", parameters);
	}

	public void LogCheckLookAds()
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			}
		};
		this.LogEvent("CheckLookAds", parameters);
	}

	public void LogRepairCheckLookAds()
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			}
		};
		this.LogEvent("RepairCheckLookAds", parameters);
	}

	public void LogLotteryLookAds()
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			}
		};
		this.LogEvent("LotteryLookAds", parameters);
	}

	public void LogTaskLookAds()
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			}
		};
		this.LogEvent("TaskLookAds", parameters);
	}

	public void LogTopBuyChapters(string param)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"param",
				param
			}
		};
		this.LogEvent("TopBuyChapters", parameters);
	}

	public void LogAcclSroceDelLookAds()
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			}
		};
		this.LogEvent("SroceLookAds", parameters);
	}

	public void LogResultMoneyDelLookAds()
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			}
		};
		this.LogEvent("ResultMoneyLookAds", parameters);
	}

	public void LogCommentLookAds()
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			}
		};
		this.LogEvent("CommentLookAds", parameters);
	}

	public void LogPveResultLookAds()
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"Account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			}
		};
		this.LogEvent("PveResultLookAds", parameters);
	}

	public void SetUserID(string userID)
	{
		if (!this.mEnabled)
		{
			return;
		}
		Flurry.SetUserId(userID);
	}

	public void EndFlurryAnalytis()
	{
		if (!this.mEnabled)
		{
			return;
		}
	}

	public void LogEvent(string eventName, Dictionary<string, string> parameters)
	{
		if (!this.mEnabled)
		{
			return;
		}
		Flurry.LogEvent(eventName, parameters);
	}

	public void LogEvent(string eventName)
	{
		if (!this.mEnabled)
		{
			return;
		}
		Dictionary<string, string> parameters = new Dictionary<string, string>
		{
			{
				"account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"channel",
				this.channel
			}
		};
		Flurry.LogEvent(eventName, parameters);
	}

	public void LogEvent(string eventName, string paramName, string paramValue)
	{
		if (!this.mEnabled)
		{
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{
				"account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"channel",
				this.channel
			}
		};
		if (!string.IsNullOrEmpty(paramName))
		{
			dictionary.Add(paramName, paramValue);
		}
		Flurry.LogEvent(eventName, dictionary);
	}

	public void LogEvent(string eventName, string paramName0, string paramValue0, string paramName1, string paramValue1)
	{
		if (!this.mEnabled)
		{
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{
				"account",
				Solarmax.Singleton<LocalAccountStorage>.Get().account
			},
			{
				"channel",
				this.channel
			}
		};
		if (!string.IsNullOrEmpty(paramName0))
		{
			dictionary.Add(paramName0, paramValue0);
		}
		if (!string.IsNullOrEmpty(paramName1))
		{
			dictionary.Add(paramName1, paramValue1);
		}
		Flurry.LogEvent(eventName, dictionary);
	}

	private string channel = string.Empty;

	private bool mEnabled;
}
