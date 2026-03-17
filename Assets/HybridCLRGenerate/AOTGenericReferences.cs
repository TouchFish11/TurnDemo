using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"CoreModule.dll",
		"System.Core.dll",
		"Unity.InputSystem.dll",
		"Unity.VisualScripting.Core.dll",
		"UnityEngine.AssetBundleModule.dll",
		"UnityEngine.CoreModule.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Core.Collection.Collection.<GetEnumerator>d__16<int,object>
	// Core.Collection.Collection.<GetEnumerator>d__16<object,object>
	// Core.Collection.Collection<int,object>
	// Core.Collection.Collection<object,object>
	// Core.Collection.Generic.UniList<object>
	// Core.Tasks.AssetBundleRequestTask.<>c__DisplayClass9_0<object>
	// Core.Tasks.AssetBundleRequestTask<object>
	// Core.Tasks.Awaiter.AssetBundleRequestAwaiter<object>
	// Core.UI.MVC.UIController.<Destroy>d__16<object,object>
	// Core.UI.MVC.UIController.<Hide>d__8<object,object>
	// Core.UI.MVC.UIController.<Init>d__6<object,object>
	// Core.UI.MVC.UIController<object,object>
	// Core.Utility.TaskUtility.<WaitForTask>d__2<object>
	// System.Action<HotUpdate.Battle.Relic.RelicEffect>
	// System.Action<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Action<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Action<UnityEngine.InputSystem.InputAction.CallbackContext>
	// System.Action<UnityEngine.Vector2>
	// System.Action<UnityEngine.Vector3>
	// System.Action<byte>
	// System.Action<float>
	// System.Action<int,int>
	// System.Action<int>
	// System.Action<object,object>
	// System.Action<object>
	// System.Action<ulong,ulong>
	// System.Action<ulong>
	// System.Collections.Generic.ArraySortHelper<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.Generic.ArraySortHelper<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<Core.Types.TypeIdentifier>
	// System.Collections.Generic.Comparer<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.Generic.Comparer<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.Generic.Comparer<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<Core.Types.TypeIdentifier,object>
	// System.Collections.Generic.Dictionary.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<Core.Types.TypeIdentifier,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.KeyCollection<Core.Types.TypeIdentifier,object>
	// System.Collections.Generic.Dictionary.KeyCollection<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<Core.Types.TypeIdentifier,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.ValueCollection<Core.Types.TypeIdentifier,object>
	// System.Collections.Generic.Dictionary.ValueCollection<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<uint,object>
	// System.Collections.Generic.Dictionary<Core.Types.TypeIdentifier,object>
	// System.Collections.Generic.Dictionary<byte,object>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<uint,object>
	// System.Collections.Generic.EqualityComparer<Core.Types.TypeIdentifier>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<uint>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.Generic.ICollection<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IComparer<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.Generic.IComparer<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<int,object>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IEnumerable<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IEnumerator<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<uint,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<Core.Types.TypeIdentifier>
	// System.Collections.Generic.IEqualityComparer<byte>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<uint>
	// System.Collections.Generic.IList<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IList<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.Generic.IList<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IReadOnlyDictionary<int,object>
	// System.Collections.Generic.IReadOnlyDictionary<object,object>
	// System.Collections.Generic.KeyValuePair<Core.Types.TypeIdentifier,object>
	// System.Collections.Generic.KeyValuePair<byte,object>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<uint,object>
	// System.Collections.Generic.List.Enumerator<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.Generic.List.Enumerator<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.Generic.List<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.Generic.List<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<Core.Types.TypeIdentifier>
	// System.Collections.Generic.ObjectComparer<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.Generic.ObjectComparer<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<Core.Types.TypeIdentifier>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<uint>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotUpdate.Battle.Relic.RelicEffect>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<HotUpdate.Battle.Relic.RelicEffect>
	// System.Comparison<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Comparison<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Func<System.Threading.Tasks.VoidTaskResult>
	// System.Func<byte>
	// System.Func<object,System.Threading.Tasks.VoidTaskResult>
	// System.Func<object,byte>
	// System.Func<object,int>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.Linq.GroupedEnumerable<object,int,object>
	// System.Linq.IGrouping<int,object>
	// System.Linq.IdentityFunction.<>c<object>
	// System.Linq.IdentityFunction<object>
	// System.Linq.Lookup.<GetEnumerator>d__12<int,object>
	// System.Linq.Lookup.Grouping.<GetEnumerator>d__7<int,object>
	// System.Linq.Lookup.Grouping<int,object>
	// System.Linq.Lookup<int,object>
	// System.Predicate<HotUpdate.Battle.Relic.RelicEffect>
	// System.Predicate<HotUpdate.Core.Battle.Point.PointInfo>
	// System.Predicate<System.ValueTuple<Core.Types.TypeIdentifier,object>>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.TaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Task<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory<object>
	// System.ValueTuple<Core.Types.TypeIdentifier,object>
	// System.ValueTuple<object,object>
	// UnityEngine.Events.UnityAction<int,int>
	// UnityEngine.Events.UnityAction<object,byte>
	// UnityEngine.Events.UnityAction<object,float>
	// UnityEngine.Events.UnityAction<object,object>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.InputSystem.InputBindingComposite<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputBindingComposite<float>
	// UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputControl<float>
	// UnityEngine.InputSystem.InputProcessor<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputProcessor<float>
	// UnityEngine.InputSystem.Utilities.InlinedArray<object>
	// }}

	public void RefMethods()
	{
		// System.Void Core.Collection.ListUtility.CollectUniList<object>(Core.Collection.Generic.UniList<object>)
		// Core.Collection.Generic.UniList<object> Core.Collection.ListUtility.GetUniList<object>()
		// object Core.Components.IEntityObject.AddComponent<object>()
		// object Core.Components.IEntityObject.GetComponent<object>()
		// object Core.Components.IEntityObject.GetComponentInChildren<object>()
		// object[] Core.Extensions.DictionaryExtensions.ToArray<object,object,object>(System.Collections.Generic.Dictionary.ValueCollection<object,object>,System.Func<object,object>)
		// System.Void Core.GlobalEvent.IEventCenter.SubscribeEvent<object>(System.Action<object>,System.Func<object,bool>)
		// System.Void Core.GlobalEvent.IEventCenter.TriggerEvent<object>(object)
		// System.Void Core.GlobalEvent.IEventCenter.UnsubscribeEvent<object>(System.Action<object>)
		// System.Void Core.Reflection.FactoryUtility.ScanAllType<object,int,object>(System.Collections.Generic.IDictionary<int,object>,System.Func<System.Type,int>,System.Func<System.Type,object>,bool,bool,System.Reflection.Assembly[])
		// System.Void Core.Reflection.FactoryUtility.ScanAllType<object,object,object>(System.Collections.Generic.IDictionary<object,object>,System.Func<System.Type,object>,System.Func<System.Type,object>,bool,bool,System.Reflection.Assembly[])
		// object Core.Reflection.IFactoryManager.GetFactory<object,object>()
		// System.Threading.Tasks.Task<object> Core.Serialize.Binary.IBinaryDataManager.LoadAsync<object>(string)
		// System.Threading.Tasks.Task Core.Serialize.Binary.IConfigLoader.LoadConfigAsync<object,object>()
		// object Core.Serialize.Json.IJsonManager.FromJson<object>(string,Core.Serialize.Json.E_JsonType,Newtonsoft.Json.JsonSerializerSettings)
		// System.Threading.Tasks.Task<object> Core.Serialize.Json.IJsonManager.FromJsonAsync<object>(string,Core.Serialize.Json.E_JsonType,Newtonsoft.Json.JsonSerializerSettings)
		// Core.Tasks.AssetBundleRequestTask<object> Core.Tasks.Extensions.TaskAwaiterExtensions.ToTask<object>(UnityEngine.AssetBundleRequest,System.Threading.CancellationToken)
		// Core.Tasks.AssetBundleRequestTask<object> Core.Tasks.TaskFactory.Create<object>(UnityEngine.AssetBundleRequest,System.Threading.CancellationToken)
		// System.Threading.Tasks.Task<object> Core.UI.IUIManager.CreateViewAsync<object,object,object>(string,Core.UI.E_UILayer,string,UnityEngine.Vector2,UnityEngine.Quaternion)
		// object Core.UI.IUIManager.GetController<object>()
		// object Core.UI.UIComponentBinder.GetControl<object>(string)
		// System.Collections.IEnumerator Core.Utility.TaskUtility.WaitForTask<object>(System.Threading.Tasks.Task<object>,System.Action<object>)
		// object System.Activator.CreateInstance<object>()
		// float[] System.Array.Empty<float>()
		// int[] System.Array.Empty<int>()
		// object[] System.Array.Empty<object>()
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<int,object>(System.Collections.Generic.IReadOnlyDictionary<int,object>,int)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<int,object>(System.Collections.Generic.IReadOnlyDictionary<int,object>,int,object)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<object,object>(System.Collections.Generic.IReadOnlyDictionary<object,object>,object)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<object,object>(System.Collections.Generic.IReadOnlyDictionary<object,object>,object,object)
		// bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<System.Linq.IGrouping<int,object>> System.Linq.Enumerable.GroupBy<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<object,int,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>,System.Func<object,int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<object,int,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>,System.Func<object,int>,System.Collections.Generic.IEqualityComparer<int>)
		// object System.Reflection.CustomAttributeExtensions.GetCustomAttribute<object>(System.Reflection.MemberInfo)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<Core.Tasks.Awaiter.AssetBundleRequestAwaiter<object>,object>(Core.Tasks.Awaiter.AssetBundleRequestAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,object>(System.Runtime.CompilerServices.TaskAwaiter&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<Core.Tasks.Awaiter.AssetBundleRequestAwaiter<object>,object>(Core.Tasks.Awaiter.AssetBundleRequestAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,object>(System.Runtime.CompilerServices.TaskAwaiter&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<object>(object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<object>(object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Core.Tasks.Awaiter.AssetBundleRequestAwaiter<object>,object>(Core.Tasks.Awaiter.AssetBundleRequestAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,object>(System.Runtime.CompilerServices.TaskAwaiter&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<object>(object&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<UnityEngine.Vector2>(UnityEngine.Vector2&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<float>(float&)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<UnityEngine.Vector2>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<float>()
		// object Unity.VisualScripting.AttributeUtility.GetAttribute<object>(System.Reflection.MemberInfo,bool)
		// object Unity.VisualScripting.AttributeUtility.AttributeCache.GetAttribute<object>(bool)
		// UnityEngine.AssetBundleRequest UnityEngine.AssetBundle.LoadAssetAsync<object>(string)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.Component.GetComponentInParent<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.Component.TryGetComponent<object>(object&)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.GameObject.TryGetComponent<object>(object&)
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputAction.CallbackContext.ReadValue<UnityEngine.Vector2>()
		// float UnityEngine.InputSystem.InputAction.CallbackContext.ReadValue<float>()
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ApplyProcessors<UnityEngine.Vector2>(int,UnityEngine.Vector2,UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>)
		// float UnityEngine.InputSystem.InputActionState.ApplyProcessors<float>(int,float,UnityEngine.InputSystem.InputControl<float>)
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ReadValue<UnityEngine.Vector2>(int,int,bool)
		// float UnityEngine.InputSystem.InputActionState.ReadValue<float>(int,int,bool)
		// object UnityEngine.Object.FindFirstObjectByType<object>()
	}
}