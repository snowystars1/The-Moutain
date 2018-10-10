using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateStuff;
using RootMotion.FinalIK;
using DG.Tweening;

public class AI : MonoBehaviour {

    //public bool switchState = false;
    public static int switchState = 3;

    public static float randomChoice = -1f;

    public Animator sBLeftAnim;
    public Animator sBRightAnim;
    public Animator sBAnim;//Defined in Start

    public CCDIK ikL;
    public CCDIK ikR;
    public static Tweener twL;
    public static Tweener twR;

    public GameObject armLIKPoint;
    public GameObject armRIKPoint;

    public GameObject player;

    public StateMachine<AI> stateMachine { get; set; }

	// Use this for initialization
	void Start () {
        stateMachine = new StateMachine<AI>(this);
        stateMachine.ChangeState(BackState.Instance);

        sBAnim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        stateMachine.Update();
	}
}
