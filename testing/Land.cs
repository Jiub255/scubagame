using Godot;

[Tool]
public partial class Land : StaticBody2D
{
	private Polygon2D Polygon2D { get; set; }
	private Line2D Line2D { get; set; }
	private CollisionPolygon2D CollisionPolygon2D { get; set; }
	private LightOccluder2D LightOccluder2D { get; set; }
	
	public override void _Ready()
	{
		base._Ready();
		Polygon2D = GetNode<Polygon2D>("Polygon2D");
		Line2D = GetNode<Line2D>("Line2D");
		CollisionPolygon2D = GetNode<CollisionPolygon2D>("CollisionPolygon2D");
		LightOccluder2D = GetNode<LightOccluder2D>("LightOccluder2D");
		
/* 		if (!Engine.IsEditorHint())
		{
			CollisionPolygon2D collisionPolygon2D = new()
			{
				Polygon = Polygon2D.Polygon
			};
			AddChild(collisionPolygon2D);
		} */
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		if (Engine.IsEditorHint())
		{
			CollisionPolygon2D.Polygon = Polygon2D.Polygon;
			Line2D.Points = Polygon2D.Polygon;
			LightOccluder2D.Occluder.Polygon = Polygon2D.Polygon;
		}
	}
}
