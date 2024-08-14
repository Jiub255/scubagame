#if TOOLS
using Godot;

[Tool]
public partial class KanbanPlugin : EditorPlugin
{
	private PackedScene KanbanPackedScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_board.tscn");
	private KanbanBoard KanbanInstance { get; set; }
	private EditorInterface EditorInterface { get; set; } = EditorInterface.Singleton;
	private KanbanSaver KanbanSaver { get; set; } = new();
	
	
	public override void _EnterTree()
	{
		BoardData boardData = KanbanSaver.LoadGame();
		
		KanbanInstance = (KanbanBoard)KanbanPackedScene.Instantiate();

		// Add the main panel to the editor's main viewport.
		EditorInterface.GetEditorMainScreen().AddChild(KanbanInstance);
		
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

	private void SaveBoard()
	{
		KanbanSaver.SaveBoard(KanbanInstance.GetBoardData());
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
		return EditorInterface.GetEditorTheme().GetIcon("VBoxContainer", "EditorIcons");
	}

	public override void _Notification(int what)
	{
		base._Notification(what);
		
		if (what == NotificationWMCloseRequest)
		{
			if (KanbanInstance != null)
			{
				KanbanSaver.SaveBoard(KanbanInstance.GetBoardData());
				KanbanInstance.QueueFree();
				GetTree().Quit();
			}
		}
	}
}
#endif
