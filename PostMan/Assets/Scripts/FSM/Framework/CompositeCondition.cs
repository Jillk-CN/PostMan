using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostMan.StateMachine
{
    /// <summary>
    /// 组合条件,如果一个状态的切换条件需要多个状态同时满足,可以用这个
    /// 为了方便标识,一个状态至多一个CompositeCondition,
    /// 不支持多个CompositionCondition,如果实在需要多个,可能状态机已经很复杂了,
    /// 这个框架要改改了
    /// </summary>
    public class CompositeCondition : FSMCondition
    {
        private List<FSMCondition> conditions;
        public CompositeCondition()
        {
            this.conditions = new List<FSMCondition>();
        }
        public override bool Evaluate(MonoFSM stateMachine)
        {
            //全部条件为真就是真
            foreach (var condition in conditions)
            {
                if (!condition.Evaluate(stateMachine))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 添加条件,外部调用
        /// </summary>
        /// <param name="condition"></param>
        public void AddCondition(FSMCondition condition)
        {
            this.conditions.Add(condition);
        }
    }
}
