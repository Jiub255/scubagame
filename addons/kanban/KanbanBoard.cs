using Godot;

[GlobalClass]
[Tool]
public partial class KanbanBoard : PanelContainer
{
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_column.tscn");
	
	//private List<KanbanColumn> KanbanColumns { get; } = new();
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
		//KanbanColumns.Add(newColumn);
		
		newColumn.OnDestroyColumn += DeleteColumn;
		newColumn.OnOpenPopup += OpenPopup;
	}
	
	private void DeleteColumn(KanbanColumn column)
	{
		//KanbanColumns.Remove(column);
		
		column.OnDestroyColumn -= DeleteColumn;
		column.OnOpenPopup -= OpenPopup;
		
		column.QueueFree();
	}
	
	private void OpenPopup(KanbanCard card)
	{
		CardPopup.OpenPopup(card);
	}
}
