using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
using XLua;

/// <summary>
/// Lua和CS相互调用集合类
/// </summary>
[LuaCallCSharp]
public static class XLuaCall
{
    [CSharpCallLua]
    public static List<Type> csharpCallLuaList = new List<Type>()
    {
        //有符号整形
        typeof(UnityAction<sbyte>),
        typeof(UnityAction<short>),
        typeof(UnityAction<int>),
        typeof(UnityAction<long>),

        //无符号整形
        typeof(UnityAction<byte>),
        typeof(UnityAction<ushort>),
        typeof(UnityAction<uint>),
        typeof(UnityAction<ulong>),

        //浮点型
        typeof(UnityAction<float>),
        typeof(UnityAction<double>),
        typeof(UnityAction<decimal>),

        //特殊类型
        typeof(UnityAction<char>),
        typeof(UnityAction<string>),
        typeof(UnityAction<bool>),
        typeof(IEnumerator),
    };

    [LuaCallCSharp]
    public static List<Type> luaCallCsharpList = new List<Type>()
    {
        //框架类
        typeof(AssetBundleManager),
        typeof(LogManager),
        typeof(E_EventType),
        typeof(EventCenter),
        typeof(InputActionData),
        typeof(InputSystem),
        typeof(BinaryDataMgr),
        typeof(GameDataMgr),
        typeof(MonoManager),
        typeof(MusicData),
        typeof(MusicManager),
        typeof(PoolManager),
        typeof(ResourcesManager),
        typeof(SceneManager),
        typeof(Timer),
        typeof(TimerMgr),
        typeof(UWRMgr),
        typeof(EncryptionUtility),
        typeof(FileUtility),
        typeof(MathUtility),
        typeof(TextUtility),
		typeof(E_UILayer),

        //有符号
        typeof(sbyte),
        typeof(short),
        typeof(int),
        typeof(long),

        //无符号
        typeof(byte),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),

        //浮点型
        typeof(float),
        typeof(double),
        typeof(decimal),

        //特殊类型
        typeof(char),
        typeof(short),
        typeof(bool),
        typeof(IEnumerator),

        //UI
        typeof(UIBehaviour),
        typeof(Button),
        typeof(Slider),
        typeof(Toggle),
        typeof(ToggleGroup),
        typeof(InputField),
        typeof(ScrollRect),
        typeof(TextMeshProUGUI),
        typeof(Image),
        typeof(Dropdown),
        typeof(Canvas),

        //UnityEngine
        typeof(GameObject),
        typeof(Transform),
        typeof(Camera),
        typeof(Rigidbody),
        typeof(Rigidbody2D),
        typeof(RectTransform),
        typeof(SpriteAtlas),
        typeof(Sprite),
        typeof(Vector2),
        typeof(Vector3),
        typeof(TextAsset),
        typeof(AssetBundle),
    };
}
