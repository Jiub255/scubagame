using System;
using Godot;

[Tool]
public partial class LabelPlaceholderText : Label
{
	public event Action OnTextChanged;
	
	[Export]
	public string PlaceholderText { get; private set; }
	
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
	
	public new string Text
	{
		get => base.Text;
		set
		{
			base.Text = value;
			OnTextChanged?.Invoke();
		}
	}
}
