#region Math Variables
#if UNIGINE_DOUBLE
using Scalar = System.Double;
using Vec2 = Unigine.dvec2;
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using System.Collections.Generic;
using Unigine;


[Component(PropertyGuid = "7abd3d1dd6399bd8498bed57e699aacada031ba1")]
public class FirstPersonController : Component
{
	#region Editor parameters

	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Move forward key")]
	private Input.KEY forwardKey = Input.KEY.W;

	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Move backward key")]
	private Input.KEY backwardKey = Input.KEY.S;

	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Move right key")]
	private Input.KEY rightKey = Input.KEY.D;

	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Move left key")]
	private Input.KEY leftKey = Input.KEY.A;

	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Run mode activation key")]
	private Input.KEY runKey = Input.KEY.SHIFT;

	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Jump key")]
	private Input.KEY jumpKey = Input.KEY.SPACE;

	[ShowInEditor]
	[Parameter(Group = "Input", Tooltip = "Crouch mode activation key")]
	private Input.KEY crouchKey = Input.KEY.CTRL;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Input", Tooltip = "Mouse sensitivity multiplier")]
	private float mouseSensitivity = 0.4f;

	public enum GamepadStickSide
	{
		LEFT = 0,
		RIGHT
	}

	[ShowInEditor]
	[Parameter(Group = "Gamepad Input", Tooltip = "Move stick")]
	private GamepadStickSide moveStick = GamepadStickSide.LEFT;

	[ShowInEditor]
	[Parameter(Group = "Gamepad Input", Tooltip = "Camera stick")]
	private GamepadStickSide cameraStick = GamepadStickSide.RIGHT;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Gamepad Input", Tooltip = "Camera stick sensitivity")]
	private float cameraStickSensitivity = 0.8f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Gamepad Input", Tooltip = "Sticks dead zone")]
	private float sticksDeadZone = 0.3f;

	[ShowInEditor]
	[Parameter(Group = "Gamepad Input", Tooltip = "Run mode activation button")]
	private InputGamePad.BUTTON runButton = InputGamePad.BUTTON.SHOULDER_RIGHT;

	[ShowInEditor]
	[Parameter(Group = "Gamepad Input", Tooltip = "Jump button")]
	private InputGamePad.BUTTON jumpButton = InputGamePad.BUTTON.A;

	[ShowInEditor]
	[Parameter(Group = "Gamepad Input", Tooltip = "Crouch mode activation button")]
	private InputGamePad.BUTTON crouchButton = InputGamePad.BUTTON.SHOULDER_LEFT;

	[ShowInEditor]
	[Parameter(Group = "Body", Tooltip = "You can use current node to customize the body.\nIt should have a dummy body and a capsule shape assigned.\n" +
		"In case of incorrect settings of the specified custom body or shape, a default body and capsule shape shall be created instead.")]
	private bool useObjectBody = false;

	[ShowInEditor]
	[ParameterSlider(Group = "Body", Tooltip = "Radius of the capsule shape")]
	[ParameterCondition(nameof(useObjectBody), 0)]
	private float capsuleRadius = 0.3f;

	[ShowInEditor]
	[ParameterSlider(Group = "Body", Tooltip = "Height of the capsule shape (cylindrical part only)")]
	[ParameterCondition(nameof(useObjectBody), 0)]
	private float capsuleHeight = 1.15f;

	[ShowInEditor]
	[ParameterMask(Group = "Body", MaskType = ParameterMaskAttribute.TYPE.PHYSICS_INTERSECTION, Tooltip = "Mask used for selective detections of physics intersections (between physical objects with bodies and collider shapes, or ray intersections with collider geometry)")]
	[ParameterCondition(nameof(useObjectBody), 0)]
	private int physicsIntersectionMask = 1;

	[ShowInEditor]
	[ParameterMask(Group = "Body", MaskType = ParameterMaskAttribute.TYPE.COLLISION, Tooltip = "Mask used for selective collision detection")]
	[ParameterCondition(nameof(useObjectBody), 0)]
	private int collisionMask = 1;

	[ShowInEditor]
	[ParameterMask(Group = "Body", MaskType = ParameterMaskAttribute.TYPE.COLLISION, Tooltip = "Mask used for exclusion collision detection")]
	[ParameterCondition(nameof(useObjectBody), 0)]
	private int exclusionMask = 0;

	public enum CameraMode
	{
		NONE = 0,
		CREATE_AUTOMATICALLY,
		USE_EXTERNAL
	}

	[ShowInEditor]
	[Parameter(Group = "Camera", Tooltip = "Camera mode")]
	private CameraMode cameraMode = CameraMode.CREATE_AUTOMATICALLY;

	[ShowInEditor]
	[Parameter(Group = "Camera", Tooltip = "Camera")]
	[ParameterCondition(nameof(cameraMode), (int)CameraMode.USE_EXTERNAL)]
	private PlayerDummy camera = null;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 180.0f, Group = "Camera", Tooltip = "Fov")]
	[ParameterCondition(nameof(cameraMode), (int)CameraMode.CREATE_AUTOMATICALLY)]
	private float fov = 60.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Camera", Tooltip = "Near clipping")]
	[ParameterCondition(nameof(cameraMode), (int)CameraMode.CREATE_AUTOMATICALLY)]
	private float nearClipping = 0.01f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Camera", Tooltip = "Far clipping")]
	[ParameterCondition(nameof(cameraMode), (int)CameraMode.CREATE_AUTOMATICALLY)]
	private float farClipping = 1000.0f;

	[ShowInEditor]
	[Parameter(Group = "Camera", Tooltip = "Camera position offset")]
	[ParameterCondition(nameof(cameraMode), (int)CameraMode.CREATE_AUTOMATICALLY)]
	private vec3 cameraPositionOffset = new vec3(0.0f, 0.0f, 1.65f);

	[ShowInEditor]
	[ParameterSlider(Min = -89.9f, Max = 89.9f, Group = "Camera", Tooltip = "The minimum vertical angle of the camera (look down)")]
	[ParameterCondition(nameof(cameraMode), (int)CameraMode.CREATE_AUTOMATICALLY, (int)CameraMode.USE_EXTERNAL)]
	private float minVerticalAngle = -89.9f;

	[ShowInEditor]
	[ParameterSlider(Min = -89.9f, Max = 89.9f, Group = "Camera", Tooltip = "The maximum vertical angle of the camera (look up)")]
	[ParameterCondition(nameof(cameraMode), (int)CameraMode.CREATE_AUTOMATICALLY, (int)CameraMode.USE_EXTERNAL)]
	private float maxVerticalAngle = 89.9f;

	[ShowInEditor]
	[Parameter(Group = "Movement", Tooltip = "Use jump")]
	private bool useJump = true;

	[ShowInEditor]
	[Parameter(Group = "Movement", Tooltip = "Use crouch")]
	private bool useCrouch = true;

	[ShowInEditor]
	[Parameter(Group = "Movement", Tooltip = "Use run")]
	private bool useRun = true;

	[ShowInEditor]
	[Parameter(Group = "Movement", Tooltip = "Use run default")]
	[ParameterCondition(nameof(useRun), 1)]
	private bool useRunDefault = false;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Crouching speed of the player")]
	[ParameterCondition(nameof(useCrouch), 1)]
	private float crouchSpeed = 1.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Walking speed of the player")]
	private float walkSpeed = 3.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Running speed of the player")]
	[ParameterCondition(nameof(useRun), 1)]
	private float runSpeed = 6.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Horizontal ground acceleration of the player")]
	private float groundAcceleration = 25.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Horizontal air acceleration of the player")]
	private float airAcceleration = 10.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Horizontal ground damping of the player's speed")]
	private float groundDamping = 25.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Horizontal air damping of the player's speed")]
	private float airDamping = 0.05f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Jumping power of the player")]
	[ParameterCondition(nameof(useJump), 1)]
	private float jumpPower = 6.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Jumping crouch power of the player")]
	[ParameterCondition(nameof(useJump), 1)]
	[ParameterCondition(nameof(useCrouch), 1)]
	private float crouchJumpPower = 3.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Crouching height of the player")]
	[ParameterCondition(nameof(useCrouch), 1)]
	private Scalar crouchHeight = 1.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Movement", Tooltip = "Time the player transition to the crouching state and vice versa")]
	[ParameterCondition(nameof(useCrouch), 1)]
	private float crouchTransitionTime = 0.3f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 90.0f, Group = "Movement", Tooltip = "Maximum surface angle to which the player is considered to be standing on the ground")]
	private float maxGroundAngle = 60.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 90.0f, Group = "Movement", Tooltip = "Ьaximum surface angle to which the player is considered to be touching the ceiling")]
	private float maxCeilingAngle = 60.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 0.1f, Group = "Movement", Tooltip = "Check move ray offset")]
	private float checkMoveRayOffset = 0.01f;

	[ShowInEditor]
	[ParameterMask(Group = "Movement", MaskType = ParameterMaskAttribute.TYPE.INTERSECTION, Tooltip = "Check move mask")]
	private int checkMoveMask = 1;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 90.0f, Group = "Movement", Tooltip = "Wall stop sliding angle")]
	private float wallStopSlidingAngle = 15.0f;

	[ShowInEditor]
	[Parameter(Group = "Auto Stepping", Tooltip = "Toggles automatic walking up and down certain obstacles (stairs, for example)")]
	private bool useAutoStepping = true;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Auto Stepping", Tooltip = "Minimum height for automatic walk up.\nEnables avoiding false positives on flat surfaces.")]
	[ParameterCondition(nameof(useAutoStepping), 1)]
	private float minStepHeight = 0.05f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Auto Stepping", Tooltip = "Maximum height for automatic walk up")]
	[ParameterCondition(nameof(useAutoStepping), 1)]
	private float maxStepHeight = 0.3f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Auto Stepping", Tooltip = "Max stair angle")]
	[ParameterCondition(nameof(useAutoStepping), 1)]
	private float maxStairAngle = 30.0f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Group = "Auto Stepping", Tooltip = "Check stair ray offset")]
	[ParameterCondition(nameof(useAutoStepping), 1)]
	private float checkStairRayOffset = 0.1f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 90.0f, Group = "Auto Stepping", Tooltip = "Stairs detection angle")]
	[ParameterCondition(nameof(useAutoStepping), 1)]
	private float stairsDetectionAngle = 45.0f;

	[ShowInEditor]
	[ParameterMask(Group = "Auto Stepping", MaskType = ParameterMaskAttribute.TYPE.INTERSECTION, Tooltip = "Stair detection mask")]
	private int stairDetectionMask = 1;

	[ShowInEditor]
	[Parameter(Group = "Objects Interaction", Tooltip = "Toggles physical interaction with rigid bodies on anf off")]
	private bool useObjectsInteraction = true;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 1.0f, Group = "Objects Interaction", Tooltip = "Impulse multiplier to be applied to a rigid body colliding with the player")]
	[ParameterCondition(nameof(useObjectsInteraction), 1)]
	private float impulseMultiplier = 0.05f;

	[ShowInEditor]
	[ParameterSlider(Min = 15, Max = 240, Group = "Advanced Settings", Tooltip = "Minimum update rate for the player (in number of frames per second).\n" +
	"If this value exceeds the current framerate, the player will be updated several times per frame")]
	private int playerFps = 60;

	[ShowInEditor]
	[ParameterSlider(Min = 1, Max = 128, Group = "Advanced Settings", Tooltip = "Number of iterations to resolve collisions")]
	private int collisionIterations = 4;

	[ShowInEditor]
	[ParameterSlider(Min = 4, Max = 1000, Group = "Advanced Settings", Tooltip = "Maximum number of contacts to be processed for collisions")]
	private int contactsBufferSize = 16;

	[ShowInEditor]
	[ParameterSlider(Min = 4, Max = 1000, Group = "Advanced Settings", Tooltip = "Maximum number of contacts up to which the number of iterations specified above shall be used.\n" +
		"After exceeding this value a single iteration shall be used to avoid performance degradation.")]
	private int heavyContactsCount = 100;

#if DEBUG
	public struct DebugCamera
	{
		[Parameter(Tooltip = "Debug camera enabled")]
		public bool enabled;

		[Parameter(Tooltip = "Use debug camera with fixed angles")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool useFixedAngles;

		[HideInEditor] public PlayerDummy camera;
		[HideInEditor] public float angularSpeed;
		[HideInEditor] public float zoomSpeed;
		[HideInEditor] public float maxDistance;
		[HideInEditor] public float horizontalAngle;
		[HideInEditor] public float verticalAngle;
		[HideInEditor] public float distance;
	}

	[ShowInEditor]
	[Parameter(Group = "Debug")]
	private DebugCamera debugCamera;

	public struct DebugVisualizer
	{
		[Parameter(Tooltip = "Use visualizer")]
		public bool enabled;

		[Parameter(Tooltip = "Triangles")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool triangles;

		[Parameter(Tooltip = "Shapes")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool shapes;

		[Parameter(Tooltip = "Player shape")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool playerShape;

		[Parameter(Tooltip = "Player direction")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool playerDirection;

		[Parameter(Tooltip = "Camera")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool camera;

		[Parameter(Tooltip = "Slope basis")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool slopeBasis;

		[Parameter(Tooltip = "Applied horizontal velocity")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool appliedHorizontalVelocity;

		[Parameter(Tooltip = "Applied vertical velocity")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool appliedVerticalVelocity;

		[Parameter(Tooltip = "Up pass contacts")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool upPassContacts;

		[Parameter(Tooltip = "Side pass contacts")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool sidePassContacts;

		[Parameter(Tooltip = "Down pass contacts")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool downPassContacts;

		[Parameter(Tooltip = "Check move ray")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool checkMoveRay;

		[Parameter(Tooltip = "Stair detection ray")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool stairDetectionRay;
	}

	[ShowInEditor]
	[Parameter(Group = "Debug")]
#pragma warning disable CS0649
	private DebugVisualizer debugVisualizer;
#pragma warning restore CS0649

	public struct DebugProfiler
	{
		[Parameter(Tooltip = "Use profiler")]
		public bool enabled;

		[Parameter(Tooltip = "Applied horizontal speed")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool appliedHorizontalSpeed;

		[Parameter(Tooltip = "Applied vertical speed")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool appliedVerticalSpeed;

		[Parameter(Tooltip = "Up pass contact")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool upPassContact;

		[Parameter(Tooltip = "Side pass contact")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool sidePassContact;

		[Parameter(Tooltip = "Down pass contact")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool downPassContact;

		[Parameter(Tooltip = "Is ground")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool isGround;

		[Parameter(Tooltip = "Is ceiling")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool isCeiling;

		[Parameter(Tooltip = "Is crouch")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool isCrouch;

		[Parameter(Tooltip = "Average speed")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool averageSpeed;

		[Parameter(Tooltip = "Auto stepping")]
		[ParameterCondition(nameof(enabled), 1)]
		public bool autoStepping;
	}

	[ShowInEditor]
	[Parameter(Group = "Debug")]
#pragma warning disable CS0649
	private DebugProfiler debugProfiler;
#pragma warning restore CS0649

	public struct DebugColors
	{
		[ParameterColor] public vec4 playerShape;
		[ParameterColor] public vec4 playerDirection;
		[ParameterColor] public vec4 cameraColor;
		[ParameterColor] public vec4 appliedHorizontalVelocity;
		[ParameterColor] public vec4 appliedVerticalVelocity;
		[ParameterColor] public vec4 upPassContacts;
		[ParameterColor] public vec4 sidePassContacts;
		[ParameterColor] public vec4 downPassContacts;
		[ParameterColor] public vec4 isGround;
		[ParameterColor] public vec4 isCeiling;
		[ParameterColor] public vec4 isCrouch;
		[ParameterColor] public vec4 averageSpeed;
		[ParameterColor] public vec4 autoStepping;

		[HideInEditor] public float[] arrayAppliedHorizontalVelocity;
		[HideInEditor] public float[] arrayAppliedVerticalVelocity;
		[HideInEditor] public float[] arrayUpPassContacts;
		[HideInEditor] public float[] arraySidePassContacts;
		[HideInEditor] public float[] arrayDownPassContacts;
		[HideInEditor] public float[] arrayIsGround;
		[HideInEditor] public float[] arrayIsCeiling;
		[HideInEditor] public float[] arrayIsCrouch;
		[HideInEditor] public float[] arrayAverageSpeed;
		[HideInEditor] public float[] arrayAutoStepping;

		public DebugColors(bool useDefault)
		{
			playerShape = new vec4(0.0f, 0.0f, 1.0f, 0.098f);
			playerDirection = vec4.YELLOW;
			cameraColor = vec4.CYAN;
			appliedHorizontalVelocity = vec4.BLACK;
			appliedVerticalVelocity = vec4.GREY;
			upPassContacts = vec4.CYAN;
			sidePassContacts = vec4.MAGENTA;
			downPassContacts = vec4.RED;
			isGround = vec4.RED;
			isCeiling = vec4.GREEN;
			isCrouch = vec4.BLUE;
			averageSpeed = vec4.YELLOW;
			autoStepping = new vec4(0.66f, 0.33f, 0.0f, 1.0f);

			arrayAppliedHorizontalVelocity = null;
			arrayAppliedVerticalVelocity = null;
			arrayUpPassContacts = null;
			arraySidePassContacts = null;
			arrayDownPassContacts = null;
			arrayIsGround = null;
			arrayIsCeiling = null;
			arrayIsCrouch = null;
			arrayAverageSpeed = null;
			arrayAutoStepping = null;
		}
	}

	[ShowInEditor]
	[Parameter(Group = "Debug")]
	private DebugColors debugColors = new DebugColors(true);
#endif

	#endregion editor parameters

	#region Player properties

	public bool IsInitialized { get; private set; } = false;

	public vec3 AdditionalCameraOffset { get; set; } = vec3.ZERO;

	public quat AdditionalCameraRotation { get; set; } = quat.IDENTITY;

	public bool IsGround { get; private set; } = false;
	public bool IsCeiling { get; private set; } = false;
	public bool IsCrouch { get; private set; } = false;
	public bool IsHorizontalFrozen { get; private set; } = false;

	public Vec3 SlopeNormal { get; private set; } = Vec3.UP;
	public Vec3 SlopeAxisX { get; private set; } = Vec3.RIGHT;
	public Vec3 SlopeAxisY { get; private set; } = Vec3.FORWARD;

	public Vec3 HorizontalVelocity { get; private set; } = Vec3.ZERO; // velocity in current slope basis
	public float VerticalVelocity { get; private set; } = 0.0f;

	#endregion Player properties

	// additional constants
	static private readonly vec2 forward = new vec2(0, 1);
	static private readonly vec2 right = new vec2(1, 0);
	private const float skinWidthOffset = 0.05f;
	private const float autoSteppingSpeedThreshold = 0.1f;
	private const float largeEpsilon = 0.001f;

	private float playerIFps = 1.0f;

	private InputGamePad gamePad = null;

	private BodyDummy body = null;
	private ShapeCapsule shape = null;
	private ShapeCylinder interactionShape = null;

	private List<ShapeContact> contacts = new List<ShapeContact>();
	private bool isAvailableSideMove = false;
	private bool isAvailableStair = false;
	private bool hasBottomContacts = false;
	private bool isHeavyContacts = false;

	private float cameraVerticalAngle = 0.0f;
	private float cameraHorizontalAngle = 0.0f;
	private vec3 cameraCrouchOffset = vec3.ZERO;

	private vec2 horizontalMoveDirection = vec2.ZERO;
	private float verticalMoveDirection = 0.0f;

	private float maxAirSpeed = 0.0f;

	private bool usedAutoStepping = false;
	private Scalar lastStepHeight = 0.0f;

	private float cosGroundAngle = 0.5f;
	private float cosCeilingAngle = 0.5f;
	private float cosStairAngle = 0.5f;
	private float cosStairsDetectionAngle = 0.5f;
	private float cosWallStopSlidingAngle = 0.96f;

	private Mat4 worldTransform = Mat4.IDENTITY;

	WorldIntersectionNormal normalIntersection = new WorldIntersectionNormal();

	public enum CrouchPhase
	{
		STAND,
		MOVE_DOWN,
		CROUCH,
		MOVE_UP
	}

	private struct CrouchState
	{
		public CrouchPhase phase;
		public float currentTime;
		public Scalar currentHeight;
		public Scalar startHeight;
		public Scalar endHeight;
	}

	private CrouchState crouchState;

	#region Debug

#if DEBUG
	private float maxAppliedHorizontalSpeed = 0.0f;
	private float maxAppliedVerticalSpeed = 20.0f;
	private int maxPassContacts = 200;
	private float maxFlagValue = 1.05f;

	private float[] speedsBuffer = new float[10];
	private Vec3 lastPlayerPosition = Vec3.ZERO;

	private bool autoSteppingApplied = false;
#endif

	#endregion Debug

	private void Init() 
	{
		// check object type
		Object obj = node as Object;
		if (!obj)
		{
			Log.Error("FirstPersonController: can't cast node to Object\n");
			return;
		}

		// fix player transformation
		// player can only have vertical position, Y axis is used for forward direction
		Vec3 axisY = new Vec3(obj.WorldTransform.AxisY);
		axisY.z = 0;
		axisY = (axisY.Length2 > MathLib.EPSILON ? axisY.Normalize() : Vec3.FORWARD);
		obj.WorldTransform = MathLib.SetTo(node.WorldPosition, node.WorldPosition + axisY, vec3.UP, MathLib.AXIS.Y);

		// set dummy body
		if (useObjectBody)
		{
			body = obj.Body as BodyDummy;
			if (!body)
				Log.Warning("FirstPersonController: object doesn't contain BodyDummy, it was created automatically\n");
		}

		if (!body)
		{
			body = new BodyDummy(obj);
			body.Transform = obj.WorldTransform;
		}

		// set capsule shape
		if (useObjectBody)
		{
			if (body.NumShapes > 0)
			{
				for (int i = 0; i < body.NumShapes; i++)
					if (!shape)
						shape = body.GetShape(i) as ShapeCapsule;

				if (!shape)
					Log.Warning("FirstPersonController: body doesn't contain ShapeCapsule, it was created automatically\n");
			}
			else
				Log.Warning("FirstPersonController: body doesn't contain shapes, it was created automatically\n");
		}

		if (!shape)
		{
			shape = new ShapeCapsule(body, capsuleRadius, capsuleHeight);
			body.SetShapeTransform(body.NumShapes - 1, MathLib.Translate(vec3.UP * (capsuleRadius + 0.5f * capsuleHeight)));

			shape.PhysicsIntersectionMask = physicsIntersectionMask;
			shape.CollisionMask = collisionMask;
			shape.ExclusionMask = exclusionMask;
		}

		capsuleHeight = shape.Height;
		crouchHeight = MathLib.Clamp(crouchHeight, 2.0f * shape.Radius, shape.Height + 2.0f * shape.Radius);
		crouchState.currentHeight = shape.Height + 2.0f * shape.Radius;

		// set camera
		if (cameraMode == CameraMode.USE_EXTERNAL && !camera)
			Log.Warning("FirstPersonController: camera is null, it was created automatically\n");

		if (!camera || cameraMode == CameraMode.CREATE_AUTOMATICALLY)
		{
			camera = new PlayerDummy();
			camera.Parent = obj;
			camera.Fov = fov;
			camera.ZNear = nearClipping;
			camera.ZFar = farClipping;
			camera.WorldPosition = obj.WorldTransform * new Vec3(cameraPositionOffset);
			camera.SetWorldDirection(new vec3(axisY), vec3.UP);
			camera.MainPlayer = true;
		}

		if (camera && cameraMode != CameraMode.NONE)
		{
			cameraVerticalAngle = MathLib.GetAngle(vec3.DOWN, camera.GetWorldDirection());
			cameraVerticalAngle = MathLib.Clamp(cameraVerticalAngle, minVerticalAngle + 90.0f, maxVerticalAngle + 90.0f);

			cameraHorizontalAngle = node.GetWorldRotation().GetAngle(vec3.UP);
			cameraPositionOffset = new vec3(node.IWorldTransform * camera.WorldPosition);

			vec3 cameraDirection = vec3.FORWARD * MathLib.RotateZ(-cameraHorizontalAngle);
			cameraDirection = cameraDirection * MathLib.Rotate(MathLib.Cross(cameraDirection, vec3.UP), 90.0f - cameraVerticalAngle);
			cameraDirection.Normalize();
			camera.SetWorldDirection(cameraDirection, vec3.UP);
		}

		// create cylinder shape for interacting with objects
		if (useObjectsInteraction)
		{
			interactionShape = new ShapeCylinder();
			interactionShape.Enabled = false;
		}

		// set auxiliary parameters
		playerIFps = 1.0f / playerFps;
		crouchTransitionTime = MathLib.Max(crouchTransitionTime, MathLib.EPSILON);

		cosGroundAngle = MathLib.Cos(maxGroundAngle * MathLib.DEG2RAD);
		cosCeilingAngle = MathLib.Cos(maxCeilingAngle * MathLib.DEG2RAD);
		cosStairAngle = MathLib.Cos(maxStairAngle * MathLib.DEG2RAD);
		cosStairsDetectionAngle = MathLib.Cos(stairsDetectionAngle * MathLib.DEG2RAD + MathLib.PI05);
		cosWallStopSlidingAngle = MathLib.Cos(wallStopSlidingAngle * MathLib.DEG2RAD);

#if DEBUG
		maxPassContacts = contactsBufferSize;
#endif

		worldTransform = obj.WorldTransform;

		if (Input.CountActiveGamePads != 0)
			gamePad = Input.GetActiveGamePad(0);

		IsInitialized = true;
	}
	
	private void Update()
	{
		if (!IsInitialized)
			return;

		worldTransform = node.WorldTransform;

		float ifps = Game.IFps * Physics.Scale;

		UpdateMoveDirections(ifps);
		CheckMoveAndStair();

		if (!isAvailableSideMove)
		{
			if (hasBottomContacts)
			{
				HorizontalVelocity = Vec3.ZERO;
				horizontalMoveDirection = vec2.ZERO;
			}
		}

		float updateTime = ifps;
		hasBottomContacts = false;
		while (updateTime > 0.0f)
		{
			float adaptiveTimeStep = MathLib.Min(updateTime, playerIFps);
			updateTime -= adaptiveTimeStep;
			 
			UpdateVelocity(adaptiveTimeStep, adaptiveTimeStep / ifps);
			MathLib.Mul(out worldTransform, MathLib.Translate((HorizontalVelocity + Vec3.UP * VerticalVelocity) * adaptiveTimeStep), worldTransform);
			UpdateCollisions(adaptiveTimeStep);
		}

		UpdateCrouch(ifps);

		// update player transformation
		node.WorldTransform = worldTransform;
		body.Transform = node.WorldTransform;
		if (IsCrouch)
			shape.Transform = worldTransform * MathLib.Translate(Vec3.UP * (shape.Radius + shape.Height * 0.5f));

		UpdateCamera();
	}

	private void UpdatePhysics()
	{
		if (!IsInitialized)
			return;

		if (useObjectsInteraction)
		{
			// enable interaction cylinder shape and set parameters
			// this shape is slightly larger than the capsule shape
			// this allows to get more correct contacts with objects, especially with bottom of player
			interactionShape.Enabled = true;
			interactionShape.Radius = shape.Radius + checkMoveRayOffset + skinWidthOffset;
			interactionShape.Height = shape.Height + 2.0f * (shape.Radius + checkMoveRayOffset - skinWidthOffset);
			interactionShape.Transform = MathLib.Translate(worldTransform.Translate + Vec3.UP * (0.5f * interactionShape.Height + skinWidthOffset));

			interactionShape.GetCollision(contacts);
			int contactsCount = MathLib.Min(contactsBufferSize, contacts.Count);

			Scalar speed = HorizontalVelocity.Length + MathLib.Abs(VerticalVelocity);
			speed = MathLib.Max(speed, 1.0f);

			for (int i = 0; i < contactsCount; i++)
			{
				ShapeContact c = contacts[i];
				if (c.Object && c.Object.BodyRigid)
				{
					c.Object.BodyRigid.Frozen = false;

					// multiply by 0.5f only for normalization impulse multiplier in editor settings
					c.Object.BodyRigid.AddWorldImpulse(c.Point, -c.Normal * c.Object.BodyRigid.Mass * speed * impulseMultiplier * 0.5f);
				}
			}

			interactionShape.Enabled = false;
		}
	}

	private void UpdateMoveDirections(float ifps)
	{
		// reset all directions
		horizontalMoveDirection = vec2.ZERO;
		verticalMoveDirection = 0.0f;

		if (!App.MouseGrab)
			return;

		// update horizontal direction
		if (Input.IsKeyPressed(forwardKey))
			horizontalMoveDirection += forward;

		if (Input.IsKeyPressed(backwardKey))
			horizontalMoveDirection -= forward;

		if (Input.IsKeyPressed(rightKey))
			horizontalMoveDirection += right;

		if (Input.IsKeyPressed(leftKey))
			horizontalMoveDirection -= right;

		horizontalMoveDirection.Normalize();

		if (gamePad != null)
		{
			vec2 moveValue = (moveStick == GamepadStickSide.LEFT ? gamePad.AxesLeft : gamePad.AxesRight);
			if (moveValue.Length > sticksDeadZone && moveValue.Length2 > horizontalMoveDirection.Length2)
				horizontalMoveDirection = moveValue;
		}

		// update vertical direction
		if (useJump && IsGround && (Input.IsKeyDown(jumpKey) || gamePad && gamePad.IsButtonDown(jumpButton)))
			verticalMoveDirection = (IsCrouch ? crouchJumpPower : jumpPower) / ifps;
	}

	private void CheckMoveAndStair()
	{
		isAvailableSideMove = false;
		isAvailableStair = false;

		vec3 horizontalDirection = worldTransform.GetRotate() * new vec3(horizontalMoveDirection);
		horizontalMoveDirection.Normalize();

		// check angle of surface for possible movement
		if (horizontalMoveDirection.Length2 > 0.0f)
		{
			Vec3 p2 = worldTransform.Translate + horizontalDirection * (shape.Radius + checkMoveRayOffset) + Vec3.DOWN * checkMoveRayOffset;
			Vec3 p1 = p2 + Vec3.UP * (MathLib.Max(shape.Radius, maxStepHeight) + checkMoveRayOffset);

			var hitObj = World.GetIntersection(p1, p2, checkMoveMask, normalIntersection);
			if (hitObj)
			{
				if (MathLib.Dot(vec3.UP, normalIntersection.Normal) > cosGroundAngle)
					isAvailableSideMove = true;

				// this check allows movement through elevations
				Scalar cos = MathLib.Dot(SlopeNormal, new Vec3(normalIntersection.Normal));
				if (cos < largeEpsilon)
					isAvailableSideMove = true;
			}
			else
			{
				// allow to move in air
				isAvailableSideMove = true;
			}

#if DEBUG
			if (debugVisualizer.enabled && debugVisualizer.checkMoveRay)
			{
				if (isAvailableSideMove)
					Visualizer.RenderVector(p1, p2, vec4.GREEN);
				else
					Visualizer.RenderVector(p1, p2, vec4.RED);
			}
#endif
		}

		// check stair surface angle for auto stepping
		if (useAutoStepping && horizontalMoveDirection.Length2 > 0.0f)
		{
			Vec3 p2 = worldTransform.Translate + horizontalDirection * (shape.Radius + checkStairRayOffset) + Vec3.UP * minStepHeight;
			Vec3 p1 = p2 + vec3.UP * (maxStepHeight - minStepHeight + checkStairRayOffset);

			var hitObj = World.GetIntersection(p1, p2, stairDetectionMask, normalIntersection);
			if (hitObj)
			{
				if (MathLib.Dot(vec3.UP, normalIntersection.Normal) > cosStairAngle)
					isAvailableStair = true;
			}

#if DEBUG
			if (debugVisualizer.enabled && debugVisualizer.stairDetectionRay)
			{
				if (isAvailableStair)
					Visualizer.RenderVector(p1, p2, vec4.GREEN);
				else
					Visualizer.RenderVector(p1, p2, vec4.RED);
			}
#endif
		}
	}

	private void UpdateVelocity(float ifps, float updatePart)
	{
		// update current slope basis
		// check vectors for collinearity and, depending on this, calculate the slope basis
		Scalar cosAngle = MathLib.Dot(new Vec3(worldTransform.AxisY), SlopeNormal);
		if (MathLib.Compare(MathLib.Abs(cosAngle), 1.0f) == 0)
		{
			SlopeAxisX = MathLib.Cross(new Vec3(worldTransform.AxisY), SlopeNormal).Normalize();
			SlopeAxisY = MathLib.Cross(SlopeNormal, SlopeAxisX).Normalize();
		}
		else
		{
			SlopeAxisY = MathLib.Cross(new Vec3(worldTransform.AxisX) * MathLib.Sign(cosAngle), SlopeNormal).Normalize();
			SlopeAxisX = MathLib.Cross(SlopeAxisY, SlopeNormal).Normalize();
		}

		// get decomposition of velocity for instant change on ground
		Vec3 horizontalVelocityDecomposition = Vec3.ZERO;
		if (IsGround)
		{
			horizontalVelocityDecomposition.x = MathLib.Dot(SlopeAxisX, HorizontalVelocity);
			horizontalVelocityDecomposition.y = MathLib.Dot(SlopeAxisY, HorizontalVelocity);
			horizontalVelocityDecomposition.z = MathLib.Dot(SlopeNormal, HorizontalVelocity);
		}

		// player rotation
		if (App.MouseGrab)
		{
			worldTransform *= new Mat4(MathLib.Rotate(new quat(vec3.UP, -Input.MouseDelta.x * mouseSensitivity * updatePart)));

			float delta = -Input.MouseDelta.x * mouseSensitivity;
			if (gamePad)
			{
				vec2 rotateValue = (cameraStick == GamepadStickSide.LEFT ? gamePad.AxesLeft : gamePad.AxesRight);
				if (rotateValue.Length > sticksDeadZone && MathLib.Abs(rotateValue.x * cameraStickSensitivity) > MathLib.Abs(delta))
					delta = -rotateValue.x * cameraStickSensitivity;
			}

			cameraHorizontalAngle += delta * updatePart;
			if (cameraHorizontalAngle < -180.0f || 180.0f < cameraHorizontalAngle)
				cameraHorizontalAngle -= MathLib.Sign(cameraHorizontalAngle) * 360.0f;

			Vec3 position = worldTransform.Translate;
			worldTransform.SetRotate(Vec3.UP, cameraHorizontalAngle);
			worldTransform.SetColumn3(3, position);
		}

		// on the ground change velocity without inertia
		if (IsGround)
		{
			// again check vectors for collinearity and, depending on this, update the slope basis
			cosAngle = MathLib.Dot(new Vec3(worldTransform.AxisY), SlopeNormal);
			if (MathLib.Compare(MathLib.Abs(cosAngle), 1.0f) == 0)
			{
				SlopeAxisX = MathLib.Cross(new Vec3(worldTransform.AxisY), SlopeNormal).Normalize();
				SlopeAxisY = MathLib.Cross(SlopeNormal, SlopeAxisX).Normalize();
			}
			else
			{
				SlopeAxisY = MathLib.Cross(new Vec3(worldTransform.AxisX) * MathLib.Sign(cosAngle), SlopeNormal).Normalize();
				SlopeAxisX = MathLib.Cross(SlopeAxisY, SlopeNormal).Normalize();
			}

			// restore velocity in new basis
			HorizontalVelocity = SlopeAxisX * horizontalVelocityDecomposition.x +
								 SlopeAxisY * horizontalVelocityDecomposition.y +
								 SlopeNormal * horizontalVelocityDecomposition.z;
		}

		// add horizontal velocity in slope basis
		float acceleration = (IsGround ? groundAcceleration : airAcceleration);
		HorizontalVelocity += SlopeAxisX * horizontalMoveDirection.x * acceleration * ifps;
		HorizontalVelocity += SlopeAxisY * horizontalMoveDirection.y * acceleration * ifps;

		// update vertical velocity
		VerticalVelocity += verticalMoveDirection * ifps;
		if (!IsGround)
			VerticalVelocity += Physics.Gravity.z * ifps;

		// get current max speed
		float maxSpeed = maxAirSpeed;
		if (IsGround)
		{
			maxSpeed = (useRun && useRunDefault) ? runSpeed : walkSpeed;

			if (useRun && (Input.IsKeyPressed(runKey) || gamePad && gamePad.IsButtonPressed(runButton)))
				maxSpeed = useRunDefault ? walkSpeed : runSpeed;

			if (IsGround && IsCrouch)
				maxSpeed = crouchSpeed;

			maxAirSpeed = maxSpeed;
		}

		// apply damping to horizontal velocity when it exceeds target speed
		// or target speed too low (not pressed horizontal movement keys)
		vec2 targetSpeed = horizontalMoveDirection * maxSpeed;
		if (targetSpeed.Length < MathLib.EPSILON || targetSpeed.Length < HorizontalVelocity.Length)
			HorizontalVelocity *= MathLib.Exp((IsGround ? -groundDamping : -airDamping) * ifps);

		// clamp horizontal velocity if it greater than current max speed
		if (HorizontalVelocity.Length > maxSpeed)
			HorizontalVelocity = HorizontalVelocity.Normalize() * maxSpeed;

		// check frozen state for horizontal velocity
		// IsGround needed in case of slipping from the edges
		// contacts will be pushed player in all directions, and not just up
		IsHorizontalFrozen = IsGround && (HorizontalVelocity.Length < Physics.FrozenLinearVelocity);
	}

	private void UpdateCollisions(float ifps)
	{
		// set default collision parameters
		IsGround = false;
		IsCeiling = false;
		SlopeNormal = Vec3.UP;

		isHeavyContacts = false;

		// resolve current collisions
		for (int j = 0; j < collisionIterations; j++)
		{
			if (useAutoStepping)
			{
#if DEBUG
				autoSteppingApplied = false;
#endif
				if (isAvailableStair)
					TryMoveUp(ifps);
			}

			MoveSide(ifps);

			if (useAutoStepping && usedAutoStepping && isAvailableStair)
			{
#if DEBUG
				autoSteppingApplied = true;
#endif
				TryMoveDown(ifps);
			}

			if (isHeavyContacts)
				break;
		}
	}

	private void TryMoveUp(float ifps)
	{
		usedAutoStepping = false;
		lastStepHeight = 0.0f;

		if (horizontalMoveDirection.Length2 > 0.0f && !IsHorizontalFrozen && VerticalVelocity < 0.0f)
		{
			body.Transform = worldTransform;
			if (IsCrouch)
				shape.Transform = worldTransform * MathLib.Translate(Vec3.UP * (shape.Radius + shape.Height * 0.5f));

			// find collisions with the capsule
			shape.GetCollision(contacts);
			if (contacts.Count == 0)
				return;

			if (contacts.Count > heavyContactsCount)
				isHeavyContacts = true;

			int contactsCount = MathLib.Min(contactsBufferSize, contacts.Count);

			// find max step height
			Vec2 velocityXY = new Vec2(HorizontalVelocity);
			if (velocityXY.Length2 < autoSteppingSpeedThreshold)
			{
				// set minimal velocity for climb
				velocityXY = new Vec2(worldTransform.GetRotate() * new Vec3(horizontalMoveDirection));
				velocityXY.Normalize();
				HorizontalVelocity = new Vec3(velocityXY * walkSpeed);
			}

			for (int i = 0; i < contactsCount; i++)
			{
				ShapeContact c = contacts[i];

				Vec2 normalXY = new Vec2(c.Normal);

				// skip contacts opposite to movement
				if (MathLib.Dot(normalXY, velocityXY) > cosStairsDetectionAngle)
					continue;

				Scalar step = MathLib.Dot(c.Point - worldTransform.Translate, Vec3.UP);
				if (lastStepHeight < step)
					lastStepHeight = step;
			}

			// apply auto stepping
			if (minStepHeight < lastStepHeight && lastStepHeight < maxStepHeight)
			{
				worldTransform.SetColumn3(3, worldTransform.Translate + vec3.UP * lastStepHeight);

				// check contacts with other objects after elevating
				// and cancel automatic step if contacts exist
				body.Transform = worldTransform;
				if (IsCrouch)
					shape.Transform = worldTransform * MathLib.Translate(Vec3.UP * (shape.Radius + shape.Height * 0.5f));

				shape.GetCollision(contacts);
				if (contacts.Count == 0)
					usedAutoStepping = true;
				else
					worldTransform.SetColumn3(3, worldTransform.Translate + vec3.DOWN * lastStepHeight);
			}

#if DEBUG
			if (debugVisualizer.enabled && debugVisualizer.upPassContacts)
			{
				foreach (var c in contacts)
					Visualizer.RenderVector(c.Point, c.Point + c.Normal, debugColors.upPassContacts);
			}

			if (debugProfiler.enabled && debugProfiler.upPassContact)
				Profiler.SetValue("Up Pass Contacts ", "", contacts.Count, maxPassContacts, debugColors.arrayUpPassContacts);
#endif
		}
	}

	private void MoveSide(float ifps)
	{
		// apply new player transformation for physic body
		body.Transform = worldTransform;
		if (IsCrouch)
			shape.Transform = worldTransform * MathLib.Translate(Vec3.UP * (shape.Radius + shape.Height * 0.5f));

		// get contacts in new position and process them
		shape.GetCollision(contacts);
		if (contacts.Count == 0)
			return;

		if (contacts.Count > heavyContactsCount)
			isHeavyContacts = true;

		int contactsCount = MathLib.Min(contactsBufferSize, contacts.Count);

		// total position offset for all contacts depth
		var positionOffset = vec3.ZERO;

		// maximum angle of inclination of the surface under the player
		float maxCosAngle = 1.0f;

		float inum = 1.0f / contactsCount;
		for (int i = 0; i < contactsCount; i++)
		{
			var c = contacts[i];

			// when horizontal velocity is frozen, we can move player only in vertical direction
			// this help to avoid sliding on slopes
			// in other cases, move player in all directions
			// use epsilon offset with depth for accuracy ground detection
			if (IsHorizontalFrozen)
			{
				float depth = MathLib.Dot(vec3.UP, c.Normal) * (c.Depth - MathLib.EPSILON);
				positionOffset += vec3.UP * depth * inum;
			}
			else
			{
				positionOffset += c.Normal * (c.Depth - MathLib.EPSILON) * inum;

				// remove part of horizontal velocity that is projected onto normal of current contact
				Scalar normalSpeed = MathLib.Dot(new Vec3(c.Normal), HorizontalVelocity);
				HorizontalVelocity -= c.Normal * normalSpeed;
			}

			// stop sliding near the wall at a certain angle
			if ((c.Object && c.Object.BodyRigid == null) && shape.BottomCap.z < c.Point.z && c.Point.z < shape.TopCap.z)
			{
				float cos = MathLib.Dot(worldTransform.GetRotate() * new vec3(horizontalMoveDirection), -c.Normal);
				if (cos > cosWallStopSlidingAngle)
					HorizontalVelocity = Vec3.ZERO;
			}

			// check ground state
			// first part of expression checks that current contact belongs to bottom sphere of capsule
			// second part of expression checks that current angle of inclination of surface
			// not exceed maximum allowed angle
			if (MathLib.Dot(c.Point - shape.BottomCap, Vec3.UP) < 0.0f)
			{
				hasBottomContacts = true;
				if (MathLib.Dot(c.Normal, vec3.UP) > cosGroundAngle)
				{
					VerticalVelocity = Physics.Gravity.z * ifps;
					IsGround = true;
				}

				// find to maximum angle of inclination of surface under player
				// and save normal of this surface
				float cosAngle = MathLib.Dot(vec3.UP, c.Normal);
				if (MathLib.Compare(cosAngle, 0.0f, 0.01f) == 0 && cosAngle < maxCosAngle)
				{
					SlopeNormal = new Vec3(contacts[i].Normal);
					maxCosAngle = cosAngle;
				}
			}

			// check ceiling state
			// first part of expression checks that current angle of inclination of ceiling
			// not exceed maximum allowed angle
			// second part of expression checks that current contact belongs to top sphere of capsule
			if (MathLib.Dot(contacts[i].Normal, vec3.DOWN) > cosCeilingAngle && MathLib.Dot(contacts[i].Point - shape.TopCap, Vec3.DOWN) < 0.0f)
			{
				IsCeiling = true;

				// stop moving up
				VerticalVelocity = 0.0f;
			}
		}

		// add total position offset to player transformation
		worldTransform.SetColumn3(3, worldTransform.Translate + positionOffset);

#if DEBUG
		if (debugVisualizer.enabled && debugVisualizer.sidePassContacts)
		{
			foreach (var c in contacts)
				Visualizer.RenderVector(c.Point, c.Point + c.Normal, debugColors.sidePassContacts);
		}

		if (debugProfiler.enabled && debugProfiler.sidePassContact)
			Profiler.SetValue("Side Pass Contacts ", "", contacts.Count, maxPassContacts, debugColors.arraySidePassContacts);
#endif
	}

	private void TryMoveDown(float ifps)
	{
		// this correction allows to avoid jittering on large stairs
		if (lastStepHeight > shape.Radius)
			lastStepHeight = shape.Radius - Physics.PenetrationTolerance;

		// try to drop down the player
		worldTransform.SetColumn3(3, worldTransform.Translate - vec3.UP * lastStepHeight);

		body.Transform = worldTransform;
		if (IsCrouch)
			shape.Transform = worldTransform * MathLib.Translate(Vec3.UP * (shape.Radius + shape.Height * 0.5f));

		// find collisions with the capsule
		shape.GetCollision(contacts);
		if (contacts.Count == 0)
			return;

		if (contacts.Count > heavyContactsCount)
			isHeavyContacts = true;

		int contactsCount = MathLib.Min(contactsBufferSize, contacts.Count);

		float inumContacts = 1.0f / MathLib.Itof(contactsCount);
		for (int i = 0; i < contactsCount; i++)
		{
			ShapeContact c = contacts[i];

			float depth = MathLib.Dot(vec3.UP, c.Normal) * c.Depth;
			worldTransform.SetColumn3(3, worldTransform.Translate + vec3.UP * depth * inumContacts);

			if (MathLib.Dot(c.Normal, vec3.UP) > cosGroundAngle && MathLib.Dot(c.Point - shape.BottomCap, Vec3.UP) < 0.0f)
			{
				IsGround = true;
				VerticalVelocity = Physics.Gravity.z * ifps;
			}
		}

#if DEBUG
		if (debugVisualizer.enabled && debugVisualizer.downPassContacts)
		{
			foreach (var c in contacts)
				Visualizer.RenderVector(c.Point, c.Point + c.Normal, debugColors.downPassContacts);
		}

		if (debugProfiler.enabled && debugProfiler.downPassContact)
			Profiler.SetValue("Down Pass Contacts ", "", contacts.Count, maxPassContacts, debugColors.arrayDownPassContacts);
#endif
	}

	private void UpdateCrouch(float ifps)
	{
		if (!useCrouch)
			return;

		// get state of crouch key
		bool isKey = (Input.IsKeyPressed(crouchKey) || gamePad && gamePad.IsButtonPressed(crouchButton));

		// determine the subsequent behavior depending on the current phase
		switch (crouchState.phase)
		{
			case CrouchPhase.STAND:
				if (isKey)
				{
					// go into a state of smooth movement down
					// set begin height to full player height
					// set end height to crouch player height
					// and activate crouch state
					crouchState.phase = CrouchPhase.MOVE_DOWN;
					SwapInterpolationDirection(capsuleHeight + 2.0f * shape.Radius, crouchHeight);
					IsCrouch = true;
				}
				break;

			case CrouchPhase.MOVE_DOWN:
			case CrouchPhase.CROUCH:
				if (!isKey)
				{
					// set player’s full height and check if we can get up
					bool canStandUp = true;

					// set shape parameters for standing position
					// use width offset to avoid false wall contacts
					float radius = shape.Radius;
					shape.Radius = radius - skinWidthOffset;
					UpdatePlayerHeight(capsuleHeight + 2.0f * skinWidthOffset);

					// check current collisions
					shape.GetCollision(contacts);

					Scalar topPoint = worldTransform.Translate.z + crouchHeight;
					for (int i = 0; i < contacts.Count; i++)
						if (contacts[i].Point.z > topPoint)
						{
							// some collisions are higher than crouch height and we can't stand up
							canStandUp = false;
							break;
						}

					// set current shape parameters
					shape.Radius = radius;
					UpdatePlayerHeight(crouchState.currentHeight - 2.0f * shape.Radius);

					if (canStandUp)
					{
						// go into a state of smooth movement up
						// set begin height to crouch player height
						// set end height to full player height
						crouchState.phase = CrouchPhase.MOVE_UP;
						SwapInterpolationDirection(crouchHeight, capsuleHeight + 2.0f * shape.Radius);
					}
				}
				break;

			case CrouchPhase.MOVE_UP:
				if (IsCeiling || isKey)
				{
					// if we touched an obstacle from above or the key is pressed again,
					// we go into a state of smooth movement down
					// set begin height to full player height
					// set end height to crouch player height
					crouchState.phase = CrouchPhase.MOVE_DOWN;
					SwapInterpolationDirection(capsuleHeight + 2.0f * shape.Radius, crouchHeight);
				}
				break;

			default: break;
		}

		// handle smooth motion
		if (crouchState.currentTime > 0.0f)
		{
			// get current linear interpolation coefficient based on current phase time
			float t = 1.0f;
			if (MathLib.Compare(crouchTransitionTime, MathLib.EPSILON) == 0)
				t = MathLib.Saturate(1.0f - crouchState.currentTime / crouchTransitionTime);

			// update current player height
			crouchState.currentHeight = MathLib.Lerp(crouchState.startHeight, crouchState.endHeight, t);
			UpdatePlayerHeight(crouchState.currentHeight - 2.0f * shape.Radius);

			crouchState.currentTime -= ifps;

			// handle final step of smooth motion
			if (crouchState.currentTime <= 0.0f)
			{
				// set final time and height
				crouchState.currentTime = 0.0f;
				crouchState.currentHeight = crouchState.endHeight;

				switch (crouchState.phase)
				{
					case CrouchPhase.MOVE_DOWN:
						// set crouch player height and go into crouch phase
						UpdatePlayerHeight(crouchState.currentHeight - 2.0f * shape.Radius);
						crouchState.phase = CrouchPhase.CROUCH;
						break;

					case CrouchPhase.MOVE_UP:
						// set full player height and go into stand phase
						// also disable crouch state
						UpdatePlayerHeight(crouchState.currentHeight - 2.0f * shape.Radius);
						crouchState.phase = CrouchPhase.STAND;
						IsCrouch = false;
						break;

					default: break;
				}
			}
		}
	}

	private void UpdateCamera()
	{
		if (!camera || cameraMode == CameraMode.NONE)
			return;

		if (App.MouseGrab)
		{
			// change vertical angle of camera
			cameraVerticalAngle -= Input.MouseDelta.y * mouseSensitivity;

			float delta = -Input.MouseDelta.y * mouseSensitivity;
			if (gamePad)
			{
				vec2 rotateValue = (cameraStick == GamepadStickSide.LEFT ? gamePad.AxesLeft : gamePad.AxesRight);
				if (rotateValue.Length > sticksDeadZone && MathLib.Abs(rotateValue.y * cameraStickSensitivity) > MathLib.Abs(delta))
					delta = rotateValue.y * cameraStickSensitivity;
			}

			cameraVerticalAngle += delta;
			cameraVerticalAngle = MathLib.Clamp(cameraVerticalAngle, minVerticalAngle + 90.0f, maxVerticalAngle + 90.0f);
		}

		// update camera transformation taking into account all additional offsets of position and rotation
		camera.WorldPosition = worldTransform * (new Vec3(cameraPositionOffset) + cameraCrouchOffset + AdditionalCameraOffset);

		vec3 cameraDirection = vec3.FORWARD * MathLib.RotateZ(-cameraHorizontalAngle);
		cameraDirection = cameraDirection * MathLib.Rotate(MathLib.Cross(cameraDirection, vec3.UP), 90.0f - cameraVerticalAngle);
		cameraDirection = AdditionalCameraRotation * cameraDirection;
		cameraDirection.Normalize();
		camera.SetWorldDirection(cameraDirection, vec3.UP);
	}

	private void SwapInterpolationDirection(Scalar startHeight, Scalar endHeight)
	{
		crouchState.currentTime = MathLib.Max(MathLib.EPSILON, crouchTransitionTime - crouchState.currentTime);
		crouchState.startHeight = startHeight;
		crouchState.endHeight = endHeight;
	}

	private void UpdatePlayerHeight(Scalar height)
	{
		shape.Height = (float)height;
		cameraCrouchOffset = vec3.UP * (height - capsuleHeight);
		shape.Transform = worldTransform * MathLib.Translate(Vec3.UP * (shape.Radius + height * 0.5f));
	}

	#region Debug

#if DEBUG
	[MethodInit]
	private void InitDebug()
	{
		if (!IsInitialized)
			return;

		// debug camera
		if (debugCamera.enabled)
		{
			debugCamera.camera = new PlayerDummy(); ;
			debugCamera.angularSpeed = 90.0f;
			debugCamera.zoomSpeed = 3.0f;
			debugCamera.maxDistance = 10.0f;
			debugCamera.horizontalAngle = 0.0f;
			debugCamera.verticalAngle = 0.0f;
			debugCamera.distance = debugCamera.maxDistance * 0.5f;

			Game.Player = debugCamera.camera;

			debugCamera.camera.SetWorldDirection(vec3.FORWARD, vec3.UP);
			debugCamera.camera.WorldPosition = (worldTransform.Translate + vec3.UP * (shape.Radius + shape.Height * 0.5f)) - vec3.FORWARD * debugCamera.distance;
		}

		// use visualizer
		Visualizer.Enabled = debugVisualizer.enabled;
		if (debugVisualizer.enabled)
		{
			Render.ShowTriangles = (debugVisualizer.triangles ? 1 : 0);

			int showShapes = (debugVisualizer.shapes ? 1 : 0);
			Unigine.Console.Run($"physics_show_shapes {showShapes}");
		}

		// use profiler
		int showProfiler = (debugProfiler.enabled ? 1 : 0);
		Unigine.Console.Run($"show_profiler {showProfiler}");
		if (debugProfiler.enabled)
		{
			// applied horizontal speed
			maxAppliedHorizontalSpeed = MathLib.Max(crouchSpeed, walkSpeed);
			maxAppliedHorizontalSpeed = MathLib.Max(maxAppliedHorizontalSpeed, runSpeed);
			maxAppliedHorizontalSpeed *= 1.1f;

			debugColors.arrayAppliedHorizontalVelocity = new float[4]
			{
				debugColors.appliedHorizontalVelocity.x,
				debugColors.appliedHorizontalVelocity.y,
				debugColors.appliedHorizontalVelocity.z,
				1.0f
			};

			// applied vertical speed
			debugColors.arrayAppliedVerticalVelocity = new float[4]
			{
				debugColors.appliedVerticalVelocity.x,
				debugColors.appliedVerticalVelocity.y,
				debugColors.appliedVerticalVelocity.z,
				1.0f
			};

			// up pass contacts
			debugColors.arrayUpPassContacts = new float[4]
			{
				debugColors.upPassContacts.x,
				debugColors.upPassContacts.y,
				debugColors.upPassContacts.z,
				1.0f
			};

			// side pass contacts
			debugColors.arraySidePassContacts = new float[4]
			{
				debugColors.sidePassContacts.x,
				debugColors.sidePassContacts.y,
				debugColors.sidePassContacts.z,
				1.0f
			};

			// down pass contacts
			debugColors.arrayDownPassContacts = new float[4]
			{
				debugColors.downPassContacts.x,
				debugColors.downPassContacts.y,
				debugColors.downPassContacts.z,
				1.0f
			};

			// is ground
			debugColors.arrayIsGround = new float[4]
			{
				debugColors.isGround.x,
				debugColors.isGround.y,
				debugColors.isGround.z,
				1.0f
			};

			// is ceiling
			debugColors.arrayIsCeiling = new float[4]
			{
				debugColors.isCeiling.x,
				debugColors.isCeiling.y,
				debugColors.isCeiling.z,
				1.0f
			};

			// is crouch
			debugColors.arrayIsCrouch = new float[4]
			{
				debugColors.isCrouch.x,
				debugColors.isCrouch.y,
				debugColors.isCrouch.z,
				1.0f
			};

			// average speed
			debugColors.arrayAverageSpeed = new float[4]
			{
				debugColors.averageSpeed.x,
				debugColors.averageSpeed.y,
				debugColors.averageSpeed.z,
				1.0f
			};

			// profile auto stepping
			debugColors.arrayAutoStepping = new float[4]
			{
				debugColors.autoStepping.x,
				debugColors.autoStepping.y,
				debugColors.autoStepping.z,
				1.0f
			};

		}

		lastPlayerPosition = worldTransform.Translate;
	}

	[MethodUpdate]
	private void UpdateDebug()
	{
		if (!IsInitialized)
			return;

		// debug camera
		if (debugCamera.enabled)
		{
			if (!debugCamera.useFixedAngles)
			{
				if (Input.IsKeyPressed(Input.KEY.UP))
					debugCamera.verticalAngle += debugCamera.angularSpeed * Game.IFps;
				if (Input.IsKeyPressed(Input.KEY.DOWN))
					debugCamera.verticalAngle -= debugCamera.angularSpeed * Game.IFps;

				debugCamera.verticalAngle = MathLib.Clamp(debugCamera.verticalAngle, -89.9f, 89.9f);

				if (Input.IsKeyPressed(Input.KEY.RIGHT))
					debugCamera.horizontalAngle -= debugCamera.angularSpeed * Game.IFps;
				if (Input.IsKeyPressed(Input.KEY.LEFT))
					debugCamera.horizontalAngle += debugCamera.angularSpeed * Game.IFps;

				if (debugCamera.horizontalAngle < -180.0f || 180.0f < debugCamera.horizontalAngle)
					debugCamera.horizontalAngle -= MathLib.Sign(debugCamera.horizontalAngle) * 360.0f;
			}

			if (Input.IsKeyPressed(Input.KEY.PLUS))
				debugCamera.distance -= debugCamera.zoomSpeed * Game.IFps;
			if (Input.IsKeyPressed(Input.KEY.MINUS))
				debugCamera.distance += debugCamera.zoomSpeed * Game.IFps;

			debugCamera.distance = MathLib.Clamp(debugCamera.distance, 0.0f, debugCamera.maxDistance);

			vec3 cameraDirection = debugCamera.camera.GetDirection();
			if (debugCamera.useFixedAngles && camera)
			{
				if (MathLib.Dot(camera.GetDirection(), vec3.DOWN) < 1.0f)
					cameraDirection = camera.GetWorldDirection();
			}
			else
			{
				cameraDirection = vec3.FORWARD * MathLib.RotateZ(debugCamera.horizontalAngle);
				cameraDirection = cameraDirection * MathLib.Rotate(MathLib.Cross(cameraDirection, vec3.UP), debugCamera.verticalAngle);
			}

			debugCamera.camera.SetWorldDirection(cameraDirection, vec3.UP);
			debugCamera.camera.WorldPosition = (worldTransform.Translate + vec3.UP * (shape.Radius + shape.Height * 0.5f)) - cameraDirection * debugCamera.distance;
		}

		// use visualizer
		if (debugVisualizer.enabled)
		{
			if (debugVisualizer.playerShape)
				shape.RenderVisualizer(debugColors.playerShape);

			if (debugVisualizer.playerDirection)
			{
				Vec3 p0 = worldTransform.Translate + Vec3.UP * (shape.Radius + shape.Height * 0.5f);
				Vec3 p1 = p0 + new vec3(worldTransform.AxisY);
				Visualizer.RenderVector(p0, p1, debugColors.playerDirection);
			}

			if (debugVisualizer.camera && camera)
			{
				Vec3 p0 = camera.WorldPosition;
				Vec3 p1 = p0 + camera.GetWorldDirection();
				Visualizer.RenderVector(p0, p1, debugColors.cameraColor);

				camera.RenderVisualizer();
			}

			if (debugVisualizer.slopeBasis)
			{
				Vec3 p0 = worldTransform.Translate;
				Visualizer.RenderVector(p0, p0 + SlopeAxisX, vec4.RED);
				Visualizer.RenderVector(p0, p0 + SlopeAxisY, vec4.GREEN);
				Visualizer.RenderVector(p0, p0 + SlopeNormal, vec4.BLUE);
			}

			if (debugVisualizer.appliedHorizontalVelocity)
			{
				Vec3 p0 = worldTransform.Translate + Vec3.UP * (shape.Radius + shape.Height * 0.5f);
				Vec3 p1 = p0 + HorizontalVelocity;
				Visualizer.RenderVector(p0, p1, debugColors.appliedHorizontalVelocity);
			}

			if (debugVisualizer.appliedVerticalVelocity)
			{
				Vec3 p0 = worldTransform.Translate + Vec3.UP * (shape.Radius + shape.Height * 0.5f);
				Vec3 p1 = p0 + vec3.UP * VerticalVelocity;
				Visualizer.RenderVector(p0, p1, debugColors.appliedVerticalVelocity);
			}
		}

		// use profiler
		if (debugProfiler.enabled)
		{
			if (debugProfiler.appliedHorizontalSpeed)
				Profiler.SetValue("Applied Horizontal Speed", "m/s", (float)HorizontalVelocity.Length, maxAppliedHorizontalSpeed, debugColors.arrayAppliedHorizontalVelocity);

			if (debugProfiler.appliedVerticalSpeed)
				Profiler.SetValue("|Applied Vertical Speed|", "m/s", MathLib.Abs(VerticalVelocity), maxAppliedVerticalSpeed, debugColors.arrayAppliedVerticalVelocity);

			if (debugProfiler.isGround)
				Profiler.SetValue("Is Ground", "", (IsGround ? 1.0f : 0.0f), maxFlagValue, debugColors.arrayIsGround);

			if (debugProfiler.isCeiling)
				Profiler.SetValue("Is Ceiling", "", (IsCeiling ? 1.0f : 0.0f), maxFlagValue, debugColors.arrayIsCeiling);

			if (debugProfiler.isCrouch)
				Profiler.SetValue("Is Crouch", "", (IsCrouch ? 1.0f : 0.0f), maxFlagValue, debugColors.arrayIsCrouch);

			if (debugProfiler.averageSpeed)
			{
				for (int i = 0; i < speedsBuffer.Length - 1; i++)
					speedsBuffer[i] = speedsBuffer[i + 1];

				speedsBuffer[speedsBuffer.Length - 1] = (float)(worldTransform.Translate - lastPlayerPosition).Length / Game.IFps;
				lastPlayerPosition = worldTransform.Translate;

				float avgSpeed = 0.0f;
				for (int i = 0; i < speedsBuffer.Length; i++)
					avgSpeed += speedsBuffer[i];

				avgSpeed /= (float)speedsBuffer.Length;

				Profiler.SetValue("Avg Speed", "m/s", avgSpeed, maxAppliedHorizontalSpeed * 1.75f, debugColors.arrayAverageSpeed);
			}

			if (debugProfiler.autoStepping)
				Profiler.SetValue("Auto Stepping", "", (autoSteppingApplied ? 1.0f : 0.0f), maxFlagValue, debugColors.arrayAutoStepping);
		}
	}
#endif

	#endregion Debug

}
