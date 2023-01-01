using Godot;
using System;

public partial class player : CharacterBody3D
{
	[Export] public const float Speed = 5.0f;
	[Export] public const float JumpVelocity = 4.5f;
	[Export] public const float gravity = 9f;
	[Export(PropertyHint.Range, "0.1,1.0")] float mouseSensitivity = 0.3f;
	[Export(PropertyHint.Range, "-90,0,1")] float minMousePitch = -90f;
    [Export(PropertyHint.Range, "0,90,1")] float maxMousePitch = 50f;
	public float allDelta;
	public Marker3D camCenter;

	public override void _Ready()
	{
		camCenter = GetNode<Marker3D>("camera_center");
	}
	public override void _PhysicsProcess(double delta)
	{
		allDelta = (float)delta;
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.y -= gravity * allDelta;
	
		if (Input.IsActionJustPressed("move_jump") && IsOnFloor())
			velocity.y = JumpVelocity;
		Vector2 camRotVector = new Vector2 ((float)Math.Cos(camCenter.Rotation.y), (float)Math.Sin(camCenter.Rotation.y));
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward") * camRotVector;
		Vector3 direction = new Vector3(inputDir.x * camRotVector.x, 0, inputDir.y * camRotVector.y);
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
		if (@event is InputEventMouseMotion mouseMotion)
		{
			Vector3 camRot = camCenter.RotationDegrees;
			camRot.y -= mouseMotion.Relative.x * mouseSensitivity;
			camRot.x -= mouseMotion.Relative.y * mouseSensitivity;
            camRot.x = Mathf.Clamp(camRot.x, minMousePitch, maxMousePitch);
			camCenter.RotationDegrees = camRot;
			
		}
	}
}
