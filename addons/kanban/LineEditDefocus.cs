using Godot;

[Tool]
public partial class LineEditDefocus : LineEdit
{
	private bool MouseOverText { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		TextSubmitted += Defocus;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		MouseEntered -= OnMouseEntered;
		MouseExited -= OnMouseExited;
		TextSubmitted -= Defocus;
	}

	private void OnMouseEntered()
	{
		MouseOverText = true;
	}
	
	private void OnMouseExited()
	{
		MouseOverText = false;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (HasFocus())
		{
			if (@event is InputEventMouseButton && !MouseOverText)
			{
				ReleaseFocus();
			}
			else if (@event.IsActionPressed("ui_text_submit"))
			{
				ReleaseFocus();
			}
		}
	}
	
	private void Defocus(string _)
	{
		ReleaseFocus();
	}
}
