using Godot;

[GlobalClass]
//[Tool]
public partial class KanbanBoard : Control
{
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_column.tscn");
	
	//private List<KanbanColumn> KanbanColumns { get; } = new();
	private CardPopup CardPopup { get; set; }
	private Button CreateColumnButton { get; set; }
	private HBoxContainer Columns { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		
		CardPopup = (CardPopup)GetNode("%CardPopup");
		CreateColumnButton = (Button)GetNode("%CreateColumnButton");
		Columns = (HBoxContainer)GetNode("%Columns");

		CreateColumnButton.Pressed += CreateNewColumn;

		CardPopup.ClosePopup();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		CreateColumnButton.Pressed -= CreateNewColumn;
	}

	private void CreateNewColumn()
	{
		KanbanColumn newColumn = (KanbanColumn)ColumnScene.Instantiate();
		Columns.AddChild(newColumn);
		//KanbanColumns.Add(newColumn);
		
		newColumn.OnDestroyColumn += DestroyColumn;
		newColumn.OnOpenPopup += OpenPopup;
	}
	
	private void DestroyColumn(KanbanColumn column)
	{
		//KanbanColumns.Remove(column);
		
		column.OnDestroyColumn -= DestroyColumn;
		column.OnOpenPopup -= OpenPopup;
		
		column.QueueFree();
	}
	
	private void OpenPopup(KanbanCard card)
	{
		CardPopup.OpenPopup(card);
	}
}
