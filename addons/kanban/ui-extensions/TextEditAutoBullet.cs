using Godot;
using System.Collections.Generic;

public partial class TextEditAutoBullet : TextEdit
{
	private char BulletChar  { get; } = '*';
	private string BulletPointString { get { return BulletChar + " "; } }
	private bool JustMoved { get; set; } = false;
	
	public override void _EnterTree()
	{
		base._EnterTree();

		TextChanged += OnTextChanged;
		TextSet += OnTextSet;
		CaretChanged += MoveCaretOutOfBulletPointArea;

		SetBulletPoints();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		TextChanged -= OnTextChanged;
		TextSet -= OnTextSet;
		CaretChanged -= MoveCaretOutOfBulletPointArea;
	}

	private void OnTextChanged()
	{
		SetBulletPoints();
	}

	private void OnTextSet()
	{
		SetBulletPoints();
	}

	// TODO: Handle going backwards through bullet point area when clicking back arrow. Might have to keep a reference to the last caret position to check its movement,
	// Or check left arrow use? But that seems stupider. How to check arrow moved caret vs mouse clicked? Use Input/GuiInput?
	private void MoveCaretOutOfBulletPointArea()
	{
		if (JustMoved)
		{
			JustMoved = false;
		}
		else
		{
			int caretLine = GetCaretLine();
			int index = GetFirstNonWhiteSpaceColumn(caretLine);
			if (Text[index] == BulletChar)
			{
				if (GetCaretColumn() <= index + 1)
				{
					JustMoved = true;
					SetCaretColumn(index + 2);
				}
			}
		}
	}
	
	private void SetBulletPoints()
	{
		List<string> lines = SplitTextIntoLines();
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i] = InsertBulletPointAfterWhitespace(lines[i], CaretOnLine(i));
		}
		string text = "";
		foreach (string line in lines)
		{
			// Does this include newlines?
			text += line;
		}
		Text = text;
	}

	private bool CaretOnLine(int lineIndex)
	{
		int caretLine = GetCaretLine();
		return caretLine == lineIndex;
	}

	private string InsertBulletPointAfterWhitespace(string line, bool caretOnLine)
	{
		int i = 0;
		// Go to end of whitespace on newline. 
		while (i < line.Length - 1 && char.IsWhiteSpace(line, i))
		{
			i++;
		}
		
		// Don't insert BulletPoint if one already exists, or if line is all whitespace.
		if (line[i] != BulletChar && i != line.Length - 1)
		{
			int cursorColumn = GetCaretColumn();
			line = line.Insert(i, BulletPointString);
			if (caretOnLine)
			{
				SetCaretColumn(cursorColumn + BulletPointString.Length);
			}
		}
		return line;
	}
	
	private List<string> SplitTextIntoLines()
	{
		List<string> lines = new();
		for (int i = 0; i < GetLineCount() - 1; i++)
		{
			lines.Add(GetLine(i));
		}
		return lines;
	}
}
