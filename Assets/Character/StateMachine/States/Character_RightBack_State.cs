using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Character
{
    public class Character_RightBack_State : Character_Base_State
    {
        [SerializeField] Material right;

        public override void Enter()
        {
            base.Enter();
            context.character.SetMaterial(right);
            context.playerInput.SetRendererActive(PlayerControllerInput.AnimDirection.dxb);
        }
    }
}