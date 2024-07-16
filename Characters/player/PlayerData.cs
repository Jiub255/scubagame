using Godot;

[GlobalClass]
public partial class PlayerData : Resource
{
	// Player
	private int _health = 1;
	private int _air = 0;
	
	// TODO: Control death by health/drowning here?
	// Use events to signal Player to die?
	// Makes more sense to have player handle that, this is just data. 
	public int Health 
	{ 
		get { return _health; } 
		set
		{
			_health = value;
			EmitChanged();
		}
	}
	public int Air

	{
		get { return _air; }
		set 
		{ 
			_air = value;
			this.PrintDebug($"Air: {_air}");
			EmitChanged();
		}
	}
	public bool Reloading { get; set; } = false;
	public float AttackTimer { get; set; } = 0f;
	public Vector2 KnockbackDirection { get; set; }
	public float AirTimer { get; set; }
	
	public void RefillAir()
	{
		Air = ScubaGear.MaxAir;
		AirTimer = 1;
	}
	
	public void RefillHealth()
	{
		Health = ScubaGear.MaxHealth;
	}
	
	public void ResetPlayerData()
	{
		HarpoonGun = DefaultHarpoonGun;
		HarpoonGun.GunAcquired = false;
		ScubaGear = DefaultScubaGear;
		Health = ScubaGear.MaxHealth;
		Air = ScubaGear.MaxAir;
	}

	// Scuba Suit
	[Export]
	private ScubaGearData _scubaGear;
	[Export]
	private ScubaGearData _defaultScubaGear;
	
	public ScubaGearData ScubaGear 
	{ 
		get { return _scubaGear; } 
		set 
		{
			_scubaGear = value;
			EmitChanged();
		}
	}
	private ScubaGearData DefaultScubaGear { get { return _defaultScubaGear; } }
	
	// Harpoon Gun
	[Export]
	private HarpoonGunData _harpoonGun;
	[Export]
	private HarpoonGunData _defaultHarpoonGun;
	
	public HarpoonGunData HarpoonGun	
	{ 
		get { return _harpoonGun; } 
		set 
		{
			_harpoonGun = value;
			_harpoonGun.GunAcquired = true;
			EmitChanged();
		}
	}
	private HarpoonGunData DefaultHarpoonGun { get { return _defaultHarpoonGun; } }
}
