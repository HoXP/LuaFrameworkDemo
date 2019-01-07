using System.Collections.Generic;
using UnityEngine;

namespace togetherzy.AssetBundle
{
    [RequireComponent(typeof(AudioListener))]
    public class SoundManager : MonoBehaviour
    {
        private static volatile SoundManager ms_Instance;
        public static SoundManager Instance
        {
            get
            {
                return ms_Instance;
            }
        }
        private void Awake()
        {
            ms_Instance = this;
            Init();
        }
        #region Public Fields
        public void PlayBGM(AudioClip clip)
        {
            PlayBGM(clip, true, 0);
        }
        public void PlayBGM(AudioClip clip, bool loop)
        {
            PlayBGM(clip, loop, 0);
        }
        public void PlayBGM(AudioClip clip, bool loop , float delay)
        {
            if (clip != null)
            {
                Play(_bgm, clip, loop, delay, _bgmVolume);
            }
        }
        public void PlaySound(AudioClip clip, int soundID)
        {
            PlaySound(clip, soundID, true, 0);
        }
        public void PlaySound(AudioClip clip, int soundID, bool loop)
        {
            PlaySound(clip, soundID, loop, 0);
        }
        public void PlaySound(AudioClip clip, int soundID, bool loop, float delay)
        {
            if(_audioSrcMap.ContainsKey(soundID) && _audioSrcMap[soundID] != null)
            {
                Play(_audioSrcMap[soundID], clip, loop, delay, _soundVolume);
            }
        }

        public int GetSoundID()
        {
            int _instanceID = 0;
            AudioSource _audioSrc = null;
            var _enumerator = _audioSrcMap.GetEnumerator();
            while (_enumerator.MoveNext())
            {
                var _cur = _enumerator.Current;
                if (!_cur.Value.isPlaying && _cur.Value.time == 0)
                {
                    _audioSrc = _cur.Value;
                    _audioSrc.clip = null;
                    break;
                }
            }

            if (null == _audioSrc)
            {
                _audioSrc = NewSoundPlayer();
                _instanceID = _audioSrc.GetInstanceID();
                _audioSrcMap.Add(_instanceID, _audioSrc);
            }
            else
            {
                _instanceID = _audioSrc.GetInstanceID();
            }
            return _instanceID;
        }
        public void MuteAll()
        {
            MuteAll(true, true);
        }
        public void MuteAll(bool mute)
        {
            MuteAll(mute, true);
        }

        public void MuteAll(bool mute ,bool includeBGM) 
        {
            var _enumerator = _audioSrcMap.GetEnumerator();
            while (_enumerator.MoveNext())
            {
                var _cur = _enumerator.Current;
                _cur.Value.mute = mute;
            }

            _bgm.mute = includeBGM ? mute : _bgm.mute;
        }

        public void MuteBGM()
        {
            MuteBGM(true);
        }
        public void MuteBGM(bool mute)
        {
            _bgm.mute = mute;
        }

        public void StopSound(int soundId)
        {
            if(_audioSrcMap.ContainsKey(soundId) && _audioSrcMap[soundId] != null)
            {
                _audioSrcMap[soundId].Stop();
            }
        }

        public void PauseSound(int soundId)
        {
            if (_audioSrcMap.ContainsKey(soundId) && _audioSrcMap[soundId] != null)
            {
                _audioSrcMap[soundId].Pause();
            }
        }
        public void UnPauseSound(int soundId)
        {
            if (_audioSrcMap.ContainsKey(soundId) && _audioSrcMap[soundId] != null)
            {
                _audioSrcMap[soundId].UnPause();
            }
        }
        public bool IsPlaying(int soundId)
        {
            if (_audioSrcMap.ContainsKey(soundId) && _audioSrcMap[soundId] != null)
            {
                return _audioSrcMap[soundId].isPlaying;
            }
            return false;
        }

        public void SetBGMVolume(float volume)
        {
            _bgmVolume = volume;
            _bgm.volume = _bgmVolume;
        }

        public void SetSoundVolume(float volume)
        {
            _soundVolume = volume;
            var _enumerator = _audioSrcMap.GetEnumerator();
            while (_enumerator.MoveNext())
            {
                var _cur = _enumerator.Current;
                _cur.Value.volume = _soundVolume;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeBGM"></param>
        public void Dispose(bool includeBGM)
        {
            var _enumerator = _audioSrcMap.GetEnumerator();
            while (_enumerator.MoveNext())
            {
                var _cur = _enumerator.Current;
                _cur.Value.clip = null;
            }

            _bgm.clip = includeBGM ? null : _bgm.clip;
        }

        public void DisposeBGM()
        {
            _bgm.clip = null;
        }
        #endregion

        #region private Fields
        void Init()
        {
            GameObject _soundRoot = GameObject.Find("SoundRoot");
            if (null == _soundRoot)
            {
                _soundRoot = new GameObject("SoundRoot");
                DontDestroyOnLoad(_soundRoot);
            }
            this._soundRoot = _soundRoot.transform;

            _audioSrcMap = new Dictionary<int, AudioSource>();
            _bgm = NewSoundPlayer();
        }

        private AudioSource NewSoundPlayer()
        {
            GameObject _soundPlayer = new GameObject("SoundPlayer");
            _soundPlayer.transform.SetParent(_soundRoot.transform);
            AudioSource _audioSouce = _soundPlayer.AddComponent<AudioSource>();
            return _audioSouce;
        }

        private void Play(AudioSource audioSource, AudioClip clip)
        {
            Play(audioSource, clip, true, 0, 1.0f);
        }

        private void Play(AudioSource audioSource, AudioClip clip, bool loop)
        {
            Play(audioSource, clip, loop, 0, 1.0f);
        }
        private void Play(AudioSource audioSource, AudioClip clip, bool loop , float delay )
        {
            Play(audioSource, clip, loop, delay, 1.0f);
        }
        private void Play(AudioSource audioSource, AudioClip clip, bool loop , float delay, float volume)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.volume = volume;
            audioSource.PlayDelayed(delay);
        }

        private Transform _soundRoot;
        private float _bgmVolume = 1.0f;
        private float _soundVolume = 1.0f;
        private AudioSource _bgm;
        private Dictionary<int, AudioSource> _audioSrcMap;
        #endregion
    }
}
