using Godot;

public partial class player : CharacterBody3D
{
	public Vector3 direction;
	public Vector2 gpCamVector;
	public bool gamepadMode;
	[Export] float speed = 5.0f;
	[Export] float jumpVelocity = 5f;
	[Export] float gravity = 14f;
	[Export(PropertyHint.Range, "0.1,1.0")] float camSensitivity = 0.3f;
	[Export(PropertyHint.Range, "-90,0,1")] float minCamPitch = -50f;
	[Export(PropertyHint.Range, "0,90,1")] float maxCamPitch = 30f;

	public override void _Process(double delta)
	{
		//uncapturing the mouse disables PC movement but still simulates gravity
		if (Input.IsActionJustPressed("uncapture_mouse")) Input.MouseMode = Input.MouseModeEnum.Visible;
		if (Input.IsMouseButtonPressed(MouseButton.Left)) Input.MouseMode = Input.MouseModeEnum.Captured;
		
		/**body rotation is in regular process because it lags in physicsprocess and is more a animation anyway
			maybe rotate extra collisions separately for invisible lag that may occur**/
		if (direction != Vector3.Zero && Input.MouseMode == Input.MouseModeEnum.Captured | gamepadMode)
		{
			Vector3 bodyRotation = GetNode<MeshInstance3D>("collision/body").Rotation;
			bodyRotation.y = Mathf.LerpAngle(bodyRotation.y,Mathf.Atan2(-direction.x, -direction.z), (float)delta * speed);
			GetNode<MeshInstance3D>("collision/body").Rotation = bodyRotation;
		}
		//camera gamepad part (needs constant movement so _Input event wouldn't work with this like mouseinput)
		gpCamVector = Input.GetVector("gp_cam_left", "gp_cam_right", "gp_cam_up", "gp_cam_down");
		if(gpCamVector != Vector2.Zero)
		{
			Vector3 camRot = GetNode<Marker3D>("camera_center").RotationDegrees;
			camRot.y -= gpCamVector.x * camSensitivity * 500 * (float)delta;
			camRot.x -= gpCamVector.y * camSensitivity * 500 * (float)delta;
			camRot.x = Mathf.Clamp(camRot.x, minCamPitch, maxCamPitch); //prevents camera from going endlessly around the player
			GetNode<Marker3D>("camera_center").RotationDegrees = camRot;
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.y -= gravity * (float)delta; //characterbodys don't have physic simulations by default like rigidbody
	
		if (Input.IsActionJustPressed("move_jump") && IsOnFloor() && Input.MouseMode == Input.MouseModeEnum.Captured | gamepadMode)
			velocity.y = jumpVelocity;

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		direction = new Vector3(inputDir.x, 0, inputDir.y).Rotated(Vector3.Up, GetNode<Marker3D>("camera_center").Rotation.y).Normalized(); //rotates the input direction with camera rotation
		if (direction != Vector3.Zero && Input.MouseMode == Input.MouseModeEnum.Captured | gamepadMode)
		{
			velocity.x = direction.x * speed;
			velocity.z = direction.z * speed;
		}
		else
		{
			velocity.x = Mathf.MoveToward(Velocity.x, 0, speed);
			velocity.z = Mathf.MoveToward(Velocity.z, 0, speed);
		}
		Velocity = velocity;
		MoveAndSlide();
	}
	public override void _Input(InputEvent @event)
	{
		gamepadMode = @event is InputEventJoypadButton | @event is InputEventJoypadMotion;
		Vector3 camRot = GetNode<Marker3D>("camera_center").RotationDegrees;
		if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			camRot.y -= mouseMotion.Relative.x * camSensitivity;
			camRot.x -= mouseMotion.Relative.y * camSensitivity;
		}
		camRot.x = Mathf.Clamp(camRot.x, minCamPitch, maxCamPitch); //prevents camera from going endlessly around the player
		GetNode<Marker3D>("camera_center").RotationDegrees = camRot;
	}
}
