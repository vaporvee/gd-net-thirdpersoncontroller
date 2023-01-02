using Godot;
using System;

public partial class player : CharacterBody3D
{
	[Export] float Speed = 5.0f;
	[Export] float JumpVelocity = 5f;
	[Export] float gravity = 14f;
	[Export(PropertyHint.Range, "0.1,1.0")] float mouseSensitivity = 0.3f;
	[Export(PropertyHint.Range, "-90,0,1")] float minMousePitch = -90f;
    [Export(PropertyHint.Range, "0,90,1")] float maxMousePitch = 50f;

	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("uncapture_mouse")) Input.MouseMode = Input.MouseModeEnum.Visible;
        if (Input.IsMouseButtonPressed(MouseButton.Left)) Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.y -= gravity * (float)delta;
	
		if (Input.IsActionJustPressed("move_jump") && IsOnFloor() && Input.MouseMode == Input.MouseModeEnum.Captured)
			velocity.y = JumpVelocity;

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = new Vector3(inputDir.x, 0, inputDir.y).Rotated(Vector3.Up, GetNode<Marker3D>("camera_center").Rotation.y).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.x = direction.x * Speed;
			velocity.z = direction.z * Speed;
			Vector3 bodyRotation = GetNode<MeshInstance3D>("collision/body").Rotation;
			bodyRotation.y = Mathf.LerpAngle(bodyRotation.y,Mathf.Atan2(-direction.x, -direction.z), (float)delta * Speed);
			GetNode<MeshInstance3D>("collision/body").Rotation = bodyRotation;
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
		if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			Vector3 camRot = GetNode<Marker3D>("camera_center").RotationDegrees;
			camRot.y -= mouseMotion.Relative.x * mouseSensitivity;
			camRot.x -= mouseMotion.Relative.y * mouseSensitivity;
            camRot.x = Mathf.Clamp(camRot.x, minMousePitch, maxMousePitch);
            GetNode<Marker3D>("camera_center").RotationDegrees = camRot;
		}
	}
}
