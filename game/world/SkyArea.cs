using Godot;

public partial class SkyArea : Area2D
{
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		BodyEntered -= OnBodyEntered;
		BodyExited -= OnBodyExited;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is ICanExitWater waterExiter)
		{
			//this.PrintDebug("Player hit sky");
			waterExiter.ExitWater();
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body is ICanEnterWater waterEnterer)
		{
			//this.PrintDebug("Player left sky");
			waterEnterer.EnterWater();
		}
	}
}
