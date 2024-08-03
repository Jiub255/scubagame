using Godot;
using System;

[GlobalClass]
//[Tool]
public partial class KanbanColumn : PanelContainer
{
	public event Action<KanbanColumn> OnDestroyColumn;
	public event Action<KanbanCard> OnOpenPopup;
	
	// Max # of cards in column. 
	private int WipLimit { get; set; } = 6;
	
	private VBoxContainer Cards { get; set; }
	private PackedScene CardScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_card.tscn");
	//private List<KanbadCard> Cards { get; set; } = new();
	private Button CreateCardButton { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		
		Cards = (VBoxContainer)GetNode("%Cards");
		CreateCardButton = (Button)GetNode("%CreateCardButton");

		CreateCardButton.Pressed += AddCard;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		CreateCardButton.Pressed -= AddCard;
	}

	private void AddCard()
	{
		KanbanCard card = (KanbanCard)CardScene.Instantiate();
		Cards.AddChild(card);

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
