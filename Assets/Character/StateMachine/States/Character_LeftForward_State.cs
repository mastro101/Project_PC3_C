using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Character
{
    public class Character_LeftForward_State : Character_Base_State
    {
        [SerializeField] Material left;

        public override void Enter()
        {
            base.Enter();
            context.character.SetMaterial(left);
            context.playerInput.SetRendererActive(PlayerControllerInput.AnimDirection.sxf);
        }
    }
}