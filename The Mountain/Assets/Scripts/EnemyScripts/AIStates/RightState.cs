using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;
using DG.Tweening;

public class RightState : State<AI> {

    private bool isSlapping = false;
    private bool slapStep4 = false;
    private bool slapStep3 = false;
    private float timer = 0f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 slapUpPosition = new Vector3(38f, 13f, -45f);
    private Vector3 idlePosition = new Vector3(49f, 3f, -46.5f);
    private static RightState instance;

    private RightState()
    {
        if (instance != null)//If an isntance of this class already exists, return
            return;

        instance = this;
    }

    public static RightState Instance//This is the get of our property defined in StateMachine
    {
        get
        {
            if (instance == null)
                new RightState();

            return instance;
        }
    }

    public override void EnterState(AI boss)
    {
        Debug.Log("Entering Right State");
        //if(boss.sBRightAnim.GetCurrentAnimatorStateInfo(0).fullPathHash == HashTable.slimeBossRArmIdle)
        //{
        //    AI.randomChoice = boss.GenerateNumber();
        //}
        //Slap(boss);
    }

    public override void ExitState(AI boss)
    {
        Debug.Log("Exiting Right State");
        AI.twR.Kill(true);
        AI.twL.Kill(true);
        boss.sBRightAnim.enabled = true;
        boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmIdle, .3f);

        if (boss.sBRightAnim.enabled == false)//We were in the middle of a slap when the character left the trigger area
        {//THIS IS A FULL RESET OF ANYTHING WE DINK AROUND WITH DURING THE SLAP
            boss.ikR.solver.target = boss.armRIKPoint.transform;
            boss.ikR.solver.IKPositionWeight = 1f;
            isSlapping = false;
            boss.armRIKPoint.transform.position = idlePosition;
            timer = 0f;
            boss.sBRightAnim.enabled = true;
            boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmIdle, .3f);
        }
    }

    public override void UpdateState(AI boss)
    {
        //SWAPPING STATES
        if (AI.switchState == 2)
        {
            boss.stateMachine.ChangeState(LeftState.Instance);
        }
        if (AI.switchState == 3)
        {
            boss.stateMachine.ChangeState(BackState.Instance);
        }

        //STATES BASED ON RANDOM NUMBER
        if (AI.randomChoice != -1f)
        {
            if (AI.randomChoice >= 0f && AI.randomChoice <= .3f)
            {
                boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmSwipe, .1f);
            }
            if (AI.randomChoice > .3f && AI.randomChoice <= .6f)
            {
                if (!isSlapping)
                {
                    Slap(boss);
                }
            }
            if (AI.randomChoice > .6f && AI.randomChoice <= .8f)
            {
                boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmVulnerable, .5f);
                boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmVulnerable, .5f);
            }
            if (AI.randomChoice > .8f && AI.randomChoice <= .9f)
            {
                boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmRumble, .4f);
                boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmRumble, .4f);
                boss.sBAnim.CrossFadeInFixedTime(HashTable.slimeBossRumbleState, .4f);
            }
            if (AI.randomChoice > .9f && AI.randomChoice <= 1f)
            {
                boss.sBLeftAnim.CrossFadeInFixedTime(HashTable.slimeBossLArmSuck, .3f);
                boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmSuck, .3f);
                boss.sBAnim.CrossFadeInFixedTime(HashTable.slimeBossSuckState, .3f);
            }
        }

        //STEP 2 OF THE SLAP
        if (!slapStep3)//This happens when the IKR target is chasing the character around
        {
            boss.armRIKPoint.transform.position = Vector3.SmoothDamp(boss.armRIKPoint.transform.position, boss.player.transform.position, ref velocity, .5f);
        }

        if (Approximate(boss.ikR.solver.IKPositionWeight, .1f) && !slapStep3 && isSlapping)//THIS IS STEP 3 OF THE SLAP
        {
            boss.ikR.solver.IKPositionWeight = 0;//Reset weight so the bones hold position
            AI.twR.Kill(true);//Kill the last tween so we aren't stacking tweens.
            AI.twR = DOTween.To(() => boss.ikR.solver.IKPositionWeight, x => boss.ikR.solver.IKPositionWeight = x, 1f, 10);//Second Tween in the series, UpPosition->Player
            boss.ikR.solver.target = boss.armRIKPoint.transform;
            slapStep3 = true;
            timer = Time.time;//Take a snapshot of the current time, so we can add a delay between when the arm hits the ground, and when it starts to return to idle postion
        }

        /////////THESE ARE STEPS 4 AND 5 OF THE SLAP
        if (Time.time - timer > 1f && timer != 0 && isSlapping && !slapStep4)//1 second after his hand has slapped the ground
        {
            boss.ikR.solver.IKPositionWeight = 0f;//Reset weight so the bones hold position
            AI.twR.Kill(true);//Kill the last tween so we aren't stacking tweens.
            boss.armRIKPoint.transform.position = idlePosition;
            AI.twR = DOTween.To(() => boss.ikR.solver.IKPositionWeight, x => boss.ikR.solver.IKPositionWeight = x, 1f, 25);//3rd Tween in the series, Player->IdlePosition
            slapStep4 = true;
        }

        if (Time.time - timer > 3f && timer != 0 && isSlapping)//2seconds after his hand hit the ground, (1 second after returning to idle)
        {
            AI.twR.Kill(true);//Kill the last tween
            boss.ikR.solver.IKPositionWeight = 1f;//Reset the position weight so all the animations work well
            boss.sBRightAnim.enabled = true;//Back to normal animation
            timer = 0f;//make sure the timer is reset as well
            AI.randomChoice = -1f;
            slapStep4 = false;
            slapStep3 = false;
            isSlapping = false;
            boss.sBRightAnim.CrossFadeInFixedTime(HashTable.slimeBossRArmIdle, .1f);
            //AI.randomChoice = boss.GenerateNumber();
        }
        /////////
    }

    private void Slap(AI boss)//THIS IS STEP 1 OF THE SLAP
    {
        isSlapping = true;
        boss.sBRightAnim.enabled = false;
        boss.ikR.solver.target = null;//We have to dislocate the target so the target can move around and track the player in step2 while the arm stays in a single location
        boss.ikR.solver.IKPositionWeight = 0f;//Reset weight so the bones hold position
        boss.ikR.solver.SetIKPosition(slapUpPosition);//Simulating a target by setting the Ik position, while dislocating the target and having that one chase the player
        AI.twR = DOTween.To(() => boss.ikR.solver.IKPositionWeight, x => boss.ikR.solver.IKPositionWeight = x, .1f, 15);//This is the first Tween in the series Idle->UpPosition

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
