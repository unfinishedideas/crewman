using Godot;
using System;

// A lot of this code comes from the Godot docs here: https://docs.godotengine.org/en/stable/getting_started/first_2d_game/03.coding_the_player.html
// and here: https://docs.godotengine.org/en/stable/tutorials/physics/using_character_body_2d.html

public partial class Player : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

	private bool facingRight;
	AnimatedSprite2D PlayerAnimator;

    [Signal] public delegate void PlayerInteractingEventHandler();
    [Signal] public delegate void PlayerStoppedInteractingEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		facingRight = true;
		PlayerAnimator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		PlayerAnimator.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Input Check
        var velocity = Vector2.Zero; // The player's movement vector.
        if (Input.IsActionPressed("move_right")) {
            velocity.X += 1;
            facingRight = true;
        }
        if (Input.IsActionPressed("move_left")) {
            velocity.X -= 1;
            facingRight = false;
        }
        if (Input.IsActionPressed("move_down")) {
            velocity.Y += 1;
        }
        if (Input.IsActionPressed("move_up")) {
            velocity.Y -= 1;
        }

        // Animate
        // https://ask.godotengine.org/92282/why-my-character-scale-keep-changing?show=146969#a146969
        // Do not modify scale.x. Instead, set scale.y = -1 and rotation_degrees = 180. To revert, set scale.y = 1 and rotation_degrees = 0.
        if (facingRight) {
            Scale = new Vector2(1, 1);
            RotationDegrees = 0;
        } else {
            Scale = new Vector2(1, -1);
            RotationDegrees = 180;
        }

        // Interacting Signal
        bool is_interacting = false;
		if (Input.IsActionPressed("interact")) {
            is_interacting = true;
            EmitSignal(SignalName.PlayerInteracting);
		}
        else {
            EmitSignal(SignalName.PlayerStoppedInteracting);
        }

        // Move character and animate
        if (velocity.Length() > 0) {
            velocity = velocity.Normalized() * Speed;
            PlayerAnimator.Animation = "walk";
        }
        else {
            if (is_interacting == true) {
                PlayerAnimator.Animation = "interact";
            }
            else {
                PlayerAnimator.Animation = "idle";
            }
        }

        MoveAndCollide(velocity * (float)delta);
	}
}
