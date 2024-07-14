using Godot;

public abstract partial class EnemySightRange : CharacterBody2D, IDamageable
{
	[Export]
	protected int Health { get; set; } = 3;
	[Export]
	protected int Damage { get; set; } = 1;
	[Export]
	protected float Speed { get; set; } = 50f;
	protected Area2D SightRange { get; set; }
	protected Player Target { get; set; }
	protected AnimationPlayer AnimationPlayer { get; set; }
	protected Sprite2D Sprite { get; set; }

	public override void _Ready()
	{
		base._Ready();
		
		SightRange = GetNode<Area2D>("SightRange");
		SightRange.BodyEntered += OnSightRangeBodyEntered;
		SightRange.BodyExited += OnSightRangeBodyExited;
		
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		SightRange.BodyEntered -= OnSightRangeBodyEntered;
		SightRange.BodyExited -= OnSightRangeBodyExited;
	}

	private void OnSightRangeBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			Target = player;
		}
	}

	private void OnSightRangeBodyExited(Node2D body)
	{
		if (body == Target)
		{
			Target = null;
		}
	}

	public override void _PhysicsProcess(double delta)
    {
        if (Target != null)
        {
            ChasePhysicsProcess(delta);
        }
        else
        {
            IdlePhysicsProcess(delta);
        }

        Animate();

        KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);
        HandleCollision(collision);
    }

	public abstract void ChasePhysicsProcess(double delta);

	public abstract void IdlePhysicsProcess(double delta);

	public abstract void Animate();

    private void HandleCollision(KinematicCollision2D collision)
    {
        GodotObject collider = collision?.GetCollider();
        if (collider is Player player)
        {
            HitPlayer(player);
        }
    }

    private void HitPlayer(Player player)
    {
        Vector2 knockbackDirection = (player.Position - Position).Normalized();
        player.TakeDamage(Damage, knockbackDirection);
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
	{
		Health -= damage;
		if (Health <= 0)
		{
			QueueFree();
		}
	}
}
