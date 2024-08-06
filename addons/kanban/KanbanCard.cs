using Godot;
using System;

[GlobalClass]
[Tool]
public partial class KanbanCard : PanelContainer
{
	public event Action<KanbanCard> OnOpenPopupButtonPressed;
	public event Action<KanbanCard> OnDeleteButtonPressed;
	
	private Button OpenPopupButton { get; set; }
	private Button DeleteButton { get; set; }
	public Label Title { get; set; }
	public Label Description { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		
		OpenPopupButton = (Button)GetNode("%OpenPopupButton");
		DeleteButton = (Button)GetNode("%DeleteButton");
		Title = (Label)GetNode("%Title");
		Description = (Label)GetNode("%Description");
		
		OpenPopupButton.Pressed += OpenPopup;
		DeleteButton.Pressed += DeleteCard;
	}
	
	public override void _ExitTree()
	{
		OpenPopupButton.Pressed -= OpenPopup;
		DeleteButton.Pressed -= DeleteCard;
	}
	
	public void InitializeCard(CardData cardData)
	{
		Title.Text = cardData.Title;
		Description.Text = cardData.Description;
	}

	private void OpenPopup()
	{
		OnOpenPopupButtonPressed?.Invoke(this);
	}
	
	private void DeleteCard()
	{
		OnDeleteButtonPressed?.Invoke(this);
	}
}
