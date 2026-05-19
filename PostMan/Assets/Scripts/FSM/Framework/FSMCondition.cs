using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.StateMachine
{
    /// <summary>
    /// 条件类基类
    /// 实现类命名规范："PostMan.StateMachine."+枚举名 + "Condition"
    /// 例:PostMan.StateMachine.LostTargetCondition
    /// </summary>
    public abstract class FSMCondition
    {
         /// <summary>
         /// 条件枚举,条件子类在初始化的时候要设置好自己的枚举
         /// </summary>
         public enum ConditionID 
         {
            /// <summary>
            /// 组合条件
            /// </summary>
            Composite,
            /// <summary>
            /// 玩家切换到走路的条件
            /// </summary>
            PlayerWalk,
            /// <summary>
            /// 玩家切换到奔跑的条件
            /// </summary>
            PlayerRun,
            /// <summary>
            /// 玩家停止奔跑的条件
            /// </summary>
            PlayerStopRun,
            /// <summary>
            /// 玩家停止走路的条件
            /// </summary>
            PlayerStopWalk
            /*
            /// <summary>
            /// 玩家切换蹲下状态的条件
            /// </summary>
            PlayerSwitchCrouch,
            /// <summary>
            /// 玩家可以站起来的条件
            /// </summary>
            PlayerCanStand, 
             */
         }

        public ConditionID ID { get;protected set; }
        /// <summary>
        /// 子类在实例化时至少要给自己的ID变量赋值
        /// </summary>
        public FSMCondition()
        {

        }

        /// <summary>
        /// 判断是否满足当前条件
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <returns></returns>
        public abstract bool Evaluate(MonoFSM stateMachine);
    }
}