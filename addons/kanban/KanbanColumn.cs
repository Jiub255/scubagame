using Godot;
using System;

[GlobalClass]
[Tool]
public partial class KanbanColumn : PanelContainer
{
	public event Action<KanbanColumn> OnDestroyColumn;
	public event Action<KanbanCard> OnOpenPopup;
	
	// Max # of cards in column. 
	private int WipLimit { get; set; } = 6;
	
	public TextEdit Title { get; private set; }
	public VBoxContainer Cards { get; private set; }
	private PackedScene CardScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_card.tscn");
	//private List<KanbadCard> Cards { get; set; } = new();
	private Button CreateCardButton { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		
		Title = (TextEdit)GetNode("%ColumnTitle");
		Cards = (VBoxContainer)GetNode("%Cards");
		CreateCardButton = (Button)GetNode("%CreateCardButton");

		CreateCardButton.Pressed += CreateNewBlankCard;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		CreateCardButton.Pressed -= CreateNewBlankCard;
	}
	
	public void InitializeColumn(ColumnData columnData)
	{
		Title.Text = columnData.Title;
		foreach (CardData cardData in columnData.Cards)
		{
			CreateNewCard(cardData);
		}
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

		card.OnDeleteButtonPressed += DestroyCard;
		card.OnOpenPopupButtonPressed += OpenPopup;
		
		//Cards.Add(card);
	}
	
	private void DestroyCard(KanbanCard card)
	{
		//Cards.Remove(card);
		
		card.OnDeleteButtonPressed -= DestroyCard;
		card.OnOpenPopupButtonPressed -= OpenPopup;

		card.QueueFree();
	}
	
	private void OpenPopup(KanbanCard card)
	{
		OnOpenPopup?.Invoke(card);
	}
	
	private void DestroyColumn()
	{
		OnDestroyColumn?.Invoke(this);
	}
}
