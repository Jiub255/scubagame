using Godot;
using System;

[GlobalClass]
//[Tool]
public partial class KanbanCard : PanelContainer
{
	public event Action<KanbanCard> OnOpenPopupButtonPressed;
	public event Action<KanbanCard> OnDeleteButtonPressed;
	
	private Button OpenPopupButton { get; set; }
	private Button DeleteButton { get; set; }
	public Label TitleLabel { get; set; }
	public Label DescriptionLabel { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		
		OpenPopupButton = (Button)GetNode("%OpenPopupButton");
		DeleteButton = (Button)GetNode("%DeleteButton");
		TitleLabel = (Label)GetNode("%Title");
		DescriptionLabel = (Label)GetNode("%Description");
		
		OpenPopupButton.Pressed += OpenPopup;
		DeleteButton.Pressed += Delete;
	}
	
	public override void _ExitTree()
	{
		OpenPopupButton.Pressed -= OpenPopup;
		DeleteButton.Pressed -= Delete;
	}

	private void OpenPopup()
	{
		OnOpenPopupButtonPressed?.Invoke(this);
	}
	
	private void Delete()
	{
		OnDeleteButtonPressed?.Invoke(this);
	}
}