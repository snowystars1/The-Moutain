using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RockShatter : MonoBehaviour{

    private Animator slimeBossAnim;

    public GameObject shatteredRock;
    private GameObject shatteredRockClone;

    private Tweener tw;

    private MeshRenderer meshR;
    private Rigidbody fallingRockRb;
    public static bool noBreak = false;//Use this bool to stop the rocks from breaking on impact whenever the slimeboss is using the suck ability.


    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("SlimeBoss").GetComponent<Animator>() != null)
        {
            slimeBossAnim = GameObject.FindGameObjectWithTag("SlimeBoss").GetComponent<Animator>();//UGH
        }
        meshR = GetComponent<MeshRenderer>();
        fallingRockRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(slimeBossAnim!=null && slimeBossAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.slimeBossSuckState)
        {
            noBreak = true;
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if(meshR!=null && meshR.enabled == true && !noBreak)
        {
            Vector3 velocityStore = fallingRockRb.velocity;
            Rigidbody rb;
            meshR.enabled = false;//Disable this one so we can't see it. We can't destroy this cause we need the script in order for the shattered rock to shrink and dissapear.
            shatteredRockClone = Instantiate(shatteredRock, transform.position, Quaternion.Euler(transform.rotation.x+90f,transform.rotation.y,transform.rotation.z));
            rb = shatteredRockClone.GetComponent<Rigidbody>();
            rb.velocity = velocityStore;
            foreach (Transform child in shatteredRockClone.transform)
            {
                if (child != null)
                {
                    tw = DOTween.To(() => child.transform.localScale, x => child.transform.localScale = x, new Vector3(0f, 0f, 0f), 4);
                }
            }

            Destroy(shatteredRockClone, 4f);
            Destroy(this.gameObject, 5f);
        }
    }

    private void OnDestroy()//When this object is destroyed
    {
        tw.Kill();
    }

}
