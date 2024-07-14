public partial class PlayerLocationStateMachine : PlayerStateMachine3
{
	public new PlayerLocationState CurrentState 
	{ 
		get 
		{ 
			return (PlayerLocationState)base.CurrentState; 
		} 
		set
		{
			CurrentState = value;
		}
	}
	
	public PlayerLocationStateMachine(Player3 player) : base(player)
	{
		ChangeState(new WaterState(player));
	}
}
