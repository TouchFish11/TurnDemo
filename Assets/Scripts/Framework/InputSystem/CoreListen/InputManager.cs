using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace Framework
{
    /// <summary>
    /// 输入管理器
    /// </summary>
    [Obsolete("使用FrameworkInputSystem", true)]
    public class InputManager : SingletonBase<InputManager>, IInputManager
    {
        //存储输入数据
        private Dictionary<E_EventType, InputData> _inputDataDic = new Dictionary<E_EventType, InputData>();
        //当前输入数据
        private InputData _nowInputData;
        //是否检测输入
        private bool _isCheckInput;

        private InputManager()
        {
            MonoManager.Instance.AddUpdateListener(UpdateInput);
        }

        /// <summary>
        /// 初始化输入系统(Main主函数调用)
        /// </summary>
        public void InitSystem()
        {
            //读取数据
            _inputDataDic = GameDataManager.Instance.InputDataContainer.InputDataDic;
            //判断是否是第一次进入游戏，第一次进入游戏该字典没有长度
            if (_inputDataDic.Count == 0)
            {
                //添加默认输入数据
            }
        }

        /// <summary>
        /// 开启或关闭输入
        /// </summary>
        /// <param name="isStart">是否开启</param>
        public void StartOrCloseInput(bool isStart)
        {
            _isCheckInput = isStart;
        }

        /// <summary>
        /// 编辑键位
        /// </summary>
        /// <param name="oldKeyBoard">老键位</param>
        /// <param name="callBack">改建结束回调</param>
        public void EditInput(Key oldKeyBoard, UnityAction callBack)
        {
            MonoManager.Instance.StartCoroutine(EditoInput_Cor());

            IEnumerator EditoInput_Cor()
            {
                Array keyArr = Enum.GetValues(typeof(Key));
                yield return null;

                while (true)
                {
                    if (Keyboard.current.anyKey.wasPressedThisFrame)
                    {
                        //检测键盘输入
                        foreach (Key keyBorad in keyArr)
                        {
                            if (Keyboard.current[keyBorad].wasPressedThisFrame)
                            {
                                EditKeyBoardData(oldKeyBoard, keyBorad);
                                break;
                            }
                        }
                        callBack?.Invoke();
                        yield break;
                    }
                    yield return null;
                }
            }
        }

        /// <summary>
        /// 更新输入
        /// </summary>
        private void UpdateInput()
        {
            if (!_isCheckInput)
                return;

            //检测键盘输入
            foreach (E_EventType eventType in _inputDataDic.Keys)
            {
                _nowInputData = _inputDataDic[eventType];
                if (_nowInputData.InputType == E_InputType.Key)
                {
                    switch (_nowInputData.InputMode)
                    {
                        case E_InputMode.Down:
                            if (Keyboard.current[_nowInputData.Key].wasPressedThisFrame)
                                EventCenter.Instance.TriggerEvent(eventType);
                            break;
                        case E_InputMode.Up:
                            if (Keyboard.current[_nowInputData.Key].wasReleasedThisFrame)
                                EventCenter.Instance.TriggerEvent(eventType);
                            break;
                        case E_InputMode.Press:
                            if (Keyboard.current[_nowInputData.Key].isPressed)
                                EventCenter.Instance.TriggerEvent(eventType);
                            break;
                    }
                }
                else
                {
                    //检测鼠标输入
                    switch (_nowInputData.InputMode)
                    {
                        case E_InputMode.Down:
                            if (GetCurrentMouseButton(_nowInputData.Mouse).wasPressedThisFrame)
                                EventCenter.Instance.TriggerEvent(eventType);
                            break;
                        case E_InputMode.Up:
                            if (GetCurrentMouseButton(_nowInputData.Mouse).wasReleasedThisFrame)
                                EventCenter.Instance.TriggerEvent(eventType);
                            break;
                        case E_InputMode.Press:
                            if (GetCurrentMouseButton(_nowInputData.Mouse).isPressed)
                                EventCenter.Instance.TriggerEvent(eventType);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前触发的鼠标按键
        /// </summary>
        /// <param name="mouseButton">鼠标按键枚举</param>
        /// <returns></returns>
        private ButtonControl GetCurrentMouseButton(MouseButton mouseButton)
        {
            return mouseButton switch
            {
                MouseButton.Left => Mouse.current.leftButton,
                MouseButton.Right => Mouse.current.rightButton,
                MouseButton.Middle => Mouse.current.middleButton,
                MouseButton.Forward => Mouse.current.forwardButton,
                MouseButton.Back => Mouse.current.backButton,
                _ => null,
            };
        }

        /// <summary>
        /// 获取键盘输入数据
        /// </summary>
        /// <param name="oldKey">老键位</param>
        /// <param name="newKey">新键位</param>
        /// <returns></returns>
        private void EditKeyBoardData(Key oldKey, Key newKey)
        {
            foreach (InputData inputData in _inputDataDic.Values)
            {
                if (inputData.Key == oldKey)
                {
                    inputData.Key = newKey;
                }
            }
        }
    }
}
