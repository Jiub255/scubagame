using System.Collections.Generic;
using Godot;

[System.Serializable]
public struct BoardData
{
	public List<ColumnData> Columns { get; set; } = new();

	public BoardData() {}
	
	public BoardData(FileAccess file)
	{
		int count = (int)file.Get32();
		for (int i = 0; i < count; i++)
		{
			Columns.Add(new ColumnData(file));
		}
	}
	
	public void SaveBoard(FileAccess file)
	{
		file.Store32((uint)Columns.Count);
		foreach (ColumnData column in Columns)
		{
			column.SaveColumn(file);
		}
	}
}
