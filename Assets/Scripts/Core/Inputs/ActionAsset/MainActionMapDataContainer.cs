using System;
using System.Collections.Generic;

namespace Core.Inputs.ActionAsset
{
	/// <summary>
	/// MainActionMapData���붯������数据容器类
	/// </summary>
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
