using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookCharacterController : MonoBehaviour {

    public GameObject grapplingHook;
    private Rigidbody grapplingHookRb;
    private GameObject cloneHook;

    public float grappleHookDistance;
    public float GrapplingHookSpeed;
    public float grapSpeedThreshold;
    public static bool GrapplingHookMode = false;
    public float springStrength = 5f;

    public GameObject rope;
    private GameObject cloneRope;
    private Vector3 ropeDir;
    private Transform actualRope;

    public LineRenderer lineR;
    public Transform gauntlet;

    public Transform mainCam;

    public Transform leftHand;
    public Animator playerAnim;
    public Rigidbody playerRb;
    private SpringJoint spring;

    int layerMask;

    private Vector3 leftHandPosition;
    //private GameObject cloneRope2;
	// Use this for initialization
	void Start () {
	}

    private void Update()
    {
        if (cloneHook != null)
        {
            lineR.SetPosition(0, new Vector3(0f,0f,0f));
            lineR.SetPosition(1, cloneHook.transform.position);
        }
    }

    void FixedUpdate()
    {
        leftHandPosition = transform.InverseTransformPoint(leftHand.transform.position);//This finds the position of the left hand relative to the rigidbody's position
        if(cloneHook != null)
        {
            //ropeDir = cloneHook.transform.position - leftHand.transform.position;
            cloneHook.transform.rotation = Quaternion.LookRotation(cloneHook.transform.position - leftHand.transform.position);
        }
        //if(cloneRope != null)
        //{
        //    actualRope = cloneRope.transform.GetChild(0);
        //    cloneRope.transform.position = (cloneHook.transform.position + leftHand.transform.position) / 2;
        //    cloneRope.transform.rotation = Quaternion.LookRotation(ropeDir);
        //    actualRope.localScale = new Vector3(.05f, Vector3.Distance(cloneHook.transform.position, leftHand.transform.position) / 2, .05f);
        //}

        if (GrapplingHookMode)
        {
            if((playerRb.velocity.x > grapSpeedThreshold || playerRb.velocity.x < -grapSpeedThreshold) 
                || (playerRb.velocity.y > grapSpeedThreshold || playerRb.velocity.y < -grapSpeedThreshold) 
                || (playerRb.velocity.z > grapSpeedThreshold || playerRb.velocity.z < -grapSpeedThreshold))
            {
                spring.damper = 80f;
            }
            else
            {
                spring.damper = 10f;
            }
            //playerRb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;// | RigidbodyConstraints.FreezeRotationX;

            spring.anchor = leftHandPosition;
            spring.connectedAnchor = grapplingHook.transform.position;

            //Transform actualRope = cloneRope.transform.GetChild(0);
            //cloneRope.transform.position = (cloneHook.transform.position + leftHand.transform.position)/2;
            //cloneRope.transform.rotation = Quaternion.LookRotation(cloneHook.transform.position - leftHand.transform.position);
            //actualRope.localScale = new Vector3(.05f, Vector3.Distance(cloneHook.transform.position, leftHand.transform.position)/2, .05f);
        }
        else
        {
            //playerRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

    }

    public bool FireGrappleHook()
    {
        if (cloneHook != null)
        {
            return false;
        }

        layerMask = 1 << 8;
        layerMask = ~layerMask;


        //SET UP THE GRAPPLING HOOK
        RaycastHit hit;
        //Debug.DrawRay(mainCam.transform.position, InputManager.camForward, Color.red, 5f);
        if (Physics.Raycast(leftHand.transform.position, mainCam.transform.forward.normalized, out hit, grappleHookDistance, layerMask))
        {
            Debug.DrawRay(leftHand.transform.position, mainCam.transform.forward.normalized * grappleHookDistance, Color.green, 5f);
            cloneHook = Instantiate(grapplingHook, leftHand.transform.position + new Vector3(0f,-.01f,0f), Quaternion.Euler(0f,0f,0f)) as GameObject;
            grapplingHookRb = cloneHook.transform.GetComponent<Rigidbody>();
            grapplingHookRb.AddForce(mainCam.transform.forward.normalized * GrapplingHookSpeed, ForceMode.VelocityChange);

            lineR.enabled = true;
        }
        else
        {
            Debug.Log("GRAPPLING HOOK COULD NOT FIND TARGET");
            return false;
        }
        return true;

        //SET UP THE ROPE
        //cloneRope = Instantiate(rope, (cloneHook.transform.position + leftHand.transform.position) / 2, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
    }

    public void Grapple()
    {
        //Set up the component
        spring = this.gameObject.AddComponent(typeof(SpringJoint)) as SpringJoint;
        spring.connectedBody = grapplingHookRb;
        spring.autoConfigureConnectedAnchor = false;
        //spring.anchor = cloneHook.transform.position;
        spring.spring = springStrength;

        //Set up the Rope


        GrapplingHookMode = true;
        playerAnim.applyRootMotion = false;
    }

    public void DestroyGrappleHook()
    {
        lineR.enabled = false;
        if(cloneHook != null)
        {
            if(spring != null)
            {
                Destroy(spring);//Component
            }
            Destroy(cloneHook);//Prefab Hook
            //Destroy(cloneRope);//Prefab Rope
        }
    }
}
