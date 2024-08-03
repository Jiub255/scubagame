using Godot;

[GlobalClass]
//[Tool]
public partial class DeleteButton : Button
{
	public override void _Ready()
	{
		base._Ready();

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		
		Hide();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		MouseEntered -= OnMouseEntered;
		MouseExited -= OnMouseExited;
	}

	private void OnMouseEntered()
	{
		Show();
	}
	
	private void OnMouseExited()
	{
		Hide();
	}
}
