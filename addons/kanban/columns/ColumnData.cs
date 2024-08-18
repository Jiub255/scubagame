using System.Collections.Generic;
using Godot;

[System.Serializable]
public struct ColumnData
{
	public string Title { get; set; }
	public List<CardData> Cards { get; set; } = new();
	
	public ColumnData()
	{
		Title = "";
	}
	
	public ColumnData(string title)
	{
		Title = title;
	}
	
	public ColumnData(FileAccess file)
	{
		Title = file.GetPascalString();
		int cardCount = (int)file.Get32();
		for (int i = 0; i < cardCount; i++)
		{
			Cards.Add(new CardData(file));
		}
	}
	
	public void SaveColumn(FileAccess file)
	{
		file.StorePascalString(Title);
		file.Store32((uint)Cards.Count);
		foreach (CardData card in Cards)
		{
			card.SaveCard(file);
		}
	}
}
