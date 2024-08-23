using System;
using Godot;

[Tool]
public partial class CardPopup : Button
{
	public event Action OnClosePopup;
	
	private KanbanCard Card;
	private LineEdit Title { get; set; }
	private TextEditAutoBullet Description { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		
		Title = (LineEdit)GetNode("%Title");
		Description = (TextEditAutoBullet)GetNode("%Description");

		Pressed += Close;
	}

	public override void _Ready()
	{
		base._Ready();
		
		Close();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		Pressed -= Close;
	}

	public void Open(KanbanCard card)
	{
		Card = card;
		Title.Text = card.Title.StoredText;
		Description.Text = card.Description.Text;
		Description.SetBulletPoints();
		Show();
	}
	
	public void Close()
	{
		if (Card != null)
		{
			Card.Title.StoredText = Title.Text;
			Card.Description.Text = Description.Text;
		}
		Hide();
		OnClosePopup?.Invoke();
	}
}
