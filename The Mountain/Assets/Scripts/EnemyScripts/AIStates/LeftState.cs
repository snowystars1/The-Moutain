using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;
using DG.Tweening;

public class LeftState : State<AI>
{

    private bool isSlapping = false;
    private bool slapStep3 = false;
    private bool slapStep4 = false;
    private float timer = 0f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 slapUpPosition = new Vector3(-38f, 13f, -45f);
    private Vector3 idlePosition = new Vector3(-49f, 3f, -46.5f);
    private static LeftState instance;

    private LeftState()
    {
        if (instance != null)//If an isntance of this class already exists, return
            return;

        instance = this;
    }

    public static LeftState Instance//This is the get of our property defined in StateMachine
    {
        get
        {
            if (instance == null)
                new LeftState();

            return instance;
        }
    }

    public override void EnterState(AI boss)
    {
        Debug.Log("Entering Left State");
        //if (boss.sBLeftAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.slimeBossLArmIdle)
        //{
        //    AI.randomChoice = boss.GenerateNumber();
        //}
        //Slap(boss);
    }

    public override void ExitState(AI boss)
    {
        Debug.Log("Exiting Left State");
        AI.twR.Kill(true);
        AI.twL.Kill(true);
        boss.sBLeftAnim.enabled = true;
        boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmIdle, .3f);
        if (boss.sBLeftAnim.enabled == false)//We were in the middle of a slap when the character left the trigger area
        {//THIS IS A FULL RESET OF ANYTHING WE DINK AROUND WITH DURING THE SLAP
            //AI.twL.Kill(true);
            boss.ikL.solver.target = boss.armLIKPoint.transform;
            boss.ikL.solver.IKPositionWeight = 1f;
            isSlapping = false;
            boss.armLIKPoint.transform.position = idlePosition;
            timer = 0f;
            boss.sBLeftAnim.enabled = true;
            boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmIdle, .3f);
        }
    }

    public override void UpdateState(AI boss)
    {
        if (AI.switchState == 1)
        {
            boss.stateMachine.ChangeState(RightState.Instance);
        }

        if (AI.switchState == 3)
        {
            boss.stateMachine.ChangeState(BackState.Instance);
        }

        //STATES BASED ON RANDOM NUMBER
        if (AI.randomChoice != -1f && EnemyHealth.enemyHealth > 0)
        {
            if (AI.randomChoice >= 0f && AI.randomChoice <= .3f)//30% chance to swipe
            {
                boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmSwipe, .3f);
            }
            if (AI.randomChoice > .3f && AI.randomChoice <= .6f)//30% chance to slap
            {
                if (!isSlapping)
                {
                    Slap(boss);
                }
            }
            if (AI.randomChoice > .6f && AI.randomChoice <= .7f)//10% chance to heal
            {
                EnemyHealth.heal = false;
                boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmVulnerable, .5f);
                boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmVulnerable, .5f);
            }
            if (AI.randomChoice > .7f && AI.randomChoice <= .9f)//25% chance to rumble
            {
                boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmRumble, .4f);
                boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmRumble, .4f);
                boss.sBAnim.CrossFadeInFixedTime(HashTable.slimeBossRumbleState, .4f);
            }
            if (AI.randomChoice > .9f && AI.randomChoice <= 1f)//10% chance to suck/blow
            {
                boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmSuck, .3f);
                boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmSuck, .3f);
                boss.sBAnim.CrossFadeInFixedTime(HashTable.slimeBossSuckState, .3f);
            }
        }


        //STEP 2 OF THE SLAP
        if (!slapStep3)//This happens when the IKL target is chasing the character around
        {
            boss.armLIKPoint.transform.position = Vector3.SmoothDamp(boss.armLIKPoint.transform.position, boss.player.transform.position, ref velocity, .5f);
        }

        if (Approximate(boss.ikL.solver.IKPositionWeight, .1f) && isSlapping && !slapStep3)//THIS IS STEP 3 OF THE SLAP
        {
            boss.ikL.solver.IKPositionWeight = 0;//Reset weight so the bones hold position
            AI.twL.Kill(true);//Kill the last tween so we aren't stacking tweens.
            AI.twL = DOTween.To(() => boss.ikL.solver.IKPositionWeight, x => boss.ikL.solver.IKPositionWeight = x, 1f, 10);//Second Tween in the series, UpPosition->Player
            boss.ikL.solver.target = boss.armLIKPoint.transform;
            slapStep3 = true;
            timer = Time.time;//Take a snapshot of the current time, so we can add a delay between when the arm hits the ground, and when it starts to return to idle postion
        }

        /////////THESE ARE STEPS 4 AND 5 OF THE SLAP
        if (Time.time - timer > 1f && timer != 0 && isSlapping && !slapStep4)//1 second after his hand has slapped the ground
        {
            boss.ikL.solver.IKPositionWeight = 0f;//Reset weight so the bones hold position
            AI.twL.Kill(true);//Kill the last tween so we aren't stacking tweens.
            boss.armLIKPoint.transform.position = idlePosition;
            AI.twL = DOTween.To(() => boss.ikL.solver.IKPositionWeight, x => boss.ikL.solver.IKPositionWeight = x, 1f, 25);//3rd Tween in the series, Player->IdlePosition
            slapStep4 = true;//We dont want to run this fxn more than once
        }

        if (Time.time - timer > 3f && timer != 0 && isSlapping)//2seconds after his hand hit the ground, (1 second after returning to idle)
        {
            AI.twL.Kill();//Kill the last tween
            boss.ikL.solver.IKPositionWeight = 1f;//Reset the position weight so all the animations work well
            boss.sBLeftAnim.enabled = true;//Back to normal animation
            timer = 0f;//make sure the timer is reset as well
            AI.randomChoice = -1f;
            slapStep4 = false;
            slapStep3 = false;
            isSlapping = false;
            boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmIdle, .5f);
            //AI.randomChoice = boss.GenerateNumber();
        }
        /////////
    }

    private void Slap(AI boss)//THIS IS STEP 1 OF THE SLAP
    {
        isSlapping = true;
        boss.sBLeftAnim.enabled = false;
        boss.ikL.solver.target = null;//We have to dislocate the target so the target can move around and track the player in step2 while the arm stays in a single location
        boss.ikL.solver.IKPositionWeight = 0f;//Reset weight so the bones hold position
        boss.ikL.solver.SetIKPosition(slapUpPosition);//Simulating a target by setting the Ik position, while dislocating the target and having that one chase the player
        AI.twL = DOTween.To(() => boss.ikL.solver.IKPositionWeight, x => boss.ikL.solver.IKPositionWeight = x, .1f, 15);//This is the first Tween in the series Idle->UpPosition

    }

    bool Approximate(float first, float second)
    {
        //All three values have to be within .1f of the actual target in order to be considered true.
        if (Mathf.Abs(first - second) < .07f)//This .07 number is essentially the delay between when the arm goes up in the air and when it comes down to hit the player.
        {
            return true;
        }
        return false;
    }
}