using System;
using System.Reflection;
using Godot;

public partial class DebugHud : CanvasLayer
{
	[Export]
	private PlayerData Data { get; set; }

	[Export]
	private LineEdit MinAcceleration { get; set; }
	[Export]
	private LineEdit MaxAcceleration { get; set; }
	[Export]
	private LineEdit MaxSpeed { get; set; }
	[Export]
	private LineEdit Jerk { get; set; }
	[Export]
	private LineEdit Deceleration { get; set; }
	
	private Defocus MarginContainer { get; set; }

	public override void _Ready()
	{
		base._Ready();
		
		MarginContainer = GetNode<Defocus>("MarginContainer");
		MarginContainer.OnDefocus += ReleaseFocus;

		//Data.ScubaGear.Changed += UpdateDebugHUD;
		MinAcceleration.TextChanged += (text) => OnStatChanged(text, "MinAcceleration");
		MaxAcceleration.TextChanged += (text) => OnStatChanged(text, "MaxAcceleration");
		MaxSpeed.TextChanged += (text) => OnStatChanged(text, "MaxSpeed");
		Jerk.TextChanged += (text) => OnStatChanged(text, "Jerk");
		Deceleration.TextChanged += (text) => OnStatChanged(text, "Deceleration");

		InitializeDebugHUD();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		MarginContainer.OnDefocus -= ReleaseFocus;

		//Data.ScubaGear.Changed -= UpdateDebugHUD;
		MinAcceleration.TextChanged -= (text) => OnStatChanged(text, "MinAcceleration");
		MaxAcceleration.TextChanged -= (text) => OnStatChanged(text, "MaxAcceleration");
		MaxSpeed.TextChanged -= (text) => OnStatChanged(text, "MaxSpeed");
		Jerk.TextChanged -= (text) => OnStatChanged(text, "Jerk");
		Deceleration.TextChanged -= (text) => OnStatChanged(text, "Deceleration");
	}
	
	private void ReleaseFocus()
	{
		MinAcceleration.ReleaseFocus();
		MaxAcceleration.ReleaseFocus();
		MaxSpeed.ReleaseFocus();
		Jerk.ReleaseFocus();
		Deceleration.ReleaseFocus();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
		if (@event.IsActionPressed("ui_text_submit"))
		{
			ReleaseFocus();
		}
	}

	private void InitializeDebugHUD()
	{
		// TODO: Populate all LineEdits with data. 
		MinAcceleration.Text = Data.ScubaGear.MinAcceleration.ToString();
		MaxAcceleration.Text = Data.ScubaGear.MaxAcceleration.ToString();
		MaxSpeed.Text = Data.ScubaGear.MaxSpeed.ToString();
		Jerk.Text = Data.ScubaGear.Jerk.ToString();
		Deceleration.Text = Data.ScubaGear.Deceleration.ToString();
	}
	
	private void OnStatChanged(string newText, string statName)
	{
		if (int.TryParse(newText, out var value))
		{
			Type type = Data.ScubaGear.GetType();
			
			PropertyInfo propertyInfo = type.GetProperty(statName);
			
			if (propertyInfo != null)
			{
				propertyInfo.SetValue(Data.ScubaGear, value, null);
			}
			else
			{
				GD.PushError($"{statName} property not found on ScubaGear");
			}
		}
	}
}
