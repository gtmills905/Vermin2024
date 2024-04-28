using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartButtonScript : MonoBehaviour
{
    public AudioSource gunShotSoundAudioSource;
    public AudioClip gunShotSoundClip;

    public void OnClick()
    {
        gunShotSoundAudioSource.PlayOneShot(gunShotSoundClip);
        SceneManager.LoadScene("Vermin");

    }
}
