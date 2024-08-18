using System;
using Godot;

[Tool]
public partial class KanbanBoard : PanelContainer
{
	public event Action OnBoardChanged;
	
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/columns/kanban_column.tscn");
	
	private CardPopup CardPopup { get; set; }
	private Button CreateColumnButton { get; set; }
	public HBoxContainer Columns { get; private set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		
		CardPopup = (CardPopup)GetNode("%CardPopup");
		CreateColumnButton = (Button)GetNode("%CreateColumnButton");
		Columns = (HBoxContainer)GetNode("%Columns");

		CardPopup.OnClosePopup += SaveBoard;
		CreateColumnButton.Pressed += CreateNewBlankColumn;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		CreateColumnButton.Pressed -= CreateNewBlankColumn;
	}
	
	public void InitializeBoard(BoardData boardData)
	{
		foreach (ColumnData columnData in boardData.Columns)
		{
			CreateNewColumn(columnData);
		}
	}
	
	private void SaveBoard()
	{
		OnBoardChanged?.Invoke();
	}
	
	public BoardData GetBoardData()
	{
		BoardData boardData = new BoardData();
		foreach (KanbanColumn column in Columns.GetChildren())
		{
			ColumnData columnData = column.GetColumnData();
			boardData.Columns.Add(columnData);
		}
		return boardData;
	}
	
	private void CreateNewBlankColumn()
	{
		ColumnData columnData = new();
		CreateNewColumn(columnData);
	}

	private void CreateNewColumn(ColumnData columnData)
	{
		KanbanColumn newColumn = (KanbanColumn)ColumnScene.Instantiate();
		
		Columns.AddChild(newColumn);
		newColumn.InitializeColumn(columnData);
		
		newColumn.OnDestroyColumn += DeleteColumn;
		newColumn.Cards.OnOpenPopup += OpenPopup;
		newColumn.OnMoveColumnToPosition += MoveColumnToPosition;
		newColumn.OnColumnDragStart += SetColumnsFiltersToIgnore;
		newColumn.Cards.OnCardDragStart += SetCardsFiltersToIgnore;
		newColumn.OnColumnChanged += SaveBoard;

		SaveBoard();
	}
	
	private void DeleteColumn(KanbanColumn column)
	{
		column.OnDestroyColumn -= DeleteColumn;
		column.Cards.OnOpenPopup -= OpenPopup;
		column.OnMoveColumnToPosition -= MoveColumnToPosition;
		column.OnColumnDragStart -= SetColumnsFiltersToIgnore;
		column.Cards.OnCardDragStart -= SetCardsFiltersToIgnore;
		column.OnColumnChanged -= SaveBoard;
		
		column.QueueFree();

		SaveBoard();
	}
	
	private void OpenPopup(KanbanCard card)
	{
		CardPopup.OpenPopup(card);
	}
	
	private void MoveColumnToPosition(KanbanColumn columnToMove, KanbanColumn columnWithIndex)
	{
		int index = GetColumnIndex(columnWithIndex);
		if (index != -1)
		{
			Columns.MoveChild(columnToMove, index);
		}
		else
		{
			this.PrintDebug("Column index not found.");
		}
	}
	
	private int GetColumnIndex(KanbanColumn column)
	{
		Godot.Collections.Array<Node> children = Columns.GetChildren();
		for (int i = 0; i < children.Count; i++)
		{
			if (children[i] == column)
			{
				return i;
			}
		}
		
		return -1;
	}
	
	public override void _Notification(int what)
	{
		base._Notification(what);
		
		if (what == NotificationDragEnd)
		{
			ResetMouseFilters();
			
			if (GetViewport().GuiIsDragSuccessful())
			{
				SaveBoard();
			}
		}
	}
	
	private void SetColumnsFiltersToIgnore()
	{
		foreach (KanbanColumn column in Columns.GetChildren())
		{
			column.SetFiltersToIgnore(column);
		}
	}
	
	private void SetCardsFiltersToIgnore()
	{
		foreach (KanbanColumn column in Columns.GetChildren())
		{
			foreach (KanbanCard card in column.Cards.GetChildren())
			{
				card.SetFiltersToIgnore(card);
			}
		}
	}
	
	private void ResetMouseFilters()
	{
		foreach (KanbanColumn column in Columns.GetChildren())
		{
			column.ResetMouseFilters(column);
		}
	}
}
