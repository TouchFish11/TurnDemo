using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Log;
using Core.Service;
using Core.Singleton;
using Core.Tasks.Extensions;
using Core.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Core.Input.ActionAsset
{
    /// <summary>
    /// 输入系统核心管理类
    /// 负责输入动作的初始化、启用/禁用、按键修改、冲突检测等核心逻辑
    /// 继承单例基类，保证全局唯一实例；实现IInputSystem接口（接口未展示）
    /// </summary>
    public class InputSystem : SingletonBase<InputSystem>, IInputSystem
    {
        // 输入配置的JSON原始数据
        private string _jsonInputData;
        // 玩家输入组件引用，关联InputActionAsset
        private PlayerInput _playerInput;
        // 按键交换的回调委托（用于处理按键冲突时的交换逻辑）
        private UnityAction ExchangeKeyAction;
        // 记录旧的动作映射枚举（修改按键时的原动作映射）
        private E_MainActionMap oldKeyMap;
        // 记录旧的按键（修改前的按键）
        private Key oldKey;
        // 记录新的按键（修改后的按键）
        private Key newKey;
        // 记录新的按键路径（InputSystem的标准路径格式）
        private string newPath;
        // 数据容器
        private MainActionMapDataContainer _mapDataContainer;
        
        /// <summary>
        /// 私有构造函数
        /// 单例模式，禁止外部实例化
        /// </summary>
        private InputSystem()
        {

        }

        /// <summary>
        /// 初始化输入系统
        /// </summary>
        /// <param name="abName"></param>
        public async Task InitAsync(string abName)
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            // 从AssetBundle加载输入配置JSON
            var assetBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(abName);
            var json = await assetBundle.LoadAssetAsync<TextAsset>(FileUtility.InputActionLocalFileName).ToTask<TextAsset>();
            _jsonInputData = json.text;
#else
            // 编辑器模式：从编辑器资源管理器加载输入配置JSON
            TextAsset json = ServiceLocator.Get<IEditorResManager>().LoadEditorAsset<TextAsset>(FileUtility.InputActionLocalFileName, "None");
            _jsonInputData = json.text;
            UpdateActions();
            await Task.CompletedTask;
#endif
        }

        /// <summary>
        /// 初始化玩家输入组件
        /// </summary>
        /// <param name="playerInput">玩家输入组件实例</param>
        /// <param name="container"></param>
        /// <param name="onActionTrigger">输入动作触发时的回调方法</param>
        /// <returns>异步任务</returns>
        public void InitPlayerInput(PlayerInput playerInput, MainActionMapDataContainer container, Action<InputAction.CallbackContext> onActionTrigger)
        {
            // 缓存玩家输入组件引用
            _playerInput = playerInput;
            // 缓存数据容器
            _mapDataContainer = container;
            // 设置通知行为为调用C#事件
            _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            // 注册动作触发回调
            if (playerInput && onActionTrigger != null)
            {
                _playerInput.onActionTriggered += onActionTrigger;
            }
            UpdateActions();
        }

        /// <summary>
        /// 启用所有输入动作
        /// 使输入系统响应玩家输入
        /// </summary>
        public void EnableInput()
        {
            _playerInput.actions?.Enable();
        }

        /// <summary>
        /// 禁用所有输入动作
        /// 使输入系统停止响应玩家输入
        /// </summary>
        public void DisableInput()
        {
            _playerInput.actions?.Disable();
        }

        /// <summary>
        /// 根据动作名称获取输入动作实例
        /// </summary>
        /// <param name="actionName">输入动作名称（与InputActionAsset中配置一致）</param>
        /// <returns>对应的InputAction实例</returns>
        public InputAction GetInputAction(string actionName)
        {
            return _playerInput.actions[actionName];
        }

        /// <summary>
        /// 编辑输入按键（核心修改逻辑）
        /// 监听玩家新的按键输入，替换原有按键配置
        /// </summary>
        /// <param name="keyMap">要修改的动作映射枚举</param>
        /// <param name="oldKey">原按键</param>
        /// <param name="overCallBack">修改完成/冲突时的回调（返回冲突类型）</param>
        public void EditInput(E_MainActionMap keyMap, Key oldKey, UnityAction<E_KeyConflict> overCallBack)
        {
            // 监听任意按键按下事件（仅触发一次）
            UnityEngine.InputSystem.InputSystem.onAnyButtonPress.CallOnce((inputControl =>
            {
                // 解析按键路径，格式转换为InputSystem标准路径（<设备>/按键名）
                var originalPaths = inputControl.path.Split('/');
                var newpath = $"<{originalPaths[1]}>/{originalPaths[2]}";

                // 尝试将按键名转换为Key枚举（不区分大小写）
                // 非键盘按键会转换失败
                if (!Enum.TryParse(typeof(Key), originalPaths[2], true, out var result))
                {
                    // 回调：非键盘按键（不支持修改）
                    overCallBack?.Invoke(E_KeyConflict.NotKeyboard);
                    return;
                }

                var newTempKey = (Key)result;
                // 检查是否为特殊按键（系统保留/不允许修改的按键）
                if (IsSpecialKey(newTempKey))
                {
                    // 回调：特殊按键冲突
                    overCallBack?.Invoke(E_KeyConflict.SpecialKey);
                    return;
                }
                // 检查新按键是否与现有配置冲突
                if (IsKeyConflict(keyMap, oldKey, newTempKey, newpath))
                {
                    // 回调：按键已存在冲突
                    overCallBack?.Invoke(E_KeyConflict.ExistKey);
                    return;
                }

                // 更新配置容器中的按键映射
                _mapDataContainer.actionMap[keyMap] = new KeyPathMap(newTempKey, newpath);
                // 刷新输入动作配置
                UpdateActions();
                // 回调：修改完成（无冲突）
                overCallBack?.Invoke(E_KeyConflict.Over);
            }));
        }

        /// <summary>
        /// 获取最新的输入动作资源
        /// 根据配置容器中的自定义按键，替换原始JSON中的默认按键路径
        /// </summary>
        /// <returns>动态生成的InputActionAsset实例</returns>
        private InputActionAsset GetInputActionAsset()
        {
            // 基于原始JSON数据构建新的配置字符串
            var sb = new StringBuilder();
            sb.Append(_jsonInputData);

            // 遍历所有动作映射枚举，替换对应的按键路径
            var enumType = typeof(E_MainActionMap);
            foreach (var enumValue in Enum.GetValues(enumType))
            {
                var enumName = enumValue.ToString();
                var memberInfo = enumType.GetMember(enumName)[0];
                // 获取枚举上的替换标记特性
                var attribute = memberInfo.GetCustomAttribute<ActionMapReplaceKeyAttribute>();

                // 如果存在替换标记且配置容器中有对应映射，则替换路径
                if (attribute != null && _mapDataContainer.actionMap.TryGetValue((E_MainActionMap)enumValue, out var keyPathMap))
                {
                    sb.Replace(attribute.ReplaceKey, keyPathMap.path);
                }
            }
            // 从修改后的JSON生成InputActionAsset
            return InputActionAsset.FromJson(sb.ToString());
        }

        /// <summary>
        /// 执行按键交换逻辑
        /// 处理按键冲突时的双向替换（A键替换B键，B键替换A键）
        /// </summary>
        public void InvokeExchangeKey()
        {
            // 无有效交换数据时直接返回
            if (oldKeyMap == E_MainActionMap.None && oldKey == Key.None && newKey == Key.None && newPath == null)
                return;

            // 执行交换回调
            ExchangeKeyAction?.Invoke();
            // 清空交换数据（重置状态）
            ExchangeKeyAction = null;
            oldKeyMap = E_MainActionMap.None;
            oldKey = Key.None;
            newKey = Key.None;
            newPath = null;
        }

        /// <summary>
        /// 更新输入动作配置
        /// 将最新的InputActionAsset赋值给PlayerInput，使修改生效
        /// </summary>
        /// <param name="playerInput">可选：指定新的PlayerInput实例</param>
        public void UpdateActions(PlayerInput playerInput = null)
        {
            if (playerInput)
            {
                // 赋值新的InputActionAsset并更新引用
                playerInput.actions = GetInputActionAsset();
                _playerInput = playerInput;
            }
            else if (_playerInput)
            {
                // 刷新现有PlayerInput的动作配置
                _playerInput.actions = GetInputActionAsset();
                LogManager.Log($"输入配置更新成功，{_playerInput.actions}");
            }
            else
            {
                // 日志：PlayerInput为空，更新失败
                LogManager.LogError($"输入配置获取失败，{playerInput}");
                return;
            }
        }

        /// <summary>
        /// 检查是否为特殊按键（系统保留/不允许修改的按键）
        /// </summary>
        /// <param name="newKey">待检查的按键</param>
        /// <returns>true=特殊按键，false=普通按键</returns>
        private static bool IsSpecialKey(Key newKey)
        {
            // 定义特殊按键列表：无、退出、回车、结束、输入法选择
            return newKey is Key.None or Key.Escape or Key.Enter or Key.End or Key.IMESelected;
        }

        /// <summary>
        /// 检查按键是否冲突
        /// 1. 同一动作映射下的相同路径不判定为冲突
        /// 2. 新按键已被其他动作映射使用则判定为冲突
        /// </summary>
        /// <param name="oldKeyMap">原动作映射</param>
        /// <param name="oldKey">原按键</param>
        /// <param name="newKey">新按键</param>
        /// <param name="newPath">新按键路径</param>
        /// <returns>true=冲突，false=无冲突</returns>
        private bool IsKeyConflict(E_MainActionMap oldKeyMap, Key oldKey, Key newKey, string newPath)
        {
            // 同一动作映射下，路径未变化（仅重复点击原按键），不判定为冲突
            if (_mapDataContainer.actionMap[oldKeyMap].path == newPath)
            {
                return false;
            }

            // 遍历所有已配置的按键映射，检查新按键是否已被占用
            foreach (var map in _mapDataContainer.actionMap.Values)
            {
                if (newKey != map.key)
                {
                    continue;
                }
                
                // 注册按键交换回调（后续执行双向替换）
                ExchangeKeyAction += ExchangeKey;
                // 记录冲突相关数据
                this.oldKeyMap = oldKeyMap;
                this.oldKey = oldKey;
                this.newKey = newKey;
                this.newPath = newPath;
                return true;
            }

            // 新按键未被占用，无冲突
            return false;
        }

        /// <summary>
        /// 交换冲突按键的配置
        /// 当新按键已被占用时，将原动作映射与占用动作映射的按键互换
        /// </summary>
        private void ExchangeKey()
        {
            // 遍历所有动作映射，找到占用新按键的映射
            foreach (var keyMap in _mapDataContainer.actionMap.Keys)
            {
                var keyPathMap = _mapDataContainer.actionMap[keyMap];

                // 找到占用新按键的动作映射
                if (keyPathMap.key != newKey)
                {
                    continue;
                }
                
                // 缓存原动作映射的配置
                var tempKeyPathMap = _mapDataContainer.actionMap[oldKeyMap];
                // 替换原动作映射的按键为新按键
                _mapDataContainer.actionMap[oldKeyMap] = new KeyPathMap(newKey, newPath);
                // 将占用映射的按键替换为原按键
                _mapDataContainer.actionMap[keyMap] = tempKeyPathMap;
                // 刷新输入配置使交换生效
                UpdateActions();
                return;
            }
        }

        /// <summary>
        /// 初始化输入动作配置容器
        /// 反射读取指定类型的静态属性，初始化默认按键映射
        /// </summary>
        /// <typeparam name="T">存储默认按键配置的类型（静态属性）</typeparam>
        /// <param name="container">要初始化的配置容器</param>
        public static void InitContainer<T>(MainActionMapDataContainer container)
        {
            var type = typeof(T);
            // 获取类型的所有公共静态属性
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var property in properties)
            {
                // 属性名对应动作映射枚举名
                var name = property.Name;
                var actionEnumName = (E_MainActionMap)Enum.Parse(typeof(E_MainActionMap), name);
                // 属性值对应按键路径
                var value = property.GetValue(null).ToString();

                // 获取属性上的按键映射特性
                var memberInfo = type.GetMember(name)[0];
                var attribute = memberInfo.GetCustomAttribute<ActionKeyMapAttribute>();
                if (attribute == null)
                {
                    continue;
                }
                
                // 根据特性类型初始化按键映射（键盘按键/鼠标值/鼠标按钮）
                if (attribute.Key != Key.None)
                {
                    container.actionMap.Add(actionEnumName, new KeyPathMap(attribute.Key, value));
                }
                else if (attribute.MouseValue != E_MouseValue.None)
                {
                    container.actionMap.Add(actionEnumName, new KeyPathMap(attribute.MouseValue, value));
                }
                else
                {
                    container.actionMap.Add(actionEnumName, new KeyPathMap(attribute.MouseButton, value));
                }
            }
        }
    }
}