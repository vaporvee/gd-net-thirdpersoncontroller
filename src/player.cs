using Godot;
using System;

public partial class player : CharacterBody3D
{
	public float gotDelta;
	public const float Speed = 5.0f;
    [Export] float JumpVelocity = 5.5f;
	[Export] float gravity = 14f;
	[Export (PropertyHint.Range, "0.1,1.0")] float mouseSensitivity = 0.3f;
	[Export (PropertyHint.Range, "-90,0,1")] float minCamPitch = -90f;
	[Export (PropertyHint.Range, "0,90,1")] float maxCamPitch = 90f;
	public Marker3D cameraCenter;
	public Camera3D camera;
	public Vector3 camPosition;
	public Vector3 playerResetPosition;
	//TODO: Add camera max rotation on ground (Preventing turning camera to upside down)
	//TODO: just rotate model not just everything
	//NOTICE: Since Beta 7 of Godot 4 the turning isn't smooth anymore
    public override void _Ready()
	{
        cameraCenter = GetNode<Marker3D>("camera_center");
        camera = GetNode<Camera3D>("camera_center/spring_arm/camera");
		mouseSensitivity = mouseSensitivity / 2;
		minCamPitch = minCamPitch + (float)Math.PI / 2;
		maxCamPitch = maxCamPitch + (float)Math.PI / 2;
		Input.MouseMode = Input.MouseModeEnum.Captured;
    }
	public override void _Process(double delta)
	{
		gotDelta = (float)delta;
        if (Input.IsActionJustPressed("uncapture_mouse")) Input.MouseMode = Input.MouseModeEnum.Visible;
        if (Input.IsMouseButtonPressed(MouseButton.Left)) Input.MouseMode = Input.MouseModeEnum.Captured;//just uncapture to also disable player and camera movement
    }
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.y -= gravity * (float)delta;

		if (Input.IsActionJustPressed("move_jump" ) && Input.MouseMode == Input.MouseModeEnum.Captured && IsOnFloor())
			velocity.y = JumpVelocity;

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();
		if (direction != Vector3.Zero && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			velocity.x = direction.x * Speed;
			velocity.z = direction.z * Speed;

            //checks for movement and adjusts the character direction with camera roatation
            playerResetPosition = Rotation;
            camPosition = cameraCenter.Rotation;
			playerResetPosition.y = camPosition.y * -1;
            Rotation -= playerResetPosition;
            cameraCenter.Rotation = new Vector3(camPosition.x, 0, 0);
        }
		else
		{
			velocity.x = Mathf.MoveToward(Velocity.x, 0, Speed);
			velocity.z = Mathf.MoveToward(Velocity.z, 0, Speed);
        }

		Velocity = velocity;

		MoveAndSlide();
	}
	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseMotion motionEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			Vector3 generalRot = cameraCenter.Rotation;
            generalRot.y -= motionEvent.Relative.x * mouseSensitivity * (float)gotDelta;
            generalRot.x -= motionEvent.Relative.y * mouseSensitivity * (float)gotDelta;
            //generalRot.x = Mathf.Clamp(generalRot.x, minCamPitch, maxCamPitch); // doesn't do anything?
			cameraCenter.Rotation = generalRot;
            generalRot = cameraCenter.Rotation;
        }
	}
}
