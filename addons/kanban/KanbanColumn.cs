using Godot;
using System;

[GlobalClass]
[Tool]
public partial class KanbanColumn : PanelContainer
{
	public event Action<KanbanColumn> OnDestroyColumn;
	public event Action<KanbanCard> OnOpenPopup;
	public event Action<KanbanColumn, KanbanColumn> OnMoveColumnToPosition;
	public event Action OnColumnDragStart;
	public event Action OnCardDragStart;
	
	public TextEdit Title { get; private set; }
	public VBoxContainer Cards { get; private set; }
	private PackedScene CardScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_card.tscn");
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_column.tscn");
	private Button CreateCardButton { get; set; }
	private Button DeleteColumnButton { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		
		CreateCardButton = (Button)GetNode("%CreateCardButton");
		DeleteColumnButton = (Button)GetNode("%DeleteColumnButton");

		CreateCardButton.Pressed += CreateNewBlankCard;
		DeleteColumnButton.Pressed += DeleteColumn;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		CreateCardButton.Pressed -= CreateNewBlankCard;
		DeleteColumnButton.Pressed -= DeleteColumn;
	}
	
	public void InitializeColumn(ColumnData columnData)
	{
		Title = (TextEdit)GetNode("%ColumnTitle");
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

	private void SubscribeToCardEvents(KanbanCard card)
	{
		card.OnDeleteButtonPressed += DeleteCard;
		card.OnOpenPopupButtonPressed += OpenPopup;
		card.OnMoveCardToPosition += MoveCardToPosition;
		card.OnCardDragStart += CardDragStart;
		card.OnRemoveCard += RemoveCard;
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
	}

	private void DeleteCard(KanbanCard card)
	{
		UnsubscribeFromCardEvents(card);

		card.QueueFree();
	}

	private void UnsubscribeFromCardEvents(KanbanCard card)
	{
		card.OnDeleteButtonPressed -= DeleteCard;
		card.OnOpenPopupButtonPressed -= OpenPopup;
		card.OnMoveCardToPosition -= MoveCardToPosition;
		card.OnCardDragStart -= CardDragStart;
		card.OnRemoveCard -= RemoveCard;
	}

	private void OpenPopup(KanbanCard card)
	{
		OnOpenPopup?.Invoke(card);
	}
	
	private void DeleteColumn()
	{
		OnDestroyColumn?.Invoke(this);
	}

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
				AddCard(card, Cards.GetChildren().Count - 1);
			}
		}
	}
	
	public void SetChildrenToIgnore(Node parent)
	{
		foreach (Node child in parent.GetChildren())
		{
			if (child.GetChildren().Count > 0)
			{
				SetChildrenToIgnore(child);
			}
			if (child is Control control)
			{
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
			switch (child) 
			{
				case Container container1:
					container1.MouseFilter = container1 is KanbanCard ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
					break;
				case Label label1:
					label1.MouseFilter = MouseFilterEnum.Ignore;
					break;
				// This covers TextEdit (for column title) and Buttons.
				case Control control1:
					control1.MouseFilter = MouseFilterEnum.Stop;
					break;
			}
		}
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
}
