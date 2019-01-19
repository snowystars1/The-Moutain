using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;

public class BackState : State<AI>
{

    private static BackState instance;

    private BackState()
    {
        if (instance != null)//If an isntance of this class already exists, return
            return;

        instance = this;
    }

    public static BackState Instance//This is the get of our property defined in StateMachine
    {
        get
        {
            if (instance == null)
                new BackState();

            return instance;
        }
    }

    public override void EnterState(AI boss)
    {
        Debug.Log("Entering Back State");
    }

    public override void ExitState(AI boss)
    {
        Debug.Log("Exiting Back State");
    }

    public override void UpdateState(AI boss)
    {
        if (AI.switchState == 1)
        {
            boss.stateMachine.ChangeState(RightState.Instance);
        }

        if (AI.switchState == 2)
        {
            boss.stateMachine.ChangeState(LeftState.Instance);
        }

        if (AI.randomChoice != -1f && EnemyHealth.enemyHealth > 0)
        {
            if (AI.randomChoice >= 0f && AI.randomChoice <= .45f)
            {
                boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmGlobCharge, .3f);
                boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmGlobCharge, .3f);
                boss.sBAnim.CrossFadeInFixedTime(HashTable.slimeBossGlobChargeState, .3f);
            }

            if (AI.randomChoice > .45f && AI.randomChoice <= .9f)
            {
                boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmSuck, .3f);
                boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmSuck, .3f);
                boss.sBAnim.CrossFadeInFixedTime(HashTable.slimeBossSuckState, .3f);
            }

            if (AI.randomChoice > .9f && AI.randomChoice <= 1f)
            {
                boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmVulnerable, .5f);
                boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmVulnerable, .5f);
            }


        }
    }
}