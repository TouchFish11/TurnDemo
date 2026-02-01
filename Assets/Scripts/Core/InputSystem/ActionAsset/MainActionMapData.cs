using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Core.InputSystem.ActionAsset
{
	/// <summary>
	/// MainActionMapData输入动作数据
	/// <summary>
	public class MainActionMapData
	{
		[ActionKeyMap(Key.W)]
		public static string Up => "<Keyboard>/w";

		[ActionKeyMap(Key.S)]
		public static string Down => "<Keyboard>/s";

		[ActionKeyMap(Key.A)]
		public static string Left => "<Keyboard>/a";

		[ActionKeyMap(Key.D)]
		public static string Right => "<Keyboard>/d";

		[ActionKeyMap(MouseButton.Left)]
		public static string NormalAttack => "<Mouse>/leftButton";

		[ActionKeyMap(Key.F)]
		public static string Interact => "<Keyboard>/f";

		[ActionKeyMap(E_MouseValue.Delta)]
		public static string MouseMove => "<Mouse>/delta";

		[ActionKeyMap(E_MouseValue.Scroll)]
		public static string ScrollZoom => "<Mouse>/scroll";

		[ActionKeyMap(Key.LeftAlt)]
		public static string MouseVisible => "<Keyboard>/leftAlt";

	}
}
