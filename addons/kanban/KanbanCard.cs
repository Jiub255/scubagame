using Godot;
using System;

[Tool]
public partial class KanbanCard : PanelContainer
{
	public event Action<KanbanCard> OnOpenPopupButtonPressed;
	public event Action<KanbanCard> OnDeleteButtonPressed;
	public event Action<KanbanCard, KanbanCard> OnMoveCardToPosition;
	public event Action OnCardDragStart;
	public event Action<KanbanCard> OnRemoveCard;
	
	private Button OpenPopupButton { get; set; }
	private Button DeleteButton { get; set; }
	public Label Title { get; set; }
	public Label Description { get; set; }
	private PackedScene CardScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_card.tscn");

	public override void _EnterTree()
	{
		base._EnterTree();
		
		OpenPopupButton = (Button)GetNode("%OpenPopupButton");
		DeleteButton = (Button)GetNode("%DeleteButton");

		ConnectEvents();
	}
	
	public override void _ExitTree()
	{
		DisconnectEvents();
	}
	
	public void ConnectEvents()
	{
		OpenPopupButton.Pressed += OpenPopup;
		DeleteButton.Pressed += DeleteCard;
	}
	
	public void DisconnectEvents()
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
		CardData cardData = new(Title.Text, Description.Text);
		return cardData;
	}

	private void OpenPopup()
	{
		OnOpenPopupButtonPressed?.Invoke(this);
	}
	
	public void RemoveCard()
	{
		OnRemoveCard?.Invoke(this);
	}
	
	public void DeleteCard()
	{
		OnDeleteButtonPressed?.Invoke(this);
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		OnCardDragStart?.Invoke();
		
		KanbanCard previewCard = MakePreview();
		SetDragPreview(previewCard);
		return this;
	}

	private KanbanCard MakePreview()
	{
		KanbanCard previewCard = (KanbanCard)CardScene.Instantiate();
		previewCard.InitializeCard(GetCardData());
		return previewCard;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		KanbanCard card = data.As<KanbanCard>();
		if (card == null)
		{
			return false;
		}
		return true;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		KanbanCard card = data.As<KanbanCard>();
		if (card != null)
		{
			OnMoveCardToPosition?.Invoke(card, this);
		}
	}
	
	
	public void SetFiltersToIgnore(Node parent)
	{
		foreach (Node child in parent.GetChildren())
		{
			if (child.GetChildren().Count > 0)
			{
				SetFiltersToIgnore(child);
			}
			if (child is Control control)
			{
				control.MouseFilter = MouseFilterEnum.Ignore;
			}
		}
	}
}
