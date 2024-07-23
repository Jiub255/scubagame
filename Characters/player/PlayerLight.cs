using Godot;

public partial class PlayerLight : PointLight2D
{
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsActionPressed("toggle-light"))
		{
			Enabled = !Enabled;
		}
	}
}
