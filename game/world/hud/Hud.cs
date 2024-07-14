using Godot;

public partial class Hud : CanvasLayer
{
	[Export]
	private Inventory _inventory;
	[Export]
	private PlayerData _data;
	
	private Inventory Inventory { get { return _inventory; } }
	private PlayerData Data { get { return _data; } }
	private RichTextLabel CoinsLabel { get; set; }
	private RichTextLabel AirLabel { get; set; }
	private RichTextLabel HealthLabel { get; set; }
	
	public override void _Ready()
	{
		// TODO: Get this reference in a better way. Export? Instantiate? 
		CoinsLabel = GetNode<RichTextLabel>("%CoinsLabel");
		AirLabel = GetNode<RichTextLabel>("%AirLabel");
		HealthLabel = GetNode<RichTextLabel>("%HealthLabel");
		Inventory.Changed += UpdateHUD;
		Data.Changed += UpdateHUD;
		UpdateHUD();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		Inventory.Changed -= UpdateHUD;
		Data.Changed -= UpdateHUD;
	}

	private void UpdateHUD()
	{
		CoinsLabel.Text = $"Coins: {Inventory.Coins}";
		AirLabel.Text = $"Air: {Data.Air}";
		HealthLabel.Text = $"Health: {Data.Health}";
	}
}
