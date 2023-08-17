using Godot;
using System;

public partial class CameraController : Camera2D
{
	CharacterBody2D player;
	private bool isShaking = false;
	
	[Export]
	public float shakeTime = 20.0f;
	[Export]
	public double shakeAmount = 10.0f;

	private float currentShakeTime = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>("/root/Main/Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = new Vector2(player.Position.X, player.Position.Y);
		if (currentShakeTime <= 0f && isShaking == true){
			isShaking = false;
			this.Offset = new Vector2(0,0);
		}
		if (isShaking == true){
			ShakeCamera();
			currentShakeTime -= 1f;
		}
	}

	private void cannon_fired_handler()
	{
		isShaking = true;
		currentShakeTime = shakeTime;
	}

	private void ShakeCamera()
	{
		this.Offset = new Vector2(
			(float)GD.RandRange(-shakeAmount, shakeAmount),
			(float)GD.RandRange(-shakeAmount, shakeAmount)
		);
	}

	// Signals
	// This is terrible and I am sure there is a better way to handle it
	// but it will do for a prototype
	private void _on_cannon_top_1_cannon_fired() 
	{
		cannon_fired_handler();
	}
	private void _on_cannon_top_2_cannon_fired() 
	{
		cannon_fired_handler();
	}
	private void _on_cannon_top_3_cannon_fired() 
	{
		cannon_fired_handler();
	}
	private void _on_cannon_bottom_1_cannon_fired() 
	{
		cannon_fired_handler();
	}
	private void _on_cannon_bottom_2_cannon_fired() 
	{
		cannon_fired_handler();
	}
	private void _on_cannon_bottom_3_cannon_fired() 
	{
		cannon_fired_handler();
	}
}	
