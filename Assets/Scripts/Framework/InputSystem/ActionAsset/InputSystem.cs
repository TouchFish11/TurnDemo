using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Framework
{
    /// <summary>
    /// 输入系统
    /// </summary>
    public class InputSystem : SingletonBase<InputSystem>, IInputSystem
    {
        // 输入动作json数据
        private string _jsonInputData;
        // 玩家输入组件
        private PlayerInput _playerInput;
        // 存储交换按键函数的委托
        private UnityAction ExchangeKeyAction;
        // 记录旧键位映射
        private E_MainActionMap oldKeyMap;
        // 记录旧按键
        private Key oldKey;
        // 记录新按键
        private Key newKey;
        // 记录新路径
        private string newPath;

        /// <summary>
        /// 输入行为触发事件
        /// </summary>
        public event Action<InputAction.CallbackContext> OnActionTrigger;

        private InputSystem()
        {

        }

        /// <summary>
        /// 初始化玩家输入
        /// </summary>
        /// <param name="callBack"></param>
        public async Task InitPlayerInput(PlayerInput playerInput, Action<InputAction.CallbackContext> onActionTrigger)
        {
            // 存储玩家输入组件
            _playerInput = playerInput;
            // 设置通知行为
            _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            // 订阅行为触发事件
            if (playerInput != null && onActionTrigger != null)
            {
                _playerInput.onActionTriggered += onActionTrigger;
            }

#if EDITOR_TEST_AB || !UNITY_EDITOR
            TextAsset json = await AssetBundleManager.Instance.LoadAssetAsync<TextAsset>(E_AssetBundleType.InputData, FileUtility.InputActionLocalFileName);
            _jsonInputData = json.text;
            UpdateActions();
#else
            TextAsset json = EditorResManager.Instance.LoadEditorAsset<TextAsset>(FileUtility.InputActionLocalFileName, "None");
            _jsonInputData = json.text;
            UpdateActions();
            await Task.CompletedTask;
#endif
        }

        public void EnableInput()
        {
            _playerInput.actions.Enable();
        }

        public void DisableInput()
        {
            _playerInput.actions.Disable();
        }

        /// <summary>
        /// 获取输入动作
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public InputAction GetInputAction(string actionName)
        {
            return _playerInput.actions[actionName];
        }

        /// <summary>
        /// 编辑输入(改键)
        /// </summary>
        /// <param name="keyMap">键位对应的行为映射类型</param>
        /// <param name="oldKey">行为对应键位</param>
        /// <param name="overCallBack">结束回调</param>
        public void EditInput(E_MainActionMap keyMap, Key oldKey, UnityAction<E_KeyConflict> overCallBack)
        {
            UnityEngine.InputSystem.InputSystem.onAnyButtonPress.CallOnce((inputControl =>
            {
                //拼接格式
                string[] originalPaths = inputControl.path.Split('/');
                string newpath = $"<{originalPaths[1]}>/{originalPaths[2]}";

                //将输入的字符串转枚举
                //按下的键不是键盘的键，是鼠标就会转换失败
                if (!Enum.TryParse(typeof(Key), originalPaths[2], true, out object result))
                {
                    //非键盘按键冲突
                    overCallBack?.Invoke(E_KeyConflict.NotKeyboard);
                }

                Key newKey = (Key)result;
                //判断特殊键位
                if (IsSpecialKey(newKey))
                {
                    //特殊键位冲突
                    overCallBack?.Invoke(E_KeyConflict.SpecialKey);
                    return;
                }
                //判断按键冲突
                if (IsKeyConflict(keyMap, oldKey, newKey, newpath))
                {
                    //键位冲突
                    overCallBack?.Invoke(E_KeyConflict.ExistKey);
                    return;
                }

                //修改对应行为的按键和路径
                GameDataManager.Instance.InputActionContainer.actionMap[keyMap] = new KeyPathMap(newKey, newpath);
                //更新数据
                UpdateActions();
                //执行回调
                overCallBack?.Invoke(E_KeyConflict.Over);
            }));
        }

        /// <summary>
        /// 获取输入动作资源
        /// </summary>
        /// <returns>输入动作资源</returns>
        private InputActionAsset GetInputActionAsset()
        {
            MainActionMapDataContainer container = GameDataManager.Instance.InputActionContainer;

            StringBuilder sb = new StringBuilder();
            sb.Append(_jsonInputData);

            // 通过反射获取所有需要替换的关键字
            Type enumType = typeof(E_MainActionMap);
            foreach (object enumValue in Enum.GetValues(enumType))
            {
                string enumName = enumValue.ToString();
                var memberInfo = enumType.GetMember(enumName)[0];
                var attribute = memberInfo.GetCustomAttribute<ActionMapReplaceKeyAttribute>();

                if (attribute != null && container.actionMap.TryGetValue((E_MainActionMap)enumValue, out var keyPathMap))
                {
                    sb.Replace(attribute.ReplaceKey, keyPathMap.path);
                }
            }
            return InputActionAsset.FromJson(sb.ToString());
        }

        /// <summary>
        /// 执行交换键位（调用后即可改建）
        /// </summary>
        public void InvokeExchangeKey()
        {
            if (oldKeyMap == E_MainActionMap.None && oldKey == Key.None && newKey == Key.None && newPath == null)
                return;

            ExchangeKeyAction?.Invoke();
            ExchangeKeyAction = null;
            oldKeyMap = E_MainActionMap.None;
            oldKey = Key.None;
            newKey = Key.None;
            newPath = null;
        }

        /// <summary>
        /// 更新动作数据
        /// </summary>
        /// <param name="playerInput">玩家输入组件</param>
        public void UpdateActions(PlayerInput playerInput = null)
        {
            if (playerInput != null)
            {
                playerInput.actions = GetInputActionAsset();
                playerInput.actions.Enable();
                _playerInput = playerInput;
            }
            else if (_playerInput != null)
            {
                _playerInput.actions = GetInputActionAsset();
                _playerInput.actions.Enable();
                LogManager.Log("玩家输入组件激活成功");
            }
            else
            {
                LogManager.LogError("玩家输入组件获取失败");
                return;
            }
        }

        /// <summary>
        /// 是否是特殊键位
        /// </summary>
        /// <param name="newKey">新键位</param>
        /// <returns>true：是特殊键位；false：不是特殊键位</returns>
        private bool IsSpecialKey(Key newKey)
        {
            if (newKey == Key.None || newKey == Key.Escape || newKey == Key.Enter || newKey == Key.End || newKey == Key.IMESelected)
                return true;
            return false;
        }

        /// <summary>
        /// 键位是否冲突
        /// </summary>
        /// <param name="oldKeyMap">旧键位映射</param>
        /// <param name="oldKey">旧键位</param>
        /// <param name="newKey">新键位</param>
        /// <param name="newPath">新键位路径</param>
        /// <returns>是否冲突</returns>
        private bool IsKeyConflict(E_MainActionMap oldKeyMap, Key oldKey, Key newKey, string newPath)
        {
            //若改的键是同一个键，且键位修改为自身，不冲突
            //eg：左转行为原来对应A，现在我又改为了A，说明是自己改为自己，不用处理
            if (GameDataManager.Instance.InputActionContainer.actionMap[oldKeyMap].path == newPath)
            {
                return false;
            }

            //改的键和原来的键不一样，eg：左转行为原来对应A，现在我改为了D，说明要处理，处理该D键有没有和其它行为的键冲突
            foreach (KeyPathMap map in GameDataManager.Instance.InputActionContainer.actionMap.Values)
            {
                //如果新key等于了数据中的任何其中一个key，说明按键冲突
                if (newKey == map.key)
                {
                    //存储委托
                    ExchangeKeyAction += ExchangeKey;
                    //记录冲突键位和新键位
                    this.oldKeyMap = oldKeyMap;
                    this.oldKey = oldKey;
                    this.newKey = newKey;
                    this.newPath = newPath;
                    return true;
                }
            }

            //新key都不等于数据中的任何一个key，说明改了不同的键，不冲突
            return false;
        }

        /// <summary>
        /// 交换键位
        /// </summary>
        private void ExchangeKey()
        {
            MainActionMapDataContainer container = GameDataManager.Instance.InputActionContainer;
            foreach (E_MainActionMap keyMap in container.actionMap.Keys)
            {
                KeyPathMap keyPathMap = container.actionMap[keyMap];

                //判断容器中存不存在这个键，存在即冲突了
                if (keyPathMap.key == newKey)
                {
                    // 临时存储老按键
                    KeyPathMap tempKeyPathMap = container.actionMap[oldKeyMap];
                    // 交换键位
                    // 让老键Key等于newKey，让老键的path等于新path
                    container.actionMap[oldKeyMap] = new KeyPathMap(newKey, newPath);
                    // 让冲突的Key等于oldKey，冲突的path等于老键的path
                    container.actionMap[keyMap] = tempKeyPathMap;
                    //更新数据
                    UpdateActions();
                    return;
                }
            }
        }

        /// <summary>
        /// 初始化动作容器
        /// </summary>
        /// <typeparam name="T">输入动作数据类型</typeparam>
        /// <param name="container"></param>
        public static void InitContainer<T>(MainActionMapDataContainer container)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (PropertyInfo property in properties)
            {
                string name = property.Name;
                E_MainActionMap actionEnumName = (E_MainActionMap)Enum.Parse(typeof(E_MainActionMap), name);
                string value = property.GetValue(null).ToString();

                MemberInfo memberInfo = type.GetMember(name)[0];
                var attribute = memberInfo.GetCustomAttribute<ActionKeyMapAttribute>();
                if (attribute != null)
                {
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
}
