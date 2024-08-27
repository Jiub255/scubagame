public partial class PlayerActionStateMachine : PlayerStateMachine
{
	public new PlayerActionState CurrentState 
	{ 
		get 
		{ 
			return (PlayerActionState)base.CurrentState; 
		} 
		set
		{
			base.CurrentState = value;
		}
	}
	
	public PlayerActionStateMachine(Player player) : base(player)
	{
		ChangeState(new PlayerMovementState(player));
	}
}
