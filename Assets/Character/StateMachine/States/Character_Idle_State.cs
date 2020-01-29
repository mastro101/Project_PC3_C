using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Character
{
    public class Character_Idle_State : Character_Base_State
    {
        public override void Enter()
        {
            base.Enter();
            context.character.SetMaterial(context.character.originalMaterial);
        }
    }
}