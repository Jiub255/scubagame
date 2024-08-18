using Godot;

[System.Serializable]
public struct CardData
{
	public string Title { get; set; }
	public string Description { get; set; }
	
	public CardData()
	{
		Title = "";
		Description = "";
	}
	
	public CardData(string title, string description)
	{
		Title = title;
		Description = description;
	}
	
	public CardData(FileAccess file)
	{
		Title = file.GetPascalString();
		Description = file.GetPascalString();
	}
	
	public void SaveCard(FileAccess file)
	{
		file.StorePascalString(Title);
		file.StorePascalString(Description);
	}
}
