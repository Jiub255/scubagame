using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
[Tool]
public partial class KanbanColumn : PanelContainer
{
	public event Action<KanbanColumn> OnDestroyColumn;
	public event Action<KanbanCard> OnOpenPopup;
	public event Action<KanbanColumn, KanbanColumn> OnMoveColumnToPosition;
	public event Action OnDragStart;
	public event Action OnDragEnd;
	
	public TextEdit Title { get; private set; }
	public VBoxContainer Cards { get; private set; }
	private PackedScene CardScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_card.tscn");
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_column.tscn");
	private Button CreateCardButton { get; set; }
	private Button DeleteColumnButton { get; set; }
	//private Dictionary<ulong, MouseFilterEnum> mouseFilterDict { get; set; } = new();

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
		ColumnData columnData = new ColumnData(Title.Text);
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

		card.OnDeleteButtonPressed += DeleteCard;
		card.OnOpenPopupButtonPressed += OpenPopup;
		card.OnMoveColumnToPosition += MoveColumnToPosition;
	}
	
	private void DeleteCard(KanbanCard card)
	{
		card.OnDeleteButtonPressed -= DeleteCard;
		card.OnOpenPopupButtonPressed -= OpenPopup;
		card.OnMoveColumnToPosition -= MoveColumnToPosition;

		card.QueueFree();
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
		//this.PrintDebug("Get drag data called.");
		// TODO: Send signal to board to set all children of columns (including card and its children)
		// to mouse filter ignore. 
		// Keep each control's original value in a dict? For resetting when drag ends. 
		OnDragStart?.Invoke();
		
		KanbanColumn previewColumn = MakePreview();
		SetDragPreview(previewColumn);
		return this;
	}

	public override void _Notification(int what)
	{
		base._Notification(what);
		
		if (what == NotificationDragEnd)
		{
			// TODO: Send signal to board to reset mouse filter on all children of columns. 
			//this.PrintDebug("Drag end notification recieved.");
			OnDragEnd?.Invoke();
		}
	}

	private KanbanColumn MakePreview()
	{
		KanbanColumn previewColumn = (KanbanColumn)ColumnScene.Instantiate();
		previewColumn.InitializeColumn(GetColumnData());
		return previewColumn;
	}

	// TODO: Check if data.As column or board or button or title or whatever else is over the column,
	// so you can drop anywhere on a column. 
	// But then, how do you handle drop? Same way, just handle each case.
	// TODO: Need to put this and drop data on every child of the column? Or just change all children to 
	// mouse ignore while in dragging? But then how to change back when drag is done? 
	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		Node ancestor = data.As<Node>().GetAncestorOfType<KanbanColumn>();
		//this.PrintDebug($"ancestor: {ancestor.Name}");
		Control control = data.As<Control>();
		//this.PrintDebug($"control: {control.Name}");
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
		//this.PrintDebug("Drop data called.");
		Control control = data.As<Control>();
		KanbanColumn column = control.GetAncestorOfType<KanbanColumn>();
		if (column != null)
		{
			OnMoveColumnToPosition?.Invoke(column, this);
		}
	}
	
	private void MoveColumnToPosition(KanbanColumn column)
	{
		OnMoveColumnToPosition?.Invoke(column, this);
	}
	
	public void SetChildrenToIgnore(Node parent)
	{
		//this.PrintDebug($"Set children to ignore called on {parent.Name}.");
		foreach (Node child in parent.GetChildren())
		{
			if (child.GetChildren().Count > 0)
			{
				SetChildrenToIgnore(child);
			}
			if (child is Control control)
			{
/* 				this.PrintDebug($"{control.Name}'s mouse filter: {MouseFilter}");
				if (mouseFilterDict.ContainsKey(control.GetInstanceId()))
				{
					mouseFilterDict[control.GetInstanceId()] = MouseFilter;
				}
				else
				{
					mouseFilterDict.Add(control.GetInstanceId(), MouseFilter);
				} */
				control.MouseFilter = MouseFilterEnum.Ignore;
			}
		}
	}
	
	public void ResetChildrensMouseFilter(Node parent)
	{
		//this.PrintDebug($"Reset childrens mouse filter called on {parent.Name}.");
		foreach (Node child in parent.GetChildren())
		{
			if (child.GetChildren().Count > 0)
			{
				ResetChildrensMouseFilter(child);
			}
			if (child is Container container)
			{
				container.MouseFilter = MouseFilterEnum.Ignore;
			}
			else if (child is Label label)
			{
				label.MouseFilter = MouseFilterEnum.Ignore;
			}
			else if (child is Control control)
			{
				control.MouseFilter = MouseFilterEnum.Stop;
			}
/* 			if (child is Control control)
			{
				this.PrintDebug($"{control.Name}'s mouse filter dict value: {mouseFilterDict[control.GetInstanceId()]}");
				control.MouseFilter = mouseFilterDict[control.GetInstanceId()];
			} */
		}
	}
}
