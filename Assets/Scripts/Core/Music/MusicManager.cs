using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Loader.Audio;
using Core.Log;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.Singleton;
using UnityEngine;

namespace Core.Music
{
    /// <summary>
    /// 音乐管理器
    /// 负责背景音乐和音效的加载、播放、暂停、停止、音量调节等核心逻辑
    /// 音效采用对象池管理，减少频繁创建销毁对象的性能开销
    /// </summary>
    public class MusicManager : SingletonBase<MusicManager>, IMusicManager
    {
        public override int Priority => -1;

        // 音效播放器列表
        private readonly Dictionary<int, AudioSource> _sounds = new();
        // 待移除的音频源Id
        private readonly List<int> _soundIds = new();
        // 背景音乐播放器
        private AudioSource _backgroundMusic;
        // 音效总开关标记（控制所有音效是否可播放）
        private bool isOpenSounds;
        // 音频源id
        private int auidoId;
        private int priority;

        /// <summary>
        /// 私有构造函数（单例模式）
        /// 注册帧更新监听，用于检测音效播放状态并回收无效音效对象
        /// </summary>
        private MusicManager(){}

        public override Task InitAsync()
        {
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 帧更新回调
        /// 核心逻辑：检测并回收已停止播放的音效对象，放回对象池
        /// </summary>
        private void OnUpdate()
        {
            // 音效总开关关闭时，直接返回不处理
            if (!isOpenSounds)
            {
                return;
            }

            foreach (var id in _sounds.Keys)
            {
                if (_sounds[id].isPlaying)
                {
                    continue;
                }
                
                // 检测到未播放的音效
                var audioSource = _sounds[id];
                // 停止播放、清空音频片段、回收对象到池
                audioSource.Stop();
                audioSource.clip = null;
                ServiceLocator.Get<IPoolManager>().PushObj(audioSource.gameObject);
                _soundIds.Add(id);
            }
            
            for (var i = 0; i < _soundIds.Count; i++)
            {
                _sounds.Remove(_soundIds[i]);
            }
            _soundIds.Clear();
        }

        /// <summary>
        /// 创建新背景音乐
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="musicName">背景音乐名称（对应资源包内的音频文件名）</param>
        /// <param name="Volume">音量值（0~1）</param>
        /// <param name="open">是否开启播放（true=播放，false=静音）</param>
        /// <param name="isLoop">是否循环播放（默认=true）</param>
        public async Task CreateBackgroundMusic(string abName, string musicName, float Volume, bool open, bool isLoop = true)
        {
            // 初始化背景音乐播放器对象（首次播放时创建）
            if (_backgroundMusic == null)
            {
                var bkMusicObj = new GameObject($"BackgroundMusic/{musicName}");
                _backgroundMusic = bkMusicObj.AddComponent<AudioSource>();
                // 标记为跨场景不销毁
                Object.DontDestroyOnLoad(bkMusicObj);
            }

            // 释放上次播放的音乐文件资源
            ServiceLocator.Get<IAudioLoader>().UnloadClip(abName, _backgroundMusic.clip.name);
            // 从资源包异步加载背景音乐资源
            var audioClip = await ServiceLocator.Get<IAudioLoader>().LoadAudioClipAsync(musicName);
            // 配置背景音乐播放器参数
            _backgroundMusic.clip = audioClip;
            _backgroundMusic.loop = isLoop;
            _backgroundMusic.volume = Volume;
            _backgroundMusic.mute = !open;
            // 开始播放
            _backgroundMusic.Play();
        }

        /// <summary>
        /// 暂停当前背景音乐
        /// </summary>
        public void PauseBackgroundMusic()
        {
            if (_backgroundMusic == null)
            {
                LogManager.LogError("背景音乐播放器为Null，无法执行暂停操作");
                return;
            }

            _backgroundMusic.Pause();
        }

        /// <summary>
        /// 停止当前背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (_backgroundMusic == null)
            {
                LogManager.LogError("背景音乐播放器为Null，无法执行停止操作");
                return;
            }
            _backgroundMusic.Stop();
        }

        /// <summary>
        /// 播放当前背景音乐
        /// </summary>
        public void PlayBackgroundMusic()
        {
            if (_backgroundMusic == null)
            {
                LogManager.LogError("背景音乐播放器为Null，无法执行停止操作");
                return;
            }
            _backgroundMusic.Play();
        }

        /// <summary>
        /// 修改背景音乐音量
        /// </summary>
        /// <param name="value">音量值（0~1）</param>
        public void ChangeBackgroundMusicVolume(float value)
        {
            if (_backgroundMusic != null)
            {
                _backgroundMusic.volume = value;
            }
            else
            {
                LogManager.LogError("背景音乐播放器为Null，无法修改音量");
            }
        }

        /// <summary>
        /// 创建并播放音效
        /// </summary>
        /// <param name="soundName">音效名称（对应资源包内的音频文件名）</param>
        /// <param name="Volume">音量值（0~1）</param>
        /// <param name="open">是否开启播放（true=播放，false=静音）</param>
        /// <param name="isLoop">是否循环播放（默认=false）</param>
        /// <returns>创建的音效播放器对象</returns>
        public async Task<int> CreateSoundAsync(string soundName, float Volume, bool open, bool isLoop = false)
        {
            // 从资源包异步加载音效资源
            var audioClip = await ServiceLocator.Get<IAudioLoader>().LoadAudioClipAsync(soundName);
            // 从对象池获取音效播放器
            var sound = ServiceLocator.Get<IPoolManager>().GetObj<AudioSource>($"Sound_{soundName}");
            // 配置音效播放器参数
            sound.clip = audioClip;
            sound.loop = isLoop;
            sound.volume = Volume;
            sound.mute = !open;
            // 添加到音效列表管理
            _sounds.Add(++auidoId, sound);
            // 开始播放
            sound.Play();
            return auidoId;
        }

        /// <summary>
        /// 播放所有音效（恢复暂停的音效）
        /// 同时开启音效总开关
        /// </summary>
        public void PlaySound()
        {
            foreach (var sounds in _sounds.Values)
            {
                sounds.Play();
            }
            isOpenSounds = true;
        }

        /// <summary>
        /// 暂停所有音效
        /// 同时关闭音效总开关
        /// </summary>
        public void PauseSound()
        {
            foreach (var sounds in _sounds.Values)
            {
                sounds.Pause();
            }
            isOpenSounds = false;
        }

        /// <summary>
        /// 停止所有音效
        /// 同时关闭音效总开关
        /// </summary>
        public void StopSound()
        {
            foreach (var sounds in _sounds.Values)
            {
                sounds.Stop();
            }
            isOpenSounds = false;
        }

        /// <summary>
        /// 暂停指定的循环音效
        /// 非循环音效不处理
        /// </summary>
        /// <param name="audioId">音效ID</param>
        /// <returns>是否暂停成功（true=成功，false=失败）</returns>
        public bool PauseSound(int audioId)
        {
            // 仅处理有音频片段且开启循环的音效
            if (!_sounds.TryGetValue(audioId, out var sound))
            {
                return false;
            }
            
            if (sound.clip != null && sound.loop)
            {
                sound.Pause();
            }
            return true;
        }

        /// <summary>
        /// 停止指定的循环音效
        /// 非循环音效不处理
        /// </summary>
        /// <param name="audioId">音效ID</param>
        /// <returns>是否停止成功（true=成功，false=失败）</returns>
        public bool StopSound(int audioId)
        {
            // 仅处理有音频片段且开启循环的音效
            if (!_sounds.TryGetValue(audioId, out var sound))
            {
                return false;
            }
            
            if (sound.clip != null && sound.loop)
            {
                sound.Stop();
            }
            return true;
        }

        /// <summary>
        /// 修改所有音效的音量
        /// </summary>
        /// <param name="value">音量值（0~1）</param>
        public void ChangeSoundVolume(float value)
        {
            foreach (var sound in _sounds.Values)
            {
                sound.volume = value;
            }
        }

        /// <summary>
        /// 清空所有音效
        /// 停止播放、清空音频片段、回收对象到池、清空音效列表
        /// </summary>
        /// <param name="abName"></param>
        public void ClearSound(string abName)
        {
            foreach (var source in _sounds.Values)
            {
                // 停止音效播放
                source.Stop();
                // 释放上次播放的音乐文件资源
                ServiceLocator.Get<IAudioLoader>().UnloadClip(abName, source.clip.name);
                // 清空音频片段引用
                source.clip = null;
                // 将音效对象回收至对象池
                ServiceLocator.Get<IPoolManager>().PushObj(source.gameObject);
            }
            
            // 清空列表
            _sounds.Clear();
        }
    }
}