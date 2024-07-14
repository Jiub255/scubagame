public class WaterState : PlayerLocationState, ICanExitWater
{
	public WaterState(Player3 characterBody2D) : base(characterBody2D)
	{
	}

	public override bool CanMove => true;

	public override void EnterState() {}

	public override void ExitState() {}

	public void ExitWater()
	{
		ChangeState(new AirState(CharacterBody2D));
	}

	public override void PhysicsProcessState(double delta) {}

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
				//ChangeState(CharacterBody2D.Machine.Factory.DieState);
			}
		}
	}
}
