using System;
using UnityEngine;

public class AnnularParticles : MonoBehaviour
{
	private void Awake()
	{
		this.mTrans = base.transform;
		int num = UnityEngine.Random.Range(this.ParticleCountRange.x, this.ParticleCountRange.y);
		this.mParticles = new AnnularParticles.Particle[num];
		for (int i = 0; i < this.mParticles.Length; i++)
		{
			this.mParticles[i] = this.SpawnOneParticle();
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < this.mParticles.Length; i++)
		{
			this.mParticles[i].Destroy();
			this.mParticles[i] = null;
		}
		this.mParticles = null;
	}

	private void Update()
	{
		for (int i = 0; i < this.mParticles.Length; i++)
		{
			this.mParticles[i].Update();
		}
	}

	private AnnularParticles.Particle SpawnOneParticle()
	{
		if (null == this.ParticlePrefab)
		{
			return null;
		}
		GameObject go = UnityEngine.Object.Instantiate<GameObject>(this.ParticlePrefab, this.mTrans);
		float revolutionRadius = UnityEngine.Random.Range(this.RevolutionRadiusRange.x, this.RevolutionRadiusRange.y);
		float revolutionAngularVelocity = UnityEngine.Random.Range(this.RevolutionAngularVelocityRange.x, this.RevolutionAngularVelocityRange.y);
		return new AnnularParticles.Particle(go, revolutionRadius, revolutionAngularVelocity);
	}

	public GameObject ParticlePrefab;

	public Vector2Int ParticleCountRange = new Vector2Int(50, 60);

	public Vector2 RevolutionRadiusRange = new Vector2(4f, 6f);

	public Vector2 RevolutionAngularVelocityRange = new Vector2(5f, 10f);

	private AnnularParticles.Particle[] mParticles;

	private Transform mTrans;

	private class Particle
	{
		public Particle(GameObject go, float revolutionRadius, float revolutionAngularVelocity)
		{
			this.mGo = go;
			this.mTrans = go.transform;
			this.mRevolutionRadius = revolutionRadius;
			this.mRevolutionAngularVelocity = revolutionAngularVelocity;
			this.mCurAngle = UnityEngine.Random.Range(0f, 360f);
		}

		public void Update()
		{
			this.mCurAngle += Time.deltaTime * this.mRevolutionAngularVelocity;
			if (this.mCurAngle > 360f)
			{
				this.mCurAngle -= (float)((int)this.mCurAngle / 360 * 360);
			}
			float num = this.mCurAngle * 0.0027777778f;
			int num2 = MyCircle.Positions.Length;
			float num3 = num * (float)num2;
			int num4 = (int)num3;
			Vector3 a = MyCircle.Positions[num4 % num2];
			Vector3 b = MyCircle.Positions[(num4 + 1) % num2];
			Vector3 a2 = Vector3.Lerp(a, b, num3 - (float)num4);
			this.mTrans.localPosition = a2 * this.mRevolutionRadius;
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.mGo);
			this.mGo = null;
			this.mTrans = null;
		}

		private const float Inv360 = 0.0027777778f;

		private GameObject mGo;

		private Transform mTrans;

		private readonly float mRevolutionRadius;

		private readonly float mRevolutionAngularVelocity;

		private float mCurAngle;
	}
}
