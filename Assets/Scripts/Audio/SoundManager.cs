using UnityEngine;

public enum SoundType
{
    ARRASTA_OBJETO,
    TIROLESA,
    SOLD_EMPURRA,
    SOLD_PASSO,
    SOLD_TROCA,
    SOLD_PULO,
    SOLD_ATERRISSA

}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    public static SoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume); //talvez problema seja oneshot
    }
}
