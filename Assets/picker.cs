using UnityEngine;

public class picker : MonoBehaviour
{
    public bool grey = false;
	// Use this for initialization
	void Start () {
        if (grey)
        {
            var sr = gameObject.GetComponent<SpriteRenderer>();
            Color c = sr.color * 0.5f;
            c.a = 1f;
            sr.color = c;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

	void OnMouseDown()
	{
	    if (grey) return;
		if (this.gameObject.name == "AlphaBlock") {
			Application.LoadLevel ("AlphaBlockGame");
		} else if (this.gameObject.name == "MarbleGame") {
			Application.LoadLevel ("MarbleGame");
		} else if (this.gameObject.name == "JigsawGame") {
			Application.LoadLevel ("JigsawGame");
		} else if (this.gameObject.name == "AlphaMatching") {
			Application.LoadLevel ("AlphaMatching");
		} else if (this.gameObject.name == "MissingLetterGame") {
			Application.LoadLevel ("MissingLetterGame");
		} else if (this.gameObject.name == "BackButton") {
			Application.LoadLevel("cps613");
		}

	}
}
