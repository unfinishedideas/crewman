using Godot;
using System;

public partial class Cannon : Area2D
{
	AnimatedSprite2D BarrelAnimator;
	enum CannonState {
		empty,
		loading,
		ready
	}
	private CannonState state = CannonState.ready;
	private bool being_interacted = false;
	private float loading_status = 0.0f;

	[Export] public float loading_speed = 0.1f;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BarrelAnimator = GetNode<AnimatedSprite2D>("BarrelAnimator");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (state == CannonState.loading && being_interacted == true)
		{
			if (loading_status >= 100.0f)
			{
				loading_status = 0;
				state = CannonState.ready;
			}
			else 
			{
				GD.Print(loading_status);
				loading_status += loading_speed;
			}
		}
	}

	private void InteractHandler()
	{
		if (being_interacted)
		{
			switch(state)
			{
				case CannonState.empty:
					state = CannonState.loading;
					break;

				case CannonState.loading:
					break;

				case CannonState.ready:
					FireCannon();
					break;

				default:
					break;
			}
		}
	}

	private void FireCannon()
	{
		state = CannonState.empty;
		BarrelAnimator.Visible = true;
		BarrelAnimator.Animation = "fire";
		BarrelAnimator.Play();
	}

	// Signals ---------------------------------------------------------------------------------------------------------
	private void _on_body_entered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			being_interacted = true;
		}
	}

	private void _on_body_exited(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			being_interacted = false;
		}
	}

	private void _on_barrel_animator_animation_finished() 
	{
		BarrelAnimator.Visible = false;
	}

	private void _on_player_player_interacting()
	{
		InteractHandler();
	}
}
