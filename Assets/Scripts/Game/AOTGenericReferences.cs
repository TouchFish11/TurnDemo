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
	// Core.AssetBundles.Management.GameAsset.<>c__DisplayClass10_0<object>
	// Core.AssetBundles.Management.GameAsset.<>c__DisplayClass9_0<object>
	// Core.AssetBundles.Management.GameAsset.<LoadAssetAsync>d__10<object>
	// Core.AssetBundles.Management.ObjectSpawner.<SpawnAsync>d__6<object>
	// Core.Collection.Collection.<GetEnumerator>d__17<int,object>
	// Core.Collection.Collection<int,object>
	// Core.Tasks.TaskUtility.<WaitForTask>d__3<object>
	// Core.UI.ILogicView<object,object>
	// Core.UI.IUILogic<object,object>
	// Core.UI.ReactiveProperty.Subscription<System.ValueTuple<float,float>>
	// Core.UI.ReactiveProperty.Subscription<byte>
	// Core.UI.ReactiveProperty.Subscription<float>
	// Core.UI.ReactiveProperty.Subscription<int>
	// Core.UI.ReactiveProperty.Subscription<object>
	// Core.UI.ReactiveProperty<System.ValueTuple<float,float>>
	// Core.UI.ReactiveProperty<byte>
	// Core.UI.ReactiveProperty<float>
	// Core.UI.ReactiveProperty<int>
	// Core.UI.ReactiveProperty<object>
	// Core.UI.ViewController.UIController.<Activate>d__11<object>
	// Core.UI.ViewController.UIController.<Dispose>d__29<object>
	// Core.UI.ViewController.UIController.<InActivate>d__12<object>
	// Core.UI.ViewController.UIController.<Init>d__10<object>
	// Core.UI.ViewController.UIController<object>
	// System.Action<Core.AssetBundles.Management.AssetHandle>
	// System.Action<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Action<HotUpdate.Game.Battle.Skill.Base.HitResult>
	// System.Action<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Action<HotUpdate.Game.Point.PointInfo>
	// System.Action<System.ValueTuple<float,float>>
	// System.Action<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Action<System.ValueTuple<object,object>>
	// System.Action<UnityEngine.InputSystem.InputAction.CallbackContext>
	// System.Action<UnityEngine.Vector2>
	// System.Action<UnityEngine.Vector3>
	// System.Action<byte>
	// System.Action<float>
	// System.Action<int,int>
	// System.Action<int>
	// System.Action<object,UnityEngine.Vector2>
	// System.Action<object,byte>
	// System.Action<object,float>
	// System.Action<object,int>
	// System.Action<object,object>
	// System.Action<object>
	// System.Action<ulong,ulong>
	// System.Action<ulong>
	// System.Collections.Generic.ArraySortHelper<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.ArraySortHelper<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.ArraySortHelper<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.ArraySortHelper<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<object,object>>
	// System.Collections.Generic.ArraySortHelper<byte>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Comparer<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.Comparer<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.Comparer<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.Comparer<System.Nullable<long>>
	// System.Collections.Generic.Comparer<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<float>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<System.Nullable<long>,int>
	// System.Collections.Generic.Dictionary.Enumerator<byte,float>
	// System.Collections.Generic.Dictionary.Enumerator<byte,int>
	// System.Collections.Generic.Dictionary.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary.Enumerator<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<System.Nullable<long>,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<byte,float>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<byte,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<System.Nullable<long>,int>
	// System.Collections.Generic.Dictionary.KeyCollection<byte,float>
	// System.Collections.Generic.Dictionary.KeyCollection<byte,int>
	// System.Collections.Generic.Dictionary.KeyCollection<byte,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary.KeyCollection<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<System.Nullable<long>,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<byte,float>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<byte,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<System.Nullable<long>,int>
	// System.Collections.Generic.Dictionary.ValueCollection<byte,float>
	// System.Collections.Generic.Dictionary.ValueCollection<byte,int>
	// System.Collections.Generic.Dictionary.ValueCollection<byte,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary.ValueCollection<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<System.Nullable<long>,int>
	// System.Collections.Generic.Dictionary<byte,float>
	// System.Collections.Generic.Dictionary<byte,int>
	// System.Collections.Generic.Dictionary<byte,object>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.Dictionary<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.EqualityComparer<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.EqualityComparer<System.Nullable<long>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<float,float>>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.ICollection<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.ICollection<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.ICollection<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.Nullable<long>,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<byte,float>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<byte,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle<object>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.Generic.ICollection<System.ValueTuple<object,object>>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.IComparer<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IComparer<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.IComparer<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.IComparer<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.Generic.IComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.IComparer<byte>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.IEnumerable<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IEnumerable<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.IEnumerable<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.Nullable<long>,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<byte,float>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<byte,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle<object>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.IEnumerator<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IEnumerator<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.IEnumerator<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<System.Nullable<long>,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<byte,float>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<byte,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<byte,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle<object>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<System.Nullable<long>>
	// System.Collections.Generic.IEqualityComparer<byte>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<long>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.IList<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.IList<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.IList<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.IList<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.Generic.IList<System.ValueTuple<object,object>>
	// System.Collections.Generic.IList<byte>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IReadOnlyDictionary<int,object>
	// System.Collections.Generic.IReadOnlyDictionary<object,object>
	// System.Collections.Generic.KeyValuePair<System.Nullable<long>,int>
	// System.Collections.Generic.KeyValuePair<byte,float>
	// System.Collections.Generic.KeyValuePair<byte,int>
	// System.Collections.Generic.KeyValuePair<byte,object>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.KeyValuePair<object,Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.List.Enumerator<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.List.Enumerator<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.List.Enumerator<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<object,object>>
	// System.Collections.Generic.List.Enumerator<byte>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.List<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.List<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.List<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.List<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.Generic.List<System.ValueTuple<object,object>>
	// System.Collections.Generic.List<byte>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.LowLevelList<object>
	// System.Collections.Generic.LowLevelListWithIList.Enumerator<object>
	// System.Collections.Generic.LowLevelListWithIList<object>
	// System.Collections.Generic.ObjectComparer<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.ObjectComparer<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.Generic.ObjectComparer<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.Generic.ObjectComparer<HotUpdate.Game.Point.PointInfo>
	// System.Collections.Generic.ObjectComparer<System.Nullable<long>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<object,object>>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<float>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Collections.Generic.ObjectEqualityComparer<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.Generic.ObjectEqualityComparer<System.Nullable<long>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<float,float>>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<int>
	// System.Collections.Generic.Queue<int>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<Core.AssetBundles.Management.AssetHandle>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Collections.ObjectModel.ReadOnlyCollection<HotUpdate.Game.Point.PointInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<object,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<byte>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<Core.AssetBundles.Management.AssetHandle>
	// System.Comparison<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Comparison<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Comparison<HotUpdate.Game.Point.PointInfo>
	// System.Comparison<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Comparison<System.ValueTuple<object,object>>
	// System.Comparison<byte>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Converter<Core.AssetBundles.Management.AssetHandle,object>
	// System.Converter<object,object>
	// System.Func<Core.AssetBundles.Management.AssetHandle<object>>
	// System.Func<HotUpdate.Game.Battle.Core.BattleResult,object>
	// System.Func<System.Threading.Tasks.VoidTaskResult>
	// System.Func<System.ValueTuple<int,byte>,object>
	// System.Func<System.ValueTuple<object,object>>
	// System.Func<byte>
	// System.Func<int,object>
	// System.Func<object,Core.AssetBundles.Management.AssetHandle<object>>
	// System.Func<object,System.Threading.Tasks.VoidTaskResult>
	// System.Func<object,System.ValueTuple<object,object>>
	// System.Func<object,byte>
	// System.Func<object,int>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.Lazy<object>
	// System.Linq.Buffer<object>
	// System.Linq.Enumerable.<ReverseIterator>d__79<object>
	// System.Linq.GroupedEnumerable<object,int,object>
	// System.Linq.IGrouping<int,object>
	// System.Linq.IdentityFunction.<>c<object>
	// System.Linq.IdentityFunction<object>
	// System.Linq.Lookup.<GetEnumerator>d__12<int,object>
	// System.Linq.Lookup.Grouping.<GetEnumerator>d__7<int,object>
	// System.Linq.Lookup.Grouping<int,object>
	// System.Linq.Lookup<int,object>
	// System.Nullable<long>
	// System.Predicate<Core.AssetBundles.Management.AssetHandle>
	// System.Predicate<HotUpdate.Game.Battle.Relic.RelicEffect>
	// System.Predicate<HotUpdate.Game.Battle.Turn.WaveData>
	// System.Predicate<HotUpdate.Game.Point.PointInfo>
	// System.Predicate<System.ValueTuple<int,int,System.Nullable<long>>>
	// System.Predicate<System.ValueTuple<object,object>>
	// System.Predicate<byte>
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
	// System.ValueTuple<float,float>
	// System.ValueTuple<int,byte>
	// System.ValueTuple<int,int,System.Nullable<long>>
	// System.ValueTuple<int,int>
	// System.ValueTuple<object,object>
	// UnityEngine.Events.UnityAction<int,int>
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
		// object Core.AssetBundles.Management.ObjectSpawner.GetPoolObject<object>(string,UnityEngine.Transform,UnityEngine.Vector3,UnityEngine.Quaternion,bool)
		// object Core.AssetBundles.Management.ObjectSpawner.Instantiate<object>(Core.AssetBundles.Management.AssetHandle,string,UnityEngine.Transform,UnityEngine.Vector3,UnityEngine.Quaternion,bool)
		// bool Core.AssetBundles.Management.ObjectSpawner.Release<object>(object,bool)
		// int Core.AssetBundles.Management.ObjectSpawner.Release<object>(System.Collections.Generic.IEnumerable<object>,bool)
		// object Core.AssetBundles.Management.ObjectSpawner.Spawn<object>(string,UnityEngine.Transform,UnityEngine.Vector3,UnityEngine.Quaternion,bool)
		// System.Threading.Tasks.Task<object> Core.AssetBundles.Management.ObjectSpawner.SpawnAsync<object>(string,UnityEngine.Transform,UnityEngine.Vector3,UnityEngine.Quaternion,bool)
		// System.Void Core.DI.DIContainer.BindSingleton<object,object>()
		// System.Void Core.DI.DIContainer.BindType<object,object>()
		// object Core.Exceptions.ExceptionHelper.Throw<object>(string,System.Exception)
		// object Core.GlobalEvent.EventSource.Get<object>()
		// object Core.Pool.IPoolManager.Get<object>(string)
		// System.Void Core.Pool.IPoolManager.PushObj<object>(object)
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
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<int,object>(System.Collections.Generic.IReadOnlyDictionary<int,object>,int)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<int,object>(System.Collections.Generic.IReadOnlyDictionary<int,object>,int,object)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<object,object>(System.Collections.Generic.IReadOnlyDictionary<object,object>,object)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<object,object>(System.Collections.Generic.IReadOnlyDictionary<object,object>,object,object)
		// System.Collections.Generic.List<object> System.Collections.Generic.List<object>.ConvertAll<object>(System.Converter<object,object>)
		// bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// int System.Linq.Enumerable.Count<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<System.Linq.IGrouping<int,object>> System.Linq.Enumerable.GroupBy<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Reverse<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.ReverseIterator<object>(System.Collections.Generic.IEnumerable<object>)
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
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,object>(System.Runtime.CompilerServices.TaskAwaiter&,object&)
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
		// System.Threading.Tasks.Task<object> System.Threading.Tasks.Task.FromResult<object>(object)
		// System.Threading.Tasks.Task<object[]> System.Threading.Tasks.Task.InternalWhenAll<object>(System.Threading.Tasks.Task<object>[])
		// System.Threading.Tasks.Task<object[]> System.Threading.Tasks.Task.WhenAll<object>(System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<object>>)
		// System.Threading.Tasks.Task<object[]> System.Threading.Tasks.Task.WhenAll<object>(System.Threading.Tasks.Task<object>[])
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<UnityEngine.Vector2>(UnityEngine.Vector2&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<float>(float&)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<UnityEngine.Vector2>()
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<float>()
		// object Unity.VisualScripting.AttributeUtility.GetAttribute<object>(System.Reflection.MemberInfo,bool)
		// object Unity.VisualScripting.AttributeUtility.AttributeCache.GetAttribute<object>(bool)
		// object Unity.VisualScripting.ComponentHolderProtocol.GetComponent<object>(UnityEngine.Object)
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
		// object UnityEngine.Object.Instantiate<object>(object)
		// string string.Join<object>(string,System.Collections.Generic.IEnumerable<object>)
		// string string.JoinCore<object>(System.Char*,int,System.Collections.Generic.IEnumerable<object>)
	}
}