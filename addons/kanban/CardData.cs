public struct CardData
{
	public string Title { get; set; }
	public string Description { get; set; }
	
	public CardData()
	{
		Title = "Card Title";
		Description = "Card Description";
	}
	
	public CardData(string title, string description)
	{
		Title = title;
		Description = description;
	}
}
