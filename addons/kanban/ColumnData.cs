using System.Collections.Generic;

public struct ColumnData
{
	public string Title { get; set; }
	public List<CardData> Cards { get; set; }
	
	public ColumnData()
	{
		Title = "Column Title";
		Cards = new List<CardData>();
	}
	
	public ColumnData(string title)
	{
		Title = title;
		Cards = new List<CardData>();
	}
}
