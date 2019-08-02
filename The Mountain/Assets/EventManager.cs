using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public static EventManager instance = null;

    public delegate void VoidNoParams();
    public static event VoidNoParams Jump;
    public static event VoidNoParams Glide;
    public static event VoidNoParams GlideCancel;
    public static event VoidNoParams DodgeRoll;
    public static event VoidNoParams GroundAttacks;

    public delegate void VoidInputs(float x, float y);
    public static event VoidInputs Movement;
    public static event VoidNoParams MoveCancel;


    public float controllerInputX;
    public float controllerInputY;

    public Animator playerAnim;

    void Awake()
    {

        if (instance == null)//This will ensure that there is only a single instance of this class in the game at all times.
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        DontDestroyOnLoad(gameObject); //This object will persist through scenes

    }

    void Update()
    {
        controllerInputY = Input.GetAxis("Vertical");
        controllerInputX = Input.GetAxis("Horizontal");

        //This is a band-aid. When I rework glide movement I will remove the X and Y animation parameters and replace it with a single "forward" parameter.
        playerAnim.SetFloat(HashTable.controllerXParam, controllerInputX);
        playerAnim.SetFloat(HashTable.controllerYParam, controllerInputY);

        //Left Stick Movement
        if ((controllerInputX > .1f || controllerInputX < -.1f) || (controllerInputY > .1f || controllerInputY < -.1f))
        {
            Movement?.Invoke(controllerInputX, controllerInputY);
        }
        else
        {
            MoveCancel?.Invoke();
        }

        //Jump/Glide Initiation
        if (Input.GetKeyDown("joystick button 0"))//User presses the jump input, they can be on the ground, in the air, or in the gliding state.
        {//The 'A' Button

            if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.jumpState)
                Glide?.Invoke();
            else
            {
                if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.glideState)
                    GlideCancel?.Invoke();
                else
                    Jump?.Invoke();
            }
        }

        //Dodge Roll
        if (Input.GetKeyDown("joystick button 1") && (playerAnim.GetAnimatorTransitionInfo(0).fullPathHash != HashTable.motionToDodge))
        {
            DodgeRoll?.Invoke();
        }

        //GROUND ATTACKS
        if (Input.GetKeyDown("joystick button 2") && playerAnim.GetBool(HashTable.onGroundParam))//This is for initiating a ground combo
        {//The 'X' Button
            GroundAttacks?.Invoke();
        }


    }
}
