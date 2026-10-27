using System;
using Solarmax;
using UnityEngine;

public class AirShip : MonoBehaviour
{
	private void Awake()
	{
		this.mTrans = base.transform;
		this.mOriPos = this.mTrans.localPosition;
		this.mMaterial = base.GetComponent<SpriteRenderer>().material;
		if (null != this.SprTrail)
		{
			this.mTransTrail = this.SprTrail.transform;
			this.mTrailOriScale = this.mTransTrail.localScale;
		}
		this.SetIdle();
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		AirShip.State state = this.mState;
		if (state != AirShip.State.Idle)
		{
			if (state == AirShip.State.Fly)
			{
				this.UpdateFly();
			}
		}
		else
		{
			this.UpdateIdle();
		}
	}

	private void UpdateIdle()
	{
		this.mStateTime += Time.deltaTime;
		BGManager.Inst.Scroll(-this.IdleMoveSpeed * Time.deltaTime);
		Vector3 localPosition = this.mTrans.localPosition;
		localPosition.y = this.mOriPos.y + this.ShakeRange * Mathf.Sin(this.ShakeFrequency * this.mStateTime * 3.1415927f * 2f);
		this.mTrans.localPosition = localPosition;
	}

	private void UpdateFly()
	{
		if (!this.mCurFlyTrack.AutoUpdate)
		{
			return;
		}
		this.mStateTime += Time.deltaTime;
		if (this.mStateTime > this.mCurFlyTrack.Duration)
		{
			this.mStateTime = this.mCurFlyTrack.Duration;
		}
		float nt = (this.mCurFlyTrack.Duration <= 0.01f) ? 0f : (this.mStateTime / this.mCurFlyTrack.Duration);
		this.UpdateFlyImpl(nt);
	}

	private void SetTrailScale(float s)
	{
		if (null == this.mTransTrail)
		{
			return;
		}
		float x = Mathf.Clamp(this.mTrailOriScale.x * s, 1f, 100f);
		this.mTransTrail.localScale = new Vector3(x, this.mTrailOriScale.y, this.mTrailOriScale.z);
	}

	private void SetIdle()
	{
		this.mStateTime = 0f;
		this.mState = AirShip.State.Idle;
		this.mCurFlyTrack = null;
		this.SetTrailScale(this.IdleTrailScale);
		Shader.DisableKeyword("MOTIONVECTOR_ON");
	}

	public void Fly(int trackIndex, float showWindowDelayTime, ShowWindowParams showWindowParams)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("AirShip  Fly", new object[0]);
		if (trackIndex < 0 || trackIndex >= this.FlyTracks.Length)
		{
			Debug.LogErrorFormat("Invalid fly track index of {0}", new object[]
			{
				trackIndex
			});
			return;
		}
		this.mCurFlyTrack = this.FlyTracks[trackIndex];
		Solarmax.Singleton<AudioManger>.Get().PlayEffect(this.FlyTracks[trackIndex].audioName);
		this.mState = AirShip.State.Fly;
		this.mStateTime = 0f;
		this.mPreCurveValue = 0f;
		this.mWindowShowed = showWindowParams.IsNone;
		this.mShowWindowDelayTime = showWindowDelayTime;
		this.mShowWindowParams = showWindowParams;
		Shader.EnableKeyword("MOTIONVECTOR_ON");
		this.mMaterial.SetFloat(BGManager.SPIdMotionVectorLength, 0f);
		BGManager.Inst.SetMotionVectorLength(0f);
		this.ApplyFly(0f, 0f, 0f);
	}

	private void ApplyFly(float t, float dt, float s)
	{
		if (Mathf.Abs(this.mCurFlyTrack.BackgroundScrollDist) > 0.01f)
		{
			BGManager.Inst.Scroll(dt * this.mCurFlyTrack.BackgroundScrollDist);
			BGManager.Inst.SetMotionVectorLength(s * this.BGMotionVectorLengthCoef);
		}
		if (Math.Abs(this.mCurFlyTrack.ShipDstPos - this.mCurFlyTrack.ShipSrcPos) > 0.01f)
		{
			Vector3 localPosition = this.mTrans.localPosition;
			localPosition.x = Mathf.Lerp(this.mCurFlyTrack.ShipSrcPos, this.mCurFlyTrack.ShipDstPos, t);
			this.mTrans.localPosition = localPosition;
			this.mMaterial.SetFloat(BGManager.SPIdMotionVectorLength, s * this.ShipMotionVectorLengthCoef);
		}
		if (!this.mWindowShowed && !this.mShowWindowParams.IsNone && t > this.mShowWindowDelayTime)
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow(this.mShowWindowParams);
			this.mWindowShowed = true;
		}
	}

	public void UpdateFly(float nt)
	{
		if (this.mState != AirShip.State.Fly)
		{
			return;
		}
		if (this.mCurFlyTrack == null)
		{
			return;
		}
		if (this.mCurFlyTrack.AutoUpdate)
		{
			return;
		}
		this.UpdateFlyImpl(nt);
	}

	private void UpdateFlyImpl(float nt)
	{
		if (nt > 0.999f)
		{
			this.ApplyFly(1f, 0f, 0f);
			this.SetIdle();
			return;
		}
		float num = this.mCurFlyTrack.Curve.Evaluate(nt);
		float num2 = num - this.mPreCurveValue;
		float num3 = Mathf.Clamp(0.001f, Time.deltaTime, 0.3f);
		float num4 = num2 / num3;
		this.ApplyFly(num, num2, num4);
		float trailScale = Mathf.Max(num4 * this.TrailScaleCoef, this.IdleTrailScale);
		this.SetTrailScale(trailScale);
		this.mPreCurveValue = num;
	}

	public AirShip.FlyTrack[] FlyTracks;

	public SpriteRenderer SprTrail;

	public float IdleMoveSpeed = 0.1f;

	public float ShakeRange;

	public float ShakeFrequency;

	public float TrailScaleCoef = 1f;

	public float IdleTrailScale = 5f;

	public float BGMotionVectorLengthCoef = 0.1f;

	public float ShipMotionVectorLengthCoef = 0.1f;

	private Transform mTrans;

	private Transform mTransTrail;

	private Material mMaterial;

	private AirShip.State mState;

	private float mStateTime;

	private Vector3 mTrailOriScale;

	private float mPreCurveValue;

	private Vector3 mOriPos;

	private AirShip.FlyTrack mCurFlyTrack;

	private bool mWindowShowed;

	private float mShowWindowDelayTime;

	private ShowWindowParams mShowWindowParams;

	private enum State
	{
		Idle,
		Fly,
		Num
	}

	[Serializable]
	public class FlyTrack
	{
		public AnimationCurve Curve;

		public float Duration = 1f;

		public float ShipSrcPos;

		public float ShipDstPos;

		public float BackgroundScrollDist;

		public bool AutoUpdate;

		public string audioName;
	}
}
