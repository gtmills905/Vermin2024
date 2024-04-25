using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartButtonScript : MonoBehaviour
{
    public AudioSource gunShotSoundAudioSource;
    public AudioClip gunShotSoundClip;


    public void OnClick()
    {

        StartCoroutine(PlaySoundAndLoadScene());

    }

    private IEnumerator PlaySoundAndLoadScene()
    {
        gunShotSoundAudioSource.PlayOneShot(gunShotSoundClip);
        yield return new WaitForSeconds(gunShotSoundClip.length); // Waits for the duration of the sound clip

        SceneManager.LoadScene("Clay Pigeon");
       
    }
}
