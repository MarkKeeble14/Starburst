using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TemporaryAudioSource : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    public void Play(AudioClip clip, float volume, float pitch)
    {
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        StartCoroutine(DestroyWhenDone());
    }

    private IEnumerator DestroyWhenDone()
    {
        yield return new WaitUntil(() => !source.isPlaying);

        Destroy(gameObject);
    }
}
