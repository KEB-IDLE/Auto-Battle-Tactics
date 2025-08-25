using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private float volume = 0.3f;

    private AudioSource src;

    private void Awake()
    {
        src = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        src.playOnAwake = true;
        src.loop = true;
        src.spatialBlend = 0f; // 2D
        src.volume = volume;
        src.clip = clip;

        if (clip != null) src.Play();
    }

    private void OnDestroy()
    {
        if (src != null && src.isPlaying) src.Stop();
    }
}
