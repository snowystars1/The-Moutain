using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //bool isAxisDown = false;

    Animator playerAnim;
    Animator gliderAnim;

    public GameObject playerSword;
    private CapsuleCollider swordCollider;

    //public Transform playerTrans;
    public Transform CamTrans;
    public Rigidbody playerRb;

    //Mesh Renderers
    public SkinnedMeshRenderer playerModel;
    public SkinnedMeshRenderer playerClothes;
    public MeshRenderer swordModel;
    public SkinnedMeshRenderer gliderModel;
    public MeshRenderer gauntletModel;
    public SkinnedMeshRenderer scarfModel;

    public static Vector3 camForward;
    public static Vector3 newMove;

    //Movement/Speed
    public float grapplingHookForce = 5f;
    public float jumpDelayPercent = .05f;
    public float airAttackMovementSpeed = 3f;
    public float CameraMoveDistanceOnDPad = 1f;
    public int RevTimeMoveSpeed = 1;

    //Stopwatch
    private bool stopWatchParam;
    private ParticleSystem clockParticles;

    //public static bool airAttackForce = false;//Public so it can be accessed by BattleCombos script (ComboEvents)
    //private bool firstJumpPush = true;//This is used to push the player up for the first time, then, as they hold down jump, this will block certain elements of the jump "if". Reset when the player touches the ground. 

    //private float playerHeight;

    //private float controllerInputX;
    //private float controllerInputY;

    //GrapplingHookCharacterController hookController;
    //PlayerPhysics playerPhys;

    //Equipment
    //OKAY LISTEN UP YOU FUCK
    //This current system is designed so that whenever the game is launched, I will have a finite (knowable) number of items
    //The array of items will be this size.
    //HOWEVER the character might have some of those items locked, because he hasn't picked them up yet.
    //Therefore, we need to define what is locked and what is unlocked. !ALSO! We need to skip over items that are locked when switching items.
    private struct item
    {
        public GameObject itemObj;
        public bool unlocked;

        public item(GameObject a, bool b)
        {
            itemObj = a;
            unlocked = b;
        }
    }
    private item[] itemArray = new item[2];//List of structs which represent each item
    private int currentItem = 0;

    public GameObject GrapplingHookPicture;
    public GameObject PocketWatchPicture;



    void OnEnable()
    {
        EventManager.Movement += LeftStick;
        EventManager.MoveCancel += LeftStickStop;
        EventManager.Jump += Jump;
        EventManager.Glide += Glide;
        EventManager.GlideCancel += GlideCancel;
        EventManager.DodgeRoll += DodgeRoll;
        EventManager.GroundAttacks += GroundAttack;
        EventManager.AirAttacks += AirAttack;
        EventManager.CycleItemsForward += CycleItemsForward;
        EventManager.CycleItemsBackward += CycleItemsBackward;
        EventManager.ZoomIn += ZoomIn;
        EventManager.ZoomOut += ZoomOut;
        EventManager.ChooseItem += ChooseItem;
        EventManager.CameraTargetMode += CameraTargetMode;

    }

    void OnDisable()
    {
        EventManager.Movement -= LeftStick;
        EventManager.MoveCancel -= LeftStickStop;
        EventManager.Jump -= Jump;
        EventManager.Glide -= Glide;
        EventManager.GlideCancel -= GlideCancel;
        EventManager.DodgeRoll -= DodgeRoll;
        EventManager.GroundAttacks -= GroundAttack;
        EventManager.AirAttacks -= AirAttack;
        EventManager.CycleItemsForward -= CycleItemsForward;
        EventManager.CycleItemsBackward -= CycleItemsBackward;
        EventManager.ZoomIn -= ZoomIn;
        EventManager.ZoomOut -= ZoomOut;
        EventManager.ChooseItem -= ChooseItem;
        EventManager.CameraTargetMode -= CameraTargetMode;
    }

    void Start()
    {
        //Default Equipment
        itemArray[0] = new item(GrapplingHookPicture, true);
        itemArray[1] = new item(PocketWatchPicture, true);

        swordCollider = playerSword.GetComponent<CapsuleCollider>();
        //playerPhys = GetComponent<PlayerPhysics>();
        clockParticles = GetComponent<ParticleSystem>();
        clockParticles.Pause(true);
        //hookController = GetComponent<GrapplingHookCharacterController>();
        playerAnim = GetComponent<Animator>();
        playerAnim.SetBool(HashTable.onGroundParam, true);
        playerAnim.SetBool(HashTable.gravityParam, true);
        InvokeRepeating("AddBattleTimer", 1f, 1f);
    }

    void LeftStick(float controllerInputX, float controllerInputY)
    {
        //This block of code gives us direction of movement and turn parameter for the animator fields.
        camForward = Vector3.Scale(CamTrans.forward, new Vector3(1f, 0, 1f)).normalized;

        //Debug.DrawRay(transform.position + new Vector3(0f, 1f, 0f), camForward, Color.blue, 3f);
        //Debug.DrawRay(transform.position + new Vector3(0f, 1f, 0f), CamTrans.right, Color.green, 3f);

        //Adding the Camera forward vector and the camera right vector gives up the 3rd side of the triangle created by those two vectors. This is the direction the character will move
        newMove = (controllerInputY * camForward) + (controllerInputX * CamTrans.right);
        if (newMove.magnitude > 1f)//To control magnitude
            newMove.Normalize();
        newMove = transform.TransformDirection(newMove);
        newMove = transform.InverseTransformDirection(newMove); //Get the direction local to this transform
        //newMove = Vector3.ProjectOnPlane(newMove, PlayerPhysics.groundNormal);//For uneven surfaces


        if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash != HashTable.glideState)//Rotation will be handled differently while gliding
        {

            float speed = 10 * Time.deltaTime;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, newMove, speed, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);

        }

        playerAnim.SetFloat("Forward", Mathf.Clamp(Mathf.Abs(controllerInputX) + Mathf.Abs(controllerInputY), 0f, 1f), .1f, Time.deltaTime);

        if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.jumpState)//Motion while in the air.
        {
            GameManagerScript.instance.playerPhys.MotionInAir(newMove);
        }

        if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.glideState)
        {
            GameManagerScript.instance.playerPhys.GliderMotion(controllerInputX, controllerInputY, newMove);
        }
    }

    void LeftStickStop()
    {
        playerAnim.SetFloat("Turn", 0f);
        playerAnim.SetFloat("Forward", 0f);
    }

    void Jump()
    {
        if ((playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.motionState || (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.battleMotionState)) && playerAnim.GetBool(HashTable.onGroundParam))//This ensures the character is well into one of the two motion states when he tries to jump
        {
            ResetBattleTimer();
            playerAnim.CrossFadeInFixedTime(HashTable.jumpState, .25f);//Forces for the jump is applied in animation event for "Jump_2Initial"
            playerAnim.applyRootMotion = false;
            GameManagerScript.instance.playerPhys.ApplyJumpForce();
        }
    }

    void Glide()
    {
        playerAnim.CrossFadeInFixedTime(HashTable.glideState, .5f);
        playerAnim.SetBool(HashTable.glidingParam, true);
        GameManagerScript.instance.playerPhys.GliderInitiation();
    }

    public void GlideCancel()
    {
        GameManagerScript.instance.playerPhys.GliderCancel();
        playerAnim.SetBool(HashTable.glidingParam, false);
    }

    void DodgeRoll()
    {
        playerAnim.SetTrigger(HashTable.dodgeParam);
    }

    void GroundAttack()
    {
        ResetBattleTimer();
        //This is set to true here because the player must always be able to press x and activate the combo parameter
        playerAnim.SetBool(HashTable.ComboParam, true);//This is set to false in the comboBeahviour script in the function OnStateEnter().

        if ((playerAnim.GetInteger(HashTable.ComboCountParam) == 0)
            && ((playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.battleMotionState) || (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.motionState)))//One can only start a combo if they are in one of the motion states and not already in an attack state.
        {
            swordCollider.enabled = true;
            playerAnim.SetInteger(HashTable.ComboCountParam, 1);
            playerAnim.CrossFade(HashTable.swordSwing2State, .25f);
        }
    }

    void AirAttack()
    {
        playerRb.drag = 1;
        //This is set to true here because the player must always be able to press x and activite the combo parameter
        playerAnim.SetBool(HashTable.ComboParam, true);//This is set to false in the comboBeahviour script in the function OnStateEnter().

        if (playerAnim.GetInteger(HashTable.ComboCountParam) == 0)//This only works at the start of a combo
        {
            //ResetBattleTimer();
            playerAnim.SetInteger(HashTable.ComboCountParam, 1);

            ResetBattleTimer();//They have started an action so they should stay in the battle stance
            playerAnim.SetBool(HashTable.gravityParam, false);//Turn off gravity while they are attacking in the air
            PlayerPhysics.airAttackForce = true;
            swordCollider.enabled = true;
            playerAnim.CrossFade(HashTable.airUpSwing1State, .25f);
        }
    }

    void CycleItemsForward()
    {
        itemArray[currentItem].itemObj.SetActive(false);//Disable the current object
        if (currentItem == itemArray.Length - 1)//Go to the next item
        {
            currentItem = 0;//Effectively added
        }
        else
        {
            currentItem++;
        }
        while (itemArray[currentItem].unlocked == false)
        {
            currentItem++;//This will skip over items we have yet to unlock.
            if (currentItem == itemArray.Length - 1)//Avoid IndexOutOfBounds exception.
            {
                currentItem = 0;
            }
        }
        itemArray[currentItem].itemObj.SetActive(true);//Enable the new object
    }

    void CycleItemsBackward()
    {
        itemArray[currentItem].itemObj.SetActive(false);
        if (currentItem == 0)//Go to the next item
        {
            currentItem = itemArray.Length - 1;//Effectively added
        }
        else
        {
            currentItem--;
        }
        while (itemArray[currentItem].unlocked == false)
        {
            currentItem--;//This will skip over items we have yet to unlock.
            if (currentItem == itemArray.Length - 1)//Avoid IndexOutOfBounds exception.
            {
                currentItem = 0;
            }
        }
        itemArray[currentItem].itemObj.SetActive(true);
    }

    void ZoomIn()
    {
        MouseOrbitImproved.distance -= CameraMoveDistanceOnDPad;//The addition and subtraction of these two if statements are swapped for game "feel" purposes
        Mathf.Lerp(MouseOrbitImproved.distance, MouseOrbitImproved.distance - CameraMoveDistanceOnDPad, .2f);
    }

    void ZoomOut()
    {
        MouseOrbitImproved.distance += CameraMoveDistanceOnDPad;
    }

    void ChooseItem()
    {
        switch (currentItem)
        {
            //GRAPPLING HOOK
            case 0:
                if (!playerAnim.GetBool(HashTable.grapplingParam))
                {//The LeftBumper button
                 //DO SOME ANIMATION CALL
                 //WE HAVE TWO ACTIVATE BOOLEANS, ONE CALLED GrapplingHookCharacterController.GrapplingHookMode which activates when the grapple hits a target, and another called HashTable.grapplingParam which activates when you press the grapple button
                    playerAnim.SetBool(HashTable.grapplingParam, true);
                    GameManagerScript.instance.hookController.FireGrappleHook();

                }
                else
                {
                    if (playerAnim.GetBool(HashTable.grapplingParam))//This is to remove the grappling hook
                    {
                        //This is done to reduce having multiple instances of the same script in a scene.
                        GameManagerScript.instance.hookController.DestroyGrappleHook();
                        playerAnim.SetBool(HashTable.grapplingParam, false);
                        //This will already be false if the hook hasn't hit anything, it the hook has made contact, it will will be true, therefore this line will actually do something.
                        GrapplingHookCharacterController.GrapplingHookMode = false;
                        if (playerAnim.GetBool(HashTable.onGroundParam))
                        {
                            //This is turned on here and in IdleBehaviour, when the character is flying through the air or being controlled by forces other than the animator, this should be off.
                            playerAnim.applyRootMotion = true;
                        }
                    }
                }
                break;

            //POCKET WATCH
            case 1:
                if (!stopWatchParam)
                {
                    stopWatchParam = true;
                    playerAnim.enabled = false;
                    clockParticles.Play(true);
                    playerModel.enabled = false;
                    playerClothes.enabled = false;
                    swordModel.enabled = false;
                    gliderModel.enabled = false;
                    gauntletModel.enabled = false;
                    scarfModel.enabled = false;
                    Vector3 toReversePos = PlayerPhysics.posData.reversePositions[(PlayerPhysics.posData.count + 1) % 13] - transform.position;
                    playerRb.velocity = toReversePos * RevTimeMoveSpeed;
                    break;
                }
                if (stopWatchParam)
                {
                    stopWatchParam = false;
                    playerAnim.enabled = true;
                    playerModel.enabled = true;
                    playerClothes.enabled = true;
                    swordModel.enabled = true;
                    gliderModel.enabled = true;
                    gauntletModel.enabled = true;
                    scarfModel.enabled = true;
                    clockParticles.Stop();

                }
                break;

            default:
                break;
        }
    }

    void CameraTargetMode()
    {
        if (MouseOrbitImproved.targetMode)
        {
            Debug.Log("TargetMode Off");
            MouseOrbitImproved.targetMode = false;
        }
        else
        {
            Debug.Log("TargetMode On");
            MouseOrbitImproved.targetMode = true;
        }
    }

    void AddBattleTimer()
    {
        if (playerAnim.GetInteger(HashTable.battleIdleTimerParam) == 6)
        {
            playerAnim.SetInteger(HashTable.battleIdleTimerParam, 0);
        }
        if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("BattleMotion"))
        {
            int storage = playerAnim.GetInteger(HashTable.battleIdleTimerParam);
            playerAnim.SetInteger(HashTable.battleIdleTimerParam, ++storage);//Had to use a storage variable to increment because unity doesnt recognize playerAnim.GetInteger as a variable. That means I couldn't use the ++ operator.
        }
    }

    void ResetBattleTimer()
    {
        if (playerAnim.GetInteger(HashTable.battleIdleTimerParam) == 0)
            return;
        if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("BattleMotion"))
            playerAnim.SetInteger(HashTable.battleIdleTimerParam, 0); //Reset the battle timer if there is an input while he is sitting in battleIdle. This means he could be running currently
    }

}