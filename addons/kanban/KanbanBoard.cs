using Godot;

[GlobalClass]
[Tool]
public partial class KanbanBoard : PanelContainer
{
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_column.tscn");
	
	private CardPopup CardPopup { get; set; }
	private Button CreateColumnButton { get; set; }
	public HBoxContainer Columns { get; private set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		
		CardPopup = (CardPopup)GetNode("%CardPopup");
		CreateColumnButton = (Button)GetNode("%CreateColumnButton");
		Columns = (HBoxContainer)GetNode("%Columns");

		CreateColumnButton.Pressed += CreateNewBlankColumn;

		CardPopup.ClosePopup();
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
		ColumnData columnData = new ColumnData();
		CreateNewColumn(columnData);
	}

	private void CreateNewColumn(ColumnData columnData)
	{
		KanbanColumn newColumn = (KanbanColumn)ColumnScene.Instantiate();
		
		Columns.AddChild(newColumn);
		newColumn.InitializeColumn(columnData);
		
		newColumn.OnDestroyColumn += DeleteColumn;
		newColumn.OnOpenPopup += OpenPopup;
		newColumn.OnMoveColumnToPosition += MoveColumnToPosition;
		newColumn.OnDragStart += SetColumnsChildrenToIgnore;
		newColumn.OnDragEnd += ResetColumnsChildrensMouseFilters;
	}
	
	private void DeleteColumn(KanbanColumn column)
	{
		column.OnDestroyColumn -= DeleteColumn;
		column.OnOpenPopup -= OpenPopup;
		column.OnMoveColumnToPosition -= MoveColumnToPosition;
		column.OnDragStart -= SetColumnsChildrenToIgnore;
		column.OnDragEnd -= ResetColumnsChildrensMouseFilters;
		
		column.QueueFree();
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
		for (int i = 0; i < Columns.GetChildren().Count; i++)
		{
			if (Columns.GetChildren()[i] == column)
			{
				return i;
			}
		}
		
		return -1;
	}
	
	private void SetColumnsChildrenToIgnore()
	{
		foreach (KanbanColumn column in Columns.GetChildren())
		{
			column.SetChildrenToIgnore(column);
		}
	}
	
	private void ResetColumnsChildrensMouseFilters()
	{
		foreach (KanbanColumn column in Columns.GetChildren())
		{
			column.ResetChildrensMouseFilter(column);
		}
	}
}
