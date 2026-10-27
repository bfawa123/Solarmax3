using System;
using Solarmax;
using UnityEngine;

public class CameraGestureController : MonoBehaviour
{
	public static bool IsControllingCamera { get; private set; }

	public float clampX = 6f;
	public float clampY = 4f;
	public float inertiaDamping = 0.92f;
	public float panSpeed = 1f;
	public float keyboardPanSpeed = 10f;

	private Camera cam;
	private Vector3 initialPosition;
	private Vector2 lastMidpoint;
	private Vector2 velocity;
	private bool wasControlling;
	private bool cameraModified;
	private bool wasGameActive;

	// 背景拉伸相关：移动摄像头时上下各多出clampY单位，背景需要相应拉大
	private Transform bgTransform;
	private Transform cgTransform;
	private Vector3 bgOriginalScale;
	private Vector3 cgOriginalScale;
	private bool bgScaled;

	private void Awake()
	{
		this.cam = base.GetComponent<Camera>();
		if (this.cam == null)
		{
			this.cam = Camera.main;
		}
		if (this.cam != null)
		{
			this.initialPosition = this.cam.transform.position;
		}
	}

	private void Update()
	{
		if (this.cam == null)
		{
			return;
		}

		bool isGameActive = this.IsGameActive();

		// 关卡结束或退出战斗时，恢复摄像机初始位置
		if (!isGameActive)
		{
			if (this.wasGameActive && this.cameraModified)
			{
				this.ResetCamera();
			}
			this.wasGameActive = false;
			return;
		}
		this.wasGameActive = true;

		if (!this.IsMoveCameraEnabled())
		{
			return;
		}

		// 开启移动摄像头时，拉大背景避免上下出现黑边
		if (!this.bgScaled)
		{
			this.ScaleBackgroundForCamera();
		}

		bool isTouching = false;

		// 手机双指拖动平移
		if (Input.touchCount >= 2)
		{
			isTouching = true;
			this.HandleTwoFingerPan();
		}
		// 鼠标右键拖拽平移
		else if (Input.GetMouseButton(1))
		{
			isTouching = true;
			this.HandleMousePan(1);
		}
		// 鼠标中键拖拽平移（模拟器兼容）
		else if (Input.GetMouseButton(2))
		{
			isTouching = true;
			this.HandleMousePan(2);
		}

		// 键盘方向键/WASD 平移（模拟器兼容）
		this.HandleKeyboardPan();

		// 惯性滑动
		if (!isTouching && this.wasControlling)
		{
			this.velocity *= this.inertiaDamping;
			if (this.velocity.sqrMagnitude > 0.01f)
			{
				Vector3 pos = base.transform.position;
				pos.x += this.velocity.x * Time.deltaTime;
				pos.y += this.velocity.y * Time.deltaTime;
				base.transform.position = this.ClampPosition(pos);
			}
			else
			{
				this.velocity = Vector2.zero;
			}
		}

		this.wasControlling = isTouching;
		IsControllingCamera = isTouching;
	}

	private bool IsGameActive()
	{
		if (Solarmax.Singleton<BattleSystem>.Instance == null)
		{
			return false;
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData == null)
		{
			return false;
		}
		return Solarmax.Singleton<BattleSystem>.Instance.battleData.gameState == GameState.Game;
	}

	private void ResetCamera()
	{
		this.cam.transform.position = this.initialPosition;
		this.velocity = Vector2.zero;
		this.cameraModified = false;
		this.wasControlling = false;
		IsControllingCamera = false;
		this.RestoreBackgroundScale();
	}

	private void ScaleBackgroundForCamera()
	{
		// 找到背景 BG 和 CG 节点
		if (this.bgTransform == null)
		{
			GameObject bgGo = GameObject.Find("Battle/Sky/BG");
			if (bgGo != null)
			{
				this.bgTransform = bgGo.transform;
				this.bgOriginalScale = this.bgTransform.localScale;
			}
		}
		if (this.cgTransform == null)
		{
			GameObject cgGo = GameObject.Find("Battle/Sky/CG");
			if (cgGo != null)
			{
				this.cgTransform = cgGo.transform;
				this.cgOriginalScale = this.cgTransform.localScale;
			}
		}

		// 等比例放大背景：分别计算XY方向需要的缩放比，取较大值统一缩放，避免黑边且不变形
		float aspect = this.cam.aspect;
		float orthoSize = this.cam.orthographicSize;
		float scaleX = (orthoSize * aspect + this.clampX) / (orthoSize * aspect);
		float scaleY = (orthoSize + this.clampY) / orthoSize;
		float uniformScale = Mathf.Max(scaleX, scaleY);

		if (this.bgTransform != null)
		{
			Vector3 s = this.bgOriginalScale;
			this.bgTransform.localScale = new Vector3(s.x * uniformScale, s.y * uniformScale, s.z);
		}
		if (this.cgTransform != null)
		{
			Vector3 s = this.cgOriginalScale;
			this.cgTransform.localScale = new Vector3(s.x * uniformScale, s.y * uniformScale, s.z);
		}

		this.bgScaled = true;
	}

	private void RestoreBackgroundScale()
	{
		if (!this.bgScaled)
		{
			return;
		}
		if (this.bgTransform != null)
		{
			this.bgTransform.localScale = this.bgOriginalScale;
		}
		if (this.cgTransform != null)
		{
			this.cgTransform.localScale = this.cgOriginalScale;
		}
		this.bgScaled = false;
	}

	private void HandleTwoFingerPan()
	{
		Touch t0 = Input.GetTouch(0);
		Touch t1 = Input.GetTouch(1);

		Vector2 midpoint = (t0.position + t1.position) * 0.5f;

		if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
		{
			this.lastMidpoint = midpoint;
			this.velocity = Vector2.zero;
			return;
		}

		if (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)
		{
			Vector2 worldDelta = this.cam.ScreenToWorldPoint(midpoint) - this.cam.ScreenToWorldPoint(this.lastMidpoint);
			Vector3 pos = base.transform.position;
			pos.x -= worldDelta.x;
			pos.y -= worldDelta.y;
			base.transform.position = this.ClampPosition(pos);
			this.velocity = new Vector2(-worldDelta.x / Time.deltaTime, -worldDelta.y / Time.deltaTime);
			this.lastMidpoint = midpoint;
			this.cameraModified = true;
		}
	}

	private void HandleMousePan(int button)
	{
		Vector2 mousePos = Input.mousePosition;

		if (Input.GetMouseButtonDown(button))
		{
			this.lastMidpoint = mousePos;
			this.velocity = Vector2.zero;
			return;
		}

		Vector2 worldDelta = this.cam.ScreenToWorldPoint(mousePos) - this.cam.ScreenToWorldPoint(this.lastMidpoint);
		Vector3 pos = base.transform.position;
		pos.x -= worldDelta.x * this.panSpeed;
		pos.y -= worldDelta.y * this.panSpeed;
		base.transform.position = this.ClampPosition(pos);
		this.velocity = new Vector2(-worldDelta.x / Time.deltaTime, -worldDelta.y / Time.deltaTime);
		this.lastMidpoint = mousePos;
		this.cameraModified = true;
	}

	private void HandleKeyboardPan()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		if (Mathf.Abs(h) < 0.001f && Mathf.Abs(v) < 0.001f)
		{
			return;
		}

		Vector3 pos = base.transform.position;
		pos.x += h * this.keyboardPanSpeed * Time.deltaTime;
		pos.y += v * this.keyboardPanSpeed * Time.deltaTime;
		base.transform.position = this.ClampPosition(pos);
		this.cameraModified = true;
	}

	private Vector3 ClampPosition(Vector3 pos)
	{
		pos.x = Mathf.Clamp(pos.x, this.initialPosition.x - this.clampX, this.initialPosition.x + this.clampX);
		pos.y = Mathf.Clamp(pos.y, this.initialPosition.y - this.clampY, this.initialPosition.y + this.clampY);
		return pos;
	}

	private bool IsMoveCameraEnabled()
	{
		string matchId = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
		if (string.IsNullOrEmpty(matchId))
		{
			return false;
		}
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(matchId);
		return data != null && data.IfMoveCamera();
	}
}
