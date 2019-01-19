using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour {

    public Rigidbody grapplingHookRb;
    public GameObject player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        grapplingHookRb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
        GameManagerScript.instance.hookController.Grapple();

    }
}
