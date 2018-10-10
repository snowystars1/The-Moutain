using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossRArmAnimationEvents : MonoBehaviour {

    public ParticleSystem dustParticles;
    public GameObject dustKickGameObject;

    public GameObject VulnerableParticlesPrefab;
    public Transform slimeBossTrans;

    public void GenerateNumber1()
    {
        if (AI.switchState != 1)
        {
            return;
        }
        Random.InitState(System.DateTime.Now.Millisecond);//Fucking seed
        AI.randomChoice = Random.Range(0f, 1f);
        Debug.Log(AI.randomChoice);
    }

    public void DustSwipe()
    {
        dustParticles.Play();
    }

    public void DustKickDisable()
    {
        dustKickGameObject.SetActive(false);
    }

    public void DustKick()
    {
        dustKickGameObject.SetActive(true);
    }

    public void VulnerableParticles()
    {
        GameObject particles = Instantiate(VulnerableParticlesPrefab, slimeBossTrans.position + new Vector3(0f, -6.3f, 0f), Quaternion.Euler(0f, 0f, 0f));
        Destroy(particles, 7.8f);
    }
}
