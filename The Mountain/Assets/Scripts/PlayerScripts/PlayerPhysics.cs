using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour {

    public Animator playerAnim;
    public Animator gliderAnim;
    public CapsuleCollider playerCollider;
    public Rigidbody playerRb;
    public Camera main;
    public static Vector3 groundNormal;

    public float jumpHeight = 15f;
    public float jumpForwardPush = 3f;

    private AnimatorStateInfo currentBaseState;

    public struct ReversePositionData
    {
        public Vector3[] reversePositions;//3.25 seconds of data (To account for inclusivity we have to make it one datasize bigger)
        public int count;
    }
    public static ReversePositionData posData;
    [SerializeField]
    private int revTimeSeconds = 5;

    private int layerMask = 1 << 9;//This makes the onGround raycast only collide with ground


    public float sphereCastShotDistance = .49f;
    public float sphereCastRadius = .17f;

    void Start ()
    {
        posData = new ReversePositionData();
        posData.reversePositions = new Vector3[(revTimeSeconds*4)+1];//multiplied by 4 cause we store vectors 4 times every second, +1 is to account for inclusivity
        InvokeRepeating("AddReversePos", 0f, .25f);//Store a vector position every .25f
	}

    void FixedUpdate ()
    {
        //Every fixedUpdate loop, a raycast is shot down to keep track of when the character walks off a cliff or something.

        RaycastHit hit;
        if (Physics.SphereCast(playerCollider.center + this.transform.position, sphereCastRadius, Vector3.down, out hit, sphereCastShotDistance, layerMask))
        {
            playerAnim.SetBool(HashTable.onGroundParam, true);
            //groundNormal = hit.normal;
        }
        else
        {
            playerAnim.SetBool(HashTable.onGroundParam, false);
            //groundNormal = Vector3.up;
        }

        ////GLIDE FORCES
        //if(playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.glideState)
        //{
        //}
        //if((playerAnim.GetAnimatorTransitionInfo(0).fullPathHash == HashTable.glideToJump) || (playerAnim.GetAnimatorTransitionInfo(0).fullPathHash == HashTable.glideToMotion))
        //{
        //}
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

    public void GliderMotion(float controllerInputX, float controllerInputY)
    {
        gliderAnim.SetBool(HashTable.glideGliderParam, true);
        playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;//Release the Y constraint so he can rotate in the air

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