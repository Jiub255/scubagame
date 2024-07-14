using Godot;

[GlobalClass]
public partial class ScubaGearData : Resource
{
	[Export]
	private int _maxHealth = 3;
	[Export]
	private int _maxAir = 30;
	[Export]
	private Texture2D _spritesheet;
	[Export]
	private float _minAcceleration = 250f;
	[Export]
	private float _maxAcceleration = 600f;
	[Export]
	private float _maxSpeed = 150f;
	[Export]
	private float _jerk = 500f;
	[Export]
	private float _deceleration = 400f;
	
	public int MaxHealth { get { return _maxHealth; } }
	/// <summary>
	/// Measured in seconds.
	/// </summary>
	public int MaxAir { get { return _maxAir; } }
	public Texture2D Spritesheet { get { return _spritesheet; } }	
	public float MinAcceleration { get { return _minAcceleration; } }
	public float MaxAcceleration { get { return _maxAcceleration; } }
	public float MaxSpeed { get { return _maxSpeed; } }
	public float Jerk { get { return _jerk; } }
	public float Deceleration { get { return _deceleration; } }
}
