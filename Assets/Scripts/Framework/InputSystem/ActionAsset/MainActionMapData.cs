using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Framework
{
	/// <summary>
	/// MainActionMapData输入动作数据
	/// <summary>
	public class MainActionMapData
	{
		[ActionKeyMapAttribute(Key.W)]
		public static string Up => "<Keyboard>/w";

		[ActionKeyMapAttribute(Key.S)]
		public static string Down => "<Keyboard>/s";

		[ActionKeyMapAttribute(Key.A)]
		public static string Left => "<Keyboard>/a";

		[ActionKeyMapAttribute(Key.D)]
		public static string Right => "<Keyboard>/d";

		[ActionKeyMapAttribute(MouseButton.Left)]
		public static string NormalAttack => "<Mouse>/leftButton";

		[ActionKeyMapAttribute(Key.F)]
		public static string Initeract => "<Keyboard>/f";

		[ActionKeyMapAttribute(E_MouseValue.Delta)]
		public static string MouseMove => "<Mouse>/delta";

		[ActionKeyMapAttribute(E_MouseValue.Scroll)]
		public static string ScrollZoom => "<Mouse>/scroll";

		[ActionKeyMapAttribute(Key.LeftAlt)]
		public static string MouseVisible => "<Keyboard>/leftAlt";

	}
}
