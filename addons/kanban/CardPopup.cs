using System;
using System.Collections.Generic;
using Godot;

[Tool]
public partial class CardPopup : Button
{
	public event Action OnClosePopup;
	
	private KanbanCard Card;
	private LineEdit Title { get; set; }
	private TextEdit Description { get; set; }
	private List<Control> Controls { get; set; } = new();

	public override void _EnterTree()
	{
		base._EnterTree();
		
		Title = (LineEdit)GetNode("%Title");
		Description = (TextEdit)GetNode("%Description");

		Pressed += ClosePopup;
	}

	public override void _Ready()
	{
		base._Ready();
		
		Controls = GetAllControlChildren(this);

		ClosePopup();
	}

	
	private List<Control> GetAllControlChildren(Node node)
	{
		List<Control> controls = new();
		Godot.Collections.Array<Node> children = node.GetChildren();
		foreach (Node child in children)
		{
			if (child is Control control)
			{
				controls.Add(control);
			}
			
			if (child.GetChildCount() > 0)
			{
				controls.AddRange(GetAllControlChildren(child));
			}
		}
		return controls;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		Pressed -= ClosePopup;
	}

	public void OpenPopup(KanbanCard card)
	{
		Card = card;
		Title.Text = card.Title.Text;
		Description.Text = card.Description.Text;
		SetAllMouseFilters(MouseFilterEnum.Stop);
		Show();
	}
	
	private void SetAllMouseFilters(MouseFilterEnum mouseFilterEnum)
	{
		MouseFilter = mouseFilterEnum;
		foreach (Control control in Controls)
		{
			control.MouseFilter = mouseFilterEnum;
		}
	}
	
	public void ClosePopup()
	{
		if (Card != null)
		{
			Card.Title.Text = Title.Text;
			Card.Description.Text = Description.Text;
		}
		SetAllMouseFilters(MouseFilterEnum.Ignore);
		Hide();
		OnClosePopup?.Invoke();
	}
}
