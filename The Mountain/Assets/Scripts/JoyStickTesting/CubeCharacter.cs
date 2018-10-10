using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCharacter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 newDir = new Vector3(Input.GetAxisRaw("Horizontal"),0f,Input.GetAxisRaw("Vertical"));
        if (newDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(newDir);
        }
	}
}
