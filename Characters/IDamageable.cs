using Godot;

public interface IDamageable
{
	public void TakeDamage(int damage, Vector2 knockbackDirection);
}
