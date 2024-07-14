using Godot;
using System;

public partial class HarpoonGun : Sprite2D
{
	private AnimationPlayer AnimationPlayer { get; set; }
	private string HarpoonPath { get; } = "res://Characters/player/harpoon/harpoon.tscn";
	private PackedScene HarpoonScene { get; set; }
	
	// TODO: Put this in HarpoonGunData? 
/* 	private bool _gunAcquired = true;
	public bool GunAcquired 
	{ 
		get { return _gunAcquired; } 
		set
		{
			_gunAcquired = value;
			ProcessMode = GunAcquired ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
		}
	} */
	public Vector2 ShootDirection { get; set; } = Vector2.Right;
	
	public override void _Ready()
	{
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		HarpoonScene = ResourceLoader.Load<PackedScene>(HarpoonPath);
		
		//ProcessMode = GunAcquired ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
	}

	// TODO: Have harpoon gun point towards mouse. 
	public void RotateGun(Vector2 velocity)
	{
		if (/* GunAcquired &&  */velocity.Length() > 0)
		{
			ShootDirection = velocity.Normalized();
			Rotation = ShootDirection.Angle();
		}
	}
	
	
	public void Shoot(int damage, float speed)
	{
		// TODO: Instantiate harpoon when attack timer resets. Place it behind the gun so it looks loaded. 
		// Instantiate harpoon.
		Harpoon harpoon = HarpoonScene.Instantiate() as Harpoon;
		GetTree().Root.AddChild(harpoon);
		harpoon.Position = GlobalPosition;
		harpoon.Rotation = Rotation;

		// Have it set itself up.
		harpoon.SetupHarpoon(damage, ShootDirection * speed);
		
		// Play little kickback animation. 
		AnimationPlayer.Play("kickback");
	}
}
