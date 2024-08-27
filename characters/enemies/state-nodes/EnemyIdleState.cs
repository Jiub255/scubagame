using Godot;

[GlobalClass]
public abstract partial class EnemyIdleState : EnemyStateNode
{
	protected int Speed { get; private set; }

	public override void InitializeState(EnemyIdleChase enemy)
	{
		base.InitializeState(enemy);

		Speed = Enemy.Data.IdleSpeed;
	}
}
