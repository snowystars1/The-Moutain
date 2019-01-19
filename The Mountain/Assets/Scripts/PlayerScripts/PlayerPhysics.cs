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

    void Start ()
    {
        posData = new ReversePositionData();
        posData.reversePositions = new Vector3[(revTimeSeconds*4)+1];//multiplied by 4 cause we store vectors 4 times every second, +1 is to account for inclusivity
        InvokeRepeating("AddReversePos", 0f, .25f);//Store a vector position every .25f
	}



    // Update is called once per frame
    void FixedUpdate ()
    {
        //Every fixedUpdate loop, a raycast is shot down to keep track of when the character walks off a cliff or something.
#if UNITY_EDITOR
        //Debug.DrawRay(playerCollider.center + new Vector3(this.transform.position.x, this.transform.position.y - .61f, this.transform.position.z), Vector3.down * .2f, Color.red, 10f);
#endif
        RaycastHit hit;
        if (Physics.Raycast(playerCollider.center + new Vector3(this.transform.position.x, this.transform.position.y - .61f, this.transform.position.z), Vector3.down, out hit, .2f, layerMask))//Fire a raycast down to detect if the character is on the ground or not
        {
            playerAnim.SetBool(HashTable.onGroundParam, true);
            groundNormal = hit.normal;
        }else
        {
            playerAnim.SetBool(HashTable.onGroundParam, false);
            groundNormal = Vector3.up;
        }

        //GLIDE FORCES
        if(playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.glideState)
        {
            gliderAnim.SetBool(HashTable.glideGliderParam, true);
            playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;//Release the Y constraint so he can rotate in the air

            playerRb.velocity = new Vector3(playerRb.velocity.x, playerRb.velocity.y / 1.05f, playerRb.velocity.z);//This makes the character fall slower
            playerRb.AddRelativeForce(Vector3.forward * playerAnim.GetFloat(HashTable.controllerYParam) * 10);//This pushes him forward/back
            playerRb.AddRelativeForce(Vector3.right * playerAnim.GetFloat(HashTable.controllerXParam) * 8);// This pushes him right/left
            playerRb.angularVelocity = new Vector3(0f, playerAnim.GetFloat(HashTable.controllerXParam), 0f);//This turns him in midair
            playerRb.velocity = Vector3.ClampMagnitude(playerRb.velocity, 30f);
        }
        if((playerAnim.GetAnimatorTransitionInfo(0).fullPathHash == HashTable.glideToJump) || (playerAnim.GetAnimatorTransitionInfo(0).fullPathHash == HashTable.glideToMotion))
        {
            playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;//Reinstate the y constraint so he doesnt rotate wildly after gliding
            gliderAnim.SetBool(HashTable.glideGliderParam, false);
        }

        if (playerAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.jumpState)//Motion while in the air.
        {
            if(InputManager.newMove.z > .25f || InputManager.newMove.z < -.25f)
                playerRb.AddForce(transform.forward.normalized * 7f * InputManager.newMove.z, ForceMode.Force);

            if ((playerRb.velocity.x > 4f || playerRb.velocity.x < -4f) || (playerRb.velocity.z > 4f || playerRb.velocity.z < -4f))
            {
                playerRb.AddForce(new Vector3(playerRb.velocity.x,0f,playerRb.velocity.z) * -4f, ForceMode.Force);
            }

        }
    }




    public void ApplyJumpForce()//Applied in animation event for Jump_2Initial
    {
        playerRb.AddForce(new Vector3(0f, jumpHeight*1.1f, 0f), ForceMode.Impulse);
        //playerRb.AddForce(transform.forward.normalized * jumpForwardPush * InputManager.newMove.z, ForceMode.Impulse);
    }

    public void ApplyForceDown()
    {
        playerRb.AddForce(new Vector3(0f, -1.5f, 0f), ForceMode.Impulse);
    }

    public void RunToJumpForceSmoothing()
    {
        playerRb.AddForce(new Vector3(0f, jumpHeight*.8f, 0f), ForceMode.Force);//This will apply the force over multiple calls.
        //playerRb.AddForce(new Vector3(0f, jumpHeight, 0f), ForceMode.Impulse);
        //playerRb.AddForce(transform.forward * jumpForwardPush * InputManager.newMove.z, ForceMode.Impulse);
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
}