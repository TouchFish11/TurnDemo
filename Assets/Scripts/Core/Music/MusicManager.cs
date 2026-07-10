using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Mono;
using Core.Pool;
using Core.Utility;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.Music
{
    /// <summary>
    /// 音乐管理器
    /// 负责背景音乐和音效的加载、播放、暂停、停止、音量调节等核心逻辑
    /// 音效采用对象池管理，减少频繁创建销毁对象的性能开销
    /// </summary>
    public class MusicManager : IMusicManager
    {
        private readonly IPoolManager _poolManager;
        private readonly IMonoAdapter _monoAdapter;
        // 音频ID对应具体的音频源，缓存存活的音频源
        private readonly Dictionary<int, AudioSource> _sounds = new();
        // 资源键到句柄的映射缓存
        private readonly Dictionary<string, AssetHandle<AudioClip>> _audioHandles = new();
        // 待移除的音频源Id
        private readonly List<int> _soundIds = new();
        // 背景音乐播放器
        private AudioSource _backgroundMusic;
        // 音效总开关标记（控制所有音效是否可播放）
        private bool isOpenSounds;
        // 音频源id
        private static int audioId;

        /// <summary>
        /// 私有构造函数（单例模式）
        /// 注册帧更新监听，用于检测音效播放状态并回收无效音效对象
        /// </summary>
        private MusicManager(IMonoAdapter monoAdapter, IPoolManager poolManager)
        {
            monoAdapter.AddUpdateListener(OnUpdate);
            _monoAdapter = monoAdapter;
            _poolManager = poolManager;
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
                // 停止播放、清空音频片段、回收对象到池、释放/移除句柄
                audioSource.Stop();
                var key = audioSource.clip.name;
                var handle = _audioHandles[key];
                GameAsset.Release(handle);
                _audioHandles.Remove(key);
                audioSource.clip = null;
                _poolManager.PushObj(audioSource.gameObject);
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
        /// <param name="musicName">背景音乐名称（对应资源包内的音频文件名）</param>
        /// <param name="Volume">音量值（0~1）</param>
        /// <param name="open">是否开启播放（true=播放，false=静音）</param>
        /// <param name="delay">延迟播放时间，默认为0，不延迟</param>
        /// <param name="isLoop">是否循环播放（默认=true）</param>
        public async Task CreateBackgroundMusic(string musicName, float Volume, bool open, float delay = 0, bool isLoop = true)
        {
            // 初始化背景音乐播放器对象（首次播放时创建）
            if (!_backgroundMusic)
            {
                var bkMusicObj = new GameObject($"BackgroundMusic/{musicName}");
                _backgroundMusic = bkMusicObj.AddComponent<AudioSource>();
                // 标记为跨场景不销毁
                Object.DontDestroyOnLoad(bkMusicObj);
            }

            // 若重复播放相同的背景音乐，则重新开始播放该音乐
            if (_backgroundMusic.clip && _backgroundMusic.clip.name == musicName)
            {
                _backgroundMusic.Play();
                return;
            }
            
            // 释放上次播放的音乐文件资源
            GameAsset.Release(_audioHandles[_backgroundMusic.clip.name]);
            
            // 从资源包异步加载背景音乐资源
            var handle = await GameAsset.LoadAssetAsync<AudioClip>(musicName);
            // 缓存资源句柄
            _audioHandles.Add(musicName, handle);
            // 配置背景音乐播放器参数
            _backgroundMusic.clip = handle.Asset;
            _backgroundMusic.loop = isLoop;
            _backgroundMusic.volume = Volume;
            _backgroundMusic.mute = !open;

            // 延迟开始播放
            if (delay != 0)
            {
                _backgroundMusic.PlayDelayed(delay);
            }
            // 立即播放
            else
            {
                _backgroundMusic.Play();
            }
        }

        /// <summary>
        /// 暂停当前背景音乐，若开启变化，则内部通过开启一个协程来处理逻辑
        /// </summary>
        /// <param name="fade">是否逐渐停止</param>
        /// <param name="fadeRate">若是逐渐停止，则指定变化速率，最终速度为Time.deltaTime * fadeRate的乘积；若为负数，则取绝对值</param>
        public void PauseBackgroundMusic(bool fade = false, float fadeRate = 0)
        {
            if (!_backgroundMusic)
            {
                Logger.LogError(TODO, $"{nameof(MusicManager)}:The background music player is Null and cannot perform the pause operation.");
                return;
            }

            if(fade)
                _monoAdapter.StartCoroutine(FadePause_Cor());
            else
                _backgroundMusic.Pause();
            return;

            IEnumerator FadePause_Cor()
            {
                // 记录上次的播放剪辑，避免切换后影响当前播放的音乐音量
                var lastClip = _backgroundMusic.clip;
                while (_backgroundMusic && _backgroundMusic.clip && _backgroundMusic.volume > 0)
                {
                    // 说明暂停时切换了背景音乐，则停止减少音量
                    if(_backgroundMusic.clip != lastClip)
                        yield break;
                    
                    _backgroundMusic.volume -= TimeUtil.DeltaTime * Mathf.Abs(fadeRate);
                    yield return null;
                }
                _backgroundMusic?.Pause();
            }
        }

        /// <summary>
        /// 停止当前背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (!_backgroundMusic)
            {
                Logger.LogError(TODO, $"{nameof(MusicManager)}:The background music player is Null and cannot perform the pause operation.");
                return;
            }
            _backgroundMusic.Stop();
        }

        /// <summary>
        /// 重新播放当前背景音乐
        /// </summary>
        public void PlayBackgroundMusic()
        {
            if (!_backgroundMusic)
            {
                Logger.LogError(TODO, $"{nameof(MusicManager)}:The background music player is Null and cannot perform the pause operation.");
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
            if (!_backgroundMusic)
            {
                Logger.LogError(TODO, $"{nameof(MusicManager)}:The background music player is Null and cannot perform the pause operation.");
                return;
            }
            _backgroundMusic.volume = value;
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
            if (!_audioHandles.TryGetValue(soundName, out var handle))
            {
                // 异步加载音效资源
                handle = await GameAsset.LoadAssetAsync<AudioClip>(soundName);
                // 缓存资源句柄
                _audioHandles.Add(soundName, handle);
            }
            
            // 从对象池获取音效播放器
            var sound = _poolManager.Get<AudioSource>(soundName);
            // 没有就创建
            if (!sound)
            {
                sound = new GameObject().GetComponent<AudioSource>();
            }
            
            // 配置音效播放器参数
            sound.clip = handle.Asset;
            sound.loop = isLoop;
            sound.volume = Volume;
            sound.mute = !open;
            // 添加到音效列表管理
            _sounds.Add(++audioId, sound);
            // 开始播放
            sound.Play();
            return audioId;
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
        /// 播放指定音效
        /// </summary>
        /// <param name="audioId"></param>
        public void PlaySound(int audioId)
        {
            if (_sounds.TryGetValue(audioId, out var sound))
            {
                sound.Play();
            }
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
            
            if (sound.clip && sound.loop)
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
        public void ClearSounds()
        {
            foreach (var source in _sounds.Values)
            {
                // 停止音效播放
                source.Stop();
                // 释放上次播放的音乐文件资源
                var key = source.clip.name;
                var handle = _audioHandles[key];
                GameAsset.Release(handle);
                _audioHandles.Remove(key);
                // 清空音频片段引用
                source.clip = null;
                // 将音效对象回收至对象池
                _poolManager.PushObj(source.gameObject);
            }
            
            // 清空列表
            _sounds.Clear();
        }
    }
}