using System;
using System.Collections.Generic;

public enum PlayerStateNames
{
	MOVEMENT,
	AIR,
	ATTACK,
	TAKEDAMAGE,
	DIE
}

public class PlayerStateFactory
{
	public event Action<PlayerStateNames> OnStateChanged;
	private PlayerMovementState MovementState { get; set; }
	private PlayerAirState AirState { get; set; }
	private PlayerTakeDamageState TakeDamageState { get; set; }
	private PlayerDieState DieState { get; set; }
	public Dictionary<PlayerStateNames, PlayerState> StateDict { get; private set; } = new();
	
	public PlayerStateFactory(Player player)
	{
		//GD.Print("Factory created");
		SetupDict(player);
	}

	public void SetupDict(Player player)
	{
		MovementState = new(player);
		AirState = new(player);
		TakeDamageState = new(player);
		DieState = new(player);
		
		MovementState.OnStateChanged += (stateName) => OnStateChanged?.Invoke(stateName);
		AirState.OnStateChanged += (stateName) => OnStateChanged?.Invoke(stateName);
		TakeDamageState.OnStateChanged += (stateName) => OnStateChanged?.Invoke(stateName);
		DieState.OnStateChanged += (stateName) => OnStateChanged?.Invoke(stateName);
		
		StateDict.Add(PlayerStateNames.MOVEMENT, MovementState);
		StateDict.Add(PlayerStateNames.AIR, AirState);
		StateDict.Add(PlayerStateNames.TAKEDAMAGE, TakeDamageState);
		StateDict.Add(PlayerStateNames.DIE, DieState);
	}
	
	public void ExitTree()
	{
		MovementState.OnStateChanged -= (stateName) => OnStateChanged?.Invoke(stateName);
		AirState.OnStateChanged -= (stateName) => OnStateChanged?.Invoke(stateName);
		TakeDamageState.OnStateChanged -= (stateName) => OnStateChanged?.Invoke(stateName);
		DieState.OnStateChanged -= (stateName) => OnStateChanged?.Invoke(stateName);
	}
}
