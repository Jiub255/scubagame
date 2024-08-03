#if TOOLS
using Godot;

//[Tool]
public partial class KanbanPlugin : EditorPlugin
{
	private PackedScene KanbanPackedScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/kanban_board.tscn");
	private KanbanBoard KanbanInstance { get; set; }
	private EditorInterface EditorInterface { get; set; } = EditorInterface.Singleton;
	
	
	public override void _EnterTree()
	{
		//this.PrintDebug($"Packed scene null: {KanbanPackedScene == null}");
		KanbanInstance = (KanbanBoard)KanbanPackedScene.Instantiate();
		//this.PrintDebug($"Instance null: {KanbanInstance == null}");
		
		// Add the main panel to the editor's main viewport.
		EditorInterface.GetEditorMainScreen().AddChild(KanbanInstance);
		
		// Hide the main panel. Very much required.
		_MakeVisible(false);
	}

	public override void _ExitTree()
	{
		if (KanbanInstance != null)
		{
			KanbanInstance.QueueFree();
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		
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
		return EditorInterface.GetEditorTheme().GetIcon("Node", "EditorIcons");
	}
}
#endif
