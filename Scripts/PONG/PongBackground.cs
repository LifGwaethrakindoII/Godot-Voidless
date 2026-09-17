using Godot;
using System;

[Tool]
public partial class PongBackground : ColorRect
{
	Viewport _viewport;
	
	public Viewport viewport
	{
		get
		{
			if(_viewport == null) _viewport = GetViewport();
			return _viewport;
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateBackgroundSize();
		viewport.SizeChanged += UpdateBackgroundSize;
	}
	
	public override void _ExitTree()
	{
		if(viewport == null) return;
		
		viewport.SizeChanged -= UpdateBackgroundSize;
	}
	
	private void UpdateBackgroundSize()
	{
		if(viewport == null) return;
		
		SetSize(viewport.GetVisibleRect().Size);
	}
}
