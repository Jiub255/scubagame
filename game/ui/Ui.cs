using Godot;
using System;

public partial class Ui : CanvasLayer
{
	[Export]
	private Button _saveButton;
	[Export]
	private Button _loadButton;
	[Export]
	private Button _exitButton;
	[Export]
	private Inventory _saveableDataResource;
	
	private Button SaveButton 
	{
		get { return _saveButton; }
	}
	private Button LoadButton 
	{
		get { return _loadButton; }
	}
	private Button ExitButton 
	{
		get { return _exitButton; }
	}
	private Inventory SaveableDataResource
	{
		get { return _saveableDataResource; }
	}
	
	private Saver Saver { get; set; }
	
	public override void _Ready()
	{
		SaveButton.Pressed += SaveGame;
		LoadButton.Pressed += LoadGame;
		ExitButton.Pressed += ExitGame;

		Saver = new Saver(SaveableDataResource);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		SaveButton.Pressed -= SaveGame;
		LoadButton.Pressed -= LoadGame;
		ExitButton.Pressed -= ExitGame;
	}

	private void SaveGame()
	{
		GD.Print("Save");
		Saver.SaveGame();
	}
	
	private void LoadGame()
	{
		GD.Print("Load");
		Saver.LoadGame();
	}
	
	private void ExitGame()
	{
		GD.Print("Exit");
	}
}
