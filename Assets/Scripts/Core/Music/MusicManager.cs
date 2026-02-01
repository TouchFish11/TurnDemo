using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DataPersistence;
using Core.Log;
using Core.Mono;
using Core.Pool;
using Core.Singleton;
using UnityEngine;

namespace Core.Music
{
    /// <summary>
    /// ���ֹ�����
    /// </summary>
    public class MusicManager : SingletonBase<MusicManager>, IMusicManager
    {
        // ��Ч�����б�
        private readonly List<AudioSource> _sounds = new List<AudioSource>();
        // �����������
        private AudioSource _backgroundMusic;
        // �Ƿ����������Ч���
        private bool isClearSounds;

        private MusicManager()
        {
            MonoManager.Instance.AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// ֡����
        /// </summary>
        private void OnUpdate()
        {
            if (GameDataManager.Instance.MusicData == null)
            {
                return;
            }

            // ����������ͣ��ֹͣ��Ч������������뻺���
            if (!GameDataManager.Instance.MusicData.SoundIsOpen || isClearSounds)
            {
                return;
            }

            // ������Ч�б������δ���ŵ���Ч
            for (int i = _sounds.Count - 1; i >= 0 ; i--)
            {
                // ��⵽δ������Ч
                if (!_sounds[i].isPlaying)
                {
                    // ��ѭ����Ч�Ქ����ϣ��ᱻ��⵽���Զ����뻺���
                    _sounds[i].Stop();
                    _sounds[i].clip = null;
                    PoolManager.Instance.PushObj(_sounds[i].gameObject);
                    _sounds.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// ���ű�������
        /// </summary>
        /// <param name="musicName">����������</param>
        /// <param name="isLoop">�Ƿ�ѭ��</param>
        /// <param name="startPlayCallBack">��ʼ���Żص�</param>
        public async Task PlayBackgroundMusic(string musicName, bool isLoop = true)
        {
            // ���������������
            if (_backgroundMusic == null)
            {
                GameObject bkMusicObj = new GameObject("BackgroundMusic/" + musicName);
                _backgroundMusic = bkMusicObj.AddComponent<AudioSource>();
                GameObject.DontDestroyOnLoad(bkMusicObj);
            }

            // ����������Ƭ��Դ
            AudioClip audioClip = await AssetBundleManager.Instance.LoadAssetAsync<AudioClip>(EAssetBundleType.Music, musicName);
            // ��������������Բ�����
            _backgroundMusic.clip = audioClip;
            _backgroundMusic.loop = isLoop;
            _backgroundMusic.volume = GameDataManager.Instance.MusicData.MusicValue;
            _backgroundMusic.mute = !GameDataManager.Instance.MusicData.MusicIsOpen;
            _backgroundMusic.Play();
        }

        /// <summary>
        /// ��ͣ��������
        /// </summary>
        public void PauseBackgroundMusic()
        {
            if (_backgroundMusic == null)
            {
                LogManager.LogError("�����������ΪNull");
                return;
            }

            _backgroundMusic.Pause();
            GameDataManager.Instance.MusicData.MusicIsOpen = false;
        }

        /// <summary>
        /// ֹͣ��������
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (_backgroundMusic == null)
            {
                LogManager.LogError("�����������ΪNull");
                return;
            }
            _backgroundMusic.Stop();
            GameDataManager.Instance.MusicData.MusicIsOpen = false;
        }

        /// <summary>
        /// �ı䱳����������
        /// </summary>
        /// <param name="value">����ֵ��0~1</param>
        public void ChangeBackgroundMusicVolume(float value)
        {
            GameDataManager.Instance.MusicData.MusicValue = value;
            if (_backgroundMusic != null)
            {
                _backgroundMusic.volume = value;
            }
            else
            {
                LogManager.LogError("�����������ΪNull");
            }
        }

        /// <summary>
        /// ������Ч
        /// </summary>
        /// <param name="soundName">��Ч��</param>
        /// <param name="isLoop">�Ƿ�ѭ��</param>
        /// <param name="createCallBack">��ɻص�</param>
        public async Task<AudioSource> CreateSoundAsync(string soundName, bool isLoop = false)
        {
            AudioClip audioClip = await AssetBundleManager.Instance.LoadAssetAsync<AudioClip>(EAssetBundleType.Music, soundName);
            isClearSounds = false;
            // �ӻ���ػ�ȡ��Ч����
            AudioSource sound = PoolManager.Instance.GetObj<AudioSource>($"Sound_{soundName}");
            // ������Ч������Բ�����
            sound.clip = audioClip;
            sound.loop = isLoop;
            sound.volume = GameDataManager.Instance.MusicData.SoundValue;
            sound.mute = !GameDataManager.Instance.MusicData.SoundIsOpen;
            sound.Play();
            _sounds.Add(sound);
            return sound;
        }

        /// <summary>
        /// ������Ч
        /// </summary>
        public void PlaySound()
        {
            for (int i = 0; i < _sounds.Count; i++)
            {
                _sounds[i].Play();
            }
            GameDataManager.Instance.MusicData.SoundIsOpen = true;
        }

        /// <summary>
        /// ��ͣ��Ч
        /// </summary>
        public void PauseSound()
        {
            for (int i = 0; i < _sounds.Count; i++)
            {
                _sounds[i].Pause();
            }
            GameDataManager.Instance.MusicData.SoundIsOpen = false;
        }

        /// <summary>
        /// ֹͣ��Ч
        /// </summary>
        public void StopSound()
        {
            for (int i = 0; i < _sounds.Count; i++)
            {
                _sounds[i].Stop();
            }
            GameDataManager.Instance.MusicData.SoundIsOpen = false;
        }

        /// <summary>
        /// ��ָͣ��ѭ����Ч
        /// </summary>
        /// <param name="sound">ѭ����Ч</param>
        /// <returns>�Ƿ���ͣ�ɹ�</returns>
        public bool PauseSelectSound(AudioSource sound)
        {
            if(sound.clip != null && sound.loop)
            {
                sound.Pause();
                return true;
            }
            return false;
        }

        /// <summary>
        /// ָֹͣ��ѭ����Ч
        /// </summary>
        /// <param name="sound">ѭ����Ч</param>
        /// <returns>�Ƿ�ֹͣ�ɹ�</returns>
        public bool SelectStopSound(AudioSource sound)
        {
            if (sound.clip != null && sound.loop)
            {
                sound.Stop();
                return true;
            }
            return false;
        }

        /// <summary>
        /// �ı���Ч����
        /// </summary>
        /// <param name="value">����ֵ��0~1</param>
        public void ChangeSoundVolume(float value)
        {
            GameDataManager.Instance.MusicData.SoundValue = value;
            for (int i = 0; i < _sounds.Count; i++)
            {
                _sounds[i].volume = value;
            }
        }

        /// <summary>
        /// ���������Ч
        /// </summary>
        public void Clear()
        {
            isClearSounds = true;
            for (int i = _sounds.Count - 1; i >= 0 ; i--)
            {
                //ֹͣ������Ч
                _sounds[i].Stop();
                //������Ƭ�ÿ�
                _sounds[i].clip = null;
                //���뻺���
                PoolManager.Instance.PushObj(_sounds[i].gameObject);
            }
            //����б�
            _sounds.Clear();
        }
    }
}
