using Godot;
using System;

[Tool]
public partial class Cards : VBoxContainer
{
	//public event Action OnCardDragStart;
	public event Action OnCardsChanged;
	public event Action<KanbanCard> OnDeleteCardPressed;
	public event Action<KanbanCard> OnOpenPopupPressed;
	
	private PackedScene CardScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/cards/kanban_card.tscn");
	
	public void CreateNewBlankCard()
	{
		CardData cardData = new();
		KanbanCard card = CreateNewCard(cardData);
		if (card != null)
		{
			OnOpenPopupPressed?.Invoke(card);
		}
	}

	public KanbanCard CreateNewCard(CardData cardData)
	{
		KanbanCard card = (KanbanCard)CardScene.Instantiate();
		card.InitializeCard(cardData);
		AddCard(card, -1);
		return card;
	}

	public void AddCard(KanbanCard card, int index)
	{
		AddChild(card);
		MoveChild(card, index);
		SubscribeToCardEvents(card);
		OnCardsChanged?.Invoke();
	}

	private void DeleteCardPressed(KanbanCard card)
	{
		OnDeleteCardPressed?.Invoke(card);
	}

	private void DeleteCard(KanbanCard card)
	{
		UnsubscribeFromCardEvents(card);

		card.QueueFree();
		OnCardsChanged?.Invoke();
	}

	public void RemoveCard(KanbanCard card)
	{
		UnsubscribeFromCardEvents(card);
		RemoveChild(card);
	}
	private void SubscribeToCardEvents(KanbanCard card)
	{
		//card.OnCardDragStart += CardDragStart;
		card.OnDeleteButtonPressed += DeleteCardPressed;
		card.OnDeleteCard += DeleteCard;
		card.OnMoveCardToPosition += MoveCardToPosition;
		card.OnOpenPopupButtonPressed += OpenPopup;
		card.OnRemoveCard += RemoveCard;
	}

	private void UnsubscribeFromCardEvents(KanbanCard card)
	{
		//card.OnCardDragStart -= CardDragStart;
		card.OnDeleteButtonPressed -= DeleteCardPressed;
		card.OnDeleteCard -= DeleteCard;
		card.OnMoveCardToPosition -= MoveCardToPosition;
		card.OnOpenPopupButtonPressed -= OpenPopup;
		card.OnRemoveCard -= RemoveCard;
	}

	private void OpenPopup(KanbanCard card)
	{
		OnOpenPopupPressed?.Invoke(card);
	}
	
/* 	private void CardDragStart()
	{
		OnCardDragStart?.Invoke();
	} */
	
	private void MoveCardToPosition(KanbanCard cardToMove, KanbanCard cardWithIndex)
	{
		int index = GetCardIndex(cardWithIndex);
		if (index != -1)
		{
			if (GetChildren().Contains(cardToMove))
			{
				MoveChild(cardToMove, index);
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
		Godot.Collections.Array<Node> children = GetChildren();
		for (int i = 0; i < children.Count; i++)
		{
			if (children[i] == card)
			{
				return i;
			}
		}
		
		return -1;
	}
	
	public void ExpandAllCards()
	{
		foreach (Node node in GetChildren())
		{
			if (node is KanbanCard card)
			{
				card.Expand();
			}
		}
	}
	
	public void CollapseAllCards()
	{
		foreach (Node node in GetChildren())
		{
			if (node is KanbanCard card)
			{
				card.Collapse();
			}
		}
	}
}
