using PostMan.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostMan.StateMachine
{
    public class PlayerRunCondition : FSMCondition
    {
        public PlayerRunCondition():base()
        {
            this.ID = ConditionID.PlayerRun;
        }
        public override bool Evaluate(MonoFSM stateMachine)
        {
            PlayerMotion motion = stateMachine.Owner.GetComponent<PlayerMotion>();
            return stateMachine.GetCurrentStateID()==FSMState.StateID.PlayerWalk &&
                motion.GetInputSource().GetRun();
        }
    }
}
