using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarbleManager : MonoBehaviour {

  public int currentLetter = 0;
  public List<LetterMaker> letters;
  public GameObject finger;
  public List<AudioClip> clips;
  public AudioSource audioSrc;
	// Use this for initialization
	void Start () {
    q = new Queue<AudioClip>();
	  audioSrc = GetComponent<AudioSource>();
    LetterStart();
	}

  public Queue<AudioClip> q;
  private bool isPlaying;
  public void QueueClip(int i) {
    q.Enqueue(clips[i]);
    if (!isPlaying) ReadQueue();
  }

  private void ReadQueue() {
    if (q.Count <= 0) {
      isPlaying = false;
      return;
    }
    audioSrc.clip = q.Dequeue();
    audioSrc.Play();
    isPlaying = true;
    Invoke("ReadQueue", audioSrc.clip.length);

  }


  void LetterStart() {
    letters[currentLetter].gameObject.SetActive(false);
    QueueClip(0);
    //QueueClip(1);
    Invoke("LetterStart2", 2.5f);


  }
  void LetterStart2()
  {
    letters[currentLetter].gameObject.SetActive(true);

    finger.SetActive(true);
    letters[currentLetter].AutoComplete();
    letters[currentLetter].OnComplete += OnComplete1;

  }

  public void WaitForCompletion() {
    QueueClip(3);

  }

  private void OnComplete1() {

    finger.SetActive(false);
    QueueClip(2);
    Invoke("WaitForCompletion", 10);

    letters[currentLetter].Reset();
      letters[currentLetter].OnComplete += OnComplete2;
    
  }

  private void OnComplete2()
  {
    QueueClip(6);
    QueueClip(4);
    QueueClip(currentLetter == 0 ? 7 : 8); 
    QueueClip(5);

    QueueClip(currentLetter == 0 ? 7 : 8);


    CancelInvoke("WaitForCompletion");
    letters[currentLetter].Reset();
    letters[currentLetter].OnComplete += OnComplete3;
    letters[currentLetter].PointsOnly = true;
  }

  private void OnComplete3()
  {
    QueueClip(6);
    QueueClip(4);

    QueueClip(currentLetter == 0 ? 7 : 8);


    Invoke("OnDone", audioSrc.clip.length);


  }

  private void OnDone() {
    letters[currentLetter].gameObject.SetActive(false);
    currentLetter++;
    if (currentLetter >= letters.Count)
    {
      Application.LoadLevel(0);
      return;
    }
    letters[currentLetter].gameObject.SetActive(true);
    LetterStart();
  }
  // Update is called once per frame
  void Update () {
	  if(currentLetter<letters.Count)
      finger.transform.position = letters[currentLetter].marble.transform.position;
   
  }
}
