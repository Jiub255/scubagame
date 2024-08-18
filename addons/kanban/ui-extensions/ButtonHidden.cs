using Godot;

[Tool]
public partial class ButtonHidden : Button
{
	public override void _EnterTree()
	{
		base._EnterTree();

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		
		Modulate = Modulate.ChangeAlpha(0f);
		//SetButtonAlpha(0f);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		MouseEntered -= OnMouseEntered;
		MouseExited -= OnMouseExited;
	}

	private void OnMouseEntered()
	{
		Modulate = Modulate.ChangeAlpha(1f);
		//SetButtonAlpha(1f);
	}
	
	private void OnMouseExited()
	{
		Modulate = Modulate.ChangeAlpha(0f);
		//SetButtonAlpha(0f);
	}
	
/* 	private void SetButtonAlpha(float alpha)
	{
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, alpha);
	} */
}
