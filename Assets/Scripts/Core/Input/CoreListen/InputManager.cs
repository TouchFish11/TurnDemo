using System;
using Core.Singleton;

namespace Core.Input.CoreListen
{
    /// <summary>
    /// ���������
    /// </summary>
    [Obsolete("ʹ��FrameworkInputSystem", true)]
    public class InputManager : SingletonBase<InputManager>, IInputManager
    {
        //    //�洢��������
        //    private Dictionary<E_EventType, InputData> _inputDataDic = new Dictionary<E_EventType, InputData>();
        //    //��ǰ��������
        //    private InputData _nowInputData;
        //    //�Ƿ�������
        //    private bool _isCheckInput;

        //    private InputManager()
        //    {
        //        ServiceLocator.Get<IMonoManager>().AddUpdateListener(UpdateInput);
        //    }

        //    /// <summary>
        //    /// ��ʼ������ϵͳ(Main����������)
        //    /// </summary>
        //    public void InitSystem()
        //    {
        //        //��ȡ����
        //        _inputDataDic = GameDataManager.Instance.InputDataContainer.InputDataDic;
        //        //�ж��Ƿ��ǵ�һ�ν�����Ϸ����һ�ν�����Ϸ���ֵ�û�г���
        //        if (_inputDataDic.Count == 0)
        //        {
        //            //����Ĭ����������
        //        }
        //    }

        //    /// <summary>
        //    /// ������ر�����
        //    /// </summary>
        //    /// <param name="isStart">�Ƿ���</param>
        //    public void StartOrCloseInput(bool isStart)
        //    {
        //        _isCheckInput = isStart;
        //    }

        //    /// <summary>
        //    /// �༭��λ
        //    /// </summary>
        //    /// <param name="oldKeyBoard">�ϼ�λ</param>
        //    /// <param name="callBack">�Ľ������ص�</param>
        //    public void EditInput(Key oldKeyBoard, UnityAction callBack)
        //    {
        //        ServiceLocator.Get<IMonoManager>().StartCoroutine(EditoInput_Cor());

        //        IEnumerator EditoInput_Cor()
        //        {
        //            Array keyArr = Enum.GetValues(typeof(Key));
        //            yield return null;

        //            while (true)
        //            {
        //                if (Keyboard.current.anyKey.wasPressedThisFrame)
        //                {
        //                    //����������
        //                    foreach (Key keyBorad in keyArr)
        //                    {
        //                        if (Keyboard.current[keyBorad].wasPressedThisFrame)
        //                        {
        //                            EditKeyBoardData(oldKeyBoard, keyBorad);
        //                            break;
        //                        }
        //                    }
        //                    callBack?.Invoke();
        //                    yield break;
        //                }
        //                yield return null;
        //            }
        //        }
        //    }

        //    /// <summary>
        //    /// ��������
        //    /// </summary>
        //    private void UpdateInput()
        //    {
        //        if (!_isCheckInput)
        //            return;

        //        //����������
        //        foreach (E_EventType eventType in _inputDataDic.Keys)
        //        {
        //            _nowInputData = _inputDataDic[eventType];
        //            if (_nowInputData.InputType == E_InputType.Key)
        //            {
        //                switch (_nowInputData.InputMode)
        //                {
        //                    case E_InputMode.Down:
        //                        if (Keyboard.current[_nowInputData.Key].wasPressedThisFrame)
        //                            ServiceLocator.Get<IEventCenter>().TriggerEvent(eventType);
        //                        break;
        //                    case E_InputMode.Up:
        //                        if (Keyboard.current[_nowInputData.Key].wasReleasedThisFrame)
        //                            ServiceLocator.Get<IEventCenter>().TriggerEvent(eventType);
        //                        break;
        //                    case E_InputMode.Press:
        //                        if (Keyboard.current[_nowInputData.Key].isPressed)
        //                            ServiceLocator.Get<IEventCenter>().TriggerEvent(eventType);
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                //����������
        //                switch (_nowInputData.InputMode)
        //                {
        //                    case E_InputMode.Down:
        //                        if (GetCurrentMouseButton(_nowInputData.Mouse).wasPressedThisFrame)
        //                            ServiceLocator.Get<IEventCenter>().TriggerEvent(eventType);
        //                        break;
        //                    case E_InputMode.Up:
        //                        if (GetCurrentMouseButton(_nowInputData.Mouse).wasReleasedThisFrame)
        //                            ServiceLocator.Get<IEventCenter>().TriggerEvent(eventType);
        //                        break;
        //                    case E_InputMode.Press:
        //                        if (GetCurrentMouseButton(_nowInputData.Mouse).isPressed)
        //                            ServiceLocator.Get<IEventCenter>().TriggerEvent(eventType);
        //                        break;
        //                }
        //            }
        //        }
        //    }

        //    /// <summary>
        //    /// ��ȡ��ǰ��������갴��
        //    /// </summary>
        //    /// <param name="mouseButton">��갴��ö��</param>
        //    /// <returns></returns>
        //    private ButtonControl GetCurrentMouseButton(MouseButton mouseButton)
        //    {
        //        return mouseButton switch
        //        {
        //            MouseButton.Left => Mouse.current.leftButton,
        //            MouseButton.Right => Mouse.current.rightButton,
        //            MouseButton.Middle => Mouse.current.middleButton,
        //            MouseButton.Forward => Mouse.current.forwardButton,
        //            MouseButton.Back => Mouse.current.backButton,
        //            _ => null,
        //        };
        //    }

        //    /// <summary>
        //    /// ��ȡ������������
        //    /// </summary>
        //    /// <param name="oldKey">�ϼ�λ</param>
        //    /// <param name="newKey">�¼�λ</param>
        //    /// <returns></returns>
        //    private void EditKeyBoardData(Key oldKey, Key newKey)
        //    {
        //        foreach (InputData inputData in _inputDataDic.Values)
        //        {
        //            if (inputData.Key == oldKey)
        //            {
        //                inputData.Key = newKey;
        //            }
        //        }
        //    }
    }
}
