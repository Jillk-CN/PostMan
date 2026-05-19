using PostMan.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostMan.StateMachine
{
    public class PlayerStopRunCondition: FSMCondition
    {
        public PlayerStopRunCondition():base()
        {
            this.ID = ConditionID.PlayerStopRun;
        }
        public override bool Evaluate(MonoFSM stateMachine)
        {
            PlayerMotion motion = stateMachine.Owner.GetComponent<PlayerMotion>();
            return stateMachine.GetCurrentStateID()==FSMState.StateID.PlayerRun &&
                !motion.GetInputSource().GetRun();
        }
    }
}
