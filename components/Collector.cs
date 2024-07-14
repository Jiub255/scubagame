using Godot;
using System;

public partial class Collector : Area2D
{
	public Player Player { get; set; }

	public override void _Ready()
	{
		base._Ready();

		Player = GetParent<Player>();
		AreaEntered += OnCollectorAreaEntered;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		AreaEntered -= OnCollectorAreaEntered;
	}

	private void OnCollectorAreaEntered(Area2D otherArea)
	{
		//this.PrintDebug("Entered collector area");
		if (otherArea is ICollectable collectableArea)
		{
			collectableArea.GetCollected(Player);
		}
	}
}
