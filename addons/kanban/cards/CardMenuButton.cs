using Godot;
using System;

[Tool]
public partial class CardMenuButton : MenuButton
{
	public event Action OnExpandPressed;
	public event Action OnCollapsePressed;
	public event Action OnDeletePressed;
	
	private PopupMenu PopupMenu { get; set; }
	
	private const string EXPAND_LABEL_TEXT = "Expand";
	private const string COLLAPSE_LABEL_TEXT = "Collapse";
	private const string DELETE_LABEL_TEXT = "Delete";
	
	public override void _EnterTree()
	{
		base._EnterTree();

		Icon = EditorInterface.Singleton.GetEditorTheme().GetIcon("GuiTabMenu", "EditorIcons");
		
		PopupMenu = GetPopup();

		PopupMenu.IdPressed += HandlePressId;
	}
	
	public void Initialize()
	{
		PopupMenu = GetPopup();
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
			case EXPAND_LABEL_TEXT:
				ExpandCard();
				break;
			case COLLAPSE_LABEL_TEXT:
				CollapseCard();
				break;
			case DELETE_LABEL_TEXT:
				DeleteCard();
				break;
			default:
				GD.PushError($"Popup menu option {PopupMenu.GetItemText((int)id)} doesn't match any options.");
				break;
		}
	}
	
	public void SetExpandMenu()
	{
		PopupMenu.Clear();
		PopupMenu.AddItem(COLLAPSE_LABEL_TEXT, 0);
		PopupMenu.AddItem(DELETE_LABEL_TEXT, 1);
	}
	
	public void SetCollapseMenu()
	{
		PopupMenu.Clear();
		PopupMenu.AddItem(EXPAND_LABEL_TEXT, 0);
		PopupMenu.AddItem(DELETE_LABEL_TEXT, 1);
	}
	
	private void ExpandCard()
	{
		OnExpandPressed?.Invoke();
	}
	
	private void CollapseCard()
	{
		OnCollapsePressed?.Invoke();
	}
	
	private void DeleteCard()
	{
		OnDeletePressed?.Invoke();
	}
}
