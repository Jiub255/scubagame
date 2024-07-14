using Godot;
using System;

public class Saver
{
	private string SavePath { get; } = "user://save.jame";
	private Inventory Inventory { get; }
	
	public Saver(Inventory inventory)
	{
		Inventory = inventory;
	}
	
	/// <returns>true if save was successful</returns>
	public bool SaveGame()
	{		
		using (FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write))
		{
			if (file != null)
			{
				uint coins = (uint)Inventory.Coins;
				file.Store32(coins);
				return true;
			}
			else
			{
				GD.PrintErr("Save failed");
				return false;
			}
		}
	}
	
	/// <returns>true if load was successful</returns>
	public bool LoadGame()
	{
		using (FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read))
		{
			if (file != null)
			{
				// TODO: How to keep this organized when saving multiple variables?
				int coins = (int)file.Get32();
				Inventory.Coins = coins;
				return true;
			}
			else
			{
				GD.PrintErr("Load failed");
				return false;
			}
		}
	}
}
