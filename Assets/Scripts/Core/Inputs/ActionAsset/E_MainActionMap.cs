namespace Core.Inputs.ActionAsset
{
	public enum E_MainActionMap
	{
		// 预定义类型
		None,
		// 生成类型
		[ActionMapReplaceKey("<Up>")]
		Up,
		[ActionMapReplaceKey("<Down>")]
		Down,
		[ActionMapReplaceKey("<Left>")]
		Left,
		[ActionMapReplaceKey("<Right>")]
		Right,
		[ActionMapReplaceKey("<NormalAttack>")]
		NormalAttack,
		[ActionMapReplaceKey("<Interact>")]
		Interact,
		[ActionMapReplaceKey("<MouseMove>")]
		MouseMove,
		[ActionMapReplaceKey("<ScrollZoom>")]
		ScrollZoom,
		[ActionMapReplaceKey("<MouseVisible>")]
		MouseVisible,
	}
}
