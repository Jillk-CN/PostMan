using PostMan.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PostMan.StateMachine
{
    public class PlayerStopWalkCondition:FSMCondition
    {
        public PlayerStopWalkCondition():base()
        {
            this.ID = ConditionID.PlayerStopWalk;
        }
        public override bool Evaluate(MonoFSM stateMachine)
        {
            PlayerMotion motion = stateMachine.Owner.GetComponent<PlayerMotion>();
            return motion.GetInputSource().GetMove() == Vector3.zero;
        }

    }
}
