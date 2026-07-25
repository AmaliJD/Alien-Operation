using EX;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource musicSource;
    bool muted;

    public static AudioPlayer ap;

    private void Awake()
    {
        if (ap != null)
        {
            Destroy(this);
        }
        else
        {
            ap = this;
        }

        musicSource.mute = true;
        musicSource.Play();
    }

    private void Start()
    {
        musicSource.Stop();
    }

    public void PlaySfx(AudioClip audioClip, float volume)
    {
        AudioSource audioSource = Instantiate(sfxSource);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void SetMute(bool m)
    {
        muted = m;

        if (muted)
        {
            audioMixer.SetFloat("Master", -80);
        }
        else
        {
            audioMixer.SetFloat("Master", Mathf.Log10(1));
        }
    }

    public void SetMasterVolume(float value)
    {
        float logValue = Mathf.Log10(MathEX.Remap(0, 100, .0001f, 1, value)) * 20;
        audioMixer.SetFloat("MasterVolume", logValue);
    }

    public void SetSfxVolume(float value)
    {
        float logValue = Mathf.Log10(MathEX.Remap(0, 100, .0001f, 1, value)) * 20;
        audioMixer.SetFloat("SfxVolume", logValue);
    }

    public void SetMusicVolume(float value)
    {
        float logValue = Mathf.Log10(MathEX.Remap(0, 100, .0001f, 1, value)) * 20;
        audioMixer.SetFloat("MusicVolume", logValue);
    }
}
