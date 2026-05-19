using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PostMan.StateMachine
{
    public class PlayerWalkCondition : FSMCondition
    {
        public PlayerWalkCondition(): base()
        {
            this.ID = ConditionID.PlayerWalk;
        }
        public override bool Evaluate(MonoFSM stateMachine)
        {
            PlayerMotion motion = stateMachine.Owner.GetComponent<PlayerMotion>();
            return motion.GetInputSource().GetMove() != Vector3.zero;
        }
    }
}
