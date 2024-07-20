public class PlayerWaterState : PlayerLocationState, ICanExitWater
{	
	public PlayerWaterState(Player characterBody2D) : base(characterBody2D) {}

	public override bool CanMove => true;

	public void ExitWater()
	{
		ChangeState(new PlayerAirState(CharacterBody2D));
	}

	public override void ProcessState(double delta)
	{
		TickAir((float)delta);
	}
	
	protected void TickAir(float delta)
	{
		CharacterBody2D.Data.AirTimer -= (float)delta;
		if (CharacterBody2D.Data.AirTimer <= 0)
		{
			CharacterBody2D.Data.AirTimer = 1;
			CharacterBody2D.Data.Air--;
			if (CharacterBody2D.Data.Air <= 0)
			{
				// TODO: Can't change LocationState to death, it's an action state.
				// How to handle this? 
				// Call Drown() on Player? Makes sense, player needs a way to drown in a water game.
				//ChangeState(new PlayerDeathState(CharacterBody2D));
			}
		}
	}

	public override void EnterState() {}
	public override void ExitState() {}
	public override void PhysicsProcessState(double delta) {}
}
