using System;

public class GameTimeManager : Solarmax.Singleton<GameTimeManager>
{
	public void Init()
	{
		this.RegeditTimer(TimerType.T_Bomber, new Timer(0.2f));
		this.RegeditTimer(TimerType.T_Maker, new Timer(0.2f));
		this.RegeditTimer(TimerType.T_JumpCharge, new Timer(0.1f));
		this.RegeditTimer(TimerType.T_JumpStart, new Timer(0.1f));
		this.RegeditTimer(TimerType.T_JumpEnd, new Timer(0.1f));
		this.RegeditTimer(TimerType.T_Capture, new Timer(0.2f));
		this.RegeditTimer(TimerType.T_Laser, new Timer(0.1f));
		this.RegeditTimer(TimerType.T_WarpCharge, new Timer(0.1f));
		this.RegeditTimer(TimerType.T_Warp, new Timer(0.1f));
		this.RegeditTimer(TimerType.T_Defense, new Timer(0.1f));
		this.RegeditTimer(TimerType.T_Twist, new Timer(0.1f));
		this.RegeditTimer(TimerType.T_Clone, new Timer(0.1f));
		this.RegeditTimer(TimerType.T_Halo, new Timer(0.1f));
	}

	public void Release()
	{
	}

	private void RegeditTimer(TimerType type, Timer timer)
	{
		this.timerArray[(int)type] = timer;
	}

	public bool CheckTimer(TimerType type)
	{
		Timer timer = this.timerArray[(int)type];
		return timer != null && timer.CheckTimer();
	}

	private Timer[] timerArray = new Timer[13];
}
