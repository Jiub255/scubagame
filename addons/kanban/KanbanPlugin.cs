#if TOOLS
using Godot;

[Tool]
public partial class KanbanPlugin : EditorPlugin
{
	private PackedScene KanbanPackedScene { get; set; } = ResourceLoader.Load<PackedScene>("res://addons/kanban/board/kanban_board.tscn");
	private KanbanBoard KanbanInstance { get; set; }
	private EditorInterface EditorInterfaceSingleton { get; set; } = EditorInterface.Singleton;
	private KanbanSaver KanbanSaverInstance { get; set; } = new();
	
	public override void _EnterTree()
	{
		BoardData boardData = KanbanSaverInstance.LoadBoard();
		
		KanbanInstance = (KanbanBoard)KanbanPackedScene.Instantiate();
		KanbanInstance.Theme = IntegrateEditorTheme();

		// Add the main panel to the editor's main viewport.
		EditorInterfaceSingleton.GetEditorMainScreen().AddChild(KanbanInstance);
		
		KanbanInstance.InitializeBoard(boardData);
		KanbanInstance.OnBoardChanged += SaveBoard;

		//PermanentlyCopyEditorThemeIntoResource();
		
		// Hide the main panel. Very much required.
		_MakeVisible(false);
	}

	public override void _ExitTree()
	{
		if (KanbanInstance != null)
		{
			SaveBoard();
			KanbanInstance.OnBoardChanged -= SaveBoard;
			KanbanInstance.QueueFree();
		}
	}
	
	private Theme IntegrateEditorTheme()
	{
		// Copy needed types' styleboxes from editor theme into copy
		Theme copy = new Theme();
		Theme editorTheme = EditorInterfaceSingleton.GetEditorTheme();
		string[] themeTypes = { "PanelContainer", "MarginContainer", "VBoxContainer", "HBoxContainer",
			"Button", "LineEdit", "TextEdit", "Label"};
		foreach (string themeType in themeTypes)
		{
			CopyThemeType(editorTheme, copy, themeType);
		}
		
		// TODO: Try using Theme variations? Then have whatever individual controls set them on themselves
		// (or have a parent with a script do it)? 
		// Probably easier to manually override the necessary ui elements' themes with saved .tres files,
		// then just copy the data to them from here. 

		// Alterations for Kanban Board theme
		StyleBoxFlat panelContainerStylebox = (StyleBoxFlat)copy.GetStylebox("panel", "PanelContainer");
		panelContainerStylebox.DrawCenter = true;
		StyleBoxFlat buttonNormalStylebox = (StyleBoxFlat)copy.GetStylebox("normal", "Button");

		// Get editor color variations
		//string[] editorColors = editorTheme.GetColorList("Editor");
		//this.PrintDebug($"Editor colors: {editorColors.Join(", ")}");
		Color backgroundColor = editorTheme.GetColor("background", "Editor");
		Color baseColor = editorTheme.GetColor("base_color", "Editor");
		Color darkColor = editorTheme.GetColor("dark_color_1", "Editor");
		Color darkerColor = editorTheme.GetColor("dark_color_2", "Editor");
		Color darkestColor = editorTheme.GetColor("dark_color_3", "Editor");
		Color contrast_low = editorTheme.GetColor("contrast_color_1", "Editor");
		Color contrast_high = editorTheme.GetColor("contrast_color_2", "Editor");
		Color accentColor = editorTheme.GetColor("accent_color", "Editor");
		// Mellower version of accent color.
		Color fontHoverPressedColor = editorTheme.GetColor("font_hover_pressed_color", "Editor");
		Color selectionColor = editorTheme.GetColor("selection_color", "Editor");
		
		// Set board variation stylebox (do before adding borders)
		LoadAndChangeStylebox(panelContainerStylebox, backgroundColor, "res://addons/kanban/styleboxes/board_stylebox.tres");
		
		// Change panelContainerStylebox, borders needed for Dark themes.
		panelContainerStylebox.BorderWidthTop = buttonNormalStylebox.BorderWidthTop;
		panelContainerStylebox.BorderWidthBottom = buttonNormalStylebox.BorderWidthBottom;
		panelContainerStylebox.BorderWidthLeft = buttonNormalStylebox.BorderWidthLeft;
		panelContainerStylebox.BorderWidthRight = buttonNormalStylebox.BorderWidthRight;
		panelContainerStylebox.BorderColor = buttonNormalStylebox.BorderColor;

		// Set column variation stylebox
		LoadAndChangeStylebox(panelContainerStylebox, baseColor, "res://addons/kanban/styleboxes/column_stylebox.tres");
		
		// Set card variation stylebox
		LoadAndChangeStylebox(panelContainerStylebox, buttonNormalStylebox.BgColor, "res://addons/kanban/styleboxes/card_stylebox.tres");
		
		// Set card text panels variation stylebox
		LoadAndChangeStylebox(panelContainerStylebox, selectionColor, "res://addons/kanban/styleboxes/card_text_stylebox.tres");
		
		return copy;
	}
	
	private void LoadAndChangeStylebox(StyleBoxFlat variation, Color bgColor, string path)
	{
		StyleBoxFlat loadedStylebox = ResourceLoader.Load<StyleBoxFlat>(path);
		if (loadedStylebox == null)
		{
			GD.PushError($"Failed to load resource from {path}");
		}
		else
		{
			loadedStylebox.BgColor = bgColor;
			loadedStylebox.DrawCenter = true;
			loadedStylebox.BorderWidthTop = variation.BorderWidthTop;
			loadedStylebox.BorderWidthBottom = variation.BorderWidthBottom;
			loadedStylebox.BorderWidthLeft = variation.BorderWidthLeft;
			loadedStylebox.BorderWidthRight = variation.BorderWidthRight;
			loadedStylebox.BorderColor = variation.BorderColor;

			Error error = ResourceSaver.Save(loadedStylebox, path);
			if (error == Error.Ok)
			{
				this.PrintDebug($"Successfully updated and saved resource at {path}");
			}
			else
			{
				GD.PushError(error);
			}
		}
	}
	
/* 	private void OverwriteStyleboxVariationResource(StyleBoxFlat baseStyleBox, Color color, string path)
	{
		StyleBoxFlat variation = (StyleBoxFlat)baseStyleBox.Duplicate(true);
		this.PrintDebug($"{variation.BgColor} before");
		variation.BgColor = color;
		SaveResource(variation, path);
	}

	private void SaveResource(Resource resource, string path)
	{
		path = ProjectSettings.GlobalizePath(path);
		if (System.IO.File.Exists(path))
		{
			this.PrintDebug($"File at {path} exists. Deleting.");
			System.IO.File.Delete(path);
		}
		Error error = ResourceSaver.Save(resource, path);
		if (error != Error.Ok)
		{
			GD.PushError(error);
		}
		else
		{
			Resource importedResource = ResourceLoader.Load<Resource>(path);
			if (importedResource == null)
			{
				GD.PrintErr($"Failed to load resource at {path}");
			}
			else
			{				
				resource.EmitChanged();
				//importedResource.EmitChanged();
				
				// Force a rescan?
				ProjectSettings.Save();
				
				this.PrintDebug($"Successfully saved and imported {resource.ResourceName}");
			}
		}
	} */

	private void CopyThemeType(Theme sourceTheme, Theme targetTheme, string themeType)
	{
		// Copy styleboxes
		foreach (string styleboxName in sourceTheme.GetStyleboxList(themeType))
		{
			StyleBox stylebox = sourceTheme.GetStylebox(styleboxName, themeType);
			targetTheme.SetStylebox(styleboxName, themeType, stylebox);
		}

		// Copy fonts
		foreach (string fontName in sourceTheme.GetFontList(themeType))
		{
			Font font = sourceTheme.GetFont(fontName, themeType);
			targetTheme.SetFont(fontName, themeType, font);
		}

		// Copy colors
		foreach (string colorName in sourceTheme.GetColorList(themeType))
		{
			Color color = sourceTheme.GetColor(colorName, themeType);
			targetTheme.SetColor(colorName, themeType, color);
		}

		// Copy constants
		foreach (string constantName in sourceTheme.GetConstantList(themeType))
		{
			int constantValue = sourceTheme.GetConstant(constantName, themeType);
			targetTheme.SetConstant(constantName, themeType, constantValue);
		}

		// Copy icons
		foreach (string iconName in sourceTheme.GetIconList(themeType))
		{
			Texture2D icon = sourceTheme.GetIcon(iconName, themeType);
			targetTheme.SetIcon(iconName, themeType, icon);
		}
	}
	
/* 	private void PermanentlyCopyEditorThemeIntoResource()
	{
		/////////////////////////////////////////////////////////////////////////////
		//       TEMPORARY! JUST NEED TO RUN ONCE THEN COMMENT OUT OR DELETE!      //
		/////////////////////////////////////////////////////////////////////////////
		// Permanently copies the current editor theme into editor_theme_copy.tres //
		/////////////////////////////////////////////////////////////////////////////
		Theme editorTheme = EditorInterfaceSingleton.GetEditorTheme();
		//SaveResource(editorTheme, "res://addons/kanban/themes/editor_theme_copy3.tres");
		Theme editorThemeCopyResource = GD.Load<Theme>("res://addons/kanban/themes/editor_theme_copy.tres");
		editorThemeCopyResource.MergeWith(editorTheme);
		/////////////////////////////////////////////////////////////////////////////
	} */
	
	private void SaveBoard()
	{
		KanbanSaverInstance.SaveBoard(KanbanInstance.GetBoardData());
	}

	public override bool _HasMainScreen()
	{
		return true;
	}

	public override void _MakeVisible(bool visible)
	{
		if (KanbanInstance != null)
		{
			KanbanInstance.Visible = visible;
		}
	}

	public override string _GetPluginName()
	{
		return "Kanban";
	}

	public override Texture2D _GetPluginIcon()
	{
		return EditorInterfaceSingleton.GetEditorTheme().GetIcon("VBoxContainer", "EditorIcons");
	}

	public override void _Notification(int what)
	{
		base._Notification(what);
		
		if (what == NotificationWMCloseRequest)
		{
			if (KanbanInstance != null)
			{
				SaveBoard();
				KanbanInstance.QueueFree();
				GetTree().Quit();
			}
		}
	}
}
#endif
