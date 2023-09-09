using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _Instance { get; private set; }

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            _Instance = this;
        }
    }

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource alienClipSource;

    [SerializeField] private List<AudioClip> alienClipQueue = new List<AudioClip>();
    [SerializeField] private float alienClipDelayTimer;
    [SerializeField] private Vector2 minMaxAlienDelayStart;

    [SerializeField] private TemporaryAudioSource tempAudioSource;

    public void PlayOneShot(AudioClip clip, float pitch, Vector3 pos)
    {
        Instantiate(tempAudioSource, pos, Quaternion.identity).Play(clip, 1, pitch);
    }

    public void PlayOneShotFromAlienClipChannel(AudioClip clip)
    {
        alienClipQueue.Add(clip);
    }

    private void Update()
    {
        if (alienClipQueue.Count > 0)
        {
            if (alienClipSource.isPlaying) return;
            if (alienClipDelayTimer > 0) return;

            alienClipSource.pitch = Random.Range(0.8f, 1.2f);
            alienClipSource.PlayOneShot(alienClipQueue[0]);
            alienClipQueue.RemoveAt(0);
            alienClipDelayTimer = Random.Range(minMaxAlienDelayStart.x, minMaxAlienDelayStart.y);
        }

        alienClipDelayTimer -= Time.deltaTime;
    }
}
