using PostMan.InputManagement;
using PostMan.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PostMan.StateMachine
{
    public class PlayerWalkState : FSMState
    {
        private PlayerMotion motion;
        private PlayerMotionInputSource input;
        private PlayerSight sight;
        private PlayerSound sound;

        public PlayerWalkState(MonoFSM stateMachine) : base(stateMachine)
        {
            this.ID = StateID.PlayerWalk;
            motion = stateMachine.Owner.GetComponent<PlayerMotion>();
            sight = motion.GetComponentInChildren<PlayerSight>();
            sound = motion.GetComponent<PlayerSound>();
        }

        public override void EnterState()
        {
            base.EnterState();
            input = motion.GetInputSource();
            if (sight.enableShake)
            {
                sight.ApplyWalkShake();
            }
        }
        public override void StayState()
        {
            base.StayState();
            Vector3 inputDirection = input.GetMove();
            Vector3 direction =
                motion.transform.forward * inputDirection.z +
                motion.transform.right * inputDirection.x;
            //给一点向下的速度(Vector3.down)保持贴在地面的状态
            direction += Vector3.down;
            motion.Move(direction, motion.walkSpeed);
            sound.PlaySoundWalk();

            motion.Rotate(sight.GetInputSource().GetSightMove().x, sight.sensitivity);
            sight.Rotate(sight.GetInputSource().GetSightMove().y, sight.sensitivity);
            sight.SetHorizontalShakeDirection(inputDirection);//暂时在这里传数据先
        }
        public override void ExitState()
        {
            base.ExitState();
            if (sight.enableShake)
            {
                sight.StopShake();
            }
        }
    }
}
