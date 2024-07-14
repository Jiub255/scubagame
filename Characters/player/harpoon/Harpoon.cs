using Godot;

public partial class Harpoon : Area2D
{
	private int Damage { get; set; }
	private Vector2 Velocity { get; set; }
	
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		BodyEntered -= OnBodyEntered;
	}

	public void SetupHarpoon(int damage, Vector2 velocity)
	{
		Damage = damage;
		Velocity = velocity;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is IDamageable damageable)
		{
			Vector2 direction = body.Position = GlobalPosition;
			damageable.TakeDamage(Damage, direction);
		}
		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += Velocity * (float)delta;
	}
}
