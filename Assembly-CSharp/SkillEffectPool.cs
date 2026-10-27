using System;
using System.Collections.Generic;
using Solarmax;

public class SkillEffectPool : SimplePool<EffectNode>, Lifecycle2
{
	public SkillEffectPool()
	{
		this.nodeEffects = new Dictionary<string, Dictionary<string, EffectNode>>();
	}

	public virtual bool Init()
	{
		return true;
	}

	public virtual void Tick(int frame, float interval)
	{
		for (int i = this.mBusyObjects.Count - 1; i >= 0; i--)
		{
			EffectNode effectNode = this.mBusyObjects[i];
			effectNode.Tick(frame, interval);
		}
	}

	public virtual void Destroy()
	{
		for (int i = this.mBusyObjects.Count - 1; i >= 0; i--)
		{
			EffectNode t = this.mBusyObjects[i];
			this.Recycle(t);
		}
		EffectNode[] array = this.mFreeObjects.ToArray();
		int j = 0;
		int num = array.Length;
		while (j < num)
		{
			EffectNode effectNode = array[j];
			effectNode.Destroy();
			j++;
		}
		base.Clear();
		this.nodeEffects.Clear();
	}

	public override void Recycle(EffectNode t)
	{
		SkillEffect skillEffect = t as SkillEffect;
		if (skillEffect != null)
		{
			Dictionary<string, EffectNode> dictionary = null;
			if (this.nodeEffects.TryGetValue(skillEffect.hoodEntity.tag, out dictionary))
			{
				string text = string.Format("{0}:{1}", skillEffect.effectName, skillEffect.animationName);
				if (dictionary.ContainsKey(skillEffect.effectName))
				{
					dictionary.Remove(skillEffect.effectName);
				}
			}
		}
		base.Recycle(t);
	}

	public SkillEffect GetSameEffect(Node node, string effectName, string animationName)
	{
		SkillEffect skillEffect = null;
		Dictionary<string, EffectNode> dictionary = null;
		if (this.nodeEffects.TryGetValue(node.tag, out dictionary))
		{
			string key = string.Format("{0}:{1}", effectName, animationName);
			if (dictionary.ContainsKey(key))
			{
				EffectNode effectNode = dictionary[key];
				skillEffect = (effectNode as SkillEffect);
				if (skillEffect == null || !skillEffect.IsActive())
				{
					skillEffect = null;
				}
			}
		}
		return skillEffect;
	}

	public void AddReLifeEffect(SkillEffect se)
	{
		Dictionary<string, EffectNode> dictionary = null;
		if (!this.nodeEffects.TryGetValue(se.hoodEntity.tag, out dictionary))
		{
			dictionary = new Dictionary<string, EffectNode>();
			this.nodeEffects.Add(se.hoodEntity.tag, dictionary);
		}
		string key = string.Format("{0}:{1}", se.effectName, se.animationName);
		if (dictionary.ContainsKey(key))
		{
			dictionary[key] = se;
		}
		else
		{
			dictionary.Add(key, se);
		}
	}

	private Dictionary<string, Dictionary<string, EffectNode>> nodeEffects;
}
