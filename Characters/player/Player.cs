using Godot;

public partial class Player : CharacterBody2D, IDamageable
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
	private PlayerStateMachine PlayerStateMachine { get; set; }
	//public PlayerStateMachine2 Machine { get; private set; }
	
	public override void _Ready()
	{
		base._Ready();

		AnimationTree = GetNode<AnimationTree>("AnimationTree");
		Sprite = GetNode<Sprite2D>("BodySprite");
		HarpoonGun = GetNode<HarpoonGun>("HarpoonGun");
		PlayerStateMachine = new PlayerStateMachine(this);

		Data.Changed += SetSprites;

		SetSprites();
		Data.RefillAir();
		
		// Just for testing.
		Data.Health = Data.ScubaGear.MaxHealth;
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		Data.Changed -= SetSprites;
		
		PlayerStateMachine.ExitTree();
	}

	private void SetSprites()
	{
		Sprite.Texture = Data.ScubaGear.Spritesheet;
		HarpoonGun.Texture = Data.HarpoonGun.GunAcquired ? Data.HarpoonGun.Sprite : null;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		PlayerStateMachine.ProcessState(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		PlayerStateMachine.PhysicsProcessState(delta);
		HarpoonGun.RotateGun(Velocity);
		MoveAndSlide();
	}

	public void TakeDamage(int damage, Vector2 knockbackDirection)
	{
		Data.KnockbackDirection = knockbackDirection;
		PlayerStateMachine.CurrentState.MaybeTakeDamage(damage);
	}
	
	public void Jump()
	{
		PlayerStateMachine.CurrentState.Jump();
	}
	
	public void Land()
	{
		PlayerStateMachine.CurrentState.Land();
	}
}
