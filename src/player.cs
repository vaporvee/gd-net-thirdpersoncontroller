using Godot;
using System;
using System.Diagnostics;

public partial class player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	[Export (PropertyHint.Range, "0.1,1.0")] float mouseSensitivity = 0.3f;
	[Export (PropertyHint.Range, "-90,0,1")] float minCamPitch = -90f;
	[Export (PropertyHint.Range, "0,90,1")] float maxCamPitch = 90f;
	public Marker3D cameraCenter;
	public Camera3D camera;
	public Vector3 camPosition;
	public Vector3 playerResetPosition;
    public override void _Ready()
	{
        cameraCenter = GetNode<Marker3D>("camera_center");
        camera = GetNode<Camera3D>("camera_center/spring_arm/camera");
        mouseSensitivity = mouseSensitivity * 0.005f;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("uncapture_mouse")) Input.MouseMode = Input.MouseModeEnum.Visible;
        if (Input.IsMouseButtonPressed(MouseButton.Left)) Input.MouseMode = Input.MouseModeEnum.Captured;//has to be disabled when the game has a pause menu
        if (Input.IsActionJustPressed("move_forward"))
		{
			playerResetPosition = Rotation;
            camPosition = cameraCenter.Rotation;
			playerResetPosition.y = camPosition.y;
			Rotation = playerResetPosition / 2;
			Vector3 cameraResetPosition = cameraCenter.Rotation; 
			cameraCenter.Rotation = new Vector3 (cameraResetPosition.x,0,0);
		}
    }
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.y -= gravity * (float)delta;

		if (Input.IsActionJustPressed("move_jump") && IsOnFloor())
			velocity.y = JumpVelocity;

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.x = direction.x * Speed;
			velocity.z = direction.z * Speed;
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
			Vector3 generalRot = Rotation;
            if (Input.IsActionPressed("move_forward")) generalRot.y -= motionEvent.Relative.x * mouseSensitivity;
            Rotation = generalRot;

            generalRot = cameraCenter.Rotation;
            if (!Input.IsActionPressed("move_forward")) generalRot.y -= motionEvent.Relative.x * mouseSensitivity;
            generalRot.x -= motionEvent.Relative.y * mouseSensitivity;
            generalRot.x = Mathf.Clamp(generalRot.x, minCamPitch, maxCamPitch);
			cameraCenter.Rotation = generalRot;
            generalRot = cameraCenter.Rotation;
        }
	}
}
