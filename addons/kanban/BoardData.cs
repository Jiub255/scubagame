using System.Collections.Generic;

public struct BoardData
{
	public List<ColumnData> Columns { get; set; }
	
	public BoardData()
	{
		Columns = new List<ColumnData>();
	}
}
