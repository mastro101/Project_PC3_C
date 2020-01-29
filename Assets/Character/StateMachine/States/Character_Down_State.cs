using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Character
{
    public class Character_Down_State : Character_Base_State
    {
        [SerializeField] Material down;

        public override void Enter()
        {
            base.Enter();
            context.character.SetMaterial(down);
        }
    }
}