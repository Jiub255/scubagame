public partial class PlayerActionStateMachine : PlayerStateMachine3
{
	public new PlayerActionState CurrentState 
	{ 
		get 
		{ 
			return (PlayerActionState)base.CurrentState; 
		} 
		set
		{
			CurrentState = value;
		}
	}
	
	public PlayerActionStateMachine(Player3 player) : base(player)
	{
		ChangeState(new PlayerMovementState3(player));
	}
}
