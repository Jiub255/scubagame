public abstract class PlayerLocationState : PlayerState
{
	public abstract bool CanMove { get; }
	
	public PlayerLocationState(Player player) : base(player) {}
}
