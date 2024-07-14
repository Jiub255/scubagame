using Godot;
using System;

public static class Extensions
{
	public static void PrintDebug(this Object obj, string message)
	{
		GD.Print($"{obj.GetType(),-30}|    {message}");
	}
}
