using Godot;

[GlobalClass]
public partial class Inventory : Resource
{
	private int _coins;
	
	public int Coins 
	{
		get 
		{
			return _coins; 
		}
		
		set 
		{ 
			_coins = value;
			EmitChanged();
		}
	}
	
	public void AddCoins(int number)
	{
		Coins += number;
		GD.Print($"You have {Coins} coins.");
	}
	
	public void ClearInventory()
	{
		Coins = 0;
	}
}
