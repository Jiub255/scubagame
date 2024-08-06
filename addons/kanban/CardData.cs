using Godot;
using System;

public class CardData
{
	public string Title { get; set; }
	public string Description { get; set; }
	
	public CardData(string title = "Enter Card Title", string description = "Enter Card Description")
	{
		Title = title;
		Description = description;
	}
}
