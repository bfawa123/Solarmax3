using System;
using System.Runtime.CompilerServices;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening
{
	public static class DOTweenModuleUtils
	{
		public static void Init()
		{
			if (DOTweenModuleUtils._initialized)
			{
				return;
			}
			DOTweenModuleUtils._initialized = true;
			if (DOTweenModuleUtils.cache0 == null)
			{
				DOTweenModuleUtils.cache0 = new Action<PathOptions, Tween, Quaternion, Transform>(DOTweenModuleUtils.Physics.SetOrientationOnPath);
			}
			DOTweenExternalCommand.SetOrientationOnPath += DOTweenModuleUtils.cache0;
		}

		private static bool _initialized;

		[CompilerGenerated]
		private static Action<PathOptions, Tween, Quaternion, Transform> cache0;

		public static class Physics
		{
			public static void SetOrientationOnPath(PathOptions options, Tween t, Quaternion newRot, Transform trans)
			{
				trans.rotation = newRot;
			}

			public static bool HasRigidbody2D(Component target)
			{
				return false;
			}

			public static bool HasRigidbody(Component target)
			{
				return false;
			}

			public static TweenerCore<Vector3, Path, PathOptions> CreateDOTweenPathTween(MonoBehaviour target, bool tweenRigidbody, bool isLocal, Path path, float duration, PathMode pathMode)
			{
				return (!isLocal) ? target.transform.DOPath(path, duration, pathMode) : target.transform.DOLocalPath(path, duration, pathMode);
			}
		}
	}
}
