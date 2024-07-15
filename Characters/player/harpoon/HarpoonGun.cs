using Godot;

public partial class HarpoonGun : Node2D
{
	/// <summary>
	/// Just for testing.
	/// </summary>
	[Export(PropertyHint.Range, "0.0,1.0")]
	private float _deadZone = 0.5f;
	
	private AnimationPlayer AnimationPlayer { get; set; }
	// TODO: Put this in HarpoonGunData? 
	private string HarpoonPath { get; } = "res://Characters/player/harpoon/harpoon.tscn";
	private PackedScene HarpoonScene { get; set; }
	public Sprite2D Sprite { get; set; }
	public Vector2 ShootDirection { get; set; } = Vector2.Right;
	
	public override void _Ready()
	{
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		HarpoonScene = ResourceLoader.Load<PackedScene>(HarpoonPath);
		Sprite = GetNode<Sprite2D>("Sprite2D");
	}

	// Have harpoon gun point towards mouse. 
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
		if (@event is InputEventMouseMotion)
		{
			// Mouse aim
			ShootDirection = (GetLocalMousePosition() - Position).Normalized();
			Sprite.Rotation = ShootDirection.Angle();
			this.PrintDebug($"mouse dir: {ShootDirection}, rot: {ShootDirection.Angle()}");
		}
	}

	// Aim harpoon gun with controller or number keypad. 
	public void RotateGun()
	{

		Vector2 aimDirection = Input.GetVector("aim-left", "aim-right", "aim-up", "aim-down", _deadZone);
		if (aimDirection != Vector2.Zero)
		{
			// Controller right joystick aim
			ShootDirection = aimDirection.Normalized();
			Sprite.Rotation = ShootDirection.Angle();
			this.PrintDebug($"controller dir: {ShootDirection}, rot: {ShootDirection.Angle()}");
		}
	}
	
	
	public void Shoot(int damage, float speed)
	{
		// TODO: Instantiate harpoon when attack timer resets. Place it behind the gun so it looks loaded. 
		// Instantiate harpoon.
		Harpoon harpoon = HarpoonScene.Instantiate() as Harpoon;
		GetTree().Root.AddChild(harpoon);
		harpoon.Position = GlobalPosition;
		harpoon.Rotation = Sprite.Rotation;

		// Have it set itself up.
		harpoon.SetupHarpoon(damage, ShootDirection * speed);
		
		// Play little kickback animation. 
		AnimationPlayer.Play("kickback");
	}
}
