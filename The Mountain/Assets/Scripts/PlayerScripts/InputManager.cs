using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    bool isAxisDown = false;

    Animator playerAnim;

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
    //private Vector3 targetDirection;

    //Movement/Speed
    public float grapplingHookForce = 5f;
    public float jumpDelayPercent = .05f;
    public float airAttackMovementSpeed = 3f;
    public float CameraMoveDistanceOnDPad = 1f;
    public int RevTimeMoveSpeed = 1;
    private float turn;

    //private float startTime;
    private ParticleSystem clockParticles;

    public static bool airAttackForce = false;//Public so it can be accessed by BattleCombos script (ComboEvents)
    //private bool firstJumpPush = true;//This is used to push the player up for the first time, then, as they hold down jump, this will block certain elements of the jump "if". Reset when the player touches the ground. 

    //private float playerHeight;

    private float controllerInputX;
    private float controllerInputY;

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

    void FixedUpdate()
    {
        playerRb.useGravity = playerAnim.GetBool(HashTable.gravityParam);//Gravity is dictated by our animator parameter

        if (airAttackForce)
        {

            playerRb.velocity = Vector3.zero;//Cut off all forces
            if (MouseOrbitImproved.targetTrans != null)
            {
                airAttackMovementSpeed = Mathf.Clamp(MouseOrbitImproved.targetDir.magnitude, 0.0f, 3.0f);
                playerRb.AddForce(MouseOrbitImproved.targetDir.normalized * airAttackMovementSpeed, ForceMode.Impulse);//This will push the player toward the target at variable speed (less force applied if he is closer to target) NOT BALANCED
                playerRb.AddForce(-MouseOrbitImproved.targetDir.normalized * 1f, ForceMode.Force);
            }
            else
            {
                playerRb.AddForce(transform.forward.normalized * 3f, ForceMode.Impulse);//This will push the character forward when there is nothing targeted (when performing an air combo)

            }
            airAttackForce = false;

        }


    }

    void Update()
    {

        playerAnim.SetFloat(HashTable.jumpBlendParam, playerRb.velocity.y);

        controllerInputY = Input.GetAxis("Vertical");
        controllerInputX = Input.GetAxis("Horizontal");

        playerAnim.SetFloat(HashTable.controllerXParam, controllerInputX);
        playerAnim.SetFloat(HashTable.controllerYParam, controllerInputY);

        //MOVE AROUND

        //ResetBattleTimer();//Reset the battle timer to 0 if there is an input
        //if (!Targeting.targetMode)
        //{
        camForward = Vector3.Scale(CamTrans.forward, new Vector3(1, 0, 1)).normalized;
        //}
        newMove = controllerInputY * camForward + controllerInputX * CamTrans.right; //Adding the Camera forward vector and the camera right vector gives up the 3rd side of the triangle created by those two vectors. This is the direction the character will move
        if (newMove.magnitude > 1f)//To control magnitude
            newMove.Normalize();
        newMove = transform.InverseTransformDirection(newMove); //Get the direction local to this transform
        newMove = Vector3.ProjectOnPlane(newMove, PlayerPhysics.groundNormal);//For uneven surfaces
        turn = Mathf.Atan2(newMove.x, newMove.z); //Find the angle to which we want to turn

        if ((controllerInputX > .1f || controllerInputX < -.1f) || (controllerInputY > .1f || controllerInputY < -.1f))
        {//Left Stick
            if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash != HashTable.glideState)//Rotation will be handled differently while gliding
            {
                if (turn < 1.7f && turn > -1.7f)
                {
                    float turnSpeed = Mathf.Lerp(180f, 360f, newMove.z);
                    transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
                }
                else//He's rotating REALLY QUICKLY
                {
                    //This is to cap the speed at which he can rotate.
                    if (turn > 2f)
                        turn = 2f;
                    if (turn < -2f)
                        turn = -2f;

                    transform.Rotate(0, turn * 500f * Time.deltaTime, 0);//This will cap the rotation speed to 500f when he is spinning the stick very quickly
                }
            }

            playerAnim.SetFloat("Turn", turn, .1f, Time.deltaTime);
            playerAnim.SetFloat("Forward", newMove.z, .1f, Time.deltaTime);

        }
        else
        {

            playerAnim.SetFloat("Turn", 0f);
            playerAnim.SetFloat("Forward", 0f);
        }

        //JUMP
        if (Input.GetKeyDown("joystick button 0"))//User presses the jump input, they are in one of the motion states, and they are on the ground
        {//The 'A' Button

            if ((playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.motionState || (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.battleMotionState)) && playerAnim.GetBool(HashTable.onGroundParam))//This ensures the character is well into one of the two motion states when he tries to jump
            {
                ResetBattleTimer();
                playerAnim.CrossFadeInFixedTime(HashTable.jumpState, .25f);//Forces for the jump is applied in animation event for "Jump_2Initial"
                                                                           //playerHeight = this.gameObject.transform.position.y;//World Space positional y value
                playerAnim.applyRootMotion = false;
                GameManagerScript.instance.playerPhys.ApplyJumpForce();
                //playerPhys.ApplyJumpForce();
                //playerPhys.RunToJumpForceSmoothing();
            }
        }
        //if(Input.GetKeyUp("joystick button 0"))
        //{
        //    playerPhys.ApplyForceDown();
        //}

        //DODGE ROLL
        if (Input.GetKeyDown("joystick button 1") && ((playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.motionState) || (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.battleMotionState)) && (playerAnim.GetAnimatorTransitionInfo(0).fullPathHash != HashTable.motionToDodge))
        {//The 'B' button
            //playerAnim.CrossFadeInFixedTime(HashTable.dodgeRollState, .3f);
            playerAnim.SetTrigger(HashTable.dodgeParam);
        }


        //GLIDE
        //if (Input.GetKeyDown("joystick button 0") && !playerAnim.GetBool(HashTable.onGroundParam) && !playerAnim.GetBool(HashTable.glidingParam))
        //{//The 'A' Button
        //    playerAnim.CrossFadeInFixedTime(HashTable.glideState, .5f);
        //    playerAnim.SetBool(HashTable.glidingParam, true);
        //}
        //if (((playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.glideState) && Input.GetKeyDown("joystick button 0")) || playerAnim.GetBool(HashTable.onGroundParam))
        //{
        //    playerAnim.SetBool(HashTable.glidingParam, false);
        //}

        //GROUND ATTACKS
        if (Input.GetKeyDown("joystick button 2") && playerAnim.GetBool(HashTable.onGroundParam))//This is for initiating a ground combo
        {//The 'X' Button
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

        //AIR ATTACKS
        if (!playerAnim.GetBool(HashTable.onGroundParam) && (Input.GetMouseButtonDown(1) || Input.GetKeyDown("joystick button 2")))//This is for initiating an air combo
        {//The 'X' Button
            playerRb.drag = 1;
            //This is set to true here because the player must always be able to press x and activite the combo parameter
            playerAnim.SetBool(HashTable.ComboParam, true);//This is set to false in the comboBeahviour script in the function OnStateEnter().

            if (playerAnim.GetInteger(HashTable.ComboCountParam) == 0)//This only works at the start of a combo
            {
                //ResetBattleTimer();
                playerAnim.SetInteger(HashTable.ComboCountParam, 1);

                ResetBattleTimer();//They have started an action so they should stay in the battle stance
                playerAnim.SetBool(HashTable.gravityParam, false);//Turn off gravity while they are attacking in the air
                airAttackForce = true;
                swordCollider.enabled = true;
                playerAnim.CrossFade(HashTable.airUpSwing1State, .25f);
            }

        }
        //Air attack forces
        if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.jumpState && playerRb.drag != 0)//If the character has come out of the air attack, no more drag
            playerRb.drag = 0;

        //DPAD EQUIPMENT SWAP
        if (Mathf.Approximately(Input.GetAxis("DPadX"), 1f) && !isAxisDown)//FIX THIS SHIT
        {
            isAxisDown = true;
            Debug.Log("asdfa");
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
        if (Mathf.Approximately(Input.GetAxis("DPadX"), -1f) && !isAxisDown)
        {
            isAxisDown = true;
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
        if (Mathf.Approximately(Input.GetAxis("DPadX"), 0f))
        {
            isAxisDown = false;
        }

        //DPAD CAMERA ZOOM (Y-AXIS)
        if (Mathf.Approximately(Input.GetAxis("DPadY"), 1f))
        {//DPAD Up and Down
            MouseOrbitImproved.distance -= CameraMoveDistanceOnDPad;//The addition and subtraction of these two if statements are swapped for game "feel" purposes
            Mathf.Lerp(MouseOrbitImproved.distance, MouseOrbitImproved.distance - CameraMoveDistanceOnDPad, .2f);
        }
        if (Mathf.Approximately(Input.GetAxis("DPadY"), -1f))
        {
            MouseOrbitImproved.distance += CameraMoveDistanceOnDPad;
        }

        //TARGET CAMERA MODE
        if (Input.GetKeyDown("joystick button 5"))
        {//The Rightbumper Button
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

        //Reverse Time
        //if(Input.GetKey("joystick button 3"))
        //{
        //    playerAnim.enabled = false;
        //    clockParticles.Play(true);
        //    playerModel.enabled = false;
        //    playerClothes.enabled = false;
        //    swordModel.enabled = false;
        //    gliderModel.enabled = false;
        //    gauntletModel.enabled = false;
        //    scarfModel.enabled = false;
        //    Vector3 toReversePos = PlayerPhysics.posData.reversePositions[(PlayerPhysics.posData.count + 1) % 13] - transform.position;
        //    playerRb.velocity = toReversePos * RevTimeMoveSpeed;
        //}
        //if(Input.GetKeyUp("joystick button 3"))
        //{
        //    playerAnim.enabled = true;
        //    playerModel.enabled = true;
        //    playerClothes.enabled = true;
        //    swordModel.enabled = true;
        //    gliderModel.enabled = true;
        //    gauntletModel.enabled = true;
        //    scarfModel.enabled = true;
        //    //particles.Pause(true);
        //    clockParticles.Stop();
        //    //particles.Clear(true);
        //}

        //GRAPPLING HOOK
        //if (Input.GetKeyDown("joystick button 4") && !playerAnim.GetBool(HashTable.grapplingParam))
        //{//The LeftBumper button
        //    //DO SOME ANIMATION CALL
        //    playerAnim.SetBool(HashTable.grapplingParam, true);//WE HAVE TWO ACTIVATE BOOLEANS, ONE CALLED GrapplingHookCharacterController.GrapplingHookMode which activates when the grapple hits a target, and another called HashTable.grapplingParam which activates when you press the grapple button
        //    GameManagerScript.instance.hookController.FireGrappleHook();

        //}
        //else
        //{
        //    if (Input.GetKeyDown("joystick button 4") && playerAnim.GetBool(HashTable.grapplingParam))//This is to remove the grappling hook
        //    {
        //        GameManagerScript.instance.hookController.DestroyGrappleHook();//This is done to reduce having multiple instances of the same script in a scene.
        //        playerAnim.SetBool(HashTable.grapplingParam, false);
        //        GrapplingHookCharacterController.GrapplingHookMode = false;//This will already be false if the hook hasn't hit anything, it the hook has made contact, it will will be true, therefore this line will actually do something.
        //        if (playerAnim.GetBool(HashTable.onGroundParam))
        //        {
        //            playerAnim.applyRootMotion = true;//This is turned on here and in IdleBehaviour, when the character is flying through the air or being controlled by forces other than the animator, this should be off.
        //        }
        //    }
        //}

        switch (currentItem)
        {
            //GRAPPLING HOOK
            case 0:
                if (Input.GetKeyDown("joystick button 4") && !playerAnim.GetBool(HashTable.grapplingParam))
                {//The LeftBumper button
                 //DO SOME ANIMATION CALL
                    playerAnim.SetBool(HashTable.grapplingParam, true);//WE HAVE TWO ACTIVATE BOOLEANS, ONE CALLED GrapplingHookCharacterController.GrapplingHookMode which activates when the grapple hits a target, and another called HashTable.grapplingParam which activates when you press the grapple button
                    GameManagerScript.instance.hookController.FireGrappleHook();

                }
                else
                {
                    if (Input.GetKeyDown("joystick button 4") && playerAnim.GetBool(HashTable.grapplingParam))//This is to remove the grappling hook
                    {
                        GameManagerScript.instance.hookController.DestroyGrappleHook();//This is done to reduce having multiple instances of the same script in a scene.
                        playerAnim.SetBool(HashTable.grapplingParam, false);
                        GrapplingHookCharacterController.GrapplingHookMode = false;//This will already be false if the hook hasn't hit anything, it the hook has made contact, it will will be true, therefore this line will actually do something.
                        if (playerAnim.GetBool(HashTable.onGroundParam))
                        {
                            playerAnim.applyRootMotion = true;//This is turned on here and in IdleBehaviour, when the character is flying through the air or being controlled by forces other than the animator, this should be off.
                        }
                    }
                }
                break;

            //POCKET WATCH
            case 1:
                if (Input.GetKey("joystick button 4"))
                {
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
                }
                if (Input.GetKeyUp("joystick button 4"))
                {
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