using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRocks : MonoBehaviour {

    public Animator slimeBossAnim;
    public GameObject fallingRockPrefab;
    public GameObject groundIndicatorPrefab;
    private GameObject cloneRock;
    private GameObject groundIndicator;
    private Rigidbody rockRb;
    public float rockFallForce = -1f;

    public static bool intoRumble = false;

    private void Start()
    {
        InvokeRepeating("RocksFall", 1f, 2f);
    }

    // Update is called once per frame
    void Update () {

	}

    void RocksFall()
    {
        if (slimeBossAnim != null)
        {
            cloneRock = Instantiate(fallingRockPrefab, new Vector3(Random.Range(-40f, 40f), 44f, Random.Range(-50f, 0f)), Quaternion.Euler(new Vector3(Random.Range(0f, 359f), Random.Range(0f, 359f), Random.Range(0, 359f))));
            groundIndicator = Instantiate(groundIndicatorPrefab, new Vector3(cloneRock.transform.position.x, 0f, cloneRock.transform.position.z), Quaternion.Euler(0f, 0f, 0f));
            rockRb = cloneRock.GetComponent<Rigidbody>();
            rockRb.AddForce(new Vector3(0f, rockFallForce, 0f), ForceMode.Impulse);
            Destroy(cloneRock, 15f);
            Destroy(groundIndicator, 2.5f);
        }
    }
}
