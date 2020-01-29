
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    [RequireComponent(typeof(Animator))]
    public abstract class StateMachineBase : MonoBehaviour
    {
        protected Animator SM;

        protected IStateMachineContext currentContext;

        private void Awake()
        {
            SM = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            foreach (StateMachineBehaviour smB in SM.GetBehaviours<StateMachineBehaviour>())
            {
                (smB as StateBase).Setup(currentContext);
            }
        }
    }
}