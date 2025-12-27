using System;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// 输入动作数据容器
	/// <summary>
	[Serializable]
	public class MainActionMapDataContainer
	{
		public Dictionary<E_MainActionMap, KeyPathMap> actionMap = new Dictionary<E_MainActionMap, KeyPathMap>();

		public MainActionMapDataContainer()
		{
			InputSystem.InitContainer<MainActionMapData>(this);
		}
	}
}
