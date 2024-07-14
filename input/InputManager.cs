using Godot;
using System;

// Is this just a pointless wrapper on the Input class?
public partial class InputManager : Node
{
	// TODO: Make individual actions for each non movement input.
	public event Action OnAttackPressed;
	public event Action OnJumpPressed;
	//public event Action<InputEvent> OnInputEventReceived;
	public event Action<Vector2> OnMovementEventReceived;
	
	// TODO: Have state machine check this every frame. 
	// Just return movement vector here and use signals for other events?
	// Or just poll for everything with a big ol switch? Probably use events. 
	public InputEvent GetInput()
	{
		return null;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
		
	}
	
	private void ProcessInputEvent(InputEvent @event)
	{
		// Process before action. Maybe use multiple actions, one for each type of input. 
		// Or, just send the plain event and have the individual states process it? No,
		// makes more sense to have this class handle that. 
		if (@event.IsActionPressed("attack"))
		{
			OnAttackPressed?.Invoke();
		}
		else if (@event.IsActionPressed("jump"))
		{
			OnJumpPressed?.Invoke();
		}
		
		if (@event.IsActionPressed("up") ||
			@event.IsActionPressed("down") ||
			@event.IsActionPressed("left") ||
			@event.IsActionPressed("right") )
		{
			Vector2 movement = Input.GetVector("left", "right", "up", "down");
			OnMovementEventReceived?.Invoke(movement);
		}
		
		//OnInputEventReceived?.Invoke(@event);
	}
}
