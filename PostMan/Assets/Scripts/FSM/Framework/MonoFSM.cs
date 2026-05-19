using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Localization.SmartFormat.Utilities;

namespace PostMan.StateMachine
{
    /// <summary>
    /// 简单的有限状态机
    /// </summary>
    public class MonoFSM
    {
        /// <summary>
        /// 所有的状态,这里认为一个状态机对象状态都是唯一的,不会有重复的状态
        /// </summary>
        private List<FSMState> states;
        private FSMState defaultState;
        private FSMState currentState;
        public FSMState.StateID DefaultStateID { get { return defaultState.ID; } }
        /// <summary>
        /// 这个状态机属于的物体
        /// </summary>
        public GameObject Owner { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">当前状态机属于哪个物体</param>
        public MonoFSM(GameObject owner)
        {
            this.Owner = owner;
            this.states = new List<FSMState>();
        }

        /// <summary>
        /// 设置默认状态,在运行状态机之前要调用此方法
        /// </summary>
        /// <param name="defaultStateID"></param>
        public void SetDefaultState(FSMState.StateID defaultStateID)
        {
            defaultState = states.Find(state => state.ID == defaultStateID);
            if (defaultState==null)
            {
                Debug.LogWarningFormat("当前未找到状态:{0}", defaultStateID);
                return;
            }
            currentState = defaultState;
            currentState.EnterState();
        }
        /// <summary>
        /// 添加状态,状态的相关信息需要自行配置好
        /// </summary>
        /// <param name="state"></param>
        public void AddState(FSMState state)
        {
            if (state==null)
            {
                return;
            }
            this.states.Add(state);
        }
        /// <summary>
        /// 运行状态机,状态机会根据配置好的状态来执行相关的行为
        /// </summary>
        public void Execute()
        {
            currentState.StayState();
            currentState.CheckConditions();
        }

        /* 原来用的初始化的代码,已废弃,当前状态机的初始化要求外部进行
         
        private void ConfigStateMachine()
        {
             
            states = new List<FSMState>();
            Dictionary<string, Dictionary<string, string>> map =
                AIConfigurationReaderFactory.GetMap(configFilePath);
            FSMState state;
            foreach (var key in map .Keys)
            {
                state = CreateState(key);
                ConfigState(state, map[key]);
                states.Add(state);
            }
        }
        //创建状态的方法暂时写这里
        private FSMState CreateState(string stateName)
        {
           return Activator.CreateInstance
                (Type.GetType("AI.FSM." + stateName + "State")) as FSMState;
        }
        private void ConfigState(FSMState state,Dictionary<string, string> map)
        {
            FSMCondition.ConditionID triggerID;
            FSMState.StateID stateID;
            foreach (var key in map.Keys )
            {
                triggerID = (FSMCondition.ConditionID)Enum.Parse
                    (typeof(FSMCondition.ConditionID), key);
                stateID = (FSMState.StateID)Enum.Parse(typeof(FSMState.StateID), map[key]);
                state.AddTransition(triggerID, stateID);
            }
        }
         */

        /// <summary>
        /// 切换到指定的状态
        /// </summary>
        /// <param name="stateID"></param>
        public void SetActiveState(FSMState.StateID stateID)
        {
            FSMState toState = states.Find(state => state.ID == stateID);
            if (toState==null)
            {
                Debug.LogWarningFormat("当前未找到状态:{0}", stateID);
                return;
            }
            currentState.ExitState();

            currentState = toState;

            currentState.EnterState();
        }
        public FSMState.StateID GetCurrentStateID()
        {
            return currentState.ID;
        }
    }
}