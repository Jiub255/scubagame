using Godot;
using System;

[Tool]
public partial class BoardMenuButton : MenuButton
{
	public event Action OnExpandPressed;
	public event Action OnCollapsePressed;
	public event Action OnCreateColumnPressed;
	
	private const string CREATE_COLUMN_LABEL_TEXT = "Create New Column";
	private const string EXPAND_LABEL_TEXT = "Expand All";
	private const string COLLAPSE_LABEL_TEXT = "Collapse All";
	
	private PopupMenu PopupMenu { get; set; }
		
	public override void _EnterTree()
	{
		base._EnterTree();

		PopupMenu = GetPopup();

		PopupMenu.Clear();
		PopupMenu.AddItem(CREATE_COLUMN_LABEL_TEXT, 0);
		PopupMenu.AddItem(EXPAND_LABEL_TEXT, 1);
		PopupMenu.AddItem(COLLAPSE_LABEL_TEXT, 2);

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
			case CREATE_COLUMN_LABEL_TEXT:
				CreateColumn();
				break;
			case EXPAND_LABEL_TEXT:
				ExpandAll();
				break;
			case COLLAPSE_LABEL_TEXT:
				CollapseAll();
				break;
			default:
				GD.PushError($"Popup menu option {PopupMenu.GetItemText((int)id)} doesn't match any options.");
				break;
		}
	}
	
	private void CreateColumn()
	{
		OnCreateColumnPressed?.Invoke();
	}
	
	private void ExpandAll()
	{
		OnExpandPressed?.Invoke();
	}
	
	private void CollapseAll()
	{
		OnCollapsePressed?.Invoke();
	}
}
