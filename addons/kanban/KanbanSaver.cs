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
	
	public void SaveBoard(KanbanBoard kanbanBoard)
	{		
		BoardData boardData = new BoardData();
		foreach (KanbanColumn column in kanbanBoard.Columns.GetChildren())
		{
			ColumnData columnData = new ColumnData(column.Title.Text);
			foreach (KanbanCard card in column.Cards.GetChildren())
			{
				CardData cardData = new CardData(card.Title.Text, card.Description.Text);
				columnData.Cards.Add(cardData);
			}
			boardData.Columns.Add(columnData);
		}
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
