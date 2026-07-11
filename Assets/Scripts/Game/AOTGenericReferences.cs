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
	// Core.AssetBundles.Management.GameAsset.<>c__DisplayClass5_0<object>
	// Core.AssetBundles.Management.GameAsset.<>c__DisplayClass6_0<object>
	// Core.AssetBundles.Management.GameAsset.<LoadAssetAsync>d__6<object>
	// Core.AssetBundles.Management.ObjectSpawner.<SpawnAsync>d__6<object>
	// Core.Collection.Collection.<GetEnumerator>d__17<int,object>
	// Core.Collection.Collection<int,object>
	// Core.Tasks.TaskUtility.<WaitForTask>d__2<object>
	// Core.UI.ReactiveProperty.Subscription<byte>
	// Core.UI.ReactiveProperty.Subscription<float>
	// Core.UI.ReactiveProperty.Subscription<int>
	// Core.UI.ReactiveProperty.Subscription<object>
	// Core.UI.ReactiveProperty<byte>
	// Core.UI.ReactiveProperty<float>
	// Core.UI.ReactiveProperty<int>
	// Core.UI.ReactiveProperty<object>
	// Core.UI.ViewController.UIController.<Activate>d__12<object>
	// Core.UI.ViewController.UIController.<Dispose>d__30<object>
	// Core.UI.ViewController.UIController.<InActivate>d__13<object>
	// Core.UI.ViewController.UIController.<Init>d__11<object>
	// Core.UI.ViewController.UIController<object>
	// System.Action<Core.AssetBundles.Management.AssetHandle>
	// System.Action<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Action<HotUpdate.Game.Battle.Skill.Base.HitResult>
	// System.Action<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Action<HotUpdate.Game.Point.PointInfo>
	// System.Action<System.ValueTuple<object,object>>
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
	// System.Collections.Generic.ArraySortHelper<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.ArraySortHelper<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.ArraySortHelper<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.ArraySortHelper<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<object,object>>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Comparer<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.Comparer<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.Comparer<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.Comparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<byte,int>
	// System.Collections.Generic.Dictionary.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary.Enumerator<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<byte,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<byte,int>
	// System.Collections.Generic.Dictionary.KeyCollection<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<byte,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<byte,int>
	// System.Collections.Generic.Dictionary.ValueCollection<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<byte,int>
	// System.Collections.Generic.Dictionary<byte,object>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.EqualityComparer<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.ICollection<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.ICollection<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.ICollection<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<byte,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle<object>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<object,object>>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.IComparer<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IComparer<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.IComparer<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.IComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.IEnumerable<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IEnumerable<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.IEnumerable<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<byte,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle<object>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.IEnumerator<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IEnumerator<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.IEnumerator<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<byte,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle<object>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<byte>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.IList<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IList<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.IList<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.IList<System.ValueTuple<object,object>>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IReadOnlyDictionary<object,object>
	// System.Collections.Generic.KeyValuePair<byte,int>
	// System.Collections.Generic.KeyValuePair<byte,object>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.List.Enumerator<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.List.Enumerator<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.List.Enumerator<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.List<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.List<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.List<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.List<System.ValueTuple<object,object>>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.LowLevelList<object>
	// System.Collections.Generic.LowLevelListWithIList.Enumerator<object>
	// System.Collections.Generic.LowLevelListWithIList<object>
	// System.Collections.Generic.ObjectComparer<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.ObjectComparer<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.ObjectComparer<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.ObjectComparer<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.ObjectEqualityComparer<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<int>
	// System.Collections.Generic.Queue<int>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotUpdate.Game.Point.PointInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<Core.AssetBundles.Management.AssetHandle>
	// System.Comparison<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Comparison<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Comparison<HotUpdate.Game.Point.PointInfo>
	// System.Comparison<System.ValueTuple<object,object>>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Converter<Core.AssetBundles.Management.AssetHandle,object>
	// System.Converter<int,object>
	// System.Converter<object,object>
	// System.Func<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Func<HotUpdate.Game.Battle.Core.BattleResult,object>
	// System.Func<System.Threading.Tasks.VoidTaskResult>
	// System.Func<System.ValueTuple<object,object>>
	// System.Func<byte>
	// System.Func<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Func<object,System.Threading.Tasks.VoidTaskResult>
	// System.Func<object,System.ValueTuple<object,object>>
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
	// System.Predicate<Core.AssetBundles.Management.AssetHandle>
	// System.Predicate<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Predicate<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Predicate<HotUpdate.Game.Point.PointInfo>
	// System.Predicate<System.ValueTuple<object,object>>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<object,object>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<object,object>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<object,object>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<object,object>>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<object,object>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.Task.WhenAllPromise<object>
	// System.Threading.Tasks.Task<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Threading.Tasks.Task<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.Task<System.ValueTuple<object,object>>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<object,object>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Threading.Tasks.TaskFactory<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<object,object>>
	// System.Threading.Tasks.TaskFactory<object>
	// System.ValueTuple<object,object>
	// UnityEngine.Events.UnityAction<int,int>
	// UnityEngine.Events.UnityAction<object,UnityEngine.Vector2>
	// UnityEngine.Events.UnityAction<object,byte>
	// UnityEngine.Events.UnityAction<object,float>
	// UnityEngine.Events.UnityAction<object,int>
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
		// Core.AssetBundles.Management.AssetWrapper Core.AssetBundles.Management.AssetManager.LoadAsset<object>(string)
		// Core.AssetBundles.Management.AssetWrapper Core.AssetBundles.Management.BundleWrapper.LoadAsset<object>(string,string)
		// Core.AssetBundles.Management.AssetHandle<object> Core.AssetBundles.Management.GameAsset.LoadAsset<object>(string)
		// System.Threading.Tasks.Task<Core.AssetBundles.Management.AssetHandle<object>> Core.AssetBundles.Management.GameAsset.LoadAssetAsync<object>(string)
		// System.Threading.Tasks.Task<object> Core.AssetBundles.Management.ObjectSpawner.SpawnAsync<object>(string,UnityEngine.Transform,UnityEngine.Vector3,UnityEngine.Quaternion,bool)
		// System.Void Core.DI.DIContainer.BindSingleton<object,object>()
		// System.Void Core.DI.DIContainer.BindType<object,object>()
		// System.Void Core.GlobalEvent.IEventCenter.SubscribeEvent<object>(System.Action<object>,System.Func<object,bool>)
		// System.Void Core.GlobalEvent.IEventCenter.TriggerEvent<object>(object)
		// System.Void Core.GlobalEvent.IEventCenter.UnsubscribeEvent<object>(System.Action<object>)
		// System.Threading.Tasks.Task<object> Core.Serialize.Binary.IBinaryDataManager.LoadAsync<object>(string)
		// System.Threading.Tasks.Task Core.Serialize.Binary.IConfigLoader.LoadConfigAsync<object,object>()
		// object Core.Serialize.Json.IJsonManager.FromJson<object>(string,Core.Serialize.Json.E_JsonType,Newtonsoft.Json.JsonSerializerSettings)
		// System.Threading.Tasks.Task<object> Core.Serialize.Json.IJsonManager.FromJsonAsync<object>(string,Core.Serialize.Json.E_JsonType,Newtonsoft.Json.JsonSerializerSettings)
		// System.Collections.IEnumerator Core.Tasks.TaskUtility.WaitForTask<object>(System.Threading.Tasks.Task<object>,System.Action<object>)
		// System.Threading.Tasks.Task<object> Core.UI.IUIManager.CreateViewAsync<object,object>(string,Core.UI.E_UILayer,UnityEngine.Vector2,UnityEngine.Quaternion)
		// object Core.UI.IUIManager.GetController<object>()
		// object Core.UI.UIComponentBinder.GetControl<object>(string)
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<object,object>(System.Collections.Generic.IReadOnlyDictionary<object,object>,object)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<object,object>(System.Collections.Generic.IReadOnlyDictionary<object,object>,object,object)
		// System.Collections.Generic.List<object> System.Collections.Generic.List<int>.ConvertAll<object>(System.Converter<int,object>)
		// System.Collections.Generic.List<object> System.Collections.Generic.List<object>.ConvertAll<object>(System.Converter<object,object>)
		// bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<System.Linq.IGrouping<int,object>> System.Linq.Enumerable.GroupBy<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<object,int,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>,System.Func<object,int>)
		// System.Collections.Generic.Dictionary<int,int> System.Linq.Enumerable.ToDictionary<object,int,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>,System.Func<object,int>,System.Collections.Generic.IEqualityComparer<int>)
		// object System.Reflection.CustomAttributeExtensions.GetCustomAttribute<object>(System.Reflection.MemberInfo)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,object>(System.Runtime.CompilerServices.TaskAwaiter&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>,object>(System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<object,object>>,object>(System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<object,object>>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,object>(System.Runtime.CompilerServices.TaskAwaiter&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>,object>(System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<object,object>>,object>(System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<object,object>>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<object,object>>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>,object>(System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>,object>(System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<object>(object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<Core.AssetBundles.Management.AssetHandle<object>>.Start<object>(object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<object,object>>.Start<object>(object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<object>(object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,object>(System.Runtime.CompilerServices.TaskAwaiter&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>,object>(System.Runtime.CompilerServices.TaskAwaiter<Core.AssetBundles.Management.AssetHandle<object>>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<object>(object&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// System.Text.StringBuilder System.Text.StringBuilder.AppendJoin<object>(System.Char,System.Collections.Generic.IEnumerable<object>)
		// System.Text.StringBuilder System.Text.StringBuilder.AppendJoinCore<object>(System.Char*,int,System.Collections.Generic.IEnumerable<object>)
		// System.Threading.Tasks.Task<object[]> System.Threading.Tasks.Task.InternalWhenAll<object>(System.Threading.Tasks.Task<object>[])
		// System.Threading.Tasks.Task<object[]> System.Threading.Tasks.Task.WhenAll<object>(System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<object>>)
		// System.Threading.Tasks.Task<object[]> System.Threading.Tasks.Task.WhenAll<object>(System.Threading.Tasks.Task<object>[])
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<UnityEngine.Vector2>(UnityEngine.Vector2&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<float>(float&)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<UnityEngine.Vector2>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<float>()
		// object Unity.VisualScripting.AttributeUtility.GetAttribute<object>(System.Reflection.MemberInfo,bool)
		// object Unity.VisualScripting.AttributeUtility.AttributeCache.GetAttribute<object>(bool)
		// object UnityEngine.AssetBundle.LoadAsset<object>(string)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>(bool)
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