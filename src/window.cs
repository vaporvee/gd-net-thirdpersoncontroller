using Godot;
using System;

public partial class window : Node
{
	public override void _Ready()
    {
        OS.Alert("Make sure that you either capture your mouse or play with a gamepad. Jump with space or bottom gamepad face button. Remove this message in window.cs - vaporvee.com","Information");
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
