using Godot;
using System;
using System.Collections.Generic;

// This might just be a stupid idea. Would need to have an enum passed in the generic
// which matches the concrete states that the factory makes. Just make separate machines
// and factories for player/enemies. 
public abstract class StateFactory<EnumType> where EnumType : Enum
{
	public CharacterBody2D Character { get; set; }
	public Dictionary<EnumType, PlayerState> StateDict { get; private set;}

	public abstract void SetupDict(Dictionary<EnumType, PlayerState> stateDict);
	
	public StateFactory(CharacterBody2D character)
	{
		Character = character;
	}
}
