using System;
using UnityEngine;

namespace ActScript
{
	public class ArtEffect : MonoBehaviour
	{
		private void OnEnable()
		{
			this._time = 0f;
		}

		private void Update()
		{
			this._time += Time.deltaTime;
			if (this._time > this.duration)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public Action<GameObject> onClose;

		public float duration;

		private float _time;
	}
}
