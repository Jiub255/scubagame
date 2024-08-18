using Godot;

public static class Extensions
{
	private static readonly int PADDING = 70;
	
	public static void PrintDebug(this object obj, string message)
	{
		if (obj is Resource resource)
		{
			string type = resource.GetType().Name;
			if (resource.GetType().BaseType != typeof(RefCounted))
			{
				type += $" : {resource.GetType().BaseType.Name}";
			}
			GD.Print($"Resource    |    {type}".PadRight(PADDING)
				+ $" |    {message}");
		}
		else if (obj is RefCounted refCounted)
		{
			string type = refCounted.GetType().Name;
			GD.Print($"RefCounted  |    {type}".PadRight(PADDING)
				+ $" |    {message}");
		}
		else if (obj is Node node)
		{
			string name = node.Name;
			if (node.GetType().BaseType != typeof(GodotObject))
			{
				name += $" : {node.GetType().BaseType.Name}";
			}
			GD.Print($"Node        |    {name}".PadRight(PADDING)
				+ $" |    {message}");
		}
		else
		{
			string type = obj.GetType().Name;
			if (obj.GetType().BaseType != null)
			{
				type += $" : {obj.GetType().BaseType.Name}";
			}
			GD.Print($"C# Object   |    {type}".PadRight(PADDING)
				+ $" |    {message}");
		}
	}

	public static Color Brighten(this Color color, float saturationDelta, float brightnessDelta)
	{
		float h = color.H;
		float s = Mathf.Max(0f, color.S - saturationDelta);
		float v = Mathf.Min(1f, color.V + brightnessDelta);
		return Color.FromHsv(h, s, v);
	}
	
	public static Color Darken(this Color color, float saturationDelta, float brightnessDelta)
	{
		float h = color.H;
		float s = Mathf.Min(1f, color.S + saturationDelta);
		float v = Mathf.Max(0f, color.V - brightnessDelta);
		return Color.FromHsv(h, s, v);
	}
	
	public static Color ChangeAlpha(this Color color, float newAlpha)
	{
		return new Color(color.R, color.G, color.B, newAlpha);
	}
}
