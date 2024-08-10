using Godot;

[GlobalClass]
[Tool]
public partial class DeleteButton : Button
{
	public override void _Ready()
	{
		base._Ready();

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
