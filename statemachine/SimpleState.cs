using Godot;
using System;
using System.Collections.Generic;

// Based on the state machine tutorial found here: https://www.youtube.com/watch?v=dfp7FIO4GTA
public partial class SimpleState : Node
{
	private bool HasBeenInitialized = false;
	private bool OnUpdateHasFired = false;

	// State Events
	[Signal] public delegate void StateStartEventHandler();
	[Signal] public delegate void StateUpdatedEventHandler();
	[Signal] public delegate void StateExitedEventHandler();

	public virtual void OnStart(Dictionary<string, object> message)
	{
		EmitSignal(nameof(StateStartEventHandler));
		HasBeenInitialized = true;
	}

	// Only should be called once, probably wont need to overwrite but you could
	public virtual void OnUpdate()
	{
		if (!HasBeenInitialized)
		{
			return;
		}
		EmitSignal(nameof(StateUpdatedEventHandler));
		OnUpdateHasFired = true;
	}

	// called every frame. You will rewrite this
	public virtual void UpdateState(double dt)
	{
		if (!OnUpdateHasFired)
		{
			return;
		}
	}

	public virtual void OnExit(string nextState)
	{
		if (!HasBeenInitialized)
		{
			return;
		}
		EmitSignal(nameof(StateExitedEventHandler));
		HasBeenInitialized = false;
		OnUpdateHasFired = false;
	}


}
