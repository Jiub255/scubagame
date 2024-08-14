using Godot;

public partial class WheelScrollBar : ScrollContainer
{
	private int ScrollSpeed { get; set; } = 1;
	
	public override void _GuiInput(InputEvent @event)
	{
		base._GuiInput(@event);
		
		if (@event is InputEventMouseButton buttonEvent)
		{
			if (buttonEvent.ButtonIndex == MouseButton.WheelUp)
			{
				ScrollVertical += ScrollSpeed;
			}
			else if(buttonEvent.ButtonIndex == MouseButton.WheelDown)
			{
				ScrollVertical -= ScrollSpeed;
			}
		}
	}
}
