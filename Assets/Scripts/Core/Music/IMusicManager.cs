using System.Threading.Tasks;

namespace Core.Music
{
    /// <summary>
    /// ���ֹ������ӿ�
    /// </summary>
    public interface IMusicManager
    {
        void ChangeBackgroundMusicVolume(float value);
        void ChangeSoundVolume(float value);
        void ClearSounds();
        Task<int> CreateSoundAsync(string soundName, float Volume, bool open, bool isLoop = false);
        void PauseBackgroundMusic(bool fade, float fadeRate = 0F);
        bool PauseSound(int audioId);
        void PauseSound();
        Task CreateBackgroundMusic(string musicName, float Volume, bool open, float delay, bool isLoop = true);
        void PlaySound();
        bool StopSound(int audioId);
        void StopBackgroundMusic();
        void StopSound();

        /// <summary>
        /// 播放当前背景音乐
        /// </summary>
        void PlayBackgroundMusic();

        /// <summary>
        /// 播放指定音效
        /// </summary>
        /// <param name="audioId"></param>
        void PlaySound(int audioId);
    }
}
