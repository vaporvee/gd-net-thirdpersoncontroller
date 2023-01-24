using Godot;
using System;

public partial class window : Node
{
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
