using Godot;

[Tool]
public partial class Colortest : VBoxContainer
{
	// Accent: 3e683e
	// Base: 4f453b
	
	public override void _EnterTree()
	{
		SetThemeColors();
	}
	
	private void SetThemeColors()
	{
		float saturationDeltaSmallBase = 0.05f;
		float brightnessDeltaSmallBase = 0.085f;
		float saturationDeltaBigBase = 0.014f;
		float brightnessDeltaBigBase = 0.17f;
		
		float saturationDeltaSmallAccent = 0.1f;
		float brightnessDeltaSmallAccent = 0.12f;
		float saturationDeltaBigAccent = 0.17f;
		float brightnessDeltaBigAccent = 0.22f;
		
		EditorSettings editorSettings = EditorInterface.Singleton.GetEditorSettings();
		
		Color baseColor = (Color)editorSettings.GetSetting("interface/theme/base_color");
		
		Color baseLight = baseColor.Brighten(saturationDeltaSmallBase, brightnessDeltaSmallBase);
		Color baseDark = baseColor.Darken(saturationDeltaSmallBase, brightnessDeltaSmallBase);
		
		Color baseLighter = baseColor.Brighten(saturationDeltaBigBase, brightnessDeltaBigBase);
		Color baseDarker = baseColor.Darken(saturationDeltaBigBase, brightnessDeltaBigBase);
		
		Color accentColor = (Color)editorSettings.GetSetting("interface/theme/accent_color");
		
		Color accentLight = accentColor.Brighten(saturationDeltaSmallAccent, brightnessDeltaSmallAccent);
		Color accentDark = accentColor.Darken(saturationDeltaSmallAccent, brightnessDeltaSmallAccent);
		
		Color accentLighter = accentColor.Brighten(saturationDeltaBigAccent, brightnessDeltaBigAccent);
		Color accentDarker = accentColor.Darken(saturationDeltaBigAccent, brightnessDeltaBigAccent);

		HBoxContainer hBox = (HBoxContainer)GetNode("HBoxContainer");
		HBoxContainer hBox2 = (HBoxContainer)GetNode("HBoxContainer2");

		foreach (Node child in hBox.GetChildren())
		{
			child.QueueFree();
		}
		foreach (Node child in hBox2.GetChildren())
		{
			child.QueueFree();
		}

		AddColorRect(hBox, baseLighter);
		AddColorRect(hBox, baseLight);
		AddColorRect(hBox, baseColor);
		AddColorRect(hBox, baseDark);
		AddColorRect(hBox, baseDarker);
		
		AddColorRect(hBox2, accentLighter);
		AddColorRect(hBox2, accentLight);
		AddColorRect(hBox2, accentColor);
		AddColorRect(hBox2, accentDark);
		AddColorRect(hBox2, accentDarker);
	}
	
	private void AddColorRect(HBoxContainer hBox, Color color)
	{
		ColorRect colorRect = new ColorRect
		{
			Color = color,
			SizeFlagsHorizontal = SizeFlags.ExpandFill
		};
		hBox.AddChild(colorRect);
	}
}
