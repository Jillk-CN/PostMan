using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.StateMachine
{
    /// <summary>
    /// 状态类基类 
    /// 实现类命名规范："PostMan.StateMachine."+枚举名 + "State"
    /// 例:PostMan.StateMachine.IdleState
    /// </summary>
    public abstract class FSMState
    {
         /// <summary>
         /// 状态枚举,状态子类在初始化的时候要设置好自己的枚举
         /// </summary>
        public enum StateID
        {
            /// <summary>
            /// 无状态
            /// </summary>
            None,
            /// <summary>
            /// 默认
            /// </summary>
            Default,
            /// <summary>
            /// 玩家待机
            /// </summary>
            PlayerIdle,
            /// <summary>
            /// 玩家行走
            /// </summary>
            PlayerWalk,
            /// <summary>
            /// 玩家奔跑
            /// </summary>
            PlayerRun
            /*
            /// <summary>
            /// 玩家下蹲时行走
            /// </summary>
            PlayerCrouchedWalk,
            /// <summary>
            /// 玩家蹲下时待机
            /// </summary>
            PlayerCrouchedIdle,
             */
        }
        public StateID ID { get;protected set; }
        public MonoFSM StateMachine { get; protected set; }
        /// <summary>
        /// 条件-状态映射表,用枚举标识要切换到的状态
        /// </summary>
        private Dictionary<FSMCondition.ConditionID, StateID> map;
        private List<FSMCondition> conditions;
        /// <summary>
        /// 要求子类在实例化时正确给自己的StateID赋值
        /// </summary>
        /// <param name="stateMachine"></param>
        public FSMState(MonoFSM stateMachine)
        {
            conditions = new List<FSMCondition>();
            map = new Dictionary<FSMCondition.ConditionID, StateID>();
            this.StateMachine = stateMachine;
        }

        /// <summary>
        /// 添加要切换到的状态及其条件,该方法供外部配置
        /// </summary>
        /// <param name="conditionID"></param>
        /// <param name="stateID"></param>
        public void AddTransition(FSMCondition.ConditionID conditionID ,StateID stateID)
        {
            map.Add(conditionID, stateID);
            //最好不这样,但暂时这样先也行
            conditions.Add(Activator.CreateInstance
                (Type.GetType("PostMan.StateMachine."+conditionID.ToString() + "Condition")) as FSMCondition);
        }
        /// <summary>
        /// 添加要切换到的状态及其条件,条件需要外部配好
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stateID"></param>
        public void AddTransition(FSMCondition condition,StateID stateID)
        {
            map.Add(condition.ID, stateID);
            conditions.Add(condition);
        }
        
        public void CheckConditions()
        {
            for (int i = 0; i < conditions.Count ; i++)
            {
                if (conditions[i].Evaluate(this.StateMachine))
                {
                    this.StateMachine.SetActiveState(map[conditions[i].ID]);
                    return;
                }
            }
        }
        /// <summary>
        /// 状态机在切换状态时调用此方法
        /// </summary>
        public virtual void EnterState(){}
        /// <summary>
        /// 应在处于当前状态时调用
        /// </summary>
        public virtual void StayState() {}
        /// <summary>
        /// 应在离开当前状态时调用
        /// </summary>
        public virtual void ExitState() {}
    }
}