using Godot;
using System.IO;
using System.Text.Json;

// TODO: Separate out data from column/card/board and only save the raw data.
// Then put it all back together on load. 
// Is Tool necessary? 
[Tool]
public class KanbanSaver
{
	private string SavePath { get; }
	
	public KanbanSaver()
	{
		string localPath = "res://addons/kanban/kanban.json";
		SavePath = ProjectSettings.GlobalizePath(localPath);
	}
	
	public void SaveBoard(BoardData boardData)
	{
		string jsonString = JsonSerializer.Serialize(boardData);
		File.WriteAllText(SavePath, jsonString);
	}
	
	public BoardData LoadGame()
	{
		if (File.Exists(SavePath))
		{
			string jsonString = File.ReadAllText(SavePath);
			BoardData boardData = JsonSerializer.Deserialize<BoardData>(jsonString);
			return boardData;
		}
		return new BoardData();
	}
}
