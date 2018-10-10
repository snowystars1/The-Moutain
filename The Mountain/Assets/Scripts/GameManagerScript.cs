using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public static GameManagerScript instance = null;
    public GrapplingHookCharacterController hookController;
    public InputManager inputManager;
    public GrapplingHook grapplingHook;

	// Use this for initialization
	void Awake () {

		if(instance == null)//This will ensure that there is only a single instance of this class in the game at all times.
        {
            instance = this;
        }
        else
        {
            if(instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        DontDestroyOnLoad(gameObject); //This object will persist through scenes

	}
	
	void Update () {
		
	}
}
