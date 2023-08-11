using Godot;
using System;

public partial class MastVisibilityMask : Area2D
{
	Node2D ParentNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ParentNode = GetParent<Node2D>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_body_entered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			ParentNode.Modulate = new Color(0,0,0,0.2f);
		}
	} 
	private void _on_body_exited(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			ParentNode.Modulate = new Color(1,1,1,1);
		}
	} 
}
