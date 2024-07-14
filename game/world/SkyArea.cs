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
			// TODO: Disable controls, set gravity. 
			// Do in airborne state. 
			GD.Print("Player hit sky");
			waterExiter.ExitWater();
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body is ICanEnterWater waterEnterer)
		{
			// TODO: Enable controls, disable gravity. 
			// Return to movement state. 
			GD.Print("Player left sky");
			waterEnterer.EnterWater();
		}
	}
}
