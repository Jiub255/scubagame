#if TOOLS
using Godot;

[Tool]
public partial class KanbanPlugin : EditorPlugin
{
	private PackedScene KanbanPackedScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_board.tscn");
	private KanbanBoard KanbanInstance { get; set; }
	private EditorInterface EditorInterfaceSingleton { get; set; } = EditorInterface.Singleton;
	private KanbanSaver KanbanSaverInstance { get; set; } = new();
	
	
	public override void _EnterTree()
	{
		// TODO: Set stylebox colors based on editor theme here? Before instantiation?
		//SetThemeColors();
		
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
			KanbanInstance.QueueFree();
		}
	}
	
/* 	private void SetThemeColors()
	{
		EditorSettings editorSettings = EditorInterfaceSingleton.GetEditorSettings();
		Color baseColor = (Color)editorSettings.GetSetting("interface/theme/base_color");
		Color accentColor = (Color)editorSettings.GetSetting("interface/theme/accent_color");
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
				KanbanSaverInstance.SaveBoard(KanbanInstance.GetBoardData());
				KanbanInstance.QueueFree();
				GetTree().Quit();
			}
		}
	}
}
#endif
