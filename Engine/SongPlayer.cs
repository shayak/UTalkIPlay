using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SpeechRecognition
{
    public class SongPlayer
    {
        WMPLib.WindowsMediaPlayer WPlayer;

        public SongPlayer()
        {
            WPlayer = new WMPLib.WindowsMediaPlayer();
        }
        public void PlaySong(DataAccess.SongRepo repo, string title)
        {
            PlayFile(repo.GetSongPathByName(title));
        }

        public void PlayFile(string path)
        {
            if (path.EndsWith("mp3"))
                PlayMp3(path);
            else if (path.EndsWith("wav"))
                PlayWav(path);            
        }

        public void Stop()
        {
            WPlayer.controls.stop();
        }

        public void Pause()
        {
            WPlayer.controls.pause();
        }

        public void Play()
        {
            WPlayer.controls.play();
        }

        public void VolumeUp()
        {
            WPlayer.settings.volume = (WPlayer.settings.volume + 10) > 100 ? 100 : WPlayer.settings.volume + 10;
        }

        public void VolumeDown()
        {
            WPlayer.settings.volume = (WPlayer.settings.volume - 10) < 0 ? 0 : WPlayer.settings.volume - 10;
        }

        public void PlayMp3(string path)
        {            
            WPlayer.URL = path;
            WPlayer.controls.play();
        }

        public void PlayWav(string path)
        {
            // NOT TESTED
            //TODO: figure this shit out
            using (System.Media.SoundPlayer player = new System.Media.SoundPlayer())
            {
                player.SoundLocation = path;
                player.Play();
            }
        }
    }
}
