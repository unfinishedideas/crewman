using Godot;
using System;


public partial class Cannon : Area2D
{
	AnimatedSprite2D BarrelAnimator;
	AnimatedSprite2D StatusAnimator;
	AnimatedSprite2D LoadBarAnimator;
	Sprite2D CannonSprite;

	AudioStreamPlayer2D CannonFireSFX;
	AudioStreamPlayer2D CannonReloadSFX;
	AudioStreamPlayer InteractWithCannonSFX;
	
	Marker2D Muzzle;

	// Mess with these in the editor to tweak behavior!
	[Export] public float loading_speed = 0.001f;
	[Export] public float interaction_speed_bonus = 0.003f;
	[Export] public float cooldown_speed = 0.002f;
	[Export] public bool flip_status_icons = false;
	[Export] public float fired_sprite_offset = 10.0f;
	[Export] public float interacting_reload_sfx_pitch_adjust = 1.2f;

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

	// Signals
	[Signal] public delegate void CannonFiredEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BarrelAnimator = GetNode<AnimatedSprite2D>("BarrelAnimator");
		StatusAnimator = GetNode<AnimatedSprite2D>("StatusAnimator");
		LoadBarAnimator = GetNode<AnimatedSprite2D>("LoadBarAnimator");
		CannonFireSFX = GetNode<AudioStreamPlayer2D>("CannonFireSFX");
		CannonReloadSFX = GetNode<AudioStreamPlayer2D>("CannonReloadSFX");
		InteractWithCannonSFX = GetNode<AudioStreamPlayer>("InteractWithCannonSFX");
		Muzzle = GetNode<Marker2D>("Muzzle");
		CannonSprite = GetNode<Sprite2D>("CannonSprite");

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
		CannonSprite.Offset = new Vector2(fired_sprite_offset * loading_status, 0.0f);
		LoadBarAnimator.SetFrameAndProgress(
			GetBarFrameByPercentage(num_coolbar_frames, loading_status), 
			0
		);
		if (loading_status <= 0) {
			CannonSprite.Offset = new Vector2(0.0f, 0.0f);
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
		CannonSprite.Offset = new Vector2(fired_sprite_offset, 0.0f);

		CannonFireSFX.Play();

		// Load the cannonball scene and place it in the muzzle
		var CannonballScene = GD.Load<PackedScene>("res://scenes/Cannonball.tscn");
		var Ball = CannonballScene.Instantiate<Cannonball>();
		Owner.AddChild(Ball);
		Ball.Transform = Muzzle.GlobalTransform;
		
		// Emit signals 
		EmitSignal(SignalName.CannonFired);
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
					CannonReloadSFX.Play();
					LoadBarAnimator.Visible = true;
					StatusAnimator.Visible = true;
					break;
				case CannonState.ready:
					CannonReloadSFX.Stop();
					InteractWithCannonSFX.Stop();
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
				CannonReloadSFX.StreamPaused = false;
			}
		}
	}

	private void _on_body_exited(Node2D body)
	{
		if (body.IsInGroup("Player")) {
			in_interactable_range = false;
			if (state == CannonState.loading){
				CannonReloadSFX.StreamPaused = true;
				StatusAnimator.Stop();
				if (InteractWithCannonSFX.Playing == true){
					InteractWithCannonSFX.Stop();
				}
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
		if (state == CannonState.loading) {
			if (InteractWithCannonSFX.Playing == false)	{
				InteractWithCannonSFX.Play();
			}
			CannonReloadSFX.PitchScale = interacting_reload_sfx_pitch_adjust;
		}
	}
	private void _on_player_player_stopped_interacting()
	{
		being_interacted = false;
		if (state == CannonState.loading) {
			if (InteractWithCannonSFX.Playing == true) {
				InteractWithCannonSFX.Stop();
			}
			CannonReloadSFX.PitchScale = 1.0f;
		}
		if (state == CannonState.ready) {
			ready_to_fire = true;
		}
	}
}
