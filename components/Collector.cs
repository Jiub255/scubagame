using Godot;
using System;

public partial class Collector : Area2D
{
	public Player3 Player { get; set; }

	public override void _Ready()
	{
		base._Ready();

		Player = GetParent<Player3>();
		AreaEntered += OnCollectorAreaEntered;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		AreaEntered -= OnCollectorAreaEntered;
	}

	private void OnCollectorAreaEntered(Area2D otherArea)
	{
		GD.Print("Entered area");
		if (otherArea is ICollectable collectableArea)
		{
			collectableArea.GetCollected(Player);
		}
	}
}
