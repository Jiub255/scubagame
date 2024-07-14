using Godot;

public partial class Chest : Area2D, ICollectable
{
	[Export]
	public int Value { get; set; } = 1;
	
	public void GetCollected(Player3 player)
	{
		player.Inventory.AddCoins(Value);
		QueueFree();
	}
}
