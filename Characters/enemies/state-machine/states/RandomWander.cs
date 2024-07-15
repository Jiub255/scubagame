using Godot;

[GlobalClass]
public partial class RandomWander : Resource
{
	[Export]
	public float MoveTimeMin { get; private set; } = 0.5f;
	[Export]
	public float MoveTimeMax { get; private set; } = 1.5f;
	
	private RandomNumberGenerator RNG { get; set; }
	private Vector2 RandomDirection { get; set; } = Vector2.Down;
	private float Timer { get; set; }
	private float RandomDuration { get; set; }
		
	public Vector2? GetRandomDirection(double delta)
	{
		Timer -= (float)delta;
		if (Timer <= 0)
		{
			SetupNextMovement();
			return RandomDirection;
		}
		else
		{
			return null;
		}
	}
	
	private void SetupNextMovement()
	{
		RandomDirection = Vector2.Up.Rotated(RNG.RandfRange(-Mathf.Pi, Mathf.Pi));
		Timer = RNG.RandfRange(MoveTimeMin, MoveTimeMax);
	}
}
