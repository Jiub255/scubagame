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
	
	public static bool HasAncestorOfType<T>(this Node node, bool checkSelf = true) where T : Node
	{
		if (checkSelf && node is T)
		{
			return true;
		}
		Node parent = node;
		while (parent != null && parent is not T)
		{
			parent = parent.GetParent();
		}
		if (parent == null)
		{
			return false;
		}
		if (parent is T)
		{
			return true;
		}
		return false;
	}
	
	public static T GetAncestorOfType<T>(this Node node, bool checkSelf = true) where T : Node
	{
		if (checkSelf && node is T t)
		{
			return t;
		}
		Node parent = node;
		while (parent != null && parent is not T)
		{
			parent = parent.GetParent();
		}
		if (parent == null)
		{
			return null;
		}
		if (parent is T t2)
		{
			return t2;
		}
		return null;
	}
}
