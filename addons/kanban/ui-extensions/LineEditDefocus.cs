using Godot;

[Tool]
public partial class LineEditDefocus : LineEdit
{
	[Export]
	private bool ShowTooltip = true;
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

	// TODO: Should this be _GuiInput instead?
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (HasFocus())
		{
			if (@event is InputEventMouseButton && !MouseOverText)
			{
				CaretColumn = 0;
				if (ShowTooltip)
				{
					TooltipText = Text;
				}
				ReleaseFocus();
			}
		}
	}
	
	private void Defocus(string _)
	{
		CaretColumn = 0;
		if (ShowTooltip)
		{
			TooltipText = Text;
		}
		ReleaseFocus();
	}
}
