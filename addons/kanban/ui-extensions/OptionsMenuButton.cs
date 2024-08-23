using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Must be initialized with a (string, Action) dictionary from whichever class is using this.
/// The string is what will be on the button choice's label, and the corresponding Action will
/// be called when that option is selected. 
/// </summary>
[Tool]
public partial class OptionsMenuButton : MenuButton
{
	private Dictionary<string, Action> LabelToActionDict { get; set; } = new();
	private PopupMenu PopupMenu { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();

		PopupMenu = GetPopup();

		PopupMenu.IdPressed += ChooseOption;
	
		Icon = EditorInterface.Singleton.GetEditorTheme().GetIcon("GuiTabMenu", "EditorIcons");
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		PopupMenu.IdPressed -= ChooseOption;
	}
	
	/// <summary>
	/// The string is the label text, and the Action gets called when that option is chosen.
	/// </summary>
	public void Initialize(Dictionary<string, Action> labelToActionDict)
	{
		PopupMenu ??= GetPopup();
		LabelToActionDict = labelToActionDict;
		SetLabels();
	}
	
	private void SetLabels()
	{
		PopupMenu.Clear();
		foreach (string key in LabelToActionDict.Keys)
		{
			PopupMenu.AddItem(key);
		}
	}
	
	private void InvokeMethod(string label)
	{
		if (LabelToActionDict.ContainsKey(label))
		{
			LabelToActionDict[label]?.Invoke();
		}
		else
		{
			GD.PushError($"No method found for key: {label}");
		}
	}
	
	private void ChooseOption(long id)
	{
		InvokeMethod(PopupMenu.GetItemText((int)id));
	}
}
