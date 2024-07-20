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
	
	public int MaxHealth 
	{ 
		get 
		{ 
			return _maxHealth; 
		}
		set
		{
			_maxHealth = value;
			EmitChanged();
		} 
	}
	/// <summary>
	/// Measured in seconds.
	/// </summary>
	public int MaxAir 
	{ 
		get 
		{ 
			return _maxAir; 
		}
		set
		{
			_maxAir = value;
			EmitChanged();
		} 
	}
	public Texture2D Spritesheet { get { return _spritesheet; } }	
	public float MinAcceleration 
	{ 
		get 
		{ 
			return _minAcceleration; 
		}
		set
		{
			_minAcceleration = value;
			EmitChanged();
		} 
	}
	public float MaxAcceleration 
	{ 
		get 
		{ 
			return _maxAcceleration; 
		}
		set
		{
			_maxAcceleration = value;
			EmitChanged();
		} 
	}
	public float MaxSpeed 
	{ 
		get 
		{ 
			return _maxSpeed; 
		}
		set
		{
			_maxSpeed = value;
			EmitChanged();
		} 
	}
	public float Jerk 
	{ 
		get 
		{ 
			return _jerk; 
		}
		set
		{
			this.PrintDebug($"Old jerk value: {_jerk}");
			_jerk = value;
			this.PrintDebug($"New jerk value: {_jerk}");
			EmitChanged();
		} 
	}
	public float Deceleration 
	{ 
		get 
		{ 
			return _deceleration; 
		}
		set
		{
			_deceleration = value;
			EmitChanged();
		} 
	}
}
