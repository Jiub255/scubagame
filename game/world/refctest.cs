using Godot;
using System;

public partial class refctest : RefCounted
{
	public void printtest()
	{
		this.PrintDebug("ref counted test");
	}
}
