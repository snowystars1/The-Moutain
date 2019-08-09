using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    private bool inTrigger = false;
    public float cannonForce = 5f;

    private void OnEnable()
    {
        //EventManager.Interact += CannonFire;
        EventManager.Interact += LoadCannon;
    }

    private void OnDisable()
    {
        EventManager.Interact -= CannonFire;
        EventManager.Interact -= LoadCannon;
    }

    void LoadCannon()
    {
        if (inTrigger)
        {
            //Load the player into the cannon and get ready to fire
            GameManagerScript.instance.playerAnim.applyRootMotion = false;
            GameManagerScript.instance.playerCharacter.transform.position = this.transform.GetChild(0).position;
            GameManagerScript.instance.playerRb.velocity = Vector3.zero;
            GameManagerScript.instance.playerAnim.SetBool(HashTable.gravityParam, false);
            //Next time "Interact" is pressed this function won't be called
            EventManager.Interact -= LoadCannon;
            EventManager.Interact += CannonFire;

            //Disable movement
            EventManager.Movement -= GameManagerScript.instance.inputManager.LeftStick;
        }
    }

    void CannonFire()
    {

        //Add a force to push him out of the cannon and re-enable gravity
        GameManagerScript.instance.playerRb.AddForce(this.transform.GetChild(0).transform.up * cannonForce, ForceMode.Impulse);
        GameManagerScript.instance.playerAnim.SetBool(HashTable.gravityParam, true);
        //Resubscribe and unsubscribe
        EventManager.Interact += LoadCannon;
        EventManager.Interact -= CannonFire;
        EventManager.Movement += GameManagerScript.instance.inputManager.LeftStick;
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the player character (Layer 8: "Player") is inside the trigger
        if(other.gameObject.layer == 8)
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //If the player character (Layer 8: "Player") exits the trigger
        if (other.gameObject.layer == 8)
        {
            inTrigger = false;
        }
    }
}
