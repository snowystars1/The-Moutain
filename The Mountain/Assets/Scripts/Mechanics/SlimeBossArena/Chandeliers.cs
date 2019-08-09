using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chandeliers : MonoBehaviour {

    public GameObject[] chandeliers = new GameObject[3];
    int i = 0;
    public static bool chandelierFell;

    public StepButton leftButton;
    public StepButton rightButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (i < 3 && StepButton.buttonsPushed == 2)//if i is greater than 3, there are no more chandeliers.
        {
            Rigidbody rb = chandeliers[i].GetComponent<Rigidbody>();
            rb.isKinematic = false;
            StepButton.buttonsPushed = 0;//So this if wont run again until the chandelier hits the ground
        }
        if( i < 3 && chandeliers[i].transform.position.y < -20f)//This is to incentivize the player to go attack the monster and get off the button, this value will likely be adjusted
        {
            leftButton.StepButtonReset();
            rightButton.StepButtonReset();
            i++;
        }
	}
}
