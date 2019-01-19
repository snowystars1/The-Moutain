using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Doors : MonoBehaviour {

    public Animator doorAnim;
    public Transform playerTrans;
    public GameObject slimeBoss;
    public GameObject entryDoor;
    bool inTheArena = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (slimeBoss==null)
        {
            doorAnim.SetTrigger(HashTable.bigDoorOpenParam);
        }
        if((inTheArena==false) && (playerTrans.position.z < 20))
        {
            entryDoor.SetActive(true);
            inTheArena = true;
        }
        if(playerTrans.position.z < -78f)
        {
            SceneManager.LoadScene("MainMenu");
        }
	}
}
