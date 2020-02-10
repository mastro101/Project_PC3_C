using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Character
{
    public class CharacterBaseSM : StateMachineBase
    {
        [SerializeField] CharacterBase character;
        [SerializeField] PlayerControllerInput playerInput;

        protected override void Start()
        {
            currentContext = new CharacterBaseSMContext()
            {
                character = character,
                playerInput = playerInput,
            };
            base.Start();
        }
    }

    public class CharacterBaseSMContext : IStateMachineContext
    {
        public CharacterBase character;
        public PlayerControllerInput playerInput;
    }
}