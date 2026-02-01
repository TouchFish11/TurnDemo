using System;
using UnityEngine;

namespace Core.Music
{
    /// <summary>
    /// ��������
    /// </summary>
    [Serializable]
    public class MusicData
    {
        //���ִ�С
        [SerializeField] private float _musicValue = 1f;
        //��Ч��С
        [SerializeField] private float _soundValue = 1f;
        //�����Ƿ���
        [SerializeField] private bool _musicIsOpen = true;
        //��Ч�Ƿ���
        [SerializeField] private bool _soundIsOpen = true;

        /// <summary>
        /// ���ִ�С
        /// </summary>
        public float MusicValue { get => _musicValue; set => _musicValue = value; }

        /// <summary>
        /// ��Ч��С
        /// </summary>
        public float SoundValue { get => _soundValue; set => _soundValue = value; }

        /// <summary>
        /// �����Ƿ���
        /// </summary>
        public bool MusicIsOpen { get => _musicIsOpen; set => _musicIsOpen = value; }

        /// <summary>
        /// ��Ч�Ƿ���
        /// </summary>
        public bool SoundIsOpen { get => _soundIsOpen; set => _soundIsOpen = value; }
    }
}
