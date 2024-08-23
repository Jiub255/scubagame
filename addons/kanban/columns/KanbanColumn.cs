using Godot;
using System;

[Tool]
public partial class KanbanColumn : PanelContainer
{
	public event Action OnColumnChanged;
	//public event Action OnColumnDragStart;
	public event Action<KanbanColumn> OnDeleteColumn;
	public event Action<KanbanColumn> OnDeleteColumnPressed;
	public event Action<KanbanColumn, KanbanColumn> OnMoveColumnToPosition;
	
	public LineEditDefocus Title { get; private set; }
	public Cards Cards { get; private set; }
	private ColumnMenuButton MenuButton { get; set; }
	
	
	private PackedScene ColumnScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/columns/kanban_column.tscn");

#region COLUMN

	public override void _EnterTree()
	{
		base._EnterTree();
		
		Title = (LineEditDefocus)GetNode("%ColumnTitle");
		Cards = (Cards)GetNode("%Cards");
		MenuButton = (ColumnMenuButton)GetNode("%MenuButton");

		Title.TextChanged += OnTitleChanged;
		Cards.OnCardsChanged += OnCardsChanged;
		MenuButton.OnCollapsePressed += Cards.CollapseAllCards;
		MenuButton.OnCreateCardPressed += Cards.CreateNewBlankCard;
		MenuButton.OnDeletePressed += ConfirmDeleteColumn;
		MenuButton.OnExpandPressed += Cards.ExpandAllCards;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		Title.TextChanged -= OnTitleChanged;
		Cards.OnCardsChanged -= OnCardsChanged;
		MenuButton.OnCollapsePressed -= Cards.CollapseAllCards;
		MenuButton.OnCreateCardPressed -= Cards.CreateNewBlankCard;
		MenuButton.OnDeletePressed -= ConfirmDeleteColumn;
		MenuButton.OnExpandPressed -= Cards.ExpandAllCards;
	}
	
	public void InitializeColumn(ColumnData columnData)
	{
		Title ??= (LineEditDefocus)GetNode("%ColumnTitle");
		Cards ??= (Cards)GetNode("%Cards");
		Title.Text = columnData.Title;
		Title.TooltipText = Title.Text;
		foreach (CardData cardData in columnData.Cards)
		{
			Cards.CreateNewCard(cardData);
		}
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
			OnDeleteColumn?.Invoke(this);
		}
		else
		{
			OnDeleteColumnPressed?.Invoke(this);
		}
	}
	
	public void DeleteColumn()
	{
		OnDeleteColumn?.Invoke(this);
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
		//OnColumnDragStart?.Invoke();
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
	
	// TODO: Is this all necessary if nodes are all set to pass or ignore?
	// What about the buttons that need to be stop?
	// Or can they be set to pass and just handle/consume the click somehow?
/* 	public void SetFiltersToIgnore(Node parent)
	{
		foreach (Node child in parent.GetChildren())
		{
			if (child.GetChildren().Count > 0)
			{
				SetFiltersToIgnore(child);
			}
			if (child is Control control)
			{
				control.MouseFilter = MouseFilterEnum.Ignore;
			}
		}
	}
	
	public void ResetMouseFilters(Node parent)
	{
		foreach (Node child in parent.GetChildren())
		{
			if (child.GetChildren().Count > 0)
			{
				ResetMouseFilters(child);
			}
			if (child is Control control and
					(KanbanCard or
					LineEditDefocus or
					TextEdit or
					HBoxContainer or
					Button or
					ScrollContainer))
			{
				control.MouseFilter = MouseFilterEnum.Pass;
			}
			/* else if (child is Button button)
			{
				button.MouseFilter = MouseFilterEnum.Stop;
			} *8/
		}
	} */
#endregion

}
