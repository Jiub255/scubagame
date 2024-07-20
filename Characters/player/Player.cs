using Godot;

public partial class Player : CharacterBody2D, IDamageable, ICanEnterWater, ICanExitWater
{
	[Export]
	private PlayerData _data;
	[Export]
	private Inventory _inventory;
	
	public PlayerData Data { get { return _data; } }
	public Inventory Inventory { get { return _inventory; } }
	public AnimationTree AnimationTree { get; private set; }
	public Sprite2D Sprite { get; private set; }
	public HarpoonGun HarpoonGun { get; private set; }
	private PlayerLocationStateMachine LocationMachine { get; set; }
	private PlayerActionStateMachine ActionMachine { get; set; }
	
	public override void _Ready()
	{
		base._Ready();

		AnimationTree = GetNode<AnimationTree>("AnimationTree");
		Sprite = GetNode<Sprite2D>("BodySprite");
		HarpoonGun = GetNode<HarpoonGun>("HarpoonGun");
		LocationMachine = new PlayerLocationStateMachine(this);
		ActionMachine = new PlayerActionStateMachine(this);

		Data.Changed += SetSprites;
		//Data.OnHealthDeath += () => { };
		Data.OnDrowned += Drown;

		SetSprites();

		Data.RefillAir();
		
		// Just for testing.
		Data.RefillHealth();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		Data.Changed -= SetSprites;
		//Data.OnHealthDeath -= () => { };
		Data.OnDrowned -= Drown;
	}

	private void Drown()
	{
		this.PrintDebug("Drown called");
		ActionMachine.ChangeState(new PlayerDeathState(this));
	}

	private void SetSprites()
	{
		Sprite.Texture = Data.ScubaGear.Spritesheet;
		HarpoonGun.Sprite.Texture = Data.HarpoonGun.GunAcquired ? Data.HarpoonGun.Sprite : null;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		LocationMachine.ProcessState(delta);
		ActionMachine.ProcessState(delta);
		
		// TODO: Not sure if splitting up movement like this is a good idea.
		if (LocationMachine.CurrentState.CanMove)
		{
			if (ActionMachine.CurrentState is ICanMove movable)
			{
				movable.GetMovementInput();
			}
		}

		if (ActionMachine.CurrentState is ICanAttack attackable)
		{
			attackable.HandleAttack();
		}

		//this.PrintDebug($"Velocity: {Velocity}");
		//this.PrintDebug($"ActionState: {ActionMachine.CurrentState}, LocationState: {LocationMachine.CurrentState}");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		LocationMachine.PhysicsProcessState(delta);
		ActionMachine.PhysicsProcessState(delta);
		
		// TODO: Not sure if splitting up movement like this is a good idea.
		if (LocationMachine.CurrentState.CanMove)
		{
			if (ActionMachine.CurrentState is ICanMove movable)
			{
				movable.Move((float)delta);
			}
		}
		
		HarpoonGun.RotateGun();
		MoveAndSlide();
	}
	
	public void TakeDamage(int damage, Vector2 knockbackDirection)
	{
		if (ActionMachine.CurrentState is IDamageable damageable)
		{
			damageable.TakeDamage(damage, knockbackDirection);
		}
	}

	public void EnterWater()
	{
		if (LocationMachine.CurrentState is ICanEnterWater waterEnterer)
		{
			waterEnterer.EnterWater();
		}
	}

	public void ExitWater()
	{
		if (LocationMachine.CurrentState is ICanExitWater waterExiter)
		{
			waterExiter.ExitWater();
		}
	}
}
