using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StepButton : MonoBehaviour {//This class links to the chandeliers class

    public GameObject pushButton;
    public static int buttonsPushed;
    private bool pushed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!pushed)//This makes it so the player cant push the button twice.
        {
            //The only collision is with the player
            StepButtonDown();
        }
    }

    private void StepButtonDown()
    {
        pushButton.transform.DOMove(pushButton.transform.position - new Vector3(0f, .4f, 0f), 1f);
        buttonsPushed++;
        pushed = true;
    }

    public void StepButtonReset()
    {
        pushButton.transform.DOMove(pushButton.transform.position + new Vector3(0f, .4f, 0f), 1f);
        pushed = false;
    }
}
