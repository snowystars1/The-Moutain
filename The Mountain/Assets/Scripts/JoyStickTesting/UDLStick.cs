﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDLStick : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<TextMesh>().text = Input.GetAxis("Vertical").ToString();
    }
}
