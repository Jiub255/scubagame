using Godot;

[Tool]
public class KanbanSaver
{
	private string SavePath { get; } = "res://addons/kanban/kanban.data";
	
	public KanbanSaver() {}
	
	public void SaveBoard(BoardData boardData)
	{
		FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
		boardData.SaveBoard(file);
		//this.PrintDebug("Board saved.");
		file.Close();
	}
	
	public BoardData LoadBoard()
	{
		if (FileAccess.FileExists(SavePath))
		{
			//this.PrintDebug($"File exists at {SavePath}");
			FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
			BoardData boardData = new(file);
			file.Close();
			return boardData;
		}
		//this.PrintDebug($"File not found at {SavePath}");
		return new BoardData();
	}
}
