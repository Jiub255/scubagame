// TODO: Use States/SubStates enums? Could have a wrapper class in derived Factory/Machine
// to call ChangeState(BaseState<T>) that accepts enums. 
using System.Collections.Generic;

public enum States 
{
	WATER,
	AIR
}

public enum SubStates
{
	MOVEMENT,
	TAKEDAMAGE,
	DIE
}

public class PlayerStateFactory2 : BaseStateFactory<Player>
{
	// States
	public TestWaterState Water { get; set; }
	public TestAirState Air { get; set; }
	
	// Sub states
	public TestMovementState Movement { get; set; }
	public TestTakeDamageState TakeDamage { get; set; }
	public TestDieState Die { get; set; }

	public Dictionary<States, BasePlayerState> StatesDict { get; set; } = new ();
	public Dictionary<SubStates, BasePlayerState> SubStatesDict { get; set; } = new ();
	
	
	public PlayerStateFactory2(Player player) : base(player)
	{
		CreateStates(player);
	}

	public override void CreateStates(Player player)
	{
		Water = new TestWaterState(player);
		Air = new TestAirState(player);
		
		Movement = new TestMovementState(player);
		TakeDamage = new TestTakeDamageState(player);
		Die = new TestDieState(player);

		StatesDict.Add(States.WATER, Water);
		StatesDict.Add(States.AIR, Air);
		
		SubStatesDict.Add(SubStates.MOVEMENT, Movement);
		SubStatesDict.Add(SubStates.TAKEDAMAGE, TakeDamage);
		SubStatesDict.Add(SubStates.DIE, Die);

		Water.OnStateChanged += (state) => ChangeState(state);
		Air.OnStateChanged += (state) => ChangeState(state);
	}
	
	public void ChangeState(States newState)
	{
		ChangeState(StatesDict[newState]);
	}
}
