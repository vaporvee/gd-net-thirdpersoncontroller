using Godot;
using System;

public partial class window : Node
{
    public override async void _Ready()
    {
        /* VSync should be used because needing to change the PhysicsTicksPerSecond in _Process takes CPU ussage
         * You could also just set max FPS and delte this fix
         */
        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");//waits until the game has loaded some time
        if (Engine.PhysicsTicksPerSecond != (int)Engine.GetFramesPerSecond())
        {
            Engine.PhysicsTicksPerSecond = (int)Engine.GetFramesPerSecond(); //PhysicsTicksPerSecond have to be the same value like current FPS or the movement will lagg
            GD.Print("Set PhysicsTicksPerSecond to: " + Engine.PhysicsTicksPerSecond);
        }
    }
    public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("fullscreen"))
        {
            if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
    }
}
