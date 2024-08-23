using Godot;
using System.Collections.Generic;

// TODO: Split selection when moving up/down lines so it doesn't include BulletPointString and before.

// TODO: Put bullet points on lines with any chars at all, including space/tab? Or just ones with actual characters?
// Then you'd have to delete bullet point string when line gets emptied.

// TODO: Does Text get set on creation and kept as an undoable action? If so, try to stop it so you can't
// accidentally delete your TextEdit by undoing too far. 

// TODO: Undo (RARELY NOW, ALMOST FIXED) jumps caret back to start on undo if you were holding down more than one key while making the original do.

[Tool]
public partial class TextEditAutoBullet : TextEdit
{
	//private event Action OnBulletPointChanged;

	/* [Export]
	private char BulletPoint { get; set; } = '-'; */
	// TODO: Or just let the bullet point be any string, then add a space after it. 
	[Export(PropertyHint.Enum, "-,*,>,~,+,^")]
	private string _bulletPoint = "-";
	private char BulletPoint
	{
		get => _bulletPoint.Length > 0 ? _bulletPoint[0] : '-';
		set
		{
			//OnBulletPointChanged?.Invoke();
			_bulletPoint = value.ToString();
		}
	}
	private string BulletPointWithSpace { get { return BulletPoint + " "; } }
	
	private bool CaretDirectlyAfterBulletPointString { get; set; } = false;
	private bool ComplexOperationStarted { get; set; } = false;
	private bool DontSetBulletPoints { get; set; } = false;
	private bool DontMoveCaret { get; set; } = false;
	
#region INITIALIZATION
	
	public override void _EnterTree()
	{
		base._EnterTree();

		TextChanged += SetBulletPoints;
		CaretChanged += MoveCaretIfInBulletPointArea;
		CaretChanged += CheckCaretPosition;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		
		TextChanged -= SetBulletPoints;
		CaretChanged -= MoveCaretIfInBulletPointArea;
		CaretChanged -= CheckCaretPosition;
	}

	public override void _Ready()
	{
		base._Ready();

		SetBulletPoints();
	}

#endregion

#region SET BULLET POINTS

	public void SetBulletPoints()
	{
		if (DontSetBulletPoints)
		{
			DontSetBulletPoints = false;
			return;
		}

		int caretLine = GetCaretLine();
		int caretColumn = GetCaretColumn();

		List<string> lines = SplitTextIntoLines();
		int bulletPointsAddedBeforeCaret = InsertRemoveBulletPoints(lines);
		RebuildAndAssignText(lines);
		
		SetCaretLine(caretLine);
		SetCaretColumn(caretColumn + (BulletPointWithSpace.Length * bulletPointsAddedBeforeCaret));
	}

	private List<string> SplitTextIntoLines()
	{
		List<string> lines = new();
		for (int i = 0; i < GetLineCount(); i++)
		{
			lines.Add(GetLine(i));
		}
		return lines;
	}

	private int InsertRemoveBulletPoints(List<string> lines)
	{
		int bulletPointsAddedBeforeCaret = 0;
		for (int i = 0; i < lines.Count; i++)
		{
			(string line, bool addedBulletPointBeforeCaret) = InsertRemoveBulletPoint(lines[i], CaretOnOrAfterLine(i));
			lines[i] = line;
			bulletPointsAddedBeforeCaret += addedBulletPointBeforeCaret ? 1 : 0;
		}

		return bulletPointsAddedBeforeCaret;
	}

	private bool CaretOnOrAfterLine(int lineIndex)
	{
		int caretLine = GetCaretLine();
		return caretLine >= lineIndex;
	}

	private (string, bool) InsertRemoveBulletPoint(string line, bool caretOnOrAfterLine)
	{
		bool addedBulletPointBeforeCaret = false;
		if (line == "")
		{
			return (line, addedBulletPointBeforeCaret);
		}

		int firstNonWhiteCharIndex = GetFirstNonWhiteCharIndex(line);

		// Don't insert BulletPoint if one already exists, or if line is all whitespace.
		if (firstNonWhiteCharIndex < line.Length)
		{
			if (line[firstNonWhiteCharIndex] != BulletPoint)
			{
				line = line.Insert(firstNonWhiteCharIndex, BulletPointWithSpace);
				addedBulletPointBeforeCaret = caretOnOrAfterLine;
			}
			else
			{
				// TODO: Remove bullet point if there's nothing following it.
				int startOfNonBulletPointIndex = firstNonWhiteCharIndex + BulletPointWithSpace.Length;
				if (startOfNonBulletPointIndex >= line.Length)
				{
					// TODO: Remove bullet point.
					line = line.Remove(firstNonWhiteCharIndex, BulletPointWithSpace.Length);
				}
			}
		}
		
		return (line, addedBulletPointBeforeCaret);
	}

	private static int GetFirstNonWhiteCharIndex(string line)
	{
		int i = 0;
		while (i < line.Length && char.IsWhiteSpace(line[i]))
		{
			i++;
		}

		return i;
	}

	private void RebuildAndAssignText(List<string> lines)
	{
		string text = "";
		foreach (string line in lines)
		{
			text += line + "\n";
		}
		Text = text[^1] == '\n' ? text.Remove(text.Length - 1) : text;
	}

#endregion

#region MOVE CARET

	private void MoveCaretIfInBulletPointArea()
	{
		if (DontMoveCaret)
		{
			DontMoveCaret = false;
			return;
		}
		//this.PrintDebug($"MoveCaretOutOfBulletPointArea called");
		int caretLineIndex = GetCaretLine();
		int endOfBulletPointAreaIndex = GetEndOfBulletPointAreaIndex(caretLineIndex);
		if (GetCaretColumn() < endOfBulletPointAreaIndex)
		{
			MoveCaret(caretLineIndex, endOfBulletPointAreaIndex);
		}
	}
	
	private int GetEndOfBulletPointAreaIndex(int line)
	{
		int caretLineIndex = GetCaretLine();
		string caretLine = GetLine(caretLineIndex);
		int firstNonWhiteCharIndex = GetFirstNonWhiteSpaceColumn(caretLineIndex);
		char? firstNonWhiteChar = firstNonWhiteCharIndex < caretLine.Length ? caretLine[firstNonWhiteCharIndex] : null;
		if (firstNonWhiteChar == BulletPoint)
		{
			int endOfBulletPointAreaIndex = firstNonWhiteCharIndex + BulletPointWithSpace.Length;
			return endOfBulletPointAreaIndex;
		}
		else
		{
			return -1;
		}
	}

	private void MoveCaret(int caretLineIndex, int caretFriendlyTextStartIndex)
	{
		bool leftPressed = Input.IsActionPressed("ui_left") ||
			Input.IsActionPressed("ui_text_caret_left") ||
			Input.IsActionPressed("ui_text_caret_word_left");// ||
			//Input.IsActionPressed("ui_text_caret_word_left.macos");
		bool caretNotOnTopLine = caretLineIndex > 0;
		if (leftPressed && caretNotOnTopLine)
		{
			int aboveLineLength = GetLine(caretLineIndex - 1).Length;
			SetCaretLine(caretLineIndex - 1);
			SetCaretColumn(aboveLineLength);
		}
		else
		{
			SetCaretColumn(caretFriendlyTextStartIndex);
		}
	}

	private void CheckCaretPosition()
	{
		int caretLineIndex = GetCaretLine();
		CaretDirectlyAfterBulletPointString = GetCaretColumn() == GetEndOfBulletPointAreaIndex(caretLineIndex);
	}
	
#endregion

#region INPUT

	// TODO: Some keyboard ghosting(?) issues when typing fast with 3 or more keys held down together. 
	// Can't find the exact cause and it's not too bad, just need to push undo an extra time once in a while. 
	public override void _GuiInput(InputEvent @event)
	{
		//base._Input(@event);
		if (@event is InputEventKey keyEvent)
		{
			if (keyEvent.IsReleased())
			{
				//this.PrintDebug("\"Don't handle\" event called");
				if (ComplexOperationStarted)
				{
					EndComplexOperation();
					ComplexOperationStarted = false;
					//this.PrintDebug("EndComplexOperation");
				}
			}
			else if (keyEvent.Pressed)
			{
				//this.PrintDebug($"{keyEvent.AsTextKeyLabel()} will affect text: {keyEvent.WillAffectText()}");
				if (keyEvent.IsActionPressed("ui_undo") || keyEvent.IsActionPressed("ui_redo"))
				{
					DontSetBulletPoints = true;
					DontMoveCaret = true;
				}
				// Only BeginComplexOperation if Changed signals are going to get called.
				else if (keyEvent.WillAffectText() && !ComplexOperationStarted)
				{
					BeginComplexOperation();
					ComplexOperationStarted = true;
					//this.PrintDebug("BeginComplexOperation");
				}

				HandleInput(keyEvent);
			}
		}
	}

	// TODO: Handle deleting while caret at end of line similar to backspace (letter and word/all).
	// Only matters if deleting last letter/word/all.
	private void HandleInput(InputEventKey keyEvent)
	{
		//this.PrintDebug($"{keyEvent.AsTextKeyLabel()} HandleInput");
		if (CaretDirectlyAfterBulletPointString)
		{
			// Newline - Delete BulletPointString. Don't set as handled so the event still goes through. 
			if (keyEvent.IsActionPressed("ui_text_newline") ||
				keyEvent.IsActionPressed("ui_text_newline_blank") ||
				keyEvent.IsActionPressed("ui_text_newline_above"))
			{
				HandleNewlines();
			}
			// Backspace - Delete preceding whitespace chars if there are any, otherwise delete BulletPointString
			// and preceding newline so your line is at the end of the old one above you.
			else if (keyEvent.IsActionPressed("ui_text_backspace"))
			{
				HandleBackspace();
			}
			// Backspace word or all - Handle separately. Have them just delete all preceding whitespace and BulletPointString.
			else if (keyEvent.IsActionPressed("ui_text_backspace_word") ||
				//keyEvent.IsActionPressed("ui_text_backspace_word.macos") ||
				keyEvent.IsActionPressed("ui_text_backspace_all_to_left"))// ||
				//keyEvent.IsActionPressed("ui_text_backspace_all_to_left.macos"))
			{
				HandleBackspaceWordOrAll();
			}
			// TODO: Delete (char, word, and all) - Delete char/word/all.
			// Then check if there are no non whitespace chars left on line. If so, delete bullet point too.
			// TODO: Just add better checks in SetBulletPoints maybe? So it deletes bullet points if there's nothing following it on that line?
/* 			else if (keyEvent.IsActionPressed("ui_delete") ||
				keyEvent.IsActionPressed("ui_delete_word") ||
				keyEvent.IsActionPressed("ui_delete_all_to_right"))
			{
				HandleDelete();
			} */
			// Space and Tab - Put the space/tab before the bullet point instead of at the caret position. 
			else if (keyEvent.Keycode == Key.Space || keyEvent.IsActionPressed("ui_text_indent"))//Keycode == Key.Tab)
			{
				HandleSpaceOrTab(keyEvent);
			}
		}
	}

	private void HandleNewlines()
	{
		for (int i = 0; i < BulletPointWithSpace.Length; i++)
		{
			Backspace();
		}
	}

	private void HandleBackspace()
	{
		int caretColumnIndex = GetCaretColumn();
		char charBeforeBulletPoint = caretColumnIndex > 2 ? GetLine(GetCaretLine())[GetCaretColumn() - 3] : '\n';
		if (charBeforeBulletPoint == '\n')
		{
			for (int i = 0; i < BulletPointWithSpace.Length; i++)
			{
				Backspace();
			}
			//GetViewport().SetInputAsHandled();
		}
		else if (char.IsWhiteSpace(charBeforeBulletPoint))
		{
			SetCaretColumn(GetCaretColumn() - BulletPointWithSpace.Length);
			Backspace();
			SetCaretColumn(GetCaretColumn() + BulletPointWithSpace.Length);
			// TODO: Are any of these necessary? Should I use AcceptEvent instead? Or just nothing?
			//GetViewport().SetInputAsHandled();
		}
	}

	private void HandleBackspaceWordOrAll()
	{
		char charBeforeBulletPoint = GetCaretColumn() > 2 ? GetLine(GetCaretLine())[GetCaretColumn() - 3] : '\n';
		while (charBeforeBulletPoint != '\n')
		{
			SetCaretColumn(GetCaretColumn() - BulletPointWithSpace.Length);
			Backspace();
			SetCaretColumn(GetCaretColumn() + BulletPointWithSpace.Length);
			charBeforeBulletPoint = GetCaretColumn() > 2 ? GetLine(GetCaretLine())[GetCaretColumn() - 3] : '\n';
		}
		for (int i = 0; i < BulletPointWithSpace.Length; i++)
		{
			Backspace();
		}
		//GetViewport().SetInputAsHandled();
	}
	
	private void HandleDelete()
	{
		// TODO: If only one char/word/all after caret, delete char/word/all and bullet point. 
		// Should take care of everything? Delete char/word/all, then check if anything is left.
		// If not, delete bullet point.
		
	}

	private void HandleSpaceOrTab(InputEventKey keyEvent)
	{
		SetCaretColumn(GetCaretColumn() - BulletPointWithSpace.Length);
		InsertTextAtCaret(keyEvent.Keycode == Key.Space ? " " : "\t");
		SetCaretColumn(GetCaretColumn() + BulletPointWithSpace.Length);
		// TODO: AcceptEvent vs SetInputAsHandled? Both needed? Should I switch all to AcceptEvent?
		AcceptEvent();
		//GetViewport().SetInputAsHandled();
	}

#endregion

}
