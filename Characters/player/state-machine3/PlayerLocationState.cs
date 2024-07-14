public abstract class PlayerLocationState : PlayerState3
{
	public abstract bool CanMove { get; }
	
	public PlayerLocationState(Player3 player) : base(player)
	{
	}
}
