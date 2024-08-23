using Godot;
using System;
using System.Collections.Generic;

public partial class TestOptionsLabel : Label
{
	private OptionsMenuButton OptionsButton { get; set; }

	public override void _Ready()
	{
		base._Ready();

		OptionsButton = (OptionsMenuButton)GetChild(0);

		Dictionary<string, Action> labelActionDict = new()
        {
            { "Method One", Method1 },
            { "Method Two", Method2 },
            { "Method Three", Method3 }
        };

		OptionsButton.Initialize(labelActionDict);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
	}
	
	public void Method1()
	{
		Text = "Method 1 called";
	}

	private void Method2()
	{
		Text = "Method 2 called";
	}

	private void Method3()
	{
		Text = "Method 3 called";
	}
}
