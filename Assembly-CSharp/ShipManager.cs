using System;
using System.Collections.Generic;
using Solarmax;

public class ShipManager : SimplePool<Ship>, Lifecycle2
{
	public ShipManager(SceneManager smer)
	{
		this.sceneManager = smer;
		this.mEntityPoll = new Queue<EntityShip>();
		this.flyMap = new List<Ship>[LocalPlayer.MaxTeamNum];
	}

	private List<Ship>[] flyMap { get; set; }

	public bool Init()
	{
		return true;
	}

	public bool PreloadEntity()
	{
		if (this.IsPreload)
		{
			return true;
		}
		for (int i = 0; i < 1000; i++)
		{
			EntityShip entityShip = new EntityShip("ship", true);
			if (entityShip != null)
			{
				entityShip.Init();
			}
			entityShip.GetGO().layer = LayerDefine.Invisible;
			this.mEntityPoll.Enqueue(entityShip);
		}
		this.IsPreload = true;
		return true;
	}

	public EntityShip GetFreeShipEnity()
	{
		EntityShip entityShip = null;
		if (this.mEntityPoll.Count > 0)
		{
			entityShip = this.mEntityPoll.Dequeue();
		}
		if (entityShip != null)
		{
			entityShip.silent = false;
		}
		return entityShip;
	}

	public void AddFreeShipenity(EntityShip pEnity)
	{
		this.mEntityPoll.Enqueue(pEnity);
	}

	public void Tick(int frame, float interval)
	{
		int num = this.mBusyObjects.Count - 1;
		for (int i = num; i >= 0; i--)
		{
			Ship ship = this.mBusyObjects[i];
			if (ship.IsActive())
			{
				ship.Tick(frame, interval);
			}
		}
	}

	public void UpdateRender(float interval)
	{
		int num = this.mBusyObjects.Count - 1;
		for (int i = num; i >= 0; i--)
		{
			Ship ship = this.mBusyObjects[i];
			if (ship.IsActive())
			{
				ship.UpdateRender(interval);
			}
		}
	}

	public void Destroy()
	{
		for (int i = this.mBusyObjects.Count - 1; i >= 0; i--)
		{
			Ship t = this.mBusyObjects[i];
			this.Recycle(t);
		}
		this.ReleaseFly();
		Ship[] array = this.mFreeObjects.ToArray();
		int j = 0;
		int num = array.Length;
		while (j < num)
		{
			Ship ship = array[j];
			this.mEntityPoll.Enqueue(ship.entity);
			ship.Destroy();
			j++;
		}
		int count = this.mEntityPoll.Count;
		base.Clear();
	}

	private void ReleaseFly()
	{
		for (int i = 0; i < this.flyMap.Length; i++)
		{
			this.flyMap[i] = null;
		}
	}

	public List<Ship> GetFlyShip(TEAM team)
	{
		List<Ship> list = this.flyMap[(int)team];
		if (list == null)
		{
			list = new List<Ship>();
			this.flyMap[(int)team] = list;
		}
		return list;
	}

	public void AddFlyShip(Ship ship)
	{
		List<Ship> list = this.flyMap[(int)ship.team];
		if (list == null)
		{
			list = new List<Ship>();
			this.flyMap[(int)ship.team] = list;
		}
		if (list.Contains(ship))
		{
			return;
		}
		list.Add(ship);
	}

	public void RemoveFlyShip(Ship ship)
	{
		List<Ship> list = this.flyMap[(int)ship.team];
		if (list == null)
		{
			list = new List<Ship>();
			this.flyMap[(int)ship.team] = list;
		}
		if (!list.Contains(ship))
		{
			return;
		}
		list.Remove(ship);
	}

	public void SilentMode(bool status)
	{
		for (int i = this.mBusyObjects.Count - 1; i >= 0; i--)
		{
			Ship ship = this.mBusyObjects[i];
			ship.entity.SilentMode(status);
		}
	}

	public void ChangeTargetNode(Node newNode)
	{
		for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
		{
			List<Ship> flyShip = this.GetFlyShip((TEAM)i);
			for (int j = 0; j < flyShip.Count; j++)
			{
				if (flyShip[j] != null && flyShip[j].entity != null && flyShip[j].entity.targetNode.tag == newNode.tag)
				{
					flyShip[j].entity.targetNode = newNode;
				}
			}
		}
		for (int k = this.mBusyObjects.Count - 1; k >= 0; k--)
		{
			Ship ship = this.mBusyObjects[k];
			if (ship != null && ship.entity != null && ship.entity.targetNode != null && ship.entity.targetNode.tag == newNode.tag)
			{
				ship.entity.targetNode = newNode;
			}
		}
	}

	public void DestroyAllShipToTargetNode(Node node)
	{
		for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
		{
			List<Ship> flyShip = this.GetFlyShip((TEAM)i);
			int j;
			do
			{
				for (j = 0; j < flyShip.Count; j++)
				{
					if (flyShip[j] != null && flyShip[j].entity != null && flyShip[j].entity.targetNode.tag == node.tag)
					{
						flyShip[j].Bomb(NodeType.None);
						break;
					}
				}
			}
			while (j != flyShip.Count);
		}
	}

	public SceneManager sceneManager;

	protected Queue<EntityShip> mEntityPoll;

	private bool IsPreload;

	private const int MAX_PREV_SHIPENTITY_NUM = 1000;
}
