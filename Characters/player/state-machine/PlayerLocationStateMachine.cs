public partial class PlayerLocationStateMachine : PlayerStateMachine
{
	public new PlayerLocationState CurrentState 
	{ 
		get 
		{
			// TODO: This is returning the same base state for both machines.
			// Need to figure something else out. 
			// Maybe just do away with the generics for now, using a different
			// type of machine for enemies anyway. 
			return (PlayerLocationState)base.CurrentState; 
		} 
		set
		{
			base.CurrentState = value;
		}
	}
	
	public PlayerLocationStateMachine(Player player) : base(player)
	{
		ChangeState(new PlayerWaterState(player));
	}
}
