using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Core.InputSystem.CoreListen
{
    /// <summary>
    /// ��������
    /// </summary>
    [Serializable]
    public sealed class InputData
    {
        //����ö��
        private Key _key;
        //���ö��
        private MouseButton _mouse;
        //��������
        private E_InputType _inputType;
        //����ģʽ
        private E_InputMode _inputMode;

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="keyBoard">����ö��</param>
        /// <param name="inputMode">����ģʽ</param>
        public InputData(Key keyBoard, E_InputMode inputMode)
        {
            _key = keyBoard;
            _inputMode = inputMode;
            _inputType = E_InputType.Key;
        }

        /// <summary>
        /// ��ʼ���������
        /// </summary>
        /// <param name="mouseButton">���ö��</param>
        /// <param name="inputMode">����ģʽ</param>
        public InputData(MouseButton mouseButton, E_InputMode inputMode)
        {
            _mouse = mouseButton;
            _inputMode = inputMode;
            _inputType = E_InputType.Mouse;
        }

        /// <summary>
        /// ����ö��
        /// </summary>
        public Key Key { get { return _key; } set { _key = value; } }
        /// <summary>
        /// ���ö��
        /// </summary>
        public MouseButton Mouse { get { return _mouse; } }
        /// <summary>
        /// ��������
        /// </summary>
        public E_InputType InputType { get { return _inputType; } }
        /// <summary>
        /// ����ģʽ
        /// </summary>
        public E_InputMode InputMode { get { return _inputMode; } }
    }
}
