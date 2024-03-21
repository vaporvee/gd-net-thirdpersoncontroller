using Godot;

public partial class player : CharacterBody3D
{
	public Vector3 direction;
	public Vector2 gpCamVector;
	public bool gamepadMode;
	private Marker3D cameraCenter {get; set;}
	private MeshInstance3D collisionBody {get; set;}
	[Export] float speed = 5.0f;
	[Export] float jumpVelocity = 5f;
	[Export] float gravity = 14f;
	[Export(PropertyHint.Range, "0.1,1.0")] float camSensitivity = 0.3f;
	[Export(PropertyHint.Range, "-90,0,1")] float minCamPitch = -50f;
	[Export(PropertyHint.Range, "0,90,1")] float maxCamPitch = 30f;

	public override void _Ready() {
		cameraCenter = GetNode<Marker3D>("camera_center");
		collisionBody = GetNode<MeshInstance3D>("collision/body");
	}

	public override void _Process(double delta)
	{
		//uncapturing the mouse disables PC movement but still simulates gravity
		if (Input.IsActionJustPressed("uncapture_mouse")) Input.MouseMode = Input.MouseModeEnum.Visible;
		if (Input.IsMouseButtonPressed(MouseButton.Left)) Input.MouseMode = Input.MouseModeEnum.Captured;
		
		/**body rotation is in regular process because it lags in physicsprocess and is more a animation anyway
			maybe rotate extra collisions separately for invisible lag that may occur**/
		if (direction != Vector3.Zero && Input.MouseMode == Input.MouseModeEnum.Captured | gamepadMode)
		{
			Vector3 bodyRotation = collisionBody.Rotation;
			bodyRotation.Y = Mathf.LerpAngle(bodyRotation.Y,Mathf.Atan2(-direction.X, -direction.Z), (float)delta * speed);
			collisionBody.Rotation = bodyRotation;
		}
		//camera gamepad part (needs constant movement so _Input event wouldn't work with this like mouseinput)
		gpCamVector = Input.GetVector("gp_cam_left", "gp_cam_right", "gp_cam_up", "gp_cam_down");
		if(gpCamVector != Vector2.Zero)
		{
			Vector3 camRot = cameraCenter.RotationDegrees;
			camRot.Y -= gpCamVector.X * camSensitivity * (float)delta * 500;
			camRot.X -= gpCamVector.Y * camSensitivity * (float)delta * 500;
			camRot.X = Mathf.Clamp(camRot.X, minCamPitch, maxCamPitch); //prevents camera from going endlessly around the player
			cameraCenter.RotationDegrees = camRot;
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta; //characterbodys don't have physic simulations by default like rigidbody
	
		if (Input.IsActionJustPressed("move_jump") && IsOnFloor() && Input.MouseMode == Input.MouseModeEnum.Captured | gamepadMode)
			velocity.Y = jumpVelocity;

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		direction = new Vector3(inputDir.X, 0, inputDir.Y).Rotated(Vector3.Up, GetNode<Marker3D>("camera_center").Rotation.Y).Normalized(); //rotates the input direction with camera rotation
		if (direction != Vector3.Zero && Input.MouseMode == Input.MouseModeEnum.Captured | gamepadMode)
		{
			velocity.X = direction.X * speed * (float)delta * 60;
			velocity.Z = direction.Z * speed * (float)delta * 60;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}
		Velocity = velocity;
		MoveAndSlide();
	}
	public override void _Input(InputEvent @event)
	{
		gamepadMode = @event is InputEventJoypadButton | @event is InputEventJoypadMotion;
		Vector3 camRot = cameraCenter.RotationDegrees;
		if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			camRot.Y -= mouseMotion.Relative.X * camSensitivity;
			camRot.X -= mouseMotion.Relative.Y * camSensitivity;
		}
		camRot.X = Mathf.Clamp(camRot.X, minCamPitch, maxCamPitch); //prevents camera from going endlessly around the player
		cameraCenter.RotationDegrees = camRot;
	}
}
