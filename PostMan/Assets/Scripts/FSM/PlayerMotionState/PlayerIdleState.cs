using PostMan.InputManagement;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PostMan.StateMachine
{
    public class PlayerIdleState : FSMState
    {
        private PlayerMotion motion;
        private PlayerSight sight;
        public PlayerIdleState(MonoFSM stateMachine) : base(stateMachine)
        {
            this.ID = StateID.PlayerIdle;
            motion = stateMachine.Owner.GetComponent<PlayerMotion>();
            sight = motion.GetComponentInChildren<PlayerSight>();
        }
        public override void EnterState()
        {
            if (sight.enableShake)
            {
                sight.StopShake();
            }
            base.EnterState();
        }

        public override void StayState()
        {
            base.StayState();
            if (motion.autoGravity)
            {
                motion.Move(Vector3.down, motion.walkSpeed);
            }
            //Debug.Log(sight.sightInput.GetSightMove());
            motion.Rotate(sight.GetInputSource().GetSightMove().x, sight.sensitivity);
            sight.Rotate(sight.GetInputSource().GetSightMove().y, sight.sensitivity);
        }
    }
}
