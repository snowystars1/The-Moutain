using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRLStick : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<TextMesh>().text = Input.GetAxis("Horizontal").ToString();
    }
}
