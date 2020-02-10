using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Character
{
    public class Character_RightForward_State : Character_Base_State
    {
        [SerializeField] Material down;

        public override void Enter()
        {
            base.Enter();
            context.character.SetMaterial(down);
            context.playerInput.SetRendererActive(PlayerControllerInput.AnimDirection.dxf);
        }
    }
}