using Godot;
using System;
using System.Collections.Generic;

public class ColumnData
{
	public string Title { get; set; }
	public List<CardData> Cards { get; set; }
	
	public ColumnData(string title = "Enter Column Title")
	{
		Title = title;
		Cards = new List<CardData>();
	}
}
