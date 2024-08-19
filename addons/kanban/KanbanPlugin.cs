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
		BoardData boardData = KanbanSaverInstance.LoadBoard();
		
		KanbanInstance = (KanbanBoard)KanbanPackedScene.Instantiate();

		// Add the main panel to the editor's main viewport.
		EditorInterfaceSingleton.GetEditorMainScreen().AddChild(KanbanInstance);
		
		KanbanInstance.InitializeBoard(boardData);
		KanbanInstance.Theme = CopyEditorTheme();
		KanbanInstance.OnBoardChanged += SaveBoard;

		//PermanentlyCopyEditorThemeIntoResource();
		
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
	
	private Theme CopyEditorTheme()
	{
		Theme copy = new Theme();
		Theme editorTheme = EditorInterfaceSingleton.GetEditorTheme();
		copy.MergeWith(editorTheme);		
		StyleBoxFlat buttonPressedStylebox = (StyleBoxFlat)copy.GetStylebox("pressed", "Button");
		StyleBoxFlat panelContainerStylebox = (StyleBoxFlat)copy.GetStylebox("panel", "PanelContainer");
		// TODO: Would it be faster to make a new stylebox with all the below properties and then just replace 
		// panelContainerStylebox with it? It takes like 20 seconds to load this plugin because of this method. 
		panelContainerStylebox.BgColor = buttonPressedStylebox.BgColor;
		panelContainerStylebox.DrawCenter = true;
		panelContainerStylebox.BorderWidthBottom = buttonPressedStylebox.BorderWidthBottom;
		panelContainerStylebox.BorderWidthTop = buttonPressedStylebox.BorderWidthTop;
		panelContainerStylebox.BorderWidthLeft = buttonPressedStylebox.BorderWidthLeft;
		panelContainerStylebox.BorderWidthRight = buttonPressedStylebox.BorderWidthRight;
		panelContainerStylebox.BorderColor = buttonPressedStylebox.BorderColor;
		return copy;
	}
	
/* 	private void PermanentlyCopyEditorThemeIntoResource()
	{
		/////////////////////////////////////////////////////////////////////////////
		//       TEMPORARY! JUST NEED TO RUN ONCE THEN COMMENT OUT OR DELETE!      //
		/////////////////////////////////////////////////////////////////////////////
		// Permanently copies the current editor theme into editor_theme_copy.tres //
		/////////////////////////////////////////////////////////////////////////////
		// Copy editor theme into resource using MergeWith().
		Theme editorTheme = EditorInterfaceSingleton.GetEditorTheme();
		Theme editorThemeCopyResource = GD.Load<Theme>("res://addons/kanban/themes/editor_theme_copy.tres");
		editorThemeCopyResource.MergeWith(editorTheme);
		// Copy lighter base color from selected tab bar to panel container.
		StyleBoxFlat buttonPressedStylebox = (StyleBoxFlat)editorThemeCopyResource.GetStylebox("pressed", "Button");
		StyleBoxFlat panelContainerStylebox = (StyleBoxFlat)editorThemeCopyResource.GetStylebox("panel", "PanelContainer");
		panelContainerStylebox.BgColor = buttonPressedStylebox.BgColor;		
		panelContainerStylebox.DrawCenter = true;
		panelContainerStylebox.BorderWidthBottom = buttonPressedStylebox.BorderWidthBottom;
		panelContainerStylebox.BorderWidthTop = buttonPressedStylebox.BorderWidthTop;
		panelContainerStylebox.BorderWidthLeft = buttonPressedStylebox.BorderWidthLeft;
		panelContainerStylebox.BorderWidthRight = buttonPressedStylebox.BorderWidthRight;
		panelContainerStylebox.BorderColor = buttonPressedStylebox.BorderColor;
		/////////////////////////////////////////////////////////////////////////////
	} */
	
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
