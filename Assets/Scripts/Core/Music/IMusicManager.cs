using System.Threading.Tasks;
using UnityEngine;

namespace Core.Music
{
    /// <summary>
    /// ���ֹ������ӿ�
    /// </summary>
    public interface IMusicManager
    {
        void ChangeBackgroundMusicVolume(float value);
        void ChangeSoundVolume(float value);
        void Clear();
        Task<AudioSource> CreateSoundAsync(string soundName, bool isLoop = false);
        void PauseBackgroundMusic();
        bool PauseSelectSound(AudioSource sound);
        void PauseSound();
        Task PlayBackgroundMusic(string musicName, bool isLoop = true);
        void PlaySound();
        bool SelectStopSound(AudioSource sound);
        void StopBackgroundMusic();
        void StopSound();
    }
}
