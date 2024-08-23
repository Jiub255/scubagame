using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Need to initialize with (locally made) Dict in whatever class is using this.
/// 
/// </summary>
public partial class OptionsMenuButton : MenuButton
{
	private Dictionary<string, Action> LabelActionDict { get; set; } = new Dictionary<string, Action>();
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
	
	public void Initialize(Dictionary<string, Action> labelActionDict)
	{
		LabelActionDict = labelActionDict;
		SetLabels();
	}
	
	private void SetLabels()
	{
		PopupMenu.Clear();
		foreach (string key in LabelActionDict.Keys)
		{
			PopupMenu.AddItem(key);
		}
	}
	
	private void InvokeMethod(string label)
	{
		if (LabelActionDict.ContainsKey(label))
		{
			LabelActionDict[label]?.Invoke();
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
