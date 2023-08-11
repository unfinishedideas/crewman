using Godot;
using System;

public partial class Cannon : Area2D
{
	AnimatedSprite2D BarrelAnimator;
	AnimatedSprite2D StatusAnimator;
	AnimatedSprite2D LoadBarAnimator;

	// Mess with these in the editor to tweak behavior!
	[Export] public float loading_speed = 0.001f;
	[Export] public float interaction_speed_bonus = 0.003f;
	[Export] public float cooldown_speed = 0.002f;
	[Export] public bool flip_status_icons = false;

	// Private Variables for use in the script
	enum CannonState {
		empty,
		loading,
		ready,
		cooldown
	}
	private CannonState state = CannonState.empty;
	private bool has_fired = false;
	private bool ready_to_fire  = false;

	private bool in_interactable_range = false;
	private bool being_interacted = false;

	private float loading_status = 0.0f;
	private int num_loadbar_frames = 1;
	private int num_coolbar_frames = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BarrelAnimator = GetNode<AnimatedSprite2D>("BarrelAnimator");
		StatusAnimator = GetNode<AnimatedSprite2D>("StatusAnimator");
		LoadBarAnimator = GetNode<AnimatedSprite2D>("LoadBarAnimator");
		if (flip_status_icons == true) {
			StatusAnimator.RotationDegrees = 180;
			LoadBarAnimator.RotationDegrees = 180;
		}
		StatusAnimator.Animation = "loading";
		StatusAnimator.Play();
		num_loadbar_frames = LoadBarAnimator.SpriteFrames.GetFrameCount("loading");
		num_coolbar_frames = LoadBarAnimator.SpriteFrames.GetFrameCount("cooldown");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Loading The Cannon
		if (state == CannonState.loading && in_interactable_range == true){
			LoadCannon();
		}
		// Cooldown
		else if (state == CannonState.cooldown && has_fired == true){
			CooldownCannon();
		}
	}

	// Cannon Loading / Cooldown ---------------------------------------------------------------------------------------
	private void LoadCannon()
	{
		if (loading_status >= 1.0f) {
			loading_status = 1.0f;
			SetCannonState(CannonState.ready);
		}
		else {
			loading_status += loading_speed;
			if (being_interacted) {
				StatusAnimator.SpeedScale = 4.0f;
				loading_status += interaction_speed_bonus;
			}
			else
			{
				StatusAnimator.SpeedScale = 1.0f;
			}
			LoadBarAnimator.SetFrameAndProgress(
				GetBarFrameByPercentage(num_loadbar_frames, loading_status),
				0
			);
		}
	}

	private void CooldownCannon()
	{
		loading_status -= cooldown_speed;
		LoadBarAnimator.SetFrameAndProgress(
			GetBarFrameByPercentage(num_coolbar_frames, loading_status), 
			0
		);
		if (loading_status <= 0) {
			has_fired = false;
			loading_status = 0.0f;
			SetCannonState(CannonState.empty);
		}
	}

	private int GetBarFrameByPercentage(int num_frames, float status)
	{
		return (int)Math.Floor(num_frames * status);
	}

	// Interactions ----------------------------------------------------------------------------------------------------
	private void InteractHandler()
	{
		if (in_interactable_range) {
			being_interacted = true;
			switch(state)
			{
				case CannonState.empty:
					SetCannonState(CannonState.loading);
					LoadBarAnimator.Animation = "loading";
					break;

				case CannonState.loading:
					break;

				case CannonState.ready:
					if (ready_to_fire == true) {
						FireCannon();
					}
					break;
				
				case CannonState.cooldown:
					LoadBarAnimator.Animation = "cooldown";
					break;

				default:
					break;
			}
		}
	}

	private void FireCannon()
	{
		has_fired = true;
		BarrelAnimator.Visible = true;
		BarrelAnimator.Animation = "fire";
		BarrelAnimator.Play();
		SetCannonState(CannonState.cooldown);
	}

	private void SetCannonState(CannonState new_state)
	{
		if (state != new_state) {
			state = new_state;
			switch(new_state)
			{
				case CannonState.empty:
					ready_to_fire = false;
					LoadBarAnimator.Visible = false;
					StatusAnimator.Visible = false;
					break;
				case CannonState.loading:
					LoadBarAnimator.Visible = true;
					StatusAnimator.Visible = true;
					break;
				case CannonState.ready:
					LoadBarAnimator.Animation = "ready";
					StatusAnimator.Visible = false;
					break;
				case CannonState.cooldown:
					LoadBarAnimator.Animation = "cooldown";
					break;
				default:
					break;
			}
		}
	}

	// Signals ---------------------------------------------------------------------------------------------------------
	private void _on_body_entered(Node2D body)
	{
		if (body.IsInGroup("Player")) {
			in_interactable_range = true;
			if (state == CannonState.loading){
				StatusAnimator.Play();
			}
		}
	}

	private void _on_body_exited(Node2D body)
	{
		if (body.IsInGroup("Player")) {
			in_interactable_range = false;
			if (state == CannonState.loading){
				StatusAnimator.Stop();
			}
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
		if (state == CannonState.ready) {
			ready_to_fire = true;
		}
	}
}
