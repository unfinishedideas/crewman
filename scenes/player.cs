using Godot;
using System;

// A lot of this code comes from the Godot docs here: https://docs.godotengine.org/en/stable/getting_started/first_2d_game/03.coding_the_player.html
public partial class player : Area2D
{
	[Export]
    public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).
    public Vector2 ScreenSize; // Size of the game window.

	private bool facingRight;
	private bool interacting;
	AnimatedSprite2D animatedSprite2D;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		interacting = false;
		facingRight = true;
		// TODO: Error handling in case it can't find the AnimatedSprite2D
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Input Check
		if (Input.IsActionPressed("interact"))
		{
			animatedSprite2D.Animation = "interact";
		}
		else
		{
			var velocity = Vector2.Zero; // The player's movement vector.
			if (Input.IsActionPressed("move_right"))
			{
				velocity.X += 1;
				facingRight = true;
			}
			if (Input.IsActionPressed("move_left"))
			{
				velocity.X -= 1;
				facingRight = false;
			}
			if (Input.IsActionPressed("move_down"))
			{
				velocity.Y += 1;
			}
			if (Input.IsActionPressed("move_up"))
			{
				velocity.Y -= 1;
			}

			// Animate
			var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			if (facingRight) {
				Scale = new Vector2 (1, 1);
			} else {
				Scale = new Vector2 (-1, 1);
			}

			if (velocity.Length() > 0)
			{
				velocity = velocity.Normalized() * Speed;
				animatedSprite2D.Animation = "walk";
			}
			else
			{
				animatedSprite2D.Animation = "idle";
			}

			// Move the player
			Position += velocity * (float)delta;
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
				y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
			);
		}
	}
}
