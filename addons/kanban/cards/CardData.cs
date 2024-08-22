using Godot;

[System.Serializable]
public struct CardData
{
	public string Title { get; set; }
	public string Description { get; set; }
	public bool Collapsed { get; set; }
	
	public CardData()
	{
		Title = "";
		Description = "";
		Collapsed = false;
	}
	
	public CardData(string title, string description, bool collapsed)
	{
		Title = title;
		Description = description;
		Collapsed = collapsed;
	}
	
	public CardData(FileAccess file)
	{
		Title = file.GetPascalString();
		Description = file.GetPascalString();
		Collapsed = file.Get8() != 0;
	}
	
	public void SaveCard(FileAccess file)
	{
		file.StorePascalString(Title);
		file.StorePascalString(Description);
		file.Store8((byte)(Collapsed ? 1 : 0));
	}
}
