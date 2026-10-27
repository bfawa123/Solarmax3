using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class MultiTouchScan : MonoBehaviour
{
	public MultiTouchScan.ControlScheme currentScheme
	{
		get
		{
			if (MultiTouchScan.mCurrentKey == KeyCode.None)
			{
				return MultiTouchScan.ControlScheme.Touch;
			}
			if (MultiTouchScan.mCurrentKey >= KeyCode.JoystickButton0)
			{
				return MultiTouchScan.ControlScheme.Controller;
			}
			return MultiTouchScan.ControlScheme.Mouse;
		}
		set
		{
			if (value == MultiTouchScan.ControlScheme.Mouse)
			{
				this.currentKey = KeyCode.Mouse0;
			}
			else if (value == MultiTouchScan.ControlScheme.Controller)
			{
				this.currentKey = KeyCode.JoystickButton0;
			}
			else if (value == MultiTouchScan.ControlScheme.Touch)
			{
				this.currentKey = KeyCode.None;
			}
			else
			{
				this.currentKey = KeyCode.Alpha0;
			}
		}
	}

	public KeyCode currentKey
	{
		get
		{
			return MultiTouchScan.mCurrentKey;
		}
		set
		{
			if (MultiTouchScan.mCurrentKey != value)
			{
				MultiTouchScan.mCurrentKey = value;
			}
		}
	}

	public Ray currentRay
	{
		get
		{
			return (!(this.currentCamera != null) || this.currentTouch == null) ? default(Ray) : this.currentCamera.ScreenPointToRay(this.currentTouch.pos);
		}
	}

	public void Raycast(MultiTouchScan.MouseOrTouch touch)
	{
		if (!this.Raycast(touch.pos))
		{
			MultiTouchScan.mRayHitObject = this.fallThrough;
		}
		touch.last = touch.current;
		touch.current = MultiTouchScan.mRayHitObject;
	}

	public bool Raycast(Vector3 inPos)
	{
		Vector3 vector = this.currentCamera.ScreenToViewportPoint(inPos);
		if (float.IsNaN(vector.x) || float.IsNaN(vector.y))
		{
			return false;
		}
		if (vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
		{
			return false;
		}
		Ray ray = this.currentCamera.ScreenPointToRay(inPos);
		int layerMask = this.currentCamera.cullingMask & this.eventReceiverMask;
		float maxDistance = (this.rangeDistance <= 0f) ? (this.currentCamera.farClipPlane - this.currentCamera.nearClipPlane) : this.rangeDistance;
		if (Physics.Raycast(ray, out this.lastHit, maxDistance, layerMask))
		{
			MultiTouchScan.mRayHitObject = this.lastHit.collider.gameObject;
			return true;
		}
		return false;
	}

	public void Notify(GameObject go, string funcName, object obj)
	{
		if (MultiTouchScan.mNotifying > 10)
		{
			return;
		}
		if (this.currentScheme == MultiTouchScan.ControlScheme.Controller && UIPopupList.isOpen && UIPopupList.current.source == go && UIPopupList.isOpen)
		{
			go = UIPopupList.current.gameObject;
		}
		if (go && go.activeInHierarchy)
		{
			MultiTouchScan.mNotifying++;
			go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			MultiTouchScan.mNotifying--;
		}
	}

	public MultiTouchScan.MouseOrTouch GetMouse(int button)
	{
		return MultiTouchScan.mMouse[button];
	}

	public MultiTouchScan.MouseOrTouch GetTouch(int id, bool createIfMissing = false)
	{
		if (id < 0)
		{
			return this.GetMouse(-id - 1);
		}
		int i = 0;
		int count = MultiTouchScan.mTouchIDs.Count;
		while (i < count)
		{
			if (MultiTouchScan.mTouchIDs[i] == id)
			{
				return this.activeTouches[i];
			}
			i++;
		}
		if (createIfMissing)
		{
			MultiTouchScan.MouseOrTouch mouseOrTouch = new MultiTouchScan.MouseOrTouch();
			mouseOrTouch.pressTime = RealTime.time;
			mouseOrTouch.touchBegan = true;
			this.activeTouches.Add(mouseOrTouch);
			MultiTouchScan.mTouchIDs.Add(id);
			return mouseOrTouch;
		}
		return null;
	}

	public void RemoveTouch(int id)
	{
		int i = 0;
		int count = MultiTouchScan.mTouchIDs.Count;
		while (i < count)
		{
			if (MultiTouchScan.mTouchIDs[i] == id)
			{
				MultiTouchScan.mTouchIDs.RemoveAt(i);
				this.activeTouches.RemoveAt(i);
				return;
			}
			i++;
		}
	}

	private void Awake()
	{
		this.mainCamera = Camera.main;
		this.currentCamera = this.mainCamera;
		this.currentScheme = MultiTouchScan.ControlScheme.Touch;
		MultiTouchScan.mMouse[0].pos = Input.mousePosition;
		for (int i = 1; i < 3; i++)
		{
			MultiTouchScan.mMouse[i].pos = MultiTouchScan.mMouse[0].pos;
			MultiTouchScan.mMouse[i].lastPos = MultiTouchScan.mMouse[0].pos;
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Start()
	{
		if (Application.isPlaying && this.fallThrough == null)
		{
			this.fallThrough = base.gameObject;
		}
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameState == GameState.Game)
		{
			this.ProcessEvents();
		}
	}

	private void ProcessEvents()
	{
		if (CameraGestureController.IsControllingCamera)
		{
			return;
		}
		this.ProcessTouches();
		this.currentTouchID = -100;
	}

	public void ProcessMouse()
	{
		bool flag = false;
		for (int i = 0; i < 3; i++)
		{
			if (Input.GetMouseButtonDown(i))
			{
				this.currentKey = KeyCode.Mouse0 + i;
				flag = true;
			}
			else if (Input.GetMouseButton(i))
			{
				this.currentKey = KeyCode.Mouse0 + i;
				flag = true;
			}
		}
		if (this.currentScheme == MultiTouchScan.ControlScheme.Touch)
		{
			return;
		}
		this.currentTouch = MultiTouchScan.mMouse[0];
		Vector2 vector = Input.mousePosition;
		if (this.currentTouch.ignoreDelta == 0)
		{
			this.currentTouch.delta = vector - this.currentTouch.pos;
		}
		else
		{
			this.currentTouch.ignoreDelta--;
			this.currentTouch.delta.x = 0f;
			this.currentTouch.delta.y = 0f;
		}
		float sqrMagnitude = this.currentTouch.delta.sqrMagnitude;
		this.currentTouch.pos = vector;
		bool flag2 = false;
		if (this.currentScheme != MultiTouchScan.ControlScheme.Mouse)
		{
			if (sqrMagnitude < 0.001f)
			{
				return;
			}
			this.currentKey = KeyCode.Mouse0;
			flag2 = true;
		}
		else if (sqrMagnitude > 0.001f)
		{
			flag2 = true;
		}
		for (int j = 1; j < 3; j++)
		{
			MultiTouchScan.mMouse[j].pos = this.currentTouch.pos;
			MultiTouchScan.mMouse[j].delta = this.currentTouch.delta;
		}
		if (flag || flag2 || this.mNextRaycast < RealTime.time)
		{
			this.mNextRaycast = RealTime.time + 0.02f;
			this.Raycast(this.currentTouch);
			for (int k = 0; k < 3; k++)
			{
				MultiTouchScan.mMouse[k].current = this.currentTouch.current;
			}
		}
		bool flag3 = this.currentTouch.last != this.currentTouch.current;
		this.currentTouchID = -1;
		if (flag3)
		{
			this.currentKey = KeyCode.Mouse0;
		}
		if (flag2)
		{
			this.currentTouch = null;
		}
		for (int l = 0; l < 3; l++)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(l);
			bool mouseButtonUp = Input.GetMouseButtonUp(l);
			if (mouseButtonDown || mouseButtonUp)
			{
				this.currentKey = KeyCode.Mouse0 + l;
			}
			this.currentTouch = MultiTouchScan.mMouse[l];
			this.currentTouchID = -1 - l;
			this.currentKey = KeyCode.Mouse0 + l;
			if (mouseButtonDown)
			{
				this.currentTouch.pressedCam = this.currentCamera;
				this.currentTouch.pressTime = RealTime.time;
			}
			else if (this.currentTouch.pressed != null)
			{
				this.currentCamera = this.currentTouch.pressedCam;
			}
			this.ProcessTouch(mouseButtonDown, mouseButtonUp);
		}
		if (!flag && flag3)
		{
			this.currentTouch = MultiTouchScan.mMouse[0];
			this.currentTouchID = -1;
			this.currentKey = KeyCode.Mouse0;
		}
		this.currentTouch = null;
		MultiTouchScan.mMouse[0].last = MultiTouchScan.mMouse[0].current;
		for (int m = 1; m < 3; m++)
		{
			MultiTouchScan.mMouse[m].last = MultiTouchScan.mMouse[0].last;
		}
	}

	public void ProcessTouches()
	{
		int touchCount = Input.touchCount;
		for (int i = 0; i < touchCount; i++)
		{
			UnityEngine.Touch touch = Input.GetTouch(i);
			TouchPhase phase = touch.phase;
			int fingerId = touch.fingerId;
			Vector2 position = touch.position;
			int tapCount = touch.tapCount;
			this.currentTouchID = ((!this.allowMultiTouch) ? 1 : fingerId);
			this.currentTouch = this.GetTouch(this.currentTouchID, true);
			bool flag = phase == TouchPhase.Began || this.currentTouch.touchBegan;
			bool flag2 = phase == TouchPhase.Canceled || phase == TouchPhase.Ended;
			this.currentTouch.delta = position - this.currentTouch.pos;
			this.currentTouch.pos = position;
			this.currentKey = KeyCode.None;
			this.Raycast(this.currentTouch);
			if (flag)
			{
				this.currentTouch.pressedCam = this.currentCamera;
			}
			else if (this.currentTouch.pressed != null)
			{
				this.currentCamera = this.currentTouch.pressedCam;
			}
			if (tapCount > 1)
			{
				this.currentTouch.clickTime = RealTime.time;
			}
			this.ProcessTouch(flag, flag2);
			if (flag2)
			{
				this.RemoveTouch(this.currentTouchID);
			}
			this.currentTouch.touchBegan = false;
			this.currentTouch.last = null;
			this.currentTouch = null;
			if (!this.allowMultiTouch)
			{
				break;
			}
		}
		if (touchCount == 0)
		{
			if (MultiTouchScan.mUsingTouchEvents)
			{
				MultiTouchScan.mUsingTouchEvents = false;
				return;
			}
			this.ProcessMouse();
		}
		else
		{
			MultiTouchScan.mUsingTouchEvents = true;
		}
	}

	private void ProcessPress(bool pressed, float click, float drag)
	{
		if (pressed)
		{
			this.currentTouch.pressStarted = true;
			if (this.onPress != null && this.currentTouch.pressed)
			{
				this.onPress(this.currentTouch.pressed, false);
			}
			this.Notify(this.currentTouch.pressed, "OnPress", false);
			this.currentTouch.pressed = this.currentTouch.current;
			this.currentTouch.dragged = this.currentTouch.current;
			this.currentTouch.clickNotification = MultiTouchScan.ClickNotification.BasedOnDelta;
			this.currentTouch.totalDelta = Vector2.zero;
			this.currentTouch.dragStarted = false;
			if (this.onPress != null && this.currentTouch.pressed)
			{
				this.onPress(this.currentTouch.pressed, true);
			}
			this.Notify(this.currentTouch.pressed, "OnPress", true);
		}
		else if (this.currentTouch.pressed != null && (this.currentTouch.delta.sqrMagnitude != 0f || this.currentTouch.current != this.currentTouch.last))
		{
			this.currentTouch.totalDelta += this.currentTouch.delta;
			float sqrMagnitude = this.currentTouch.totalDelta.sqrMagnitude;
			bool flag = false;
			if (!this.currentTouch.dragStarted && this.currentTouch.last != this.currentTouch.current)
			{
				this.currentTouch.dragStarted = true;
				this.currentTouch.delta = this.currentTouch.totalDelta;
				if (this.onDragStart != null)
				{
					this.onDragStart(this.currentTouch.dragged);
				}
				this.Notify(this.currentTouch.dragged, "OnDragStart", null);
				if (this.onDragOver != null)
				{
					this.onDragOver(this.currentTouch.last, this.currentTouch.dragged);
				}
				this.Notify(this.currentTouch.last, "OnDragOver", this.currentTouch.dragged);
			}
			else if (!this.currentTouch.dragStarted && drag < sqrMagnitude)
			{
				flag = true;
				this.currentTouch.dragStarted = true;
				this.currentTouch.delta = this.currentTouch.totalDelta;
			}
			if (this.currentTouch.dragStarted)
			{
				bool flag2 = this.currentTouch.clickNotification == MultiTouchScan.ClickNotification.None;
				if (flag)
				{
					if (this.onDragStart != null)
					{
						this.onDragStart(this.currentTouch.dragged);
					}
					this.Notify(this.currentTouch.dragged, "OnDragStart", null);
					if (this.onDragOver != null)
					{
						this.onDragOver(this.currentTouch.last, this.currentTouch.dragged);
					}
					this.Notify(this.currentTouch.current, "OnDragOver", this.currentTouch.dragged);
				}
				else if (this.currentTouch.last != this.currentTouch.current)
				{
					if (this.onDragOut != null)
					{
						this.onDragOut(this.currentTouch.last, this.currentTouch.dragged);
					}
					this.Notify(this.currentTouch.last, "OnDragOut", this.currentTouch.dragged);
					if (this.onDragOver != null)
					{
						this.onDragOver(this.currentTouch.last, this.currentTouch.dragged);
					}
					this.Notify(this.currentTouch.current, "OnDragOver", this.currentTouch.dragged);
				}
				if (this.onDrag != null)
				{
					this.onDrag(this.currentTouch.dragged, this.currentTouch.delta);
				}
				this.Notify(this.currentTouch.dragged, "OnDrag", this.currentTouch.pos);
				this.currentTouch.last = this.currentTouch.current;
				if (flag2)
				{
					this.currentTouch.clickNotification = MultiTouchScan.ClickNotification.None;
				}
				else if (this.currentTouch.clickNotification == MultiTouchScan.ClickNotification.BasedOnDelta && click < sqrMagnitude)
				{
					this.currentTouch.clickNotification = MultiTouchScan.ClickNotification.None;
				}
			}
		}
	}

	private void ProcessRelease(bool isMouse, float drag)
	{
		if (this.currentTouch == null)
		{
			return;
		}
		this.currentTouch.pressStarted = false;
		if (this.currentTouch.pressed != null)
		{
			if (this.onPress != null)
			{
				this.onPress(this.currentTouch.pressed, false);
			}
			this.Notify(this.currentTouch.pressed, "OnPress", false);
			if (this.currentTouch.dragStarted)
			{
				if (this.onDragOut != null)
				{
					this.onDragOut(this.currentTouch.last, this.currentTouch.dragged);
				}
				this.Notify(this.currentTouch.last, "OnDragOut", this.currentTouch.dragged);
				if (this.onDragEnd != null)
				{
					this.onDragEnd(this.currentTouch.dragged);
				}
				this.Notify(this.currentTouch.dragged, "OnDragEnd", null);
			}
			if (this.currentTouch.dragStarted)
			{
				if (this.onDrop != null)
				{
					this.onDrop(this.currentTouch.current, this.currentTouch.dragged);
				}
				this.Notify(this.currentTouch.current, "OnDrop", this.currentTouch.dragged);
			}
		}
		this.currentTouch.dragStarted = false;
		this.currentTouch.pressed = null;
		this.currentTouch.dragged = null;
	}

	private bool HasCollider(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		Collider component = go.GetComponent<Collider>();
		if (component != null)
		{
			return component.enabled;
		}
		Collider2D component2 = go.GetComponent<Collider2D>();
		return component2 != null && component2.enabled;
	}

	public void ProcessTouch(bool pressed, bool released)
	{
		bool flag = this.currentScheme == MultiTouchScan.ControlScheme.Mouse;
		float num = (!flag) ? this.touchDragThreshold : this.mouseDragThreshold;
		float num2 = (!flag) ? this.touchClickThreshold : this.mouseClickThreshold;
		num *= num;
		num2 *= num2;
		if (this.currentTouch.pressed != null)
		{
			if (released)
			{
				this.ProcessRelease(flag, num);
			}
			this.ProcessPress(pressed, num2, num);
		}
		else if (flag || pressed || released)
		{
			this.ProcessPress(pressed, num2, num);
			if (released)
			{
				this.ProcessRelease(flag, num);
			}
		}
	}

	public LayerMask eventReceiverMask = -1;

	private bool allowMultiTouch = true;

	private float mouseDragThreshold = 4f;

	private float mouseClickThreshold = 10f;

	private float touchDragThreshold = 40f;

	private float touchClickThreshold = 40f;

	public float rangeDistance = -1f;

	private RaycastHit lastHit;

	private Camera currentCamera;

	private int currentTouchID = -100;

	private static KeyCode mCurrentKey = KeyCode.Alpha0;

	private MultiTouchScan.MouseOrTouch currentTouch;

	private GameObject fallThrough;

	public MultiTouchScan.BoolDelegate onPress;

	public MultiTouchScan.VectorDelegate onDrag;

	public MultiTouchScan.VoidDelegate onDragStart;

	public MultiTouchScan.ObjectDelegate onDragOver;

	public MultiTouchScan.ObjectDelegate onDragOut;

	public MultiTouchScan.VoidDelegate onDragEnd;

	public MultiTouchScan.ObjectDelegate onDrop;

	private static MultiTouchScan.MouseOrTouch[] mMouse = new MultiTouchScan.MouseOrTouch[]
	{
		new MultiTouchScan.MouseOrTouch(),
		new MultiTouchScan.MouseOrTouch(),
		new MultiTouchScan.MouseOrTouch()
	};

	private List<MultiTouchScan.MouseOrTouch> activeTouches = new List<MultiTouchScan.MouseOrTouch>();

	private static List<int> mTouchIDs = new List<int>();

	private float mNextRaycast;

	private static GameObject mRayHitObject;

	private Camera mainCamera;

	private static int mNotifying = 0;

	private static bool mUsingTouchEvents = true;

	public enum ControlScheme
	{
		Mouse,
		Touch,
		Controller
	}

	public enum ClickNotification
	{
		None,
		Always,
		BasedOnDelta
	}

	public class MouseOrTouch
	{
		public float deltaTime
		{
			get
			{
				return RealTime.time - this.pressTime;
			}
		}

		public KeyCode key;

		public Vector2 pos;

		public Vector2 lastPos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject last;

		public GameObject current;

		public GameObject pressed;

		public GameObject dragged;

		public float pressTime;

		public float clickTime;

		public MultiTouchScan.ClickNotification clickNotification = MultiTouchScan.ClickNotification.Always;

		public bool touchBegan = true;

		public bool pressStarted;

		public bool dragStarted;

		public int ignoreDelta;
	}

	public delegate void MoveDelegate(Vector2 delta);

	public delegate void VoidDelegate(GameObject go);

	public delegate void BoolDelegate(GameObject go, bool state);

	public delegate void FloatDelegate(GameObject go, float delta);

	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	public delegate void ObjectDelegate(GameObject go, GameObject obj);

	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

	public class Touch
	{
		public int fingerId;

		public TouchPhase phase;

		public Vector2 position;

		public int tapCount;
	}
}
