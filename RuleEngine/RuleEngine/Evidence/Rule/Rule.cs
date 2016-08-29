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
        /// �Ƿ�����ʽ����
        /// </summary>
        private bool chainable = false;

        /// <summary>
        /// ����ʽ
        /// </summary>
        protected string equation;

        /// <summary>
        /// ��׺���ʽ
        /// </summary>
        private List<RuleEngine.Evidence.ExpressionEvaluator.Symbol> postfixExpression;

        private List<EvidenceSpecifier> actions;
        #endregion
        #region constructor
        /// <summary>
        /// �����캯��
        /// </summary>
        /// <param name="ID">ID</param>
        /// <param name="equation">����ʽ</param>
        /// <param name="actions">��Ϊ����</param>
        /// <param name="priority"></param>
        /// <param name="chainable"></param>
        public Rule(string ID, string equation, List<EvidenceSpecifier> actions, int priority, bool chainable)
            : base(ID, priority)
        {
            if (actions == null || actions.Count < 1)
                throw new Exception("�������������һ����Ϊ");
            foreach (EvidenceSpecifier action in actions)
            {
                if (!action.truthality && chainable)
                    // �����ԵĹ����ǲ������������Ǵ���Ķ���
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

            // ����ĸ�����Ϊ����
            this.clauseEvidence = (string[])al.ToArray(typeof(string));

            //this is expensive and static, so compute now
            ExpressionEvaluator e = new ExpressionEvaluator();
            // ����ʽת��Ϊ��׺���ʽ
            e.Parse(equation); //this method is slow, do it only when needed
            // ��׺���ʽתΪ��׺���ʽ
            e.InfixToPostfix(); //this method is slow, do it only when needed

            this.postfixExpression = e.Postfix; //this method is slow, do it only when needed

            //determine the dependent facts
            // ȷ�������ı���
            string[] dependents = ExpressionEvaluator.RelatedEvidence(e.Postfix);
            dependentEvidence = dependents;

            //change event could set its value when a model is attached
            // ��һ��ģ�ͱ�����ʱ��change�¼�������������ֵ
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
        /// ���ļ���
        /// </summary>
        public override void Evaluate()
        {
            ExpressionEvaluator e = new ExpressionEvaluator();

            // ע���¼�
            e.GetEvidence += new EvidenceLookupHandler(RaiseEvidenceLookup);
            // ��׺���ʽ
            e.Postfix = this.postfixExpression;
            // ������ʽ
            ExpressionEvaluator.Symbol o = e.Evaluate(); //PERFORMANCE: this method is slow.
            //���֮ǰ����
            base.EvidenceValue.Reset();

            //result must be of this type or the expression is invalid, throw exception
            // �����IEvidenceValue���ͣ����߱��ʽΪ��Ч�ģ����׳��쳣
            IEvidenceValue result = o.value as IEvidenceValue;

            //exit if null returned
            if (o.type == ExpressionEvaluator.Type.Invalid)
            {
                return;
            }


            if (base.Value.Equals(result.Value))
                return; //no change in value, dont raise an event

            base.Value = result.Value; // �˷���������falseʱ�����������ʽ�е�����Ϊ0
            // ֵ���ı���������¼�
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
