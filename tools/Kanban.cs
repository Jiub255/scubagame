using Godot;
using System;

public partial class Kanban : CanvasLayer
{
	private KanbanColumn[] KanbanColumns { get; } = new KanbanColumn[0];
}
