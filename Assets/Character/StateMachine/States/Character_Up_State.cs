using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Character
{
    public class Character_Up_State : Character_Base_State
    {
        [SerializeField] Material up;

        public override void Enter()
        {
            base.Enter();
            context.character.SetMaterial(up);
        }
    }
}