using UnityEngine;

public class SoundSlider : MonoBehaviour
{
    public float maxVolume;

    private AudioSource audioSource;

    public void ChangeVolume(float setVolume)
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = maxVolume * setVolume;
        PlayerPrefs.SetFloat("Volume", setVolume);
    }
}
