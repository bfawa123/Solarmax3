using System;
using System.Collections.Generic;
using UnityEngine;

public class VerityReplayManager
{
	public static VerityReplayManager Get()
	{
		if (VerityReplayManager.Instance == null)
		{
			VerityReplayManager.Instance = new VerityReplayManager();
		}
		return VerityReplayManager.Instance;
	}

	public bool Init()
	{
		this.mapVerityFrames.Clear();
		return true;
	}

	public void BeginVerity()
	{
		this.curFrame = 0;
		this.curVerityFrames.Clear();
	}

	public void OnVerityFrame(TEAM t, int nFrame, Node form, Node to, int num)
	{
		VerityFrame verityFrame = this.mapVerityFrames[this.curFrame];
		if (verityFrame != null && (verityFrame.team != t || verityFrame.frame != nFrame || !verityFrame.from.Equals(form.tag) || !verityFrame.to.Equals(to.tag) || verityFrame.num != num))
		{
			Debug.LogFormat("   play frame = {0} team:{1} sender:{2}, target:{3} num:{4}", new object[]
			{
				verityFrame.frame,
				verityFrame.team,
				verityFrame.from,
				verityFrame.to,
				verityFrame.num
			});
			Debug.LogFormat(" replay frame = {0} team:{1} sender:{2}, target:{3} num:{4}", new object[]
			{
				nFrame,
				t,
				form.tag,
				to.tag,
				num
			});
		}
		this.AddReplayFrame(t, nFrame, form, to, num);
		this.curFrame++;
	}

	public void Destroy()
	{
		this.mapVerityFrames.Clear();
	}

	public void AddFrame(TEAM t, int nFrame, Node form, Node to, int num)
	{
		VerityFrame verityFrame = new VerityFrame();
		if (verityFrame != null)
		{
			verityFrame.team = t;
			verityFrame.frame = nFrame;
			verityFrame.from = form.tag;
			verityFrame.to = to.tag;
			verityFrame.num = num;
			this.mapVerityFrames.Add(verityFrame);
		}
	}

	public void AddReplayFrame(TEAM t, int nFrame, Node form, Node to, int num)
	{
		VerityFrame verityFrame = new VerityFrame();
		if (verityFrame != null)
		{
			verityFrame.team = t;
			verityFrame.frame = nFrame;
			verityFrame.from = form.tag;
			verityFrame.to = to.tag;
			verityFrame.num = num;
			this.curVerityFrames.Add(verityFrame);
		}
	}

	public static VerityReplayManager Instance;

	private List<VerityFrame> mapVerityFrames = new List<VerityFrame>();

	private List<VerityFrame> curVerityFrames = new List<VerityFrame>();

	private int curFrame;
}
