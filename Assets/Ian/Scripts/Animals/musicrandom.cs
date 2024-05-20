using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicrandom : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip[] Clips;
    // Start is called before the first frame update
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        StartCoroutine("Play");
    }

    // Update is called once per frame
    void Update()
    {

       
    }
    private IEnumerator Play()
    {
        
        int randomIndex = Random.Range(0, Clips.Length);

        // Get the audio clip at the randomly selected index
        AudioClip randomClip = Clips[randomIndex];

        // Play the randomly selected audio clip
        AudioSource.clip = randomClip;
        AudioSource.Play();
        StartCoroutine("Play");
        yield return new WaitForSeconds(60);
    }
}
