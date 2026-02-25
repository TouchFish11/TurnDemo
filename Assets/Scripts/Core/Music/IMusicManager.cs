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
        void ClearSound(string abName);
        Task<int> CreateSoundAsync(string soundName, float Volume, bool open, bool isLoop = false);
        void PauseBackgroundMusic();
        bool PauseSound(int audioId);
        void PauseSound();
        Task CreateBackgroundMusic(string abName, string musicName, float Volume, bool open, bool isLoop = true);
        void PlaySound();
        bool StopSound(int audioId);
        void StopBackgroundMusic();
        void StopSound();

        /// <summary>
        /// 播放当前背景音乐
        /// </summary>
        void PlayBackgroundMusic();
    }
}
