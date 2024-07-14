using Godot;

public partial class HarpoonGunCollectable : Area2D, ICollectable
{
	[Export]
	private HarpoonGunData _harpoonGunData;
	public HarpoonGunData HarpoonGunData { get { return _harpoonGunData; } }
	
	public void GetCollected(Player player)
	{
		player.Data.HarpoonGun = HarpoonGunData;
		QueueFree();
	}
}
