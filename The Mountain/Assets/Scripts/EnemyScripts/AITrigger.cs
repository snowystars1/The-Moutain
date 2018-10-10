using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITrigger : MonoBehaviour {


	// Use this for initialization
	void Start () {
	}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if(other.gameObject.tag == "Player" && this.name == "TriggerRight")//Then the player has entered the right zone
        {
            AI.switchState = 1;
        }

        if (other.gameObject.tag == "Player" && this.name == "TriggerLeft")//Then the player has entered the right zone
        {
            AI.switchState = 2;
        }

        if (other.gameObject.tag == "Player" && this.name == "TriggerBack")//Then the player has entered the right zone
        {
            AI.switchState = 3;
        }
    }
}
