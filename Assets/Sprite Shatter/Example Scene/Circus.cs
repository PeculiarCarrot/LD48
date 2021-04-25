using UnityEngine;
using SpriteShatter;

public class Circus : MonoBehaviour {

    //Variables.
    bool shattered = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        //If the user clicks the left mouse button, explode the circus!
        if (!shattered && Input.GetMouseButtonDown(0)) {
            GetComponent<Shatter>().shatter();
            shattered = true;
        }

        //If the user clicks the right mouse button, reset the circus!
        else if (shattered && Input.GetMouseButtonDown(1)) {
            GetComponent<Shatter>().reset();
            shattered = false;
        }
	}
}
