using Godot;

[Tool]
public partial class CardTitleTooltip : HBoxContainer
{
	private LabelPlaceholderText Title { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();

		Title = (LabelPlaceholderText)GetNode("%Title");

		Title.OnTextChanged += UpdateTooltip;

		UpdateTooltip();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		Title.OnTextChanged -= UpdateTooltip;
	}

	private void UpdateTooltip()
	{
		TooltipText = Title.Text;
	}
}
