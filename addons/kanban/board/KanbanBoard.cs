using System;
using Godot;

[Tool]
public partial class KanbanBoard : PanelContainer
{
	public event Action OnBoardChanged;
	
	private LineEdit Title { get; set; }
	private DeleteConfirmation DeleteConfirmation { get; set; }
	private BoardMenuButton MenuButton { get; set; }
	private CardPopup CardPopup { get; set; }
	
	public HBoxContainer Columns { get; private set; }
	
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/columns/kanban_column.tscn");

	public override void _EnterTree()
	{
		base._EnterTree();
		
		Title = (LineEdit)GetNode("%TitleLineEdit");
		DeleteConfirmation = (DeleteConfirmation)GetNode("%DeleteConfirmation");
		DeleteConfirmation.GetChild<Label>(1, true).HorizontalAlignment = HorizontalAlignment.Center;
		MenuButton = (BoardMenuButton)GetNode("%MenuButton");
		CardPopup = (CardPopup)GetNode("%CardPopup");
		Columns = (HBoxContainer)GetNode("%Columns");

		MenuButton.OnCreateColumnPressed += CreateNewBlankColumn;
		MenuButton.OnExpandPressed += ExpandAll;
		MenuButton.OnCollapsePressed += CollapseAll;
		CardPopup.OnClosePopup += SaveBoard;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		MenuButton.OnCreateColumnPressed -= CreateNewBlankColumn;
		MenuButton.OnExpandPressed -= ExpandAll;
		MenuButton.OnCollapsePressed -= CollapseAll;
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
		
		newColumn.OnColumnChanged += SaveBoard;
		//newColumn.OnColumnDragStart += SetColumnsFiltersToIgnore;
		newColumn.OnDeleteColumn += DeleteColumn;
		newColumn.OnDeleteColumnPressed += OpenConfirmationColumn;
		newColumn.OnMoveColumnToPosition += MoveColumnToPosition;
		//newColumn.Cards.OnCardDragStart += SetCardsFiltersToIgnore;
		newColumn.Cards.OnDeleteCardPressed += OpenConfirmationCard;
		newColumn.Cards.OnOpenPopupPressed += OpenPopup;

		SaveBoard();
	}
	
	private void DeleteColumn(KanbanColumn column)
	{
		column.OnColumnChanged -= SaveBoard;
		//column.OnColumnDragStart -= SetColumnsFiltersToIgnore;
		column.OnDeleteColumn -= DeleteColumn;
		column.OnDeleteColumnPressed -= OpenConfirmationColumn;
		column.OnMoveColumnToPosition -= MoveColumnToPosition;
		//column.Cards.OnCardDragStart -= SetCardsFiltersToIgnore;
		column.Cards.OnDeleteCardPressed -= OpenConfirmationCard;
		column.Cards.OnOpenPopupPressed -= OpenPopup;
		
		column.QueueFree();

		SaveBoard();
	}
	
	private void OpenConfirmationCard(KanbanCard card)
	{
		OpenConfirmationDialog(card);
	}
	
	private void OpenConfirmationColumn(KanbanColumn column)
	{
		OpenConfirmationDialog(null, column);
	}
	
	private void OpenConfirmationDialog(KanbanCard card, KanbanColumn column = null)
	{
		string question = "Are you sure you want to delete ";
		string title;
		if (card == null)
		{
			if (column == null)
			{
				GD.PushError("Must pass in either card or column to this method.");
				return;
			}
			question += "column:";
			title = column.Title.Text;
		}
		else
		{
			question += "card:";
			title = card.Title.Text;
		}
		string warning = "This process cannot be undone.";
		
		int length = Math.Max(question.Length, warning.Length);
		title = title.TruncateQuoteQuestion(length);

		DeleteConfirmation.DialogText = $"{question}\n{title}\n{warning}";
		DeleteConfirmation.Column = column;
		DeleteConfirmation.Card = card;
		DeleteConfirmation.Show();
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
			//ResetMouseFilters();
			
			if (GetViewport().GuiIsDragSuccessful())
			{
				SaveBoard();
			}
		}
	}
	
/* 	private void SetColumnsFiltersToIgnore()
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
	} */
}
