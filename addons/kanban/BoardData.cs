using Godot;
using System;
using System.Collections.Generic;

public class BoardData
{
	public List<ColumnData> Columns { get; set; }
	
	public BoardData()
	{
		Columns = new List<ColumnData>();
	}
}
