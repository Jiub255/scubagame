using Godot;
using System;

[Tool]
public partial class KanbanColumn : PanelContainer
{
	public event Action<KanbanColumn> OnDestroyColumn;
	public event Action<KanbanCard> OnOpenPopup;
	public event Action<KanbanColumn, KanbanColumn> OnMoveColumnToPosition;
	public event Action OnColumnDragStart;
	public event Action OnCardDragStart;
	public event Action OnColumnChanged;
	
	public LineEdit Title { get; private set; }
	public VBoxContainer Cards { get; private set; }
	private PackedScene CardScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_card.tscn");
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_column.tscn");
	private Button CreateCardButton { get; set; }
	private Button DeleteColumnButton { get; set; }

#region COLUMN

	public override void _EnterTree()
	{
		base._EnterTree();
		
		CreateCardButton = (Button)GetNode("%CreateCardButton");
		DeleteColumnButton = (Button)GetNode("%DeleteColumnButton");
		Title = (LineEdit)GetNode("%ColumnTitle");

		CreateCardButton.Pressed += CreateNewBlankCard;
		DeleteColumnButton.Pressed += DeleteColumn;
		Title.TextChanged += OnTitleChanged;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		CreateCardButton.Pressed -= CreateNewBlankCard;
		DeleteColumnButton.Pressed -= DeleteColumn;
		Title.TextChanged -= OnTitleChanged;
	}
	
	public void InitializeColumn(ColumnData columnData)
	{
		Title = (LineEdit)GetNode("%ColumnTitle");
		Cards = (VBoxContainer)GetNode("%Cards");
		Title.Text = columnData.Title;
		foreach (CardData cardData in columnData.Cards)
		{
			CreateNewCard(cardData);
		}
	}
	
	public ColumnData GetColumnData()
	{
		ColumnData columnData = new(Title.Text);
		foreach (KanbanCard card in Cards.GetChildren())
		{
			CardData cardData = card.GetCardData();
			columnData.Cards.Add(cardData);
		}
		return columnData;
	}
	
	private void DeleteColumn()
	{
		OnDestroyColumn?.Invoke(this);
	}
	
	private void OnTitleChanged(string _)
	{
		OnColumnChanged?.Invoke();
	}

	#endregion

	#region CARD

	private void SubscribeToCardEvents(KanbanCard card)
	{
		card.OnDeleteButtonPressed += DeleteCard;
		card.OnRemoveCard += RemoveCard;
		card.OnCardDragStart += CardDragStart;
		card.OnMoveCardToPosition += MoveCardToPosition;
		card.OnOpenPopupButtonPressed += OpenPopup;
	}

	private void UnsubscribeFromCardEvents(KanbanCard card)
	{
		card.OnDeleteButtonPressed -= DeleteCard;
		card.OnRemoveCard -= RemoveCard;
		card.OnCardDragStart -= CardDragStart;
		card.OnMoveCardToPosition -= MoveCardToPosition;
		card.OnOpenPopupButtonPressed -= OpenPopup;
	}

	private void CreateNewBlankCard()
	{
		CardData cardData = new CardData();
		CreateNewCard(cardData);
	}

	private void CreateNewCard(CardData cardData)
	{
		KanbanCard card = (KanbanCard)CardScene.Instantiate();
		Cards.AddChild(card);
		card.InitializeCard(cardData);

		SubscribeToCardEvents(card);
	}

	private void DeleteCard(KanbanCard card)
	{
		UnsubscribeFromCardEvents(card);

		card.QueueFree();
		OnColumnChanged?.Invoke();
	}

	public void AddCard(KanbanCard card, int index)
	{
		Cards.AddChild(card);
		Cards.MoveChild(card, index);
		SubscribeToCardEvents(card);
	}

	public void RemoveCard(KanbanCard card)
	{
		UnsubscribeFromCardEvents(card);
		Cards.RemoveChild(card);
		OnColumnChanged?.Invoke();
	}
	
	private void CardDragStart()
	{
		OnCardDragStart?.Invoke();
	}
	
	private void MoveCardToPosition(KanbanCard cardToMove, KanbanCard cardWithIndex)
	{
		int index = GetCardIndex(cardWithIndex);
		if (index != -1)
		{
			if (Cards.GetChildren().Contains(cardToMove))
			{
				Cards.MoveChild(cardToMove, index);
			}
			else
			{
				cardToMove.RemoveCard();
				AddCard(cardToMove, index);
			}
		}
		else
		{
			this.PrintDebug("Card index not found.");
		}
	}
	
	private int GetCardIndex(KanbanCard card)
	{
		Godot.Collections.Array<Node> children = Cards.GetChildren();
		for (int i = 0; i < children.Count; i++)
		{
			if (children[i] == card)
			{
				return i;
			}
		}
		
		return -1;
	}

	private void OpenPopup(KanbanCard card)
	{
		OnOpenPopup?.Invoke(card);
	}
	
#endregion

#region DRAG AND DROP

	public override Variant _GetDragData(Vector2 atPosition)
	{
		OnColumnDragStart?.Invoke();
		
		KanbanColumn previewColumn = MakePreview();
		SetDragPreview(previewColumn);
		return this;
	}

	private KanbanColumn MakePreview()
	{
		KanbanColumn previewColumn = (KanbanColumn)ColumnScene.Instantiate();
		previewColumn.InitializeColumn(GetColumnData());
		return previewColumn;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		Node node = data.As<Node>();
		if (node == null)
		{
			return false;
		}
		else
		{
			if (node is KanbanColumn || node is KanbanCard)
			{
				return true;
			}
			return false;
		}
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		Node node = data.As<Node>();
		if (node != null)
		{
			if (node is KanbanColumn column)
			{
				OnMoveColumnToPosition?.Invoke(column, this);
			}
			else if (node is KanbanCard card)
			{
				card.RemoveCard();
				AddCard(card, Cards.GetChildren().Count);
			}
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
				//control.PrintDebug($"Type: {control.GetType()}");
				control.MouseFilter = MouseFilterEnum.Ignore;
			}
		}
	}
	
	public void ResetMouseFilters(Node parent)
	{
		foreach (Node child in parent.GetChildren())
		{
			if (child.GetChildren().Count > 0)
			{
				ResetMouseFilters(child);
			}
			if (child is Control control && 
					(child is KanbanCard ||
					child is LineEdit ||
					child is Button ||
					child is ScrollContainer))
			{
				control.MouseFilter = MouseFilterEnum.Pass;
			}
		}
	}
#endregion
}
