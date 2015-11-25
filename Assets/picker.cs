using UnityEngine;

public class picker : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
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
