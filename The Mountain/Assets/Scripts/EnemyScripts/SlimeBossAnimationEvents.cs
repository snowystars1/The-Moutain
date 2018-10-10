using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossAnimationEvents : MonoBehaviour {

    public GameObject fallingRockPrefab;

    public GameObject slimeBossObj;
    private bool stopSucking = true;

    public GameObject groundIndicator;
    private GameObject groundIndicatorClone;

    public GameObject suckParticlesObj;
    public GameObject blowParticlesObj;
    private GameObject suckParticlesClone;
    private GameObject blowParticlesClone;

    Collider[] colliders;
    Rigidbody[] rbs;

    public GameObject blowPoint;
    //private Rigidbody blowPointRb;

    public LayerMask ExplosionMask;

    public float explosionForce = -300f;
    public float explosionRadius = 40f;

    // Use this for initialization
    void Awake()
    {
        suckParticlesClone = Instantiate(suckParticlesObj, slimeBossObj.transform.position + new Vector3(0f, 20f, 0f), Quaternion.Euler(0f, 0f, 0f));
        blowParticlesClone = Instantiate(blowParticlesObj, slimeBossObj.transform.position + new Vector3(0f, 20f, 0f), Quaternion.Euler(0f, 0f, 0f));
        suckParticlesClone.SetActive(false);
        blowParticlesClone.SetActive(false);
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (!stopSucking)
        {
            foreach (Rigidbody rb in rbs)//This will get the rigidbodies of a bunch of rocks in a radius, and apply the suck explosion force to them. 
            {
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, slimeBossObj.transform.position + new Vector3(0f, 20f, 10f), explosionRadius);
                }
            }
        }
    }

    public void RocksSuck()//This is called from an animation event on SlimeBoss_Suck1
    {
        blowParticlesClone.SetActive(false);//Use this to reset the non-looping on the blow particle
        suckParticlesClone.SetActive(true);

        for(int i = 0; i < 5; i++)
        {
            Instantiate(fallingRockPrefab, slimeBossObj.transform.position + new Vector3(Random.Range(-3f,3f), 20f, Random.Range(-3f,3f)), Quaternion.Euler(0f, 0f, 0f));//Spawn 5 rocks that shouldn't break because the noBreak static bool is true;
        }

        colliders = Physics.OverlapSphere(slimeBossObj.transform.position, explosionRadius, ExplosionMask); //This can be made more efficient because I think it's an arraylist and it doesn't need to be.
        rbs = new Rigidbody[colliders.Length];
        for(int i = 0; i < rbs.Length; i++)
        {
            rbs[i] = colliders[i].GetComponent<Rigidbody>();
        }
        stopSucking = false;
    }

    public void RocksBlow()//This is called from an animation event on SlimeBoss_Blow1
    {
        suckParticlesClone.SetActive(false);
        blowParticlesClone.SetActive(true);
        stopSucking = true;
        //colliders = Physics.OverlapSphere(slimeBossObj.transform.position, explosionRadius, ExplosionMask); //This can be made more efficient because I think it's an arraylist and it doesn't need to be.
        foreach(Rigidbody rb in rbs)//This will get the rigidbodies of a bunch of rocks in a radius, and apply the suck explosion force to them. 
        {
            if (rb == null)
            {
                continue;
            }
            //rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            rb.isKinematic = false;
            rb.AddForce(new Vector3(0f, 5f, 7f), ForceMode.Impulse);
            //Calculate where to place particle ground indicator
            float t;
            float z;
            t = (-5 - Mathf.Sqrt(25f-(2f*(-9.81f * rb.transform.position.y)))) / -9.81f;//Projectile motion
            //Debug.Log(t);
            z = 7f * t;

            groundIndicatorClone = Instantiate(groundIndicator, new Vector3(rb.transform.position.x, 0f, rb.transform.position.z + z), Quaternion.Euler(0f, 0f, 0f));
            Destroy(groundIndicatorClone, 3f);
        }
        RockShatter.noBreak = false;//This allows the rocks to break on impact again
    }
}
