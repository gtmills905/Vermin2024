using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Tutorial1 : MonoBehaviour
{
    public AudioSource gunShotSoundAudioSource;
    public AudioClip gunShotSoundClip;

    public void OnClick()
    {
        gunShotSoundAudioSource.PlayOneShot(gunShotSoundClip);
        StartCoroutine(LoadSceneWithDelay("Vermin Tutorial 2", 0.5f));
    }

    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}