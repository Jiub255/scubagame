using Godot;
using System;

public partial class SightRangeStateMachine : StateMachine<EnemySightRange2>
{
    public SightRangeStateMachine(EnemySightRange2 character) : base(character)
    {
    }
}
