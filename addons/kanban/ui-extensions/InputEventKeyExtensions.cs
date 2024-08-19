using Godot;

public static class InputEventKeyExtensions
{
	/// <summary>
	/// Checks against all builtin ui actions and @GlobalScope Keys that affect text.
	/// </summary>
	/// <param name="keyEvent"></param>
	/// <returns>true if keyEvent will affect text in any way, except for undo/redo.</returns>
	public static bool WillAffectText(this InputEventKey keyEvent)
	{
		// Test for key combos that affect text.
		if (keyEvent.IsActionPressed("ui_cut") ||
			keyEvent.IsActionPressed("ui_paste") ||
			//keyEvent.IsActionPressed("ui_undo") ||
			//keyEvent.IsActionPressed("ui_redo") ||
			keyEvent.IsActionPressed("ui_text_newline_blank") ||
			keyEvent.IsActionPressed("ui_text_newline_above") ||
			keyEvent.IsActionPressed("ui_text_indent") ||
			keyEvent.IsActionPressed("ui_text_dedent") ||
			keyEvent.IsActionPressed("ui_text_backspace_word") ||
			//keyEvent.IsActionPressed("ui_text_backspace_word.macos") ||
			keyEvent.IsActionPressed("ui_text_backspace_all_to_left") ||
			//keyEvent.IsActionPressed("ui_text_backspace_all_to_left.macos") ||
			keyEvent.IsActionPressed("ui_text_delete_word") ||
			//keyEvent.IsActionPressed("ui_text_delete_word.macos") ||
			keyEvent.IsActionPressed("ui_text_delete_all_to_right"))// ||
			//keyEvent.IsActionPressed("ui_text_delete_all_to_right.macos"))
		{
			return true;
		}
		// All the Godot @GlobalScope Keys that affect text.
		if (keyEvent.Keycode >= Key.Space && keyEvent.Keycode <= Key.Section)
		{
			return true;
		}
		if (keyEvent.Keycode >= Key.KpMultiply && keyEvent.Keycode <= Key.Kp9)
		{
			return true;
		}
		if (keyEvent.Keycode == Key.Tab ||
			keyEvent.Keycode == Key.Backtab ||
			keyEvent.Keycode == Key.Backspace ||
			keyEvent.Keycode == Key.Clear ||
			keyEvent.Keycode == Key.Delete ||
			keyEvent.Keycode == Key.Enter ||
			keyEvent.Keycode == Key.KpEnter)
		{
			return true;
		}
		return false;
	}
}
