using System;
using System.Collections.Generic;
using MiGameSDK;
using NetMessage;
using UnityEngine;

namespace Solarmax
{
	public class OrderManager : Solarmax.Singleton<OrderManager>, IDataHandler, Lifecycle
	{
		public bool Init()
		{
			this.padding.Clear();
            Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnVerityOrder, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
            Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnGenerateOrderID, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
			return true;
		}

		public void Tick(float interval)
		{
			if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() != ConnectionStatus.CONNECTED)
			{
				return;
			}
			this.syncVertiyTime += interval;
			if (this.syncVertiyTime > 5f)
			{
				int count = this.history.Count;
				if (count > 0)
				{
					string orderID = this.history[0];
					this.VerificationOrderID(orderID);
				}
				this.syncVertiyTime = 0f;
			}
		}

		public void Destroy()
		{
            Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalOrder();
			this.padding.Clear();
            Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(EventId.OnVerityOrder, this);
            Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(EventId.OnGenerateOrderID, this);
		}

		private void OnEventHandler(int eventId, object data, params object[] args)
		{
			if (eventId == 144)
			{
				ErrCode errCode = (ErrCode)args[0];
				string text = (string)args[1];
				if (!string.IsNullOrEmpty(text))
				{
					this.SyncOrder(text);
                    Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalOrder();
				}
			}
			else if (eventId == 143)
			{
				string orderID = (string)args[0];
				string productID = (string)args[1];
				int num = (int)args[2];
				this.BuyProduct(productID, num, orderID);
			}
		}

		public void AddPaddingOrder(string orderID)
		{
			if (string.IsNullOrEmpty(orderID))
			{
				return;
			}
			if (this.IsPadding(orderID))
			{
				return;
			}
			this.padding.Add(orderID);
		}

		public void AddhHstoryOrder(string orderID)
		{
			if (string.IsNullOrEmpty(orderID))
			{
				return;
			}
			if (this.IsHistory(orderID))
			{
				return;
			}
			this.history.Add(orderID);
		}

		private void BuyProduct(string productID, int num, string orderID)
		{
			if (string.IsNullOrEmpty(productID) || num <= 0)
			{
				return;
			}
			if (string.IsNullOrEmpty(orderID))
			{
				return;
			}
			Debug.Log("buy -> BuyProduct productID---" + productID);
			StoreConfig data = Solarmax.Singleton<storeConfigProvider>.Instance.GetData(productID);
			if (data != null)
			{
				float price = data.GetPrice();
				this.AddPaddingOrder(orderID);
				Debug.Log("buy -> BuyProduct orderID---" + orderID);
				MiPlatformSDK.pay(data.id, data.name, num, data.GetCurrencyDesc(), price, data.desc, orderID, "");
			}
		}

		public void VerificationOrder()
		{
			if (this.padding.Count <= 0)
			{
				return;
			}
			string orderID = this.padding[0];
			this.VerificationOrderID(orderID);
		}

		public void VerificationOrderID(string orderID)
		{
			if (string.IsNullOrEmpty(orderID))
			{
				return;
			}
            Solarmax.Singleton<NetSystem>.Instance.helper.StartVerityOrder(orderID);
		}

		private bool IsPadding(string orderID)
		{
			for (int i = 0; i < this.padding.Count; i++)
			{
				if (this.padding[i].Equals(orderID))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsHistory(string orderID)
		{
			for (int i = 0; i < this.history.Count; i++)
			{
				if (this.history[i].Equals(orderID))
				{
					return true;
				}
			}
			return false;
		}

		private void SyncOrder(string orderID)
		{
			Debug.Log("buy -> SyncOrder ---" + orderID);
			this.padding.Remove(orderID);
			this.history.Remove(orderID);
		}

		public List<string> padding = new List<string>();

		public List<string> history = new List<string>();

		private float syncVertiyTime;
	}
}
