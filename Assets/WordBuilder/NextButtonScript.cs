using UnityEngine;

public class NextButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnMouseDown()
    {
        print(Time.time);
        if (WordManager.currentLevel >= WordManager.words.Count)
        {
            WordManager.currentLevel = 0;
            Application.LoadLevel("cps613");
        }
        else
        {
            Application.LoadLevel(Application.loadedLevelName);
        }
    }
}
