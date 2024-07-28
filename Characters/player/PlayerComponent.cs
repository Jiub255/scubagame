using Godot;

// TODO: How to make this work with different nodes, like RayCast2D? Interface? Always have 
// plain Node parent for components? Would an extension method work? 
public partial class PlayerComponent : Node
{
	protected Player Player { get; private set; }
	
	public void InitializeComponent(Player player)
	{
		Player = player;
	}
}
