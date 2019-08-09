using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {

    public static GameManagerScript instance = null;

    [Header("References")]
    public GrapplingHookCharacterController hookController;
    public PlayerPhysics playerPhys;
    public InputManager inputManager;
    public GrapplingHook grapplingHook;

    //With this we can abuse the singleton pattern and only ever grab our references once.
    public GameObject playerCharacter;
    public Animator playerAnim;
    public Rigidbody playerRb;

	// Use this for initialization
	void Awake () {

		if(instance == null)//This will ensure that there is only a single instance of this class in the game at all times.
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
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

    public void PlayButton()
    {
        SceneManager.LoadScene("Arena-1-SlimeBoss", LoadSceneMode.Single);
    }
}
