using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{
    private bool firstTimeOutOfWall = false;
    public Animator playerAnim;
    public Transform player;
    public bool controllerSupport;
    public static float distance = 5.0f;
    [SerializeField]
    private float setDistance;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -50f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    private new Rigidbody rigidbody;

    public LayerMask layerMask;

    float x = 0.0f;
    float y = 0.0f;

    //CAMERA LERP AND SLERP CONTROLS
    private Vector3 velocity = Vector3.zero;
    //private Vector3 heightOffset = new Vector3(0f, 1f, 0f);


    //TARGET MODE VARIABLES
    public static bool targetMode;
    public static Vector3 targetDir;
    public static Transform targetTrans = null;//Make private later when there are multiple targetable objects
    public static List<GameObject> targetableObjects = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        //layerMask = 1 << 8;
        //layerMask = ~layerMask;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
    }

    void FixedUpdate()//THIS HAS TO BE FIXEDUPDATE IN ORDER TO NOT HAVE CAMERA JITTER
    {
        if(targetTrans == null && targetMode)//Select which targetable object we want to target
        {
            float currentLeastAngle = 180f;//This is 180 so that we are guaranteed to get a currentLeastAngle if there is a targetable object (Vector3.Angle will not go above 180 because its unsigned).
            GameObject bufferTargetObject = new GameObject();//Forced to declare this for safety (The list could be empty)

            foreach(GameObject obj in targetableObjects)//This can run even if the list is empty
            {
                Vector3 currentCompare = obj.transform.position - transform.position;//This gives us the current we are comparing to the trabsform.forward of the camera
                float angle = Vector3.Angle(this.transform.forward, currentCompare);

                if (angle < currentLeastAngle)
                {
                    currentLeastAngle = angle;//This will keep reference to the smallest angle relative to the camera's forward transform.
                    bufferTargetObject = obj;//This will keep reference to the gameobject with the smallest angle relative to the camera's forward transform.
                }
            }
            if (targetableObjects.Count == 0)
            {
                targetTrans = null;
                targetMode = false;
            }
            else
            {
                targetTrans = bufferTargetObject.transform;
            }
        }//TargetMode Math

        if (player && !targetMode)
        {
            targetTrans = null;
            targetDir = Vector3.zero;
            if (controllerSupport)
            {
                x += Input.GetAxis("RStickX") * xSpeed * 0.02f;
                y -= Input.GetAxis("RStickY") * ySpeed * 0.02f;
            }
            else{
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            }

            if (!playerAnim.GetBool(HashTable.onGroundParam))//The camera can go below the character when the character is in the air. This is precursor to grappling hook implementation
            {
                yMinLimit = -50f;
            }
            else
            {
                yMinLimit = -5f;
            }

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            //START CAMERA RAYCAST CALCULATIONS
            //This section of code defines what the camera should do when it clips through another object, wall/block/ whatever.
            RaycastHit hit;
            if (Physics.Linecast(player.position+new Vector3(0f,1f,0f), transform.position, out hit, layerMask))//Snaps camera back to character if there is a collider between the camera and the player
            {
                Debug.DrawLine(player.position + new Vector3(0f, 1f, 0f), transform.position, Color.red, 5f);
                distance = hit.distance;
                firstTimeOutOfWall = true;
            }
            else
            {
                RaycastHit rayHit;
                //Debug.DrawRay(player.position + new Vector3(0f, 1f, 0f), transform.position - (player.position + new Vector3(0f, 1f, 0f)), Color.blue, 5f);
                if(!Physics.Raycast(player.position + new Vector3(0f, 1f, 0f) , transform.position - (player.position + new Vector3(0f, 1f, 0f)), out rayHit, setDistance))
                {
                    if (firstTimeOutOfWall)//Right after the camera comes out of a clipped object, I want to reset it to the distance it had before it clipped into the object.
                    {
                        distance = setDistance;
                        setDistance = distance;
                        firstTimeOutOfWall = false;
                    }
                    else//This is just to update the setDistance var which we use to define the camera distance before the camera clipped into an object.
                    {
                        setDistance = distance;
                        distance = setDistance;
                    }
                }
            }//END CAMERA RAYCAST CALCULATIONS

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + player.position + new Vector3(0, 1, 0);//Places the camera on a rotation around the target position

            transform.rotation = rotation;
            transform.position = position;
        }
        else
        {
            if(targetTrans != null)//None of this code can run if there is no target
            {
                transform.LookAt(targetTrans);
                targetDir = targetTrans.position - player.position;

                Quaternion rotation = Quaternion.LookRotation(targetDir.normalized, transform.up);//This rotates the camera around the character so that the character is always between the camera and the target
                //rotation.z = 0f;
                //rotation.x = 0f;

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

                RaycastHit hit;
                if (Physics.Linecast(player.position, transform.position, out hit))//Snaps camera back to character if there is a collider between the camera and the player
                {
                    distance -= hit.distance;
                }
                Vector3 negDistance = new Vector3(0.0f, 0f, -distance);
                Vector3 position = rotation * negDistance + player.position; //+ new Vector3(0, 1, 0);//Places the camera on a rotation around the target position
                position.y = player.position.y; //+ 1f;

                //These are the calculations for smoothing the position of the camera
                transform.position = Vector3.SmoothDamp(transform.position, position/* + heightOffset*/, ref velocity, .1f, 10f);

                //These are the calculations for slerping the rotation of the camera
                //timeCount = timeCount + Time.deltaTime;
                //float step = 35f * Time.deltaTime;
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, step);
            }
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}