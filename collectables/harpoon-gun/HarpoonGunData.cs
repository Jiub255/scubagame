using Godot;

[GlobalClass]
public partial class HarpoonGunData : Resource
{
	[Export]
	private Texture2D _sprite;
	[Export]
	private float _attackTimerLength = 1f;
	[Export]
	private int _damage = 1;
	[Export]
	private float _speed = 250f;
	
	public Texture2D Sprite { get { return _sprite; } }
	public float AttackTimerLength { get { return _attackTimerLength; } }
	public int Damage { get { return _damage; } }
	public float Speed { get { return _speed; } }
	
	public bool GunAcquired { get; set; }
	
	// Use these for UI?
	public float ShotsPerSecond 
	{ 
		get 
		{
			return 1 / _attackTimerLength;
		}	
	}
	public float DPS
	{
		get
		{
			return ShotsPerSecond * _damage;
		}
	}
}
