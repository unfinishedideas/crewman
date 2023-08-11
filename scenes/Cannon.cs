using Godot;
using System;

public partial class Cannon : Area2D
{
	AnimatedSprite2D BarrelAnimator;
	AnimatedSprite2D StatusAnimator;
	AnimatedSprite2D LoadBarAnimator;
	enum CannonState {
		empty,
		loading,
		ready
	}
	private CannonState state = CannonState.ready;
	private bool in_interactable_range = false;
	private bool being_interacted = false;
	private float loading_status = 0.0f;
	int num_loadbar_frames = 1;

	[Export] public float loading_speed = 0.001f;
	[Export] public float interaction_speed_bonus = 0.003f;
	[Export] public bool flip_status = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BarrelAnimator = GetNode<AnimatedSprite2D>("BarrelAnimator");
		StatusAnimator = GetNode<AnimatedSprite2D>("StatusAnimator");
		LoadBarAnimator = GetNode<AnimatedSprite2D>("LoadBarAnimator");
		if (flip_status == true)
		{
			StatusAnimator.RotationDegrees = 180;
			LoadBarAnimator.RotationDegrees = 180;
		}

		StatusAnimator.Play();
		num_loadbar_frames = LoadBarAnimator.SpriteFrames.GetFrameCount("default");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (state == CannonState.loading && in_interactable_range == true)
		{
			if (loading_status >= 1.0f)
			{
				loading_status = 0;
				SetCannonState(CannonState.ready);
			}
			else 
			{
				loading_status += loading_speed;
				if (being_interacted)
				{
					loading_status += interaction_speed_bonus;
				}
				int frame = (int)Math.Floor(num_loadbar_frames * loading_status);
				LoadBarAnimator.SetFrameAndProgress(frame, 0);
			}
		}
	}

	private void InteractHandler()
	{
		if (in_interactable_range)
		{
			being_interacted = true;
			switch(state)
			{
				case CannonState.empty:
					SetCannonState(CannonState.loading);
					break;

				case CannonState.loading:
					SetCannonState(CannonState.loading);
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

	private void SetCannonState(CannonState new_state)
	{
		if (state != new_state)
		{
			state = new_state;
			switch(new_state)
			{
				case CannonState.empty:
					StatusAnimator.Animation = "empty";
					break;
				case CannonState.loading:
					StatusAnimator.Animation = "loading";
					LoadBarAnimator.Visible = true;
					break;
				case CannonState.ready:
					StatusAnimator.Animation = "ready";
					LoadBarAnimator.Visible = false;
					break;
				default:
					break;
			}
		}
	}

	// Signals ---------------------------------------------------------------------------------------------------------
	private void _on_body_entered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			in_interactable_range = true;
		}
	}

	private void _on_body_exited(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			in_interactable_range = false;
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
	private void _on_player_player_stopped_interacting()
	{
		being_interacted = false;
	}
}
