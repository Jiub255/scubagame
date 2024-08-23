using Godot;
using System;

[Tool]
public partial class KanbanCard : Button
{
	private const int COLLAPSED_HEIGHT = 50;
	private const int EXPANDED_HEIGHT = 150;

	public event Action<KanbanCard> OnDeleteButtonPressed;
	public event Action<KanbanCard> OnDeleteCard;
	public event Action<KanbanCard, KanbanCard> OnMoveCardToPosition;
	public event Action<KanbanCard> OnOpenPopupButtonPressed;
	public event Action<KanbanCard> OnRemoveCard;
	
	public LabelPlaceholderText Title { get; set; }
	public TextEditAutoBullet Description { get; set; }
	public bool Collapsed { get; set; }
	public PanelContainer DescriptionPanelContainer { get; set; }
	private CardMenuButton MenuButton { get; set; }
	private PackedScene CardScene { get; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/cards/kanban_card.tscn");
	
#region CARD

	public override void _EnterTree()
	{
		base._EnterTree();
		
		MenuButton ??= (CardMenuButton)GetNode("%MenuButton");

		ConnectEvents();
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		
		DisconnectEvents();
	}
	
	public void ConnectEvents()
	{
		Pressed += OpenPopup;
		MenuButton.OnCollapsePressed += Collapse;
		MenuButton.OnDeletePressed += ConfirmDeleteCard;
		MenuButton.OnExpandPressed += Expand;
	}
	
	public void DisconnectEvents()
	{
		Pressed -= OpenPopup;
		MenuButton.OnCollapsePressed -= Collapse;
		MenuButton.OnDeletePressed -= ConfirmDeleteCard;
		MenuButton.OnExpandPressed -= Expand;
	}
	
	public void InitializeCard(CardData cardData)
	{
		Title = (LabelPlaceholderText)GetNode("%Title");
		Description = (TextEditAutoBullet)GetNode("%Description");
		MenuButton ??= (CardMenuButton)GetNode("%MenuButton");
		DescriptionPanelContainer = (PanelContainer)GetNode("%DescriptionPanelContainer");
		Title.StoredText = cardData.Title;
		Description.Text = cardData.Description;
		Description.SetBulletPoints();
		MenuButton.Initialize();
		if (cardData.Collapsed)
		{
			Collapse();
		}
		else
		{
			Expand();
		}
	}

	public CardData GetCardData()
	{
		CardData cardData = new(Title.StoredText, Description.Text, Collapsed);
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
	
	public void Expand()
	{
		CustomMinimumSize = new Vector2(CustomMinimumSize.X, EXPANDED_HEIGHT);
		DescriptionPanelContainer.Show();
		MenuButton.SetExpandMenu();
		Collapsed = false;
	}
	
	public void Collapse()
	{
		CustomMinimumSize = new Vector2(CustomMinimumSize.X, COLLAPSED_HEIGHT);
		DescriptionPanelContainer.Hide();
		MenuButton.SetCollapseMenu();
		Collapsed = true;
	}

	public void DeleteCard()
	{
		OnDeleteCard?.Invoke(this);
	}
	
#endregion

#region DRAG AND DROP

	public override Variant _GetDragData(Vector2 atPosition)
	{
		//OnCardDragStart?.Invoke();
		
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
	
	
/* 	public void SetFiltersToIgnore(Node parent)
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
	} */
#endregion

}
