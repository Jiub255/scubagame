using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
	[Export]
	public int Health { get; set; } = 1;
	[Export]
	public int Damage { get; private set; } = 1;
	[Export]
	public int IdleSpeed { get; private set; } = 25;
	[Export]
	public int ChaseSpeed { get; private set; } = 50;
}
