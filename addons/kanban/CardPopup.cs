using System.Collections.Generic;
using Godot;

[GlobalClass]
//[Tool]
public partial class CardPopup : Button
{
	private KanbanCard Card;
	private TextEdit Title { get; set; }
	private TextEdit Description { get; set; }
	private List<Control> Controls { get; set; } = new();

	public override void _EnterTree()
	{
		base._EnterTree();
		
		Title = (TextEdit)GetNode("%Title");
		Description = (TextEdit)GetNode("%Description");
		Controls = GetAllControlChildren(this);

		Pressed += ClosePopup;

		ClosePopup();
	}

	private List<Control> GetAllControlChildren(Node node)
	{
		List<Control> controls = new();
		Godot.Collections.Array<Node> children = GetChildren();
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
		Title.Text = card.TitleLabel.Text;
		Description.Text = card.DescriptionLabel.Text;
		//MouseFilter = MouseFilterEnum.Stop;
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
		Card.TitleLabel.Text = Title.Text;
		Card.DescriptionLabel.Text = Description.Text;
		//MouseFilter = MouseFilterEnum.Ignore;
		SetAllMouseFilters(MouseFilterEnum.Ignore);
		Hide();
	}
}
