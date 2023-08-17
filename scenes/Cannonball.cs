using Godot;
using System;


public partial class Cannonball : Area2D
{
	[Export] public float speed = 1700f;
	[Export] public float max_distance = 600;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// So the cannonballs don't hide behind the ship when shot out the bottom
		ZIndex = 10;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	

	public override void _PhysicsProcess(double delta)
	{
		Position += -Transform.Y * speed * (float)delta;
	}
}

// func _on_Bullet_body_entered(body):
//     if body.is_in_group("mobs"):
//         body.queue_free()
//     queue_free()
