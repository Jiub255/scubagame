using Godot;

public partial class Player3 : CharacterBody2D, IDamageable, ICanEnterWater, ICanExitWater
{
	[Export]
	private PlayerData _data;
	public PlayerData Data { get { return _data; } }
	
	[Export]
	private Inventory _inventory;
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

		SetSprites();

		Data.RefillAir();
		
		// Just for testing.
		Data.RefillHealth();
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		Data.Changed -= SetSprites;
		
		//Machine.ExitTree();
	}

	private void SetSprites()
	{
		Sprite.Texture = Data.ScubaGear.Spritesheet;
		HarpoonGun.Texture = Data.HarpoonGun.GunAcquired ? Data.HarpoonGun.Sprite : null;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		LocationMachine.ProcessState(delta);
		ActionMachine.ProcessState(delta);
		
		if (LocationMachine.CurrentState.CanMove)
		{
			if (ActionMachine.CurrentState is ICanMove movable)
			{
				movable.HandleMovement();
			}
		}
		
		if (ActionMachine.CurrentState is ICanAttack attackable)
		{
			attackable.HandleAttack();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		LocationMachine.PhysicsProcessState(delta);
		ActionMachine.PhysicsProcessState(delta);
		
		HarpoonGun.RotateGun(Velocity);
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
