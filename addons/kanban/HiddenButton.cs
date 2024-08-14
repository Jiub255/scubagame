using Godot;

[Tool]
public partial class HiddenButton : Button
{
	public override void _EnterTree()
	{
		base._EnterTree();

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		
		SetButtonAlpha(0f);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		MouseEntered -= OnMouseEntered;
		MouseExited -= OnMouseExited;
	}

	private void OnMouseEntered()
	{
		SetButtonAlpha(1f);
	}
	
	private void OnMouseExited()
	{
		SetButtonAlpha(0f);
	}
	
	private void SetButtonAlpha(float alpha)
	{
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, alpha);
	}
}
