#if TOOLS
using Godot;

[Tool]
public partial class KanbanPlugin : EditorPlugin
{
	private PackedScene KanbanPackedScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/board/kanban_board.tscn");
	private KanbanBoard KanbanInstance { get; set; }
	private EditorInterface EditorInterfaceSingleton { get; set; } = EditorInterface.Singleton;
	private KanbanSaver KanbanSaverInstance { get; set; } = new();
	
	public override void _EnterTree()
	{
		SetThemeColors();
		
		BoardData boardData = KanbanSaverInstance.LoadBoard();
		
		KanbanInstance = (KanbanBoard)KanbanPackedScene.Instantiate();

		// Add the main panel to the editor's main viewport.
		EditorInterfaceSingleton.GetEditorMainScreen().AddChild(KanbanInstance);
		
		KanbanInstance.InitializeBoard(boardData);
		KanbanInstance.OnBoardChanged += SaveBoard;
		
		// Hide the main panel. Very much required.
		_MakeVisible(false);
	}

	public override void _ExitTree()
	{
		if (KanbanInstance != null)
		{
			SaveBoard();
			KanbanInstance.OnBoardChanged -= SaveBoard;
			KanbanInstance.QueueFree();
		}
	}
	
	private void SetThemeColors()
	{
		EditorSettings editorSettings = EditorInterfaceSingleton.GetEditorSettings();
		Theme editorTheme = EditorInterfaceSingleton.GetEditorTheme();
		string[] colorTypes = editorTheme.GetColorTypeList();
		this.PrintDebug($"Editor theme color types list: {string.Join(", ", colorTypes)}");
		foreach (string colorType in colorTypes)
		{
			this.PrintDebug($"{colorType}'s color list: {string.Join(", ", editorTheme.GetColorList(colorType))}");
		}
		
		float saturationDeltaSmallBase = 0.05f;
		float brightnessDeltaSmallBase = 0.085f;
		float saturationDeltaBigBase = 0.014f;
		float brightnessDeltaBigBase = 0.17f;
		float saturationDeltaBiggerBase = 0.02f;
		float brightnessDeltaBiggerBase = 0.24f;

		float saturationDeltaSmallAccent = 0.1f;
		float brightnessDeltaSmallAccent = 0.12f;
		float saturationDeltaBigAccent = 0.17f;
		float brightnessDeltaBigAccent = 0.22f;
		float saturationDeltaBiggerAccent = 0.24f;
		float brightnessDeltaBiggerAccent = 0.3f;
		
		Color baseColor = (Color)editorSettings.GetSetting("interface/theme/base_color");
		Color baseDark = baseColor.Darken(saturationDeltaSmallBase, brightnessDeltaSmallBase);
		Color baseDarker = baseColor.Darken(saturationDeltaBigBase, brightnessDeltaBigBase);
		Color baseDarkest = baseColor.Darken(saturationDeltaBiggerBase, brightnessDeltaBiggerBase);
		
		Color accentColor = (Color)editorSettings.GetSetting("interface/theme/accent_color");
		Color accentDark = accentColor.Darken(saturationDeltaSmallAccent, brightnessDeltaSmallAccent);
		Color accentDarker = accentColor.Darken(saturationDeltaBigAccent, brightnessDeltaBigAccent);
		Color accentDarkest = accentColor.Darken(saturationDeltaBiggerAccent, brightnessDeltaBiggerAccent);
		
		StyleBoxFlat bigBase = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/big_base.tres");
		StyleBoxFlat bigBaseDark = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/big_base_dark.tres");
		StyleBoxFlat bigBaseDarker = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/big_base_darker.tres");
		StyleBoxFlat bigAccent = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/big_accent.tres");
		StyleBoxFlat bigAccentDark = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/big_accent_dark.tres");
		StyleBoxFlat bigAccentDarker = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/big_accent_darker.tres");
		StyleBoxFlat smallBase = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/small_base.tres");
		StyleBoxFlat smallBaseDark = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/small_base_dark.tres");
		StyleBoxFlat smallBaseDarker = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/small_base_darker.tres");
		StyleBoxFlat smallerBaseDarker = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/smaller_base_darker.tres");
		StyleBoxFlat smallAccent = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/small_accent.tres");
		StyleBoxFlat smallAccentDark = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/small_accent_dark.tres");
		StyleBoxFlat smallAccentDarker = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/small_accent_darker.tres");
		StyleBoxFlat bgPopup = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/bg_popup.tres");
		StyleBoxFlat bgBoard = GD.Load<StyleBoxFlat>("res://addons/kanban/styleboxes/bg_board.tres");

		bigBase.BgColor = baseColor;
		bigBase.BorderColor = baseDark;
		bigBaseDark.BgColor = baseDark;
		bigBaseDark.BorderColor = baseDarker;
		bigBaseDarker.BgColor = baseDarker;
		bigBaseDarker.BorderColor = baseDarkest;
		
		bigAccent.BgColor = accentColor;
		bigAccent.BorderColor = accentDark;
		bigAccentDark.BgColor = accentDark;
		bigAccentDark.BorderColor = accentDarker;
		bigAccentDarker.BgColor = accentDarker;
		bigAccentDarker.BorderColor = accentDarkest;
		
		smallBase.BgColor = baseColor;
		smallBase.BorderColor = baseDark;
		smallBaseDark.BgColor = baseDark;
		smallBaseDark.BorderColor = baseDarker;
		smallBaseDarker.BgColor = baseDarker;
		smallBaseDarker.BorderColor = baseDarkest;
		smallerBaseDarker.BgColor = baseDarker;
		smallerBaseDarker.BorderColor = baseDarkest;
		
		smallAccent.BgColor = accentColor;
		smallAccent.BorderColor = accentDark;
		smallAccentDark.BgColor = accentDark;
		smallAccentDark.BorderColor = accentDarker;
		smallAccentDarker.BgColor = accentDarker;
		smallAccentDarker.BorderColor = accentDarkest;
		
		bgBoard.BgColor = baseDark;
		bgPopup.BgColor = accentColor.ChangeAlpha(0.5f);
	}

	private void SaveBoard()
	{
		KanbanSaverInstance.SaveBoard(KanbanInstance.GetBoardData());
	}

	public override bool _HasMainScreen()
	{
		return true;
	}

	public override void _MakeVisible(bool visible)
	{
		if (KanbanInstance != null)
		{
			KanbanInstance.Visible = visible;
		}
	}

	public override string _GetPluginName()
	{
		return "Kanban";
	}

	public override Texture2D _GetPluginIcon()
	{
		return EditorInterfaceSingleton.GetEditorTheme().GetIcon("VBoxContainer", "EditorIcons");
	}

	public override void _Notification(int what)
	{
		base._Notification(what);
		
		if (what == NotificationWMCloseRequest)
		{
			if (KanbanInstance != null)
			{
				SaveBoard();
				KanbanInstance.QueueFree();
				GetTree().Quit();
			}
		}
	}
}
#endif
