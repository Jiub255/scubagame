using Godot;

[GlobalClass]
public abstract partial class EnemyChaseState : EnemyStateNode
{
	protected int Speed { get; private set; }

	public override void InitializeState(EnemyIdleChase enemy)
	{
		base.InitializeState(enemy);

		Speed = Enemy.Data.ChaseSpeed;
	}
}
