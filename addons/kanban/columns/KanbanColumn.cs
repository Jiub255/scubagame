using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class KanbanColumn : PanelContainer
{
	public event Action OnColumnChanged;
	public event Action<KanbanColumn> OnDeleteColumnConfirmed;
	public event Action<KanbanColumn> OnDeleteColumnPressed;
	public event Action<KanbanColumn, KanbanColumn> OnMoveColumnToPosition;
	
	public LineEditDefocus Title { get; private set; }
	public Cards Cards { get; private set; }
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/columns/kanban_column.tscn");

#region COLUMN

	public override void _EnterTree()
	{
		base._EnterTree();
		
		Title = (LineEditDefocus)GetNode("%ColumnTitle");
		Cards = (Cards)GetNode("%Cards");

		Title.TextChanged += OnTitleChanged;
		Cards.OnCardsChanged += OnCardsChanged;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		Title.TextChanged -= OnTitleChanged;
		Cards.OnCardsChanged -= OnCardsChanged;
	}
	
	public void InitializeColumn(ColumnData columnData)
	{
		Title ??= (LineEditDefocus)GetNode("%ColumnTitle");
		Title.Text = columnData.Title;
		Title.TooltipText = Title.Text;
	
		Cards ??= (Cards)GetNode("%Cards");
		foreach (CardData cardData in columnData.Cards)
		{
			Cards.CreateNewCard(cardData);
		}
	
		OptionsMenuButton menuButton = (OptionsMenuButton)GetNode("%MenuButton");
		Dictionary<string, Action> labelToActionDict = new()
		{
			{ "Create New Card", Cards.CreateNewBlankCard },
			{ "Expand All", Cards.ExpandAllCards },
			{ "Collapse All", Cards.CollapseAllCards },
			{ "Delete Column", ConfirmDeleteColumn }
		};
		menuButton.Initialize(labelToActionDict);
	}
	
	public ColumnData GetColumnData()
	{
		ColumnData columnData = new(Title.Text);
		foreach (KanbanCard card in Cards.GetChildren())
		{
			CardData cardData = card.GetCardData();
			columnData.Cards.Add(cardData);
		}
		return columnData;
	}
	
	private void ConfirmDeleteColumn()
	{
		if (Title.Text == "" && Cards.GetChildren().Count == 0)
		{
			OnDeleteColumnConfirmed?.Invoke(this);
		}
		else
		{
			OnDeleteColumnPressed?.Invoke(this);
		}
	}
	
	public void DeleteColumn()
	{
		OnDeleteColumnConfirmed?.Invoke(this);
	}
	
	private void OnTitleChanged(string _)
	{
		OnColumnChanged?.Invoke();
	}
	
	private void OnCardsChanged()
	{
		OnColumnChanged?.Invoke();
	}

#endregion

#region DRAG AND DROP

	public override Variant _GetDragData(Vector2 atPosition)
	{
		Control preview = MakePreview(atPosition);
		SetDragPreview(preview);
		return this;
	}

	private Control MakePreview(Vector2 relativeMousePosition)
	{
		KanbanColumn previewColumn = (KanbanColumn)ColumnScene.Instantiate();
		previewColumn.InitializeColumn(GetColumnData());
		Control preview = new Control();
		preview.AddChild(previewColumn);
		previewColumn.Position = -1 * relativeMousePosition;
		return preview;
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		Node node = data.As<Node>();
		return node != null && (node is KanbanColumn or KanbanCard);
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		Node node = data.As<Node>();
		if (node != null)
		{
			if (node is KanbanColumn column)
			{
				OnMoveColumnToPosition?.Invoke(column, this);
			}
			else if (node is KanbanCard card)
			{
				card.RemoveCard();
				Cards.AddCard(card, Cards.GetChildren().Count);
			}
		}
	}
	
#endregion

}
