namespace Framework
{
	public enum E_MainActionMap
	{
		// 预定义类型
		None,
		// 生成类型
		[ActionMapReplaceKeyAttribute("<Move>")]
		Move,
		[ActionMapReplaceKeyAttribute("<NormalAttack>")]
		NormalAttack,
		[ActionMapReplaceKeyAttribute("<Initeract>")]
		Initeract,
		[ActionMapReplaceKeyAttribute("<MouseMove>")]
		MouseMove,
		[ActionMapReplaceKeyAttribute("<ScrollZoom>")]
		ScrollZoom,
		[ActionMapReplaceKeyAttribute("<MouseVisible>")]
		MouseVisible,
	}
}
