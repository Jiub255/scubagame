using System;
using System.Collections.Generic;
using Godot;

[Tool]
public partial class KanbanBoard : PanelContainer
{
	public event Action OnBoardChanged;
	
	private LineEdit Title { get; set; }
	private DeleteConfirmation DeleteConfirmation { get; set; }
	private CardPopup CardPopup { get; set; }
	public HBoxContainer Columns { get; private set; }
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/columns/kanban_column.tscn");

	public override void _EnterTree()
	{
		base._EnterTree();

		Title = (LineEdit)GetNode("%TitleLineEdit");
		DeleteConfirmation = (DeleteConfirmation)GetNode("%DeleteConfirmation");
		DeleteConfirmation.GetChild<Label>(1, true).HorizontalAlignment = HorizontalAlignment.Center;
		CardPopup = (CardPopup)GetNode("%CardPopup");
		Columns = (HBoxContainer)GetNode("%Columns");

		CardPopup.OnClosePopup += SaveBoard;
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		CardPopup.OnClosePopup -= SaveBoard;
	}
	
	private void ExpandAll()
	{
		foreach (Node node in Columns.GetChildren())
		{
			if (node is KanbanColumn column)
			{
				column.Cards.ExpandAllCards();
			}
		}
	}
	
	private void CollapseAll()
	{
		foreach (Node node in Columns.GetChildren())
		{
			if (node is KanbanColumn column)
			{
				column.Cards.CollapseAllCards();
			}
		}
	}
	
	public void InitializeBoard(BoardData boardData)
	{
		Title.Text = boardData.Title;
		foreach (ColumnData columnData in boardData.Columns)
		{
			CreateNewColumn(columnData);
		}

		OptionsMenuButton menuButton = (OptionsMenuButton)GetNode("%MenuButton");
		Dictionary<string, Action> labelToActionDict = new()
		{
			{ "Create New Column", CreateNewBlankColumn },
			{ "Expand All", ExpandAll },
			{ "Collapse All", CollapseAll }
		};
		menuButton.Initialize(labelToActionDict);
	}
	
	private void SaveBoard()
	{
		OnBoardChanged?.Invoke();
	}
	
	public BoardData GetBoardData()
	{
		BoardData boardData = new BoardData
		{
			Title = Title.Text
		};
		foreach (Node node in Columns.GetChildren())
		{
			if (node is KanbanColumn column)
			{
				ColumnData columnData = column.GetColumnData();
				boardData.Columns.Add(columnData);
			}
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
		
		newColumn.OnColumnChanged += SaveBoard;
		newColumn.OnDeleteColumnConfirmed += DeleteColumn;
		newColumn.OnDeleteColumnPressed += OpenConfirmationColumn;
		newColumn.OnMoveColumnToPosition += MoveColumnToPosition;
		newColumn.Cards.OnDeleteCardPressed += OpenConfirmationCard;
		newColumn.Cards.OnOpenPopupPressed += OpenPopup;

		SaveBoard();
	}
	
	private void DeleteColumn(KanbanColumn column)
	{
		column.OnColumnChanged -= SaveBoard;
		column.OnDeleteColumnConfirmed -= DeleteColumn;
		column.OnDeleteColumnPressed -= OpenConfirmationColumn;
		column.OnMoveColumnToPosition -= MoveColumnToPosition;
		column.Cards.OnDeleteCardPressed -= OpenConfirmationCard;
		column.Cards.OnOpenPopupPressed -= OpenPopup;
		
		column.QueueFree();

		SaveBoard();
	}
	
	private void OpenConfirmationCard(KanbanCard card)
	{
		DeleteConfirmation.Open(card);
	}
	
	private void OpenConfirmationColumn(KanbanColumn column)
	{
		DeleteConfirmation.Open(null, column);
	}
	
	private void OpenPopup(KanbanCard card)
	{
		CardPopup.Open(card);
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

		if (what == NotificationDragEnd && GetViewport().GuiIsDragSuccessful())
		{
			SaveBoard();
		}
	}
}
