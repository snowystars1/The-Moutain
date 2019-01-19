using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public static int enemyHealth = 100;
    private bool isOnFire = false;
    public static bool heal = false;//We turn this to false here and in the LEFT/RIGHT AI STATE FOR THE SLIMEBOSS
    private int vulnCount = 0;

    public GameObject hitParticlesBurstPrefab;
    private GameObject hitParticlesBurstClone;
    private ParticleSystem hitParticles;
    public GameObject playerSword;

    public RectTransform healthBar;
    public ParticleSystem heatlhParticles;
    private float healthMax;

    private ParticleSystem fireParticles;

    //public SkinnedMeshRenderer slimeBossSMR;
    public Animator sbAnim;
    public Animator sbLAnim;
    public Animator sbRAnim;

    void Start()
    {
        fireParticles = GetComponent<ParticleSystem>();//Get particles on this object (slimeboss)
        healthMax = healthBar.offsetMax.x;//This will always be the top value for the healthbar (full heath)
        sbAnim.SetInteger(HashTable.enemyHealthParam, enemyHealth);
        sbRAnim.SetInteger(HashTable.enemyHealthRParam, enemyHealth);
        sbLAnim.SetInteger(HashTable.enemyHealthLParam, enemyHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (AI.randomChoice > .6f && AI.randomChoice <= .7f && heal == false)//5% chance to heal
        {
            if (healthBar.offsetMax.x <= (healthMax * .95f))//if the current health is lower than 95% of the max health, we can health by 5% without going over the maxhealth
            {
                heal = true;
                enemyHealth += 5;
                sbAnim.SetInteger(HashTable.enemyHealthParam, enemyHealth);
                sbRAnim.SetInteger(HashTable.enemyHealthRParam, enemyHealth);
                sbLAnim.SetInteger(HashTable.enemyHealthLParam, enemyHealth);
                healthBar.offsetMax = new Vector2(healthBar.offsetMax.x + 23f, healthBar.offsetMax.y);//5% heal
                heatlhParticles.Play();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 16)//EnemyHazard layer (Chandelier)
        {
            Destroy(other);
            enemyHealth -= 25;
            sbAnim.SetInteger(HashTable.enemyHealthParam, enemyHealth);
            sbRAnim.SetInteger(HashTable.enemyHealthRParam, enemyHealth);
            sbLAnim.SetInteger(HashTable.enemyHealthLParam, enemyHealth);
            healthBar.offsetMax = new Vector2(healthBar.offsetMax.x - 115f, healthBar.offsetMax.y);//25% damage per hit to health bar
            heatlhParticles.Play();
            vulnCount++;
            StartCoroutine("onFire");
        }

        if (other.gameObject.layer == 11 && (isOnFire || (vulnCount == 3)))//If slimeboss is on fire, he is vulnerable. If he has been lit on fire 3 times and still not dead, he is vulnerable.
        {
            healthBar.offsetMax = new Vector2(healthBar.offsetMax.x - 4.6f, healthBar.offsetMax.y);//1.5% damage per hit to health bar
            heatlhParticles.Play();

            hitParticlesBurstClone = Instantiate(hitParticlesBurstPrefab, playerSword.transform.position, Quaternion.Euler(0f, 0f, 0f));
            hitParticles = hitParticlesBurstClone.GetComponent<ParticleSystem>();
            hitParticles.Emit(20);
            Destroy(hitParticlesBurstClone, 1f);

            enemyHealth -= 1;//Reduce the enemy's health by 1.5%
            sbAnim.SetInteger(HashTable.enemyHealthParam, enemyHealth);
            sbRAnim.SetInteger(HashTable.enemyHealthRParam, enemyHealth);
            sbLAnim.SetInteger(HashTable.enemyHealthLParam, enemyHealth);
            other.enabled = false;//This forces them to press the attack button again in order to get another collision
        }
    }

    IEnumerator onFire()
    {
        isOnFire = true;
        fireParticles.Play();
        yield return new WaitForSeconds(17.5f);
        isOnFire = false;
    }

    void Death()
    {
        Destroy(this.gameObject);
    }
}
