using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField]
    private int playerHealth = 100;
    public RectTransform healthBar;
    public ParticleSystem heatlhParticles;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other)//This will recognize all trigger colliders across all the children of the parent object (character).
    {
        if (other.collider.gameObject.layer == 12 || other.collider.gameObject.layer == 14)
        {
            healthBar.offsetMax = new Vector2(healthBar.offsetMax.x - 46f, healthBar.offsetMax.y);//Damage per hit to health bar
            heatlhParticles.Play();
            playerHealth -= 10;//Reduce the players health
            Debug.Log(playerHealth);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12)//If the other gameObject is of the layer "Enemy"
        {
            playerHealth -= 10;
            healthBar.offsetMax = new Vector2(healthBar.offsetMax.x - 46f, healthBar.offsetMax.y);//Damage per hit to health bar
            heatlhParticles.Play();
            Debug.Log(playerHealth);
        }
    }
}
