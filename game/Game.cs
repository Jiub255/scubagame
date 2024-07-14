using Godot;
using System;

public partial class Game : Node
{
	[Export]
	private Ui _ui;
	[Export]
	private Node2D _world;
	
	private Ui UI 
	{
		get { return _ui; }
	}
	private Node2D World 
	{
		get { return _world; }
	}
	

	public override void _Ready()
	{
		base._Ready();

		UI.Hide();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsActionPressed("menu"))
		{
			if (World.ProcessMode == ProcessModeEnum.Inherit)
			{
				World.ProcessMode = ProcessModeEnum.Disabled;
				UI.Show();
			}
			else
			{
				World.ProcessMode = ProcessModeEnum.Inherit;
				UI.Hide();
			}
		}
	}
}
