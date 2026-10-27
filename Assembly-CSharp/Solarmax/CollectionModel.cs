using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class CollectionModel : Solarmax.Singleton<CollectionModel>
	{
		public SkinConfig ChoosedConfig
		{
			get
			{
				return this.config;
			}
			set
			{
				if (this.config != value)
				{
					this.config = value;
					if (this.onAvatarChanged != null)
					{
						this.onAvatarChanged();
					}
				}
			}
		}

		public void EnsureInit()
		{
			this.bgConfigs = Solarmax.Singleton<SkinConfigProvider>.Get().GetAllBGData();
			this.avatarConfigs = Solarmax.Singleton<SkinConfigProvider>.Get().GetAllAvatarData();
			this.storeConfigs = Solarmax.Singleton<storeConfigProvider>.Get().dataList;
			this.cdList.Clear();
			for (int i = 0; i < this.storeConfigs.Count; i++)
			{
				if (this.storeConfigs[i].type == 0)
				{
					this.cdList.Add(this.storeConfigs[i]);
				}
			}
			this.PullImages(delegate(bool success)
			{
			});
			this.PullSceneBgs(delegate(bool success)
			{
			});
		}

		public void UnLock(int id)
		{
			for (int i = 0; i < this.avatarConfigs.Count; i++)
			{
				this.avatarConfigs[i].unlock = true;
			}
			for (int j = 0; j < this.bgConfigs.Count; j++)
			{
				this.bgConfigs[j].unlock = true;
			}
		}

		public bool IsUnLock(int id)
		{
			for (int i = 0; i < this.avatarConfigs.Count; i++)
			{
				if (this.avatarConfigs[i].id == id)
				{
					return this.avatarConfigs[i].unlock;
				}
			}
			for (int j = 0; j < this.bgConfigs.Count; j++)
			{
				if (this.bgConfigs[j].id == id)
				{
					return this.bgConfigs[j].unlock;
				}
			}
			return false;
		}

		public string GetRewardbyNum(int num)
		{
			if (num >= 0 && num < this.cdList.Count)
			{
                Solarmax.Singleton<LocalPlayer>.Get().showAdsRefreshTime = (float)this.cdList[num].cd;
				return this.cdList[num].GoldValue.ToString();
			}
			return string.Empty;
		}

		public void PullImages(CollectionModel.RequestCallback callback)
		{
		}

		public void PullSceneBgs(CollectionModel.RequestCallback callback)
		{
		}

		public bool NeedSyncAvatar()
		{
			return this.ChoosedConfig != null && !this.ChoosedConfig.skinImageName.EndsWith(this.currentAvatar);
		}

		public List<SkinConfig> bgConfigs = new List<SkinConfig>();

		public List<SkinConfig> avatarConfigs = new List<SkinConfig>();

		public List<StoreConfig> storeConfigs = new List<StoreConfig>();

		private List<StoreConfig> cdList = new List<StoreConfig>();

		public CollectionModel.OnAvatarChanged onAvatarChanged;

		private SkinConfig config;

		public string currentAvatar = string.Empty;

		public delegate void OnAvatarChanged();

		public delegate void RequestCallback(bool success);
	}
}
