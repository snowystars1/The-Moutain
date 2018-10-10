using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossBackAnimationEvents : MonoBehaviour {

    public GameObject groundIndicator;
    private GameObject groundIndicatorClone;

    public GameObject globPrefab;
    private GameObject globClone;

    public void GenerateNumber3()
    {
        if (AI.switchState != 3)
        {
            return;
        }
        Random.InitState(System.DateTime.Now.Millisecond);//Fucking seed
        AI.randomChoice = .2f;//Random.Range(0f, 1f);
        Debug.Log(AI.randomChoice);
    }

    public void ActivateGlobShot()
    {
        InvokeRepeating("GlobShoot",0f, .1f);
    }

    public void StopGlobShot()
    {
        CancelInvoke();
    }

    public void GlobShoot()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        float zPower = Random.Range(8f, 17f);
        globClone = Instantiate(globPrefab, new Vector3(Random.Range(-35f, 35f), 20f, Random.Range(-60f, -55f)), Quaternion.Euler(0f, 0f, 0f));
        Rigidbody rb = globClone.GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(0f, 10f, zPower), ForceMode.Impulse);
        float t;
        float z;
        t = (-10 - Mathf.Sqrt(100f - (2f * (-9.81f * globClone.transform.position.y)))) / -9.81f;//Projectile motion
        z = zPower * t;


        RaycastHit hit;

        int layers = 0;
        layers = (1 << 9);

        Debug.DrawRay(new Vector3(globClone.transform.position.x, 5f, globClone.transform.position.z + z), Vector3.down, Color.blue, 2f);
        if (Physics.Raycast(new Vector3(globClone.transform.position.x, 5f, globClone.transform.position.z + z), Vector3.down, out hit, 7f, layers)){
            groundIndicatorClone = Instantiate(groundIndicator, hit.point + new Vector3(0f,.5f,0f), Quaternion.Euler(0f, 0f, 0f));
        }
        else
        {
            groundIndicatorClone = Instantiate(groundIndicator, new Vector3(globClone.transform.position.x, 0f, globClone.transform.position.z + z), Quaternion.Euler(0f, 0f, 0f));
        }

        Destroy(groundIndicatorClone, 4f);
        Destroy(globClone, 5f);
    }
}
