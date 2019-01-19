using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookCharacterController : MonoBehaviour
{

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
    void Start()
    {
    }

    private void Update()
    {
        if (cloneHook != null)//Set the position of the line renderer so the rope can be shot between these two points.
        {
            lineR.positionCount = 2;
            lineR.SetPosition(0, gauntlet.position);
            lineR.SetPosition(1, cloneHook.transform.position);
        }
        else
        {//This is to fix the gimbal averaging the position of the line renderer and the character. Its fucking annoying. IT DIDNT WORK FUCK
            lineR.positionCount = 0;
            //lineR.SetPosition(0, gauntlet.position);
            //lineR.SetPosition(1, gauntlet.position);
        }
    }

    void FixedUpdate()
    {
        leftHandPosition = transform.InverseTransformPoint(leftHand.transform.position);//This finds the position of the left hand relative to the rigidbody's position
        if (cloneHook != null)
        {
            cloneHook.transform.rotation = Quaternion.LookRotation(cloneHook.transform.position - leftHand.transform.position);
        }

        if (GrapplingHookMode)
        {
            if ((playerRb.velocity.x > grapSpeedThreshold || playerRb.velocity.x < -grapSpeedThreshold)
                || (playerRb.velocity.y > grapSpeedThreshold || playerRb.velocity.y < -grapSpeedThreshold)
                || (playerRb.velocity.z > grapSpeedThreshold || playerRb.velocity.z < -grapSpeedThreshold))
            {
                spring.damper = 80f;
            }
            else
            {
                spring.damper = 10f;
            }

            spring.anchor = leftHandPosition;
            spring.connectedAnchor = grapplingHook.transform.position;
        }

    }

    public void FireGrappleHook()
    {
        layerMask = 1 << 8;
        layerMask = ~layerMask;


        //SET UP THE GRAPPLING HOOK
        RaycastHit hit;
        //Debug.DrawRay(mainCam.transform.position, InputManager.camForward, Color.red, 5f);
        if (Physics.Raycast(leftHand.transform.position, mainCam.transform.forward.normalized, out hit, grappleHookDistance, layerMask))
        {
            Debug.DrawRay(leftHand.transform.position, mainCam.transform.forward.normalized * grappleHookDistance, Color.green, 5f);
            cloneHook = Instantiate(grapplingHook, leftHand.transform.position + new Vector3(0f, -.01f, 0f), Quaternion.Euler(0f, 0f, 0f)) as GameObject;
            grapplingHookRb = cloneHook.transform.GetComponent<Rigidbody>();
            grapplingHookRb.AddForce(mainCam.transform.forward.normalized * GrapplingHookSpeed, ForceMode.VelocityChange);

            lineR.enabled = true;
            playerAnim.CrossFadeInFixedTime(HashTable.grappleShootState, .3f);
        }
        else
        {
            Debug.Log("GRAPPLING HOOK COULD NOT FIND TARGET");
        }
    }

    public void Grapple()
    {
        //Set up the component
        spring = this.gameObject.AddComponent(typeof(SpringJoint)) as SpringJoint;
        spring.autoConfigureConnectedAnchor = false;
        spring.connectedBody = grapplingHookRb;
        //spring.anchor = cloneHook.transform.position;
        spring.spring = springStrength;

        GrapplingHookMode = true;
        playerAnim.applyRootMotion = false;
    }

    public void DestroyGrappleHook()//We disable the line renderer, destroy the spring joint component, and the clonehook.
    {
        lineR.enabled = false;
        if (cloneHook != null)
        {
            if (spring != null)
            {
                Destroy(GetComponent(typeof(SpringJoint)));//Component
            }
            Destroy(cloneHook);//Prefab Hook
        }
    }
}
