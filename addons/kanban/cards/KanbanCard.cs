using Godot;
using System;

[Tool]
public partial class KanbanCard : Button
{
	public event Action<KanbanCard> OnOpenPopupButtonPressed;
	public event Action<KanbanCard> OnDeleteButtonPressed;
	public event Action<KanbanCard, KanbanCard> OnMoveCardToPosition;
	public event Action OnCardDragStart;
	public event Action<KanbanCard> OnRemoveCard;
	public event Action<KanbanCard> OnDeleteCard;
	
	private Button DeleteButton { get; set; }
	public LabelPlaceholderText Title { get; set; }
	public TextEditAutoBullet Description { get; set; }
	private PackedScene CardScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/cards/kanban_card.tscn");

#region CARD

	public override void _EnterTree()
	{
		base._EnterTree();
		
		DeleteButton = (Button)GetNode("%DeleteButton");

		ConnectEvents();
	}
	
	public override void _ExitTree()
	{
		DisconnectEvents();
	}
	
	public void ConnectEvents()
	{
		Pressed += OpenPopup;
		DeleteButton.Pressed += ConfirmDeleteCard;
	}
	
	public void DisconnectEvents()
	{
		Pressed -= OpenPopup;
		DeleteButton.Pressed -= ConfirmDeleteCard;
	}
	
	public void InitializeCard(CardData cardData)
	{
		Title = (LabelPlaceholderText)GetNode("%Title");
		Description = (TextEditAutoBullet)GetNode("%Description");
		Title.StoredText = cardData.Title;
		Description.Text = cardData.Description;
		Description.SetBulletPoints();
	}
	
	public CardData GetCardData()
	{
		CardData cardData = new(Title.StoredText, Description.Text);
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
	
	public void ConfirmDeleteCard()
	{
		if (Title.Text == Title.PlaceholderText && Description.Text == "")
		{
			OnDeleteCard?.Invoke(this);
		}
		else
		{
			OnDeleteButtonPressed?.Invoke(this);
		}
	}
	
	public void DeleteCard()
	{
		OnDeleteCard?.Invoke(this);
	}

#endregion

#region DRAG AND DROP

	public override Variant _GetDragData(Vector2 atPosition)
	{
		OnCardDragStart?.Invoke();
		
		Control preview = MakePreview(atPosition);
		SetDragPreview(preview);
		return this;
	}

	private Control MakePreview(Vector2 relativeMousePosition)
	{
		KanbanCard previewCard = (KanbanCard)CardScene.Instantiate();
		previewCard.InitializeCard(GetCardData());
		Control preview = new Control();
		preview.AddChild(previewCard);
		previewCard.Position = -1 * relativeMousePosition;
		//previewCard.Position = previewCard.Size * -0.5f;
		return preview;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		KanbanCard card = data.As<KanbanCard>();
		return card != null;
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
#endregion

}
