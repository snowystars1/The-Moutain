using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTrigs : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<TextMesh>().text = Input.GetAxis("Trigs").ToString();
    }
}
