using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public int enemyHealth = 400;

    public GameObject hitParticlesBurstPrefab;
    private GameObject hitParticlesBurstClone;
    private ParticleSystem hitParticles;
    public GameObject playerSword;

    //public SkinnedMeshRenderer slimeBossSMR;
    public Animator sbAnim;
    public Animator sbLAnim;
    public Animator sbRAnim;

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (enemyHealth <= 0)//Start the animator for death. Death actually happens on the behaviour attached to these animation states.
        {
            sbAnim.CrossFadeInFixedTime(HashTable.slimeBossDyingState, 1f);
            sbLAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmDying, 1f);
            sbRAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmDying, 1f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)//Character Weapon
        {
            hitParticlesBurstClone = Instantiate(hitParticlesBurstPrefab, playerSword.transform.position, Quaternion.Euler(0f, 0f, 0f));
            hitParticles = hitParticlesBurstClone.GetComponent<ParticleSystem>();
            hitParticles.Emit(20);
            Destroy(hitParticlesBurstClone, 1f);

            enemyHealth -= 10;//Reduce the enemy's health
            Debug.Log(enemyHealth);
            other.enabled = false;//This forces them to press the attack button again in order to get another collision
        }
    }

    void Death()
    {
        Destroy(this.gameObject);
    }
}
