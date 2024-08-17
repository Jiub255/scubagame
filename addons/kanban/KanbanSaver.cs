using Godot;
//using System.IO;
// TODO: Using this library might be causing .NET failure to unload assembly errors
// on every rebuild. Requires editor restart every time. 
// Maybe use another method to save? Or find another way to make it work. 
//using System.Text.Json;

[Tool]
public class KanbanSaver
{
	private string SavePath { get; }
	
	public KanbanSaver()
	{
/* 		string localPath = "res://addons/kanban/kanban.data";
		SavePath = ProjectSettings.GlobalizePath(localPath); */
		SavePath = "res://addons/kanban/kanban.data";
	}
	
	public void SaveBoard(BoardData boardData)
	{
		FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
		boardData.SaveBoard(file);
		this.PrintDebug("Board saved.");
		file.Close();
	}
	
	public BoardData LoadBoard()
	{
		if (FileAccess.FileExists(SavePath))
		{
			this.PrintDebug($"File exists at {SavePath}");
			FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
			BoardData boardData = new(file);
			file.Close();
			return boardData;
		}
		this.PrintDebug($"File not found at {SavePath}");
		return new BoardData();
	}
	
/* 	public void SaveBoard(BoardData boardData)
	{
		string jsonString = JsonSerializer.Serialize(boardData);
		File.WriteAllText(SavePath, jsonString);
		//this.PrintDebug($"Saved {jsonString}");
	} */
	
/* 	public BoardData LoadGame()
	{
		if (File.Exists(SavePath))
		{
			string jsonString = File.ReadAllText(SavePath);
			BoardData boardData = JsonSerializer.Deserialize<BoardData>(jsonString);
			return boardData;
		}
		return new BoardData();
	} */
}
