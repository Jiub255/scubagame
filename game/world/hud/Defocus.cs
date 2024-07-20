using Godot;
using System;

public partial class Defocus : MarginContainer
{
	public event Action OnDefocus;

	public override void _GuiInput(InputEvent @event) 
	{ 
		if (@event is InputEventMouseButton mb) 
		{ 
			if (mb.ButtonIndex == MouseButton.Left && mb.Pressed) 
			{
				this.PrintDebug("Clicked defocus");
				OnDefocus?.Invoke(); 
			} 
		} 
	}   	
}
