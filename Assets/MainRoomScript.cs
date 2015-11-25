using UnityEngine;
using System.Collections;

public class MainRoomScript : MonoBehaviour
{
    private AudioSource audio;
	// Use this for initialization
	void Start ()
	{
	    audio = GetComponent<AudioSource>();
	    StartCoroutine("playaudio");
	}

    void playsound(string name)
    {
        string clipname = "kidsounds/" + name;
        audio.clip = Resources.Load<AudioClip>(clipname);
        audio.Play();
    }
    IEnumerator playaudio()
    {
        float delay = 1f;
        playsound("01-hello");
        yield return new WaitForSeconds(audio.clip.length + delay);
        playsound("02-choose");
        yield return new WaitForSeconds(audio.clip.length + delay + 5f);

        while (true)
        {
            playsound("03-touch");
            yield return new WaitForSeconds(audio.clip.length + delay + 8f);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
