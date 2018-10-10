using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCombos : MonoBehaviour
{

    private Animator playerAnim;
    public GameObject playerSword;
    private CapsuleCollider playerSwordCollider;
    // Use this for initialization
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerSwordCollider = playerSword.GetComponent<CapsuleCollider>();
    }

    void SwordSwing2ComboEvent()//Runs at ~66% of the way through the animation
    {
        if (playerAnim.GetBool(HashTable.ComboParam))//This is set to false right when the animation starts, so if this is true, at 66% through the animation, it means they pressed the attack button again.
        {
            playerSwordCollider.enabled = true;
            playerAnim.SetInteger(HashTable.ComboCountParam, 2);//Increase the combo counter
            playerAnim.CrossFade(HashTable.swordStab2State, .25f);//The next animation in the chain
        }
        else
        {
            playerSwordCollider.enabled = false;
        }
    }

    void SwordStab2ComboEvent()
    {
        if (playerAnim.GetBool(HashTable.ComboParam))
        {
            playerSwordCollider.enabled = true;
            playerAnim.SetInteger(HashTable.ComboCountParam, 3);
            playerAnim.CrossFade(HashTable.swordSpin2State, .25f);
        }
        else
        {
            playerSwordCollider.enabled = false;
        }
    }

    void AirUpSwing1ComboEvent()
    {
        if (playerAnim.GetBool(HashTable.ComboParam))
        {
            playerSwordCollider.enabled = true;
            playerAnim.SetInteger(HashTable.ComboCountParam, 2);
            playerAnim.CrossFade(HashTable.airDownSwing1State, .25f);
            playerAnim.SetBool(HashTable.gravityParam, false);
        }
        else
        {
            playerSwordCollider.enabled = false;
        }
    }

    void AirDownSwing1ComboEvent()
    {
        if (playerAnim.GetBool(HashTable.ComboParam))
        {
            playerSwordCollider.enabled = true;
            playerAnim.SetInteger(HashTable.ComboCountParam, 3);
            playerAnim.CrossFade(HashTable.airSlashFinisher1State, .25f);
            playerAnim.SetBool(HashTable.gravityParam, false);
        }
        else
        {
            playerSwordCollider.enabled = false;
        }
    }
}
