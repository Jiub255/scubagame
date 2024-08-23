using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class KanbanCard : Button
{
	private const int COLLAPSED_HEIGHT = 50;
	private const int EXPANDED_HEIGHT = 150;
	private const string COLLAPSE_LABEL_TEXT = "Collapse";
	private const string EXPAND_LABEL_TEXT = "Expand";
	private const string DELETE_LABEL_TEXT = "Delete";

	public event Action<KanbanCard> OnDeleteButtonPressed;
	public event Action<KanbanCard> OnDeleteCard;
	public event Action<KanbanCard, KanbanCard> OnMoveCardToPosition;
	public event Action<KanbanCard> OnOpenPopupButtonPressed;
	public event Action<KanbanCard> OnRemoveCard;
	
	public LabelPlaceholderText Title { get; set; }
	public TextEditAutoBullet Description { get; set; }
	public bool Collapsed { get; set; }
	public PanelContainer DescriptionPanelContainer { get; set; }
	private OptionsMenuButton MenuButton { get; set; }
	private Dictionary<string, Action> LabelActionDict = new();
	private PackedScene CardScene { get; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/cards/kanban_card.tscn");
	
	
#region CARD

	public override void _EnterTree()
	{
		base._EnterTree();
		
		MenuButton ??= (OptionsMenuButton)GetNode("%MenuButton");

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
	}
	
	public void DisconnectEvents()
	{
		Pressed -= OpenPopup;
	}
	
	public void InitializeCard(CardData cardData)
	{
		Title = (LabelPlaceholderText)GetNode("%Title");
		Title.StoredText = cardData.Title;
		
		Description = (TextEditAutoBullet)GetNode("%Description");
		Description.Text = cardData.Description;
		Description.SetBulletPoints();

		DescriptionPanelContainer = (PanelContainer)GetNode("%DescriptionPanelContainer");
		MenuButton ??= (OptionsMenuButton)GetNode("%MenuButton");
		LabelActionDict = new()
		{
			{ EXPAND_LABEL_TEXT, Expand },
			{ COLLAPSE_LABEL_TEXT, Collapse },
			{ DELETE_LABEL_TEXT, ConfirmDeleteCard }
		};
		
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
		Dictionary<string, Action> expandedDict = new()
		{
			{ COLLAPSE_LABEL_TEXT, LabelActionDict[COLLAPSE_LABEL_TEXT]},
			{ DELETE_LABEL_TEXT, LabelActionDict[DELETE_LABEL_TEXT] }
		};
		MenuButton.Initialize(expandedDict);
		Collapsed = false;
	}
	
	public void Collapse()
	{
		CustomMinimumSize = new Vector2(CustomMinimumSize.X, COLLAPSED_HEIGHT);
		DescriptionPanelContainer.Hide();
		Dictionary<string, Action> collapsedDict = new()
		{
			{ EXPAND_LABEL_TEXT, LabelActionDict[EXPAND_LABEL_TEXT]},
			{ DELETE_LABEL_TEXT, LabelActionDict[DELETE_LABEL_TEXT] }
		};
		MenuButton.Initialize(collapsedDict);
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
	
#endregion

}
