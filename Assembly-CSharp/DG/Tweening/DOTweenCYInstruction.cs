using System;
using UnityEngine;

namespace DG.Tweening
{
	public static class DOTweenCYInstruction
	{
		public class WaitForCompletion : CustomYieldInstruction
		{
			public WaitForCompletion(Tween tween)
			{
				this.t = tween;
			}

			public override bool keepWaiting
			{
				get
				{
					return this.t.active && !this.t.IsComplete();
				}
			}

			private readonly Tween t;
		}

		public class WaitForRewind : CustomYieldInstruction
		{
			public WaitForRewind(Tween tween)
			{
				this.t = tween;
			}

			public override bool keepWaiting
			{
				get
				{
					return this.t.active && (!this.t.playedOnce || this.t.position * (float)(this.t.CompletedLoops() + 1) > 0f);
				}
			}

			private readonly Tween t;
		}

		public class WaitForKill : CustomYieldInstruction
		{
			public WaitForKill(Tween tween)
			{
				this.t = tween;
			}

			public override bool keepWaiting
			{
				get
				{
					return this.t.active;
				}
			}

			private readonly Tween t;
		}

		public class WaitForElapsedLoops : CustomYieldInstruction
		{
			public WaitForElapsedLoops(Tween tween, int elapsedLoops)
			{
				this.t = tween;
				this.elapsedLoops = elapsedLoops;
			}

			public override bool keepWaiting
			{
				get
				{
					return this.t.active && this.t.CompletedLoops() < this.elapsedLoops;
				}
			}

			private readonly Tween t;

			private readonly int elapsedLoops;
		}

		public class WaitForPosition : CustomYieldInstruction
		{
			public WaitForPosition(Tween tween, float position)
			{
				this.t = tween;
				this.position = position;
			}

			public override bool keepWaiting
			{
				get
				{
					return this.t.active && this.t.position * (float)(this.t.CompletedLoops() + 1) < this.position;
				}
			}

			private readonly Tween t;

			private readonly float position;
		}

		public class WaitForStart : CustomYieldInstruction
		{
			public WaitForStart(Tween tween)
			{
				this.t = tween;
			}

			public override bool keepWaiting
			{
				get
				{
					return this.t.active && !this.t.playedOnce;
				}
			}

			private readonly Tween t;
		}
	}
}
