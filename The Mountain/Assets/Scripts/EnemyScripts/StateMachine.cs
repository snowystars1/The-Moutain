using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StateStuff
{
    public class StateMachine<T>
    {

        public State<T> currentState { get; private set; }
        public T boss;

        public StateMachine(T boss)
        {
            this.boss = boss;
            currentState = null;
        }

        public void ChangeState(State<T> newState) {
            if (currentState != null)
                currentState.ExitState(boss);
            currentState = newState;
            currentState.EnterState(boss);
        }

        // Update is called once per frame
        public void Update()
        {
            if (currentState != null)
                currentState.UpdateState(boss);
        }
    }

    public abstract class State<T>
    {

        public abstract void EnterState(T boss);

        public abstract void ExitState(T boss);

        public abstract void UpdateState(T boss);
    }
}
