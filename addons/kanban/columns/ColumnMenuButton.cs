using Godot;
using System;

[Tool]
public partial class ColumnMenuButton : MenuButton
{
	public event Action OnCreateCardPressed;
	public event Action OnExpandPressed;
	public event Action OnCollapsePressed;
	public event Action OnDeletePressed;
	
	private const string CREATE_CARD_LABEL_TEXT = "Create New Card";
	private const string EXPAND_LABEL_TEXT = "Expand All";
	private const string COLLAPSE_LABEL_TEXT = "Collapse All";
	private const string DELETE_LABEL_TEXT = "Delete Column";
	
	private PopupMenu PopupMenu { get; set; }
		
	public override void _EnterTree()
	{
		base._EnterTree();

		PopupMenu = GetPopup();

		PopupMenu.Clear();
		PopupMenu.AddItem(CREATE_CARD_LABEL_TEXT, 0);
		PopupMenu.AddItem(EXPAND_LABEL_TEXT, 1);
		PopupMenu.AddItem(COLLAPSE_LABEL_TEXT, 2);
		PopupMenu.AddItem(DELETE_LABEL_TEXT, 3);

		PopupMenu.IdPressed += HandlePressId;

		Icon = EditorInterface.Singleton.GetEditorTheme().GetIcon("GuiTabMenu", "EditorIcons");
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		PopupMenu.IdPressed -= HandlePressId;
	}

	private void HandlePressId(long id)
	{
		switch (PopupMenu.GetItemText((int)id))
		{
			case CREATE_CARD_LABEL_TEXT:
				CreateCard();
				break;
			case EXPAND_LABEL_TEXT:
				ExpandAll();
				break;
			case COLLAPSE_LABEL_TEXT:
				CollapseAll();
				break;
			case DELETE_LABEL_TEXT:
				DeleteColumn();
				break;
			default:
				GD.PushError($"Popup menu option {PopupMenu.GetItemText((int)id)} doesn't match any options.");
				break;
		}
	}
	
	private void CreateCard()
	{
		OnCreateCardPressed?.Invoke();
	}
	
	private void ExpandAll()
	{
		OnExpandPressed?.Invoke();
	}
	
	private void CollapseAll()
	{
		OnCollapsePressed?.Invoke();
	}
	
	private void DeleteColumn()
	{
		OnDeletePressed?.Invoke();
	}
}
