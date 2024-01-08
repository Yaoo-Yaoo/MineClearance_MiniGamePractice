using System;
using UnityEngine;

namespace MineSweeper
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioClips;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            EventManager.Instance.onPlayClickAudio.Register(OnPlayClickAudio);
            EventManager.Instance.onGameWin.Register(OnGameWin);
        }

        private void OnDestroy()
        {
            EventManager.Instance.onPlayClickAudio.UnRegister(OnPlayClickAudio);
            EventManager.Instance.onGameWin.UnRegister(OnGameWin);
        }

        private void OnPlayClickAudio()
        {
            Play(audioClips[0], 0.3f);
        }

        private void OnGameWin()
        {
            Play(audioClips[1], 0.1f);
        }

        private void Play(AudioClip clip, float startPercent)
        {
            audioSource.clip = clip;
            audioSource.time = clip.length * startPercent;
            audioSource.Play();
        }
    }
}
