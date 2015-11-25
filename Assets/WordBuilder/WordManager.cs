using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class LevelInfo
{
    public string word, mask;
    public int extraLetters;

    public LevelInfo(string word, string mask, int extraLetters)
    {
        this.word = word;
        this.mask = mask;
        this.extraLetters = extraLetters;
    }
}
public enum AudioNames
{
    hello,
    choose,
    touch,
    letsTrace,
    letsbuildwords,
    WatchMe,
    TheMarbleMakes,
    NowYouTry,
    PressThis,
    TouchTheMarble,
    YouTraced,
    NowLetsTrace,
    MoveTheMarble,
    WatchMeBuild,
    ThisIsTheWord,
    DragTheLetter,
    DragTheMissing,
    ThisIsAPicture,
    DragTheLettersToSpell,
    BuildTheWordBy,
    FinishTheWord,
    YouCompletedTheWord,
}
public class WordManager : MonoBehaviour
{
    private static WordManager _instance;
    public static WordManager instance { get { return _instance; } }
    public static List<LevelInfo> words = new List<LevelInfo>()
    {
        new LevelInfo("DOG","101",3),
        new LevelInfo("CAR","101",3),
        new LevelInfo("TREE","1011",4),
        new LevelInfo("APPLE","11101",5),
        new LevelInfo("SMILE","00000",2),
    };
    public GameObject pictureSprite;
    public Sprite[] pictures = new Sprite[5];
    public GameObject finger;
    private AudioNames audioName = AudioNames.letsbuildwords;
    
    public static string GetFileName(AudioNames audioname)
    {
        int num = (int) audioname + 1;
        if (num >= 6) num++;
        return string.Format("{0:00}-", num) + audioname.ToString();
    }

    public void PlaySound(AudioNames name)
    {
        string filename = GetFileName(name);
        string clipname = "kidsounds/" + filename;
        audio.clip = Resources.Load<AudioClip>(clipname);
        audio.Play();
    }

    public void PlayWord()
    {
        string clipname = "kidsounds/words/" + currentWord.word.ToLower();
        audio.clip = Resources.Load<AudioClip>(clipname);
        audio.Play();
    }

    public static LevelInfo currentWord;
    public static int currentLevel = 0;
    //public string word = "DOG";
    //public string mask = "101";
    //public int extraLetters = 6;
    private string tempMask = "";
    public GameObject letterPrefab;
    
    private float? start = null, size = null;
    private float padding = 0.1f;
    private float topY = 3f, botY = -3f;

    public AudioSource audio;
    public bool isInteractive = false;
    void Start()
    {
        //isInteractive = true;
        _instance = this;
        audio = GetComponent<AudioSource>();
        if (currentLevel >= words.Count)
        {
            currentLevel = 0;
            Application.LoadLevel("cps613");
            return;
        }
        currentWord = words[currentLevel];
        pictureSprite.GetComponent<SpriteRenderer>().sprite = pictures[currentLevel];
        tempMask = currentWord.mask;
        if (currentWord.word.Length < 1) return;
        List<char> leftovers = new List<char>();
        for (int i = 0; i < currentWord.word.Length; i++)
        {
            char c = char.ToUpper(currentWord.word[i]);

            LetterScript letterScript = Instantiate(letterPrefab).GetComponent<LetterScript>();
            letterScript.isStatic = true;
            letterScript.letter = c;
            letterScript.ChangeSprite();
            if (start == null)
            {
                size = letterScript.gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
                float totalLen = (size.Value + padding) * currentWord.word.Length;
                start = -totalLen / 2f + size.Value;
            }
            letterScript.transform.position = new Vector3(start.Value + i * (size.Value + padding), topY, 0);

            bool show = currentWord.mask[i] == '1';
            if (!show)
            {
                leftovers.Add(c);
                var sr = letterScript.GetComponent<SpriteRenderer>();
                sr.color = new Color(0, 0, 0, 0.5f);
                sr.sortingOrder = 1;
                if (currentLevel == 0)
                {
                    fingerDest2 = letterScript.transform.position;
                }
            }
            else
            {
                var sr = letterScript.GetComponent<SpriteRenderer>();
                sr.sortingOrder = 2;
            }
        }
        List<char> deadChars = new List<char>();
        for (int i = 0; i < currentWord.extraLetters; i++)
        {
            char c = 'A';
            do
            {
                c = (char)('A' + Random.Range(0, 26));
            } while (leftovers.Contains(c) || deadChars.Contains(c)); //if leftovers contains every letter, we'll wait for infinity
            deadChars.Add(c);
        }

        foreach (char c in leftovers)
        {
            int index = Random.Range(0, deadChars.Count);
            deadChars.Insert(index, c);
        }

        float start2 = -((size.Value + padding) * deadChars.Count) / 2f + size.Value;
        for (int i = 0; i < deadChars.Count; i++)
        {
            LetterScript letterScript = Instantiate(letterPrefab).GetComponent<LetterScript>();
            letterScript.isStatic = false;
            letterScript.letter = deadChars[i];
            letterScript.ChangeSprite();
            letterScript.transform.position = new Vector3(start2 + i * (size.Value + padding), botY, 0);
            var sr = letterScript.GetComponent<SpriteRenderer>();
            sr.sortingOrder = 2;
            remainingLetters.Add(letterScript);
            
        }
        if (currentLevel == 0)
        {
            StartCoroutine("AudioPlayer");
        }
        else
        {
            PlayWord();
            isInteractive = true;
        }
    }

    public bool IsFull(int index)
    {
        return instance.tempMask[index] == '1';
    }

    public bool IsCorrect(int index, char c)
    {
        return char.ToUpper(currentWord.word[index]) == char.ToUpper(c);
    }

    private bool wordComplete = false;
    public void CheckWordComplete()
    {
        if (wordComplete) return;
        string s = new string('1', instance.tempMask.Length);
        wordComplete = instance.tempMask.Equals(s);
        if (wordComplete)
        {
            ShowGoodJob();
            instance.winTime = Time.time;
            currentLevel++;
            StartCoroutine("completeAudio");
        }
    }

    private List<LetterScript> goodjobs = new List<LetterScript>();
    private float winTime = 0f;
    private void ShowGoodJob()
    {
        string goodjob = "GOOD JOB";
        float sizeTemp = size.Value * 0.8f;
        float start2 = -(goodjob.Length * (sizeTemp + padding)) / 2f + sizeTemp / 2f;
        
        for (int i = 0; i < goodjob.Length; i++)
        {
            char c = goodjob[i];
            if (c == ' ') continue;
            LetterScript letterScript = Instantiate(instance.letterPrefab).GetComponent<LetterScript>();
            letterScript.isStatic = false;
            letterScript.letter = c;
            letterScript.ChangeSprite();
            letterScript.transform.position = new Vector3(start2 + i * (sizeTemp + padding), 1f, 0);
            letterScript.transform.localScale = Vector3.zero;
            var sr = letterScript.GetComponent<SpriteRenderer>();
            sr.sortingOrder = 3;
            instance.goodjobs.Add(letterScript);
        }
    }
    
    IEnumerator AudioPlayer()
    {
        float delay = 0.5f;
        PlaySound(AudioNames.letsbuildwords);
        yield return new WaitForSeconds(audio.clip.length + delay);
        PlaySound(AudioNames.WatchMeBuild);
        yield return new WaitForSeconds(audio.clip.length + delay);
        PlaySound(AudioNames.ThisIsTheWord);
        yield return new WaitForSeconds(audio.clip.length);
        PlayWord();
        yield return new WaitForSeconds(audio.clip.length + delay);
        PlaySound(AudioNames.WatchMeBuild);
        yield return new WaitForSeconds(0.75f);
        audio.Stop();
        finger.GetComponentInChildren<SpriteRenderer>().enabled = true;
        GameObject fingerLetterTemp = null;
        Vector3 prevLetterPos = Vector3.zero;
        
        for (int i = 0; i < remainingLetters.Count; i++)
        {
            LetterScript ls = remainingLetters[i];
            float s = ls.gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            if (char.ToLower(ls.letter) == 'o')
            {
                fingerDest = ls.gameObject.transform.position;

                prevLetterPos = ls.gameObject.transform.position;// - new Vector3(s,s,0f)/2f;
                fingerLetterTemp = ls.gameObject;
            }
        }
        yield return StartCoroutine("moveFinger");
        fingerDest = fingerDest2;
        fingerLetter = fingerLetterTemp;
        fingerLetterDest = fingerDest2;
        yield return StartCoroutine("moveFinger");
        yield return new WaitForSeconds(1.5f);
        fingerDest = new Vector3(0,-6,0);
        fingerLetter = fingerLetterTemp;
        fingerLetterDest = prevLetterPos;
        yield return StartCoroutine("moveFinger");
        isInteractive = true;
        PlaySound(AudioNames.NowYouTry);
        yield return new WaitForSeconds(1.2f);
        audio.Stop();
        yield return new WaitForSeconds(1f);
        PlaySound(AudioNames.DragTheLetter);
        yield return new WaitForSeconds(audio.clip.length + delay);
    }

    IEnumerator completeAudio()
    {
        float delay = 0.5f;
        PlaySound(AudioNames.YouCompletedTheWord);
        yield return new WaitForSeconds(audio.clip.length + delay);
        PlayWord();
        yield return new WaitForSeconds(audio.clip.length + delay);

    }
    Vector3 fingerDest = Vector3.zero, fingerDest2 = Vector3.zero;
    private Vector3 fingerLetterDest = Vector3.zero;
    private GameObject fingerLetter;
    IEnumerator moveFinger()
    {
        float speed = 0.05f, delay = 0.01f;
        while (finger.transform.position != fingerDest)
        {
            finger.transform.position = Vector3.MoveTowards(finger.transform.position, fingerDest, speed);
            if (fingerLetter != null)
            {
                fingerLetter.transform.position = Vector3.MoveTowards(fingerLetter.transform.position, fingerLetterDest, speed);
            }
            yield return new WaitForSeconds(delay);
        }

    }

    public List<LetterScript> remainingLetters = new List<LetterScript>();
    void Update()
    {
        if (!isInteractive)
        {
            return;
        }
        if (wordComplete)
        {
            float t = (Time.time - winTime) * 3f;
            bool phaseComplete = false;
            for (int i = 0; i < goodjobs.Count; i++)
            {
                LetterScript ls = goodjobs[i];
                float r = Math.Max(0f, t - i * 0.3f);

                float amp = 0.8f, yshift = 0f;
                float smallAmp = 0.1f;
                if (r > Math.PI / 2f)
                {
                    if (i == goodjobs.Count - 1)
                    {
                        phaseComplete = true;
                    }
                    yshift = amp - smallAmp;
                    amp = smallAmp;
                    
                    //r *= 3f;
                }
                float wave = Math.Abs(amp * Mathf.Sin(r) + yshift);
                ls.gameObject.transform.localScale = new Vector3(wave, wave, 1f);
            }
            if (phaseComplete)
            {
                if (nextButton == null)
                {
                    bool dead = false;
                    for (int i = 0; i < remainingLetters.Count; i++)
                    {
                        var ls = remainingLetters[i];
                        float sc = 0.01f;
                        ls.transform.localScale -= new Vector3(sc, sc, 0f);
                        if (ls.transform.localScale.x <= 0)
                        {
                            Destroy(ls.gameObject);
                            dead = true;
                        }
                    }
                    if (dead)
                    {
                        remainingLetters = new List<LetterScript>();
                        SpawnNextButton();
                    }
                }
                else
                {
                    if (nextButton.transform.localScale.x < 1f)
                    {
                        float sc = 0.01f;
                        nextButton.transform.localScale += new Vector3(sc, sc, 0f);
                        if (nextButton.transform.localScale.x >= 1f)
                        {
                            nextButton.transform.localScale = new Vector3(1f, 1f, 1f);
                        }
                        buttonLayer.transform.localScale = nextButton.transform.localScale;
                    }
                    var sr = buttonLayer.GetComponent<SpriteRenderer>();
                    float amp = 0.2f, freq = 2f;
                    float alpha = amp * Mathf.Sin(t * freq) + amp / 2f;
                    Color c = sr.color;
                    c.a = alpha;
                    sr.color = c;

                }
            }
        }
    }

    public GameObject buttonPrefab;
    private GameObject nextButton, buttonLayer;
    private void SpawnNextButton()
    {
        var buttonContainer = Instantiate(buttonPrefab);
        nextButton = buttonContainer.transform.Find("playbutton").gameObject;
        nextButton.transform.position = new Vector3(0, -2f, 0f);
        nextButton.transform.localScale = Vector3.zero;
        buttonLayer = buttonContainer.transform.Find("white").gameObject;
        buttonLayer.transform.position = nextButton.transform.position;
        buttonLayer.transform.localScale = nextButton.transform.localScale;

    }

    public void Fill(int index)
    {
        var arr = instance.tempMask.ToCharArray();
        arr[index] = '1';
        instance.tempMask = new string(arr);
    }

    public int GetLetterIndex(Vector3 pos)
    {
        int index = Mathf.FloorToInt((pos.x - (start.Value - size.Value / 2)) / (size.Value + padding));
        if (index < 0 || index >= currentWord.word.Length) return -1;
        //float candidateX = start.Value + (index*(size.Value + padding));
        //float xdiff = pos.x - candidateX;
        float yDiff = Mathf.Abs(topY - pos.y);
        if (yDiff < size.Value / 2f)
        {
            return index;
        }
        return -1;
    }

    public Vector3 GetLetterPos(int index)
    {
        if (index < 0 || index >= currentWord.word.Length) return Vector3.zero;
        return new Vector3(start.Value + index * (size.Value + padding), topY, 0f);
    }


}
