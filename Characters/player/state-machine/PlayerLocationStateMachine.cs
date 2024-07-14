public partial class PlayerLocationStateMachine : PlayerStateMachine
{
	public new PlayerLocationState CurrentState 
	{ 
		get 
		{ 
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
