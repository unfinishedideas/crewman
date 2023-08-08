using Godot;
using System;

public partial class CameraController : Camera2D
{
	CharacterBody2D player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>("/root/Main/Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = new Vector2(player.Position.X, player.Position.Y);
	}
}
