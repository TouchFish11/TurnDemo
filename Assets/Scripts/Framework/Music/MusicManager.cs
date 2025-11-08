using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 音乐管理器
    /// </summary>
    public class MusicManager : SingletonBase<MusicManager>
    {
        //存储所有播放的音效
        private readonly List<AudioSource> _sounds = new List<AudioSource>();

        //背景音乐对象
        private AudioSource _backgroundMusic;

        //是否清空所有音效
        private bool isClearSounds;

        private MusicManager()
        {
            MonoManager.Instance.AddFixedUpdateListener(FixedUpdate);
        }

        /// <summary>
        /// 自定义物理帧更新
        /// </summary>
        private void FixedUpdate()
        {
            //避免主动暂停或停止音效导入其意外放入缓存池
            if (!GameDataMgr.Instance.MusicData.SoundIsOpen || isClearSounds)
                return;

            //更新音效
            for (int i = _sounds.Count - 1; i >= 0 ; i--)
            {
                //音效没有播放
                if (!_sounds[i].isPlaying)
                {
                    //非循环音效会播放完毕，会被检测到，自动存入缓存池
                    //暂停音效
                    _sounds[i].Stop();
                    //置空cilp
                    _sounds[i].clip = null;
                    //放入缓存池
                    PoolManager.Instance.PushObj(_sounds[i].gameObject);
                    //从音效容器中移除
                    _sounds.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="musicName">背景音乐名</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="startPlayCallBack">开始播放回调</param>
        public void PlayBackgroundMusic(string musicName, bool isLoop = true, UnityAction startPlayCallBack = null)
        {
            if(_backgroundMusic == null)
            {
                GameObject bkMusicObj = new GameObject("BackgroundMusic/" + musicName);
                _backgroundMusic = bkMusicObj.AddComponent<AudioSource>();
                GameObject.DontDestroyOnLoad(bkMusicObj);
            }

            //加载音乐切片资源
            AssetBundleLoadManager.Instance.LoadAssetAsync<AudioClip>(E_AssetBundleType.Music, musicName, (clip) =>
            {
                _backgroundMusic.clip = clip;
                _backgroundMusic.loop = isLoop;
                _backgroundMusic.volume = GameDataMgr.Instance.MusicData.MusicValue;
                _backgroundMusic.mute = !GameDataMgr.Instance.MusicData.MusicIsOpen;
                _backgroundMusic.Play();
                startPlayCallBack?.Invoke();
            });
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBackgroundMusic()
        {
            if (_backgroundMusic == null)
            {
                LogMgr.LogError("背景音乐组件为Null");
                return;
            }

            _backgroundMusic.Pause();
            GameDataMgr.Instance.MusicData.MusicIsOpen = false;
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (_backgroundMusic == null)
            {
                LogMgr.LogError("背景音乐组件为Null");
                return;
            }
            _backgroundMusic.Stop();
            GameDataMgr.Instance.MusicData.MusicIsOpen = false;
        }

        /// <summary>
        /// 改变背景音乐音量
        /// </summary>
        /// <param name="value">音量值：0~1</param>
        public void ChangeBackgroundMusicVolume(float value)
        {
            GameDataMgr.Instance.MusicData.MusicValue = value;
            if (_backgroundMusic != null)
            {
                _backgroundMusic.volume = value;
            }
            else
            {
                LogMgr.LogError("背景音乐组件为Null");
            }
        }

        /// <summary>
        /// 创建音效
        /// </summary>
        /// <param name="soundName">音效名</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="createCallBack">完成回调</param>
        public void CreateSoundAsync(string soundName, bool isLoop = false, UnityAction<AudioSource> createCallBack = null)
        {
            //加载音乐剪辑资源
            AssetBundleLoadManager.Instance.LoadAssetAsync<AudioClip>(E_AssetBundleType.Music, soundName, (clip) =>
            {
                isClearSounds = false;
                AudioSource sound = PoolManager.Instance.GetObj<AudioSource>($"Sound_{soundName}");
                sound.clip = clip;
                sound.loop = isLoop;
                sound.volume = GameDataMgr.Instance.MusicData.SoundValue;
                sound.mute = !GameDataMgr.Instance.MusicData.SoundIsOpen;
                sound.Play();
                _sounds.Add(sound);
                createCallBack?.Invoke(sound);
            });
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void PlaySound()
        {
            for (int i = 0; i < _sounds.Count; i++)
            {
                _sounds[i].Play();
            }
            GameDataMgr.Instance.MusicData.SoundIsOpen = true;
        }

        /// <summary>
        /// 暂停音效
        /// </summary>
        public void PauseSound()
        {
            for (int i = 0; i < _sounds.Count; i++)
            {
                _sounds[i].Pause();
            }
            GameDataMgr.Instance.MusicData.SoundIsOpen = false;
        }

        /// <summary>
        /// 停止音效
        /// </summary>
        public void StopSound()
        {
            for (int i = 0; i < _sounds.Count; i++)
            {
                _sounds[i].Stop();
            }
            GameDataMgr.Instance.MusicData.SoundIsOpen = false;
        }

        /// <summary>
        /// 暂停指定循环音效
        /// </summary>
        /// <param name="sound">循环音效</param>
        /// <returns>是否暂停成功</returns>
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
        /// 停止指定循环音效
        /// </summary>
        /// <param name="sound">循环音效</param>
        /// <returns>是否停止成功</returns>
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
        /// 改变音效音量
        /// </summary>
        /// <param name="value">音量值：0~1</param>
        public void ChangeSoundVolume(float value)
        {
            GameDataMgr.Instance.MusicData.SoundValue = value;
            for (int i = 0; i < _sounds.Count; i++)
            {
                _sounds[i].volume = value;
            }
        }

        /// <summary>
        /// 清空所有音效
        /// </summary>
        public void Clear()
        {
            isClearSounds = true;
            for (int i = _sounds.Count - 1; i >= 0 ; i--)
            {
                //停止所有音效
                _sounds[i].Stop();
                //音乐切片置空
                _sounds[i].clip = null;
                //放入缓存池
                PoolManager.Instance.PushObj(_sounds[i].gameObject);
            }
            //清空列表
            _sounds.Clear();
        }
    }
}
