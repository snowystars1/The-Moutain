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
    public static event VoidNoParams AirAttacks;
    public static event VoidNoParams CycleItemsForward;
    public static event VoidNoParams CycleItemsBackward;
    public static event VoidNoParams ZoomIn;
    public static event VoidNoParams ZoomOut;
    public static event VoidNoParams ChooseItem;
    public static event VoidNoParams CameraTargetMode;
    public static event VoidNoParams Interact;

    public delegate void VoidInputs(float x, float y);
    public static event VoidInputs Movement;
    public static event VoidNoParams MoveCancel;


    private float controllerInputX;
    private float controllerInputY;
    private float DPadX;
    private float DPadY;

    public Animator playerAnim;

    private bool alreadyDone;

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
        DPadX = Input.GetAxis("DPadX");

        //This is a band-aid. When I rework glide movement I will remove the X and Y animation parameters and replace it with a single "forward" parameter.
        playerAnim.SetFloat(HashTable.controllerXParam, controllerInputX);
        playerAnim.SetFloat(HashTable.controllerYParam, controllerInputY);


        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //LEFT STICK MOVEMENT
        if ((controllerInputX > .1f || controllerInputX < -.1f) || (controllerInputY > .1f || controllerInputY < -.1f))
        {
            Movement?.Invoke(controllerInputX, controllerInputY);
        }
        else
        {
            MoveCancel?.Invoke();
        }

        //JUMP/GLIDE INITIATION
        //User presses the jump input, they can be on the ground, in the air, or in the gliding state.
        if (Input.GetKeyDown("joystick button 0"))
        {//The 'A' Button

            if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.jumpState)
                Glide?.Invoke();
            else
            {//Cant use switch statement because animation hashes aren't constant.
                if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.glideState)
                    GlideCancel?.Invoke();
                else
                    Jump?.Invoke();
            }
        }

        //INTERACT
        if(Input.GetKeyDown("joystick button 3"))
        {
            Interact?.Invoke();
        }

        //DODGE ROLL
        if (Input.GetKeyDown("joystick button 1") && (playerAnim.GetAnimatorTransitionInfo(0).fullPathHash != HashTable.motionToDodge))
        {//The B Button
            DodgeRoll?.Invoke();
        }

        //GROUND ATTACKS
        if (Input.GetKeyDown("joystick button 2") && playerAnim.GetBool(HashTable.onGroundParam))//This is for initiating a ground combo
        {//The 'X' Button
            GroundAttacks?.Invoke();
        }

        //AIR ATTACKS
        if (!playerAnim.GetBool(HashTable.onGroundParam) && Input.GetKeyDown("joystick button 2"))//This is for initiating an air combo
        {//The 'X' Button
            AirAttacks?.Invoke();
        }

        //CYCLE ITEMS FORWARD
        if (Mathf.Approximately(DPadX, 1f) && !alreadyDone)
        {
            alreadyDone = true;
            CycleItemsForward?.Invoke();
        }
        if(Mathf.Approximately(DPadX,0f) && alreadyDone)
        {
            alreadyDone = false;
        }

        //CYCLE ITEMS BACKWARD
        if (Mathf.Approximately(DPadX, -1f) && !alreadyDone)
        {
            alreadyDone = true;
            CycleItemsBackward?.Invoke();
        }
        if (Mathf.Approximately(DPadX, 0f) && alreadyDone)
        {
            alreadyDone = false;
        }

        //ZOOM IN CAMERA
        if (Mathf.Approximately(Input.GetAxis("DPadY"), 1f))
        {//DPad Y "Down"
            ZoomIn?.Invoke();
        }

        //ZOOM OUT CAMERA
        if (Mathf.Approximately(Input.GetAxis("DPadY"), -1f))
        {//DPad Y "Up"
            ZoomOut?.Invoke();
        }

        //CHOOSE ITEM
        if(Input.GetKeyDown("joystick button 4"))
        {//Right Bumper
            if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.glideState)
                GameManagerScript.instance.inputManager.GlideCancel();
            ChooseItem?.Invoke();
        }

        //CAMERA TARGET MODE
        if (Input.GetKeyDown("joystick button 5"))
        {//Left Bumper
            CameraTargetMode?.Invoke();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
}
