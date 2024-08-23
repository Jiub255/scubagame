using Godot;
using System;

public static class KanbanExtensions
{
	/// <summary>
	/// Checks against all builtin ui actions and @GlobalScope Keys that affect text (except undo and redo).
	/// </summary>
	/// <returns>true if keyEvent will affect text in any way (except undo and redo).</returns>
	public static bool WillAffectText(this InputEventKey keyEvent)
	{
		// Test for key combos that affect text.
		if (keyEvent.IsActionPressed("ui_cut") ||
			keyEvent.IsActionPressed("ui_paste") ||
			keyEvent.IsActionPressed("ui_text_newline") ||
			keyEvent.IsActionPressed("ui_text_newline_blank") ||
			keyEvent.IsActionPressed("ui_text_newline_above") ||
			keyEvent.IsActionPressed("ui_text_indent") ||
			keyEvent.IsActionPressed("ui_text_dedent") ||
			keyEvent.IsActionPressed("ui_text_backspace") ||
			keyEvent.IsActionPressed("ui_text_backspace_word") ||
			keyEvent.IsActionPressed("ui_text_backspace_all_to_left") ||
			keyEvent.IsActionPressed("ui_text_delete") ||
			keyEvent.IsActionPressed("ui_text_delete_word") ||
			keyEvent.IsActionPressed("ui_text_delete_all_to_right"))// ||
		{
			return true;
		}
		// Mac-specific input actions
/* 		if (OS.GetName() == "MacOS" &&
			(keyEvent.IsActionPressed("ui_text_backspace_word.macos") ||
			keyEvent.IsActionPressed("ui_text_backspace_all_to_left.macos") ||
			keyEvent.IsActionPressed("ui_text_delete_word.macos") ||
			keyEvent.IsActionPressed("ui_text_delete_all_to_right.macos")))
		{
			return true;
		} */
		// All the Godot @GlobalScope Keys that affect text.
		if (keyEvent.Keycode >= Key.Space && keyEvent.Keycode <= Key.Section)
		{
			return true;
		}
		if (keyEvent.Keycode >= Key.KpMultiply && keyEvent.Keycode <= Key.Kp9)
		{
			return true;
		}
		// TODO: Redundant (ish) checks here. If godot editor uses the built-in actions, 
		// use those over key checks whenever possible (In case of remappings).
		if (//keyEvent.Keycode == Key.Tab ||
			//keyEvent.Keycode == Key.Backtab ||
			//keyEvent.Keycode == Key.Backspace ||
			keyEvent.Keycode == Key.Clear// ||
			//keyEvent.Keycode == Key.Delete ||
			//keyEvent.Keycode == Key.Enter ||
			//keyEvent.Keycode == Key.KpEnter
			)
		{
			return true;
		}
		return false;
	}
	
	public static string TruncateQuoteQuestion(this string text, int maxLength, string ellipses = "...")
	{
		return $"\"{text.Truncate(maxLength - 3, ellipses)}\"?";
	}
	
	public static string Truncate(this string text, int maxLength, string ellipses = "...")
	{
		return text.Length <= maxLength ? text : string.Concat(text.AsSpan(0, maxLength - ellipses.Length), ellipses);
	}
}
