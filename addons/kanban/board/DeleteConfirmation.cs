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
}
