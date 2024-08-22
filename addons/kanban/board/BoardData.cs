using System.Collections.Generic;
using Godot;

[System.Serializable]
public struct BoardData
{
	public string Title { get; set; }
	public List<ColumnData> Columns { get; set; }

	public BoardData()
	{
		Title = "";
		Columns = new List<ColumnData>();
	}
	
	public BoardData(FileAccess file)
	{
		//Title = "CHANGE BOARD DATA CONSTRUCTOR AFTER LOADING AND SAVING!";
		Title = file.GetPascalString();
		Columns = new List<ColumnData>();
		int count = (int)file.Get32();
		for (int i = 0; i < count; i++)
		{
			Columns.Add(new ColumnData(file));
		}
	}
	
	public void SaveBoard(FileAccess file)
	{
		file.StorePascalString(Title);
		file.Store32((uint)Columns.Count);
		foreach (ColumnData column in Columns)
		{
			column.SaveColumn(file);
		}
	}
}
