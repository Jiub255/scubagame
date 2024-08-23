#if TOOLS
using Godot;

[Tool]
public partial class KanbanPlugin : EditorPlugin
{
	private PackedScene KanbanPackedScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/board/kanban_board.tscn");
	private KanbanBoard KanbanInstance { get; set; }
	private EditorInterface EditorInterfaceSingleton { get; set; } = EditorInterface.Singleton;
	private KanbanSaver KanbanSaverInstance { get; set; } = new();

	// TODO: Use this to resize columns/cards?
/* 	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		//Vector2 mainscreenSize = EditorInterfaceSingleton.GetEditorMainScreen().Size;
		//this.PrintDebug($"Editor main screen size: {mainscreenSize}");
	} */

	public override void _EnterTree()
	{
		SetupBoard();
	}

	private void SetupBoard()
	{
		BoardData boardData = KanbanSaverInstance.LoadBoard();
		KanbanInstance = (KanbanBoard)KanbanPackedScene.Instantiate();
		IntegrateEditorTheme();
		// Add the main panel to the editor's main viewport.
		EditorInterfaceSingleton.GetEditorMainScreen().AddChild(KanbanInstance);
		KanbanInstance.InitializeBoard(boardData);
		
		KanbanInstance.OnBoardChanged += SaveBoard;
		EditorInterfaceSingleton.GetEditorSettings().SettingsChanged += IntegrateEditorTheme;
		
		// Hide the main panel. Very much required.
		_MakeVisible(false);
	}

	public override void _ExitTree()
	{
		if (KanbanInstance != null)
		{
			SaveBoard();
			
			KanbanInstance.OnBoardChanged -= SaveBoard;
			EditorInterfaceSingleton.GetEditorSettings().SettingsChanged -= IntegrateEditorTheme;
			
			KanbanInstance.QueueFree();
		}
	}

	private void IntegrateEditorTheme()
	{
		// Copy needed types' styleboxes from editor theme into copy
		Theme copy = new Theme();
		Theme editorTheme = EditorInterfaceSingleton.GetEditorTheme();
		
		CopyThemeTypeStylebox(editorTheme, copy, "PanelContaner");
		CopyThemeTypeStylebox(editorTheme, copy, "Button");

		// Alterations for Kanban Board theme
		StyleBoxFlat panelContainerStylebox = (StyleBoxFlat)copy.GetStylebox("panel", "PanelContainer");
		panelContainerStylebox.DrawCenter = true;
		StyleBoxFlat buttonNormalStylebox = (StyleBoxFlat)copy.GetStylebox("normal", "Button");

		// Get editor color variations
		Color backgroundColor = editorTheme.GetColor("background", "Editor");
		Color baseColor = editorTheme.GetColor("base_color", "Editor");
		Color selectionColor = editorTheme.GetColor("selection_color", "Editor");
		
		// Set board variation stylebox (do before adding borders)
		LoadAndChangeStylebox(panelContainerStylebox, backgroundColor, "res://addons/kanban/styleboxes/board_stylebox.tres");
		
		// Change panelContainerStylebox, borders needed for Dark themes.
		panelContainerStylebox.BorderWidthTop = buttonNormalStylebox.BorderWidthTop;
		panelContainerStylebox.BorderWidthBottom = buttonNormalStylebox.BorderWidthBottom;
		panelContainerStylebox.BorderWidthLeft = buttonNormalStylebox.BorderWidthLeft;
		panelContainerStylebox.BorderWidthRight = buttonNormalStylebox.BorderWidthRight;
		panelContainerStylebox.BorderColor = buttonNormalStylebox.BorderColor;

		// Set column variation stylebox
		LoadAndChangeStylebox(panelContainerStylebox, baseColor, "res://addons/kanban/styleboxes/column_stylebox.tres");
		
		// Set card variation stylebox
		LoadAndChangeStylebox(panelContainerStylebox, buttonNormalStylebox.BgColor, "res://addons/kanban/styleboxes/card_stylebox.tres");
		
		// Set card text panels variation stylebox
		LoadAndChangeStylebox(panelContainerStylebox, selectionColor, "res://addons/kanban/styleboxes/card_text_stylebox.tres");
		
		KanbanInstance.Theme = editorTheme;
	}
	
	private void CopyThemeTypeStylebox(Theme sourceTheme, Theme targetTheme, string themeType)
	{
		// Copy styleboxes
		foreach (string styleboxName in sourceTheme.GetStyleboxList(themeType))
		{
			StyleBox stylebox = sourceTheme.GetStylebox(styleboxName, themeType);
			targetTheme.SetStylebox(styleboxName, themeType, stylebox);
		}
	}
	
	private void LoadAndChangeStylebox(StyleBoxFlat variation, Color bgColor, string path)
	{
		StyleBoxFlat loadedStylebox = ResourceLoader.Load<StyleBoxFlat>(path);
		if (loadedStylebox == null)
		{
			GD.PushError($"Failed to load resource from {path}");
		}
		else
		{
			loadedStylebox.BgColor = bgColor;
			loadedStylebox.DrawCenter = true;
			loadedStylebox.BorderWidthTop = variation.BorderWidthTop;
			loadedStylebox.BorderWidthBottom = variation.BorderWidthBottom;
			loadedStylebox.BorderWidthLeft = variation.BorderWidthLeft;
			loadedStylebox.BorderWidthRight = variation.BorderWidthRight;
			loadedStylebox.BorderColor = variation.BorderColor;

			Error error = ResourceSaver.Save(loadedStylebox, path);
			if (error != Error.Ok)
			{
				GD.PushError(error);
			}
		}
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
