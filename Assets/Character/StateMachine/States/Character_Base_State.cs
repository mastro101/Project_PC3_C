using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Character
{
    public class Character_Base_State : StateBase
    {
        protected CharacterBaseSMContext context;

        public override IState Setup(IStateMachineContext _context)
        {
            context = _context as CharacterBaseSMContext;
            return this;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
