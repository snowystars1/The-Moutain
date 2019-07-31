using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void StickMove();
    public static event StickMove Movement;
    public static event StickMove MoveCancel;

    private float controllerInputX;
    private float controllerInputY;

    void Update()
    {
        controllerInputY = Input.GetAxis("Vertical");
        controllerInputX = Input.GetAxis("Horizontal");

        if ((controllerInputX > .1f || controllerInputX < -.1f) || (controllerInputY > .1f || controllerInputY < -.1f))
        {
            Movement?.Invoke();
        }
        else
        {
            MoveCancel?.Invoke();
        }
    }
}
