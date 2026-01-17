namespace Framework
{
	public enum E_MainActionMap
	{
		// 预定义类型
		None,
		// 生成类型
		[ActionMapReplaceKeyAttribute("<Up>")]
		Up,
		[ActionMapReplaceKeyAttribute("<Down>")]
		Down,
		[ActionMapReplaceKeyAttribute("<Left>")]
		Left,
		[ActionMapReplaceKeyAttribute("<Right>")]
		Right,
		[ActionMapReplaceKeyAttribute("<NormalAttack>")]
		NormalAttack,
		[ActionMapReplaceKeyAttribute("<Interact>")]
		Interact,
		[ActionMapReplaceKeyAttribute("<MouseMove>")]
		MouseMove,
		[ActionMapReplaceKeyAttribute("<ScrollZoom>")]
		ScrollZoom,
		[ActionMapReplaceKeyAttribute("<MouseVisible>")]
		MouseVisible,
	}
}
