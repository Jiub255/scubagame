using Godot;
using System;

[GlobalClass]
[Tool]
public partial class KanbanCard : PanelContainer
{
	public event Action<KanbanCard> OnOpenPopupButtonPressed;
	public event Action<KanbanCard> OnDeleteButtonPressed;
	public event Action<KanbanColumn> OnMoveColumnToPosition;
	
	private Button OpenPopupButton { get; set; }
	private Button DeleteButton { get; set; }
	public Label Title { get; set; }
	public Label Description { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		
		OpenPopupButton = (Button)GetNode("%OpenPopupButton");
		DeleteButton = (Button)GetNode("%DeleteButton");
		
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
		Title = (Label)GetNode("%Title");
		Description = (Label)GetNode("%Description");
		Title.Text = cardData.Title;
		Description.Text = cardData.Description;
	}
	
	public CardData GetCardData()
	{
		CardData cardData = new CardData(Title.Text, Description.Text);
		return cardData;
	}

	private void OpenPopup()
	{
		OnOpenPopupButtonPressed?.Invoke(this);
	}
	
	private void DeleteCard()
	{
		OnDeleteButtonPressed?.Invoke(this);
	}

	// TODO: Check if data.As column or board or button or title or whatever else is over the column,
	// so you can drop anywhere on a column. 
	// But then, how do you handle drop? Same way, just handle each case.
	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		Node ancestor = data.As<Node>().GetAncestorOfType<KanbanColumn>();
		this.PrintDebug($"ancestor: {ancestor.Name}");
		Control control = data.As<Control>();
		this.PrintDebug($"control: {control.Name}");
		if (control == null)
		{
			return false;
		}
		else
		{
			if (control.HasAncestorOfType<KanbanColumn>())
			{
				return true;
			}
			return false;
		}
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		Control control = data.As<Control>();
		KanbanColumn column = control.GetAncestorOfType<KanbanColumn>();
		if (column != null)
		{
			OnMoveColumnToPosition?.Invoke(column);
		}
	}
}
