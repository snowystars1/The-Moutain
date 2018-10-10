using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigDoor : MonoBehaviour {

    public Animator doorAnim;
    public GameObject slimeBoss;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (slimeBoss==null)
        {
            doorAnim.SetTrigger(HashTable.bigDoorOpenParam);
        }
	}
}
