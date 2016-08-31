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
using System.Text;
using System.Xml;
using System.Diagnostics;

using RuleEngine.Evidence;
using RuleEngine.Evidence.EvidenceValue;

namespace RuleEngine.Decisions
{
    /// <summary>
    /// decisiontable or decisiontree for evaluation should be determined by parent in a state design pattern
    /// ����   ���߱�or�������ļ���Ӧ���ɸ����״̬����
    /// </summary>
    public class Decision
    {
        /// <summary>
        /// ִ���б�
        /// </summary>
        protected ExecutionList executionList = new ExecutionList();

        /// <summary>
        /// ���캯��
        /// </summary>
        public Decision()
        {
        }

        /// <summary>
        /// ִ�м���
        /// </summary>
        /// <param name="evidenceCollection"></param>
        /// <param name="factRelationships"></param>
        public void Evaluate(Dictionary<string, IEvidence> evidenceCollection, Dictionary<string, List<string>> factRelationships)
        {
            #region register all evidences in this rom with this instance of decision
            // �����ʵ����ע�����е�evidences�����ʵ���ľ���
            foreach (IEvidence evidence in evidenceCollection.Values)
            {
                evidence.CallbackLookup += RaiseCallback;
                evidence.EvidenceLookup += RaiseEvidenceLookup;
                evidence.ModelLookup += RaiseModelLookup;

                evidence.Changed += delegate (object sender, ChangedArgs args)
                {
                    IEvidence evidence1 = (IEvidence)sender;
                    Debug.WriteLine(string.Format("ֵ�����˸ı䣺����-{0},ID-{1}", evidence1.ValueType, evidence1.ID));
                    if (!(evidence1 is IFact))
                    {
                        Debug.WriteLine("�����ı��������FACT������ ");
                        return; //exit if not IFact
                    }

                    // �ҳ���ifactģ��
                    IFact fact = (IFact)evidence1;
                    IEvidenceValue value = (IEvidenceValue)fact.ValueObject;
                    string modelId = value.ModelId;

                    // ��������ifacts�������Щ����ͬ��ģ����ִ���б�
                    foreach (IEvidence evidence2 in evidenceCollection.Values)
                    {
                        // �ų����в���IFact֤������
                        if (!(evidence2 is IFact))
                        {
                            continue;
                        }
                        // �ų��Լ�
                        if (evidence2.ID == evidence1.ID)
                        {
                            continue;
                        }

                        // �ų�������Щ��ͬ��ifactsģ��
                        if (evidence2.ValueObject.ModelId != modelId)
                        {
                            continue;
                        }

                        executionList.Add(evidence2);
                    }
                };
            }
            #endregion

            #region load up the execution list with facts
            //load up the execution list with facts
            // �����б��ִ����ʵ
            foreach (IEvidence fact in evidenceCollection.Values)
            {
                if (!(fact is IFact))
                    continue;

                executionList.Add(fact);
                Debug.WriteLine("Added fact to execution list: " + fact.ID);
            }
            #endregion

            #region load up the execution list with chainable rules
            //load up the execution list with chainable rules
            // �����б��ִ����ʽ����
            foreach (IEvidence rule in evidenceCollection.Values)
            {
                if (rule is IRule && ((IRule)rule).isChainable)
                {
                    executionList.Add(rule);
                    Debug.WriteLine("Added rule to execution list: " + rule.ID);
                }
            }
            #endregion

            #region execute list
            //execute list
            Debug.WriteLine("Iteration");
            Debug.IndentLevel++;
            while (executionList.HasNext)
            {
                Debug.WriteLine("Execution List: " + executionList.ToString());
                Debug.WriteLine("Processing");
                Debug.IndentLevel++;
                //evaluate first item on list, it will always be the one of the lowest priority
                // ����list�ĵ�һ����������Զ��������ȼ���һ��
                string evidenceId = executionList.Read();
                IEvidence evidence = evidenceCollection[evidenceId];
                Debug.WriteLine("EvidenceId: " + evidence.ID);

                //evaluate evidence����֤��
                evidence.Evaluate();

                //add its actions, if any, to executionList, for evidence that has clauses
                // /���֤�����Ӿ�ģ�����������Ϊ
                if (evidence.ClauseEvidence != null && evidence.ClauseEvidence.Length > 0)
                {
                    Debug.WriteLine(evidence.ID + "�и������� ");
                    foreach (string clauseEvidenceId in evidence.ClauseEvidence)
                    {
                        Evidence.IEvidence clauseEvidence = (Evidence.IEvidence)evidenceCollection[clauseEvidenceId];
                        executionList.Add(clauseEvidence);
                        Debug.WriteLine("��������������ִ���б� " + clauseEvidence.ID);
                    }
                }

                //add chainable dependent facts to executionList
                // ִ���б����ʽ����ص���ʵ
                if (factRelationships.ContainsKey(evidence.ID))
                {
                    Debug.WriteLine("����������ʵ");
                    List<string> dependentFacts = factRelationships[evidence.ID];
                    foreach (string dependentFact in dependentFacts)
                    {
                        Evidence.IEvidence dependentEvidence = (Evidence.IEvidence)evidenceCollection[dependentFact];
                        executionList.Add(dependentEvidence);
                        Debug.WriteLine("����������ʵ����ִ���б�: " + dependentEvidence.ID);
                    }

                    // 
                    Debug.WriteLine("����������ʵ�Ƴ�����: " + evidence.ID);
                    factRelationships.Remove(evidence.ID);
                }

                Debug.IndentLevel--;
                Debug.WriteLine("End Processing");
                Debug.WriteLine("");
            }
            Debug.IndentLevel--;
            Debug.WriteLine("End Iteration");

            #endregion
            //complete
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return executionList.ToString();
        }






        private event ModelLookupHandler modelLookup;
        private event EvidenceLookupHandler evidenceLookup;
        private event CallbackHandler callbackLookup;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        //[System.Diagnostics.DebuggerHidden]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        protected virtual XmlNode RaiseModelLookup(object sender, ModelLookupArgs args)
        {
            //must always have a model lookup if one is needed
            return modelLookup(sender, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        protected virtual IEvidence RaiseEvidenceLookup(object sender, EvidenceLookupArgs args)
        {
            //must always have an evidence lookup if one is needed.
            return evidenceLookup(sender, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        //[System.Diagnostics.DebuggerHidden]
        protected virtual void RaiseCallback(object sender, CallbackArgs args)
        {
            callbackLookup(sender, args);
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual event ModelLookupHandler ModelLookup
        {
            add
            {
                modelLookup = value;
            }
            remove
            {
                modelLookup = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual event EvidenceLookupHandler EvidenceLookup
        {
            add
            {
                evidenceLookup = value;
            }
            remove
            {
                evidenceLookup = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual event CallbackHandler CallbackLookup
        {
            add
            {
                callbackLookup = value;
            }
            remove
            {
                callbackLookup = null;
            }
        }


    }
}
