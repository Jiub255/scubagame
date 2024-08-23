using Godot;

[Tool]
public partial class DeleteConfirmation : ConfirmationDialog
{
	private KanbanCard _card;
	private KanbanColumn _column;

	public KanbanCard Card
	{
		get => _card;
		set
		{
			if (value != null)
			{
				_card = value;
				_column = null;
			}
			else
			{
				_card = value;
			}
		}
	}
	public KanbanColumn Column
	{
		get => _column;
		set
		{
			if (value != null)
			{
				_column = value;
				_card = null;
			}
			else
			{
				_column = value;
			}
		}
	}

	public override void _EnterTree()
	{
		base._EnterTree();

		GetOkButton().Pressed += DeleteCardOrColumn;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		GetOkButton().Pressed -= DeleteCardOrColumn;
	}
	
	private void DeleteCardOrColumn()
	{
		Card?.DeleteCard();
		Column?.DeleteColumn();
		//Hide();
	}
	
	public void Open(KanbanCard card, KanbanColumn column = null)
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
		
		int length = Mathf.Max(question.Length, warning.Length);
		title = title.TruncateQuoteQuestion(length);

		DialogText = $"{question}\n{title}\n{warning}";
		Column = column;
		Card = card;
		Show();
	}
}
