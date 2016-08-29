/*
Simple Rule Engine
Copyright (C) 2005 by Sierra Digital Solutions Corp

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

using RuleEngine.Evidence;
using RuleEngine.Evidence.EvidenceValue;

namespace RuleEngine.Evidence
{
    public class Rule : AEvidence, IRule
    {
        #region IRule Members
        /// <summary>
        /// 是否是链式规则
        /// </summary>
        private bool chainable = false;

        /// <summary>
        /// 方程式
        /// </summary>
        protected string equation;

        /// <summary>
        /// 后缀表达式
        /// </summary>
        private List<RuleEngine.Evidence.ExpressionEvaluator.Symbol> postfixExpression;

        private List<EvidenceSpecifier> actions;
        #endregion
        #region constructor
        /// <summary>
        /// 规则构造函数
        /// </summary>
        /// <param name="ID">ID</param>
        /// <param name="equation">方程式</param>
        /// <param name="actions">行为动作</param>
        /// <param name="priority"></param>
        /// <param name="chainable"></param>
        public Rule(string ID, string equation, List<EvidenceSpecifier> actions, int priority, bool chainable)
            : base(ID, priority)
        {
            if (actions == null || actions.Count < 1)
                throw new Exception("规则必须至少有一个行为");
            foreach (EvidenceSpecifier action in actions)
            {
                if (!action.truthality && chainable)
                    // 连贯性的规则是不允许包含结果是错误的动作
                    throw new Exception("Chainable rules are not allowed to contain actions whos result is false.");
            }

            this.actions = actions;
            this.chainable = chainable;
            this.equation = equation;
            ArrayList al = new ArrayList();
            foreach (EvidenceSpecifier es in actions)
            {
                al.Add(es.evidenceID);
            }

            // 规则的附属行为动作
            this.clauseEvidence = (string[])al.ToArray(typeof(string));

            //this is expensive and static, so compute now
            ExpressionEvaluator e = new ExpressionEvaluator();
            // 方程式转化为中缀表达式
            e.Parse(equation); //this method is slow, do it only when needed
            // 中缀表达式转为后缀表达式
            e.InfixToPostfix(); //this method is slow, do it only when needed

            this.postfixExpression = e.Postfix; //this method is slow, do it only when needed

            //determine the dependent facts
            // 确定依赖的变量
            string[] dependents = ExpressionEvaluator.RelatedEvidence(e.Postfix);
            dependentEvidence = dependents;

            //change event could set its value when a model is attached
            // 当一个模型被连接时，change事件可以设置它的值
            Naked naked = new Naked(false, typeof(bool));
            base.EvidenceValue = naked;
        }
        /// <summary>
        /// Constructor used by clone method. Do not use.
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public Rule()
        {
        }
        #endregion
        #region core
        protected override int CalculateInternalPriority(int priority)
        {
            if (isChainable)
                return priority;
            else
                return 1000 * priority;
        }

        /// <summary>
        /// 
        /// </summary>
        public override event ChangedHandler Changed
        {
            add
            {
                base.Changed += value;
            }
            remove
            {
                base.Changed -= value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override event EvidenceLookupHandler EvidenceLookup
        {
            add
            {
                base.EvidenceLookup += value;
            }
            remove
            {
                base.EvidenceLookup -= value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override event ModelLookupHandler ModelLookup
        {
            add
            {
                base.ModelLookup += value;
            }
            remove
            {
                base.ModelLookup -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        //[System.Diagnostics.DebuggerHidden]
        public bool isChainable
        {
            get { return chainable; }
        }

        /// <summary>
        /// 核心计算
        /// </summary>
        public override void Evaluate()
        {
            ExpressionEvaluator e = new ExpressionEvaluator();

            // 注册事件
            e.GetEvidence += new EvidenceLookupHandler(RaiseEvidenceLookup);
            // 后缀表达式
            e.Postfix = this.postfixExpression;
            // 计算表达式
            ExpressionEvaluator.Symbol o = e.Evaluate(); //PERFORMANCE: this method is slow.
            //清空之前数据
            base.EvidenceValue.Reset();

            //result must be of this type or the expression is invalid, throw exception
            // 结果是IEvidenceValue类型，或者表达式为无效的，则抛出异常
            IEvidenceValue result = o.value as IEvidenceValue;

            //exit if null returned
            if (o.type == ExpressionEvaluator.Type.Invalid)
            {
                return;
            }


            if (base.Value.Equals(result.Value))
                return; //no change in value, dont raise an event

            base.Value = result.Value; // 此方法，引发false时规则条件表达式中的引用为0
            // 值被改变了则调用事件
            RaiseChanged(this, new ChangedArgs());
        }

        protected override IEvidence Value_EvidenceLookup(object sender, EvidenceLookupArgs args)
        {
            return RaiseEvidenceLookup(this, args);
        }
        protected override void Value_Changed(object sender, ChangedArgs args)
        {
            RaiseChanged(this, args);
        }
        protected override XmlNode Value_ModelLookup(object sender, ModelLookupArgs e)
        {
            return RaiseModelLookup(this, e);
        }
        public override string[] ClauseEvidence
        {
            get
            {
                List<string> list = new List<string>();

                foreach (EvidenceSpecifier es in actions)
                {
                    if ((bool)base.Value == es.truthality)
                        list.Add(es.evidenceID);
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        public override object Clone()
        {
            Rule f = (Rule)base.Clone();
            f.postfixExpression = new List<ExpressionEvaluator.Symbol>(this.postfixExpression);
            f.actions = new List<EvidenceSpecifier>(this.actions);
            return f;
        }
        #endregion

    }
}
