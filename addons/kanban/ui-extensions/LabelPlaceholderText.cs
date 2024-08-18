using Godot;

[Tool]
public partial class LabelPlaceholderText : Label
{
	[Export]
	private string PlaceholderText { get; set; }
	
	public string StoredText
	{
		get
		{
			if (Text == PlaceholderText)
			{
				return "";
			}
			else
			{
				return Text;
			}
		}
		set
		{
			if (value == "")
			{
				Text = PlaceholderText;
			}
			else
			{
				Text = value;
			}
		}
	}
}
