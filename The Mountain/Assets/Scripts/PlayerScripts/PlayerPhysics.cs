using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour {

    //References
    public Animator playerAnim;
    public Animator gliderAnim;
    private AnimatorStateInfo currentBaseState;
    public CapsuleCollider playerCollider;
    public Rigidbody playerRb;
    public Camera main;
    public static Vector3 groundNormal;

    //Jumping
    public float jumpHeight = 15f;
    public float jumpForwardPush = 3f;

    //Movement/Speed
    public static bool airAttackForce = false;//Public so it can be accessed by BattleCombos script (ComboEvents)
    public float airAttackMovementSpeed = 3f;

    //Ground Check
    private int layerMask = 1 << 9;//This makes the onGround raycast only collide with ground
    public float sphereCastShotDistance = .49f;
    public float sphereCastRadius = .17f;

    //Stopwatch
    [SerializeField]
    private int revTimeSeconds = 5;
    public static ReversePositionData posData;

    public struct ReversePositionData
    {
        public Vector3[] reversePositions;//3.25 seconds of data (To account for inclusivity we have to make it one datasize bigger)
        public int count;
    }

    void Start ()
    {
        posData = new ReversePositionData();
        posData.reversePositions = new Vector3[(revTimeSeconds*4)+1];//multiplied by 4 cause we store vectors 4 times every second, +1 is to account for inclusivity
        InvokeRepeating("AddReversePos", 0f, .25f);//Store a vector position every .25f
	}

    void FixedUpdate ()
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

        //This was originally in an update loop (Might cause problems later who knows)
        playerAnim.SetFloat(HashTable.jumpBlendParam, playerRb.velocity.y);

        OnGroundCheck();
        //Air attack forces
        //When the player attacks in the air, we set drag to 1 to slow them down over time. This is to reset drag.
        if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.jumpState && playerRb.drag != 0)
            playerRb.drag = 0;
    }

    void OnGroundCheck()
    {
        //Fires SphereCast down to ground to scan if we've hit ground or not
        RaycastHit hit;
        if (Physics.SphereCast(playerCollider.center + this.transform.position, sphereCastRadius, Vector3.down, out hit, sphereCastShotDistance, layerMask))
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.glideState)
                GameManagerScript.instance.inputManager.GlideCancel();
            playerAnim.SetBool(HashTable.onGroundParam, true);
            //groundNormal = hit.normal;
        }
        else
        {
            playerAnim.SetBool(HashTable.onGroundParam, false);
            //groundNormal = Vector3.up;
        }
    }

    public float airForce = 4f;

    public void MotionInAir(Vector3 newMove)
    {

        if (playerAnim.GetFloat(HashTable.forwardParam) > .25f)
            playerRb.AddForce(newMove * airForce, ForceMode.Force);

        //This code slows you down in the air.
        if ((playerRb.velocity.x > 4f || playerRb.velocity.x < -4f) || (playerRb.velocity.z > 4f || playerRb.velocity.z < -4f))
        {
            playerRb.AddForce(new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z) * -1f, ForceMode.Force);
        }
    }

    public void GliderInitiation()
    {
        gliderAnim.SetBool(HashTable.glideGliderParam, true);
        playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;//Release the Y constraint so he can rotate in the air
    }

    public void GliderMotion(float controllerInputX, float controllerInputY)
    {

        playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y / 1.05f, playerRb.velocity.z);//This makes the character fall slower
        playerRb.AddRelativeForce(Vector3.forward * controllerInputY * 10);//This pushes him forward/back
        playerRb.AddRelativeForce(Vector3.right * controllerInputX * 8);// This pushes him right/left
        playerRb.angularVelocity = new Vector3(0f, controllerInputX, 0f);//This turns him in midair
        playerRb.velocity = Vector3.ClampMagnitude(playerRb.velocity, 30f);
    }

    public void GliderCancel()
    {
        //Reinstate the y constraint so he doesnt rotate wildly after gliding
        playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        gliderAnim.SetBool(HashTable.glideGliderParam, false);
    }

    public void ApplyJumpForce()//Applied in animation event for Jump_2Initial
    {
        playerRb.AddForce(new Vector3(0f, jumpHeight*1.1f, 0f), ForceMode.Impulse);
        //playerRb.AddForce(transform.forward.normalized * jumpForwardPush * InputManager.newMove.z, ForceMode.Impulse);
    }

    private void AddReversePos()
    {
        posData.reversePositions[posData.count] = this.transform.position;
        //posData.count %= 12;//Once count reaches 12, it will reset to zero
        if(posData.count%(revTimeSeconds*4) != posData.count)
        {
            posData.count %= (revTimeSeconds * 4);
            return;
        }
        posData.count++;
    }

    public Vector3 ClampMagnitude(Vector3 v, float max, float min)//Doesn't clamp y
    {
        double sm = v.sqrMagnitude;
        if (sm > (double)max * (double)max)
            return new Vector3(v.normalized.x*max,v.y,v.normalized.z*max);
        else if (sm < (double)min * (double)min)
            return new Vector3(v.normalized.x * min, v.y, v.normalized.z * min);
        return v;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerCollider.center + new Vector3(this.transform.position.x, this.transform.position.y - .49f, this.transform.position.z), sphereCastRadius);
    }
}