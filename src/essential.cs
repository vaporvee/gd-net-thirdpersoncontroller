using Godot;
using System;

public partial class essential : Node
{
	public override void _Ready()
	{
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
