using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuleEngine.Decisions;
//-----------------------------------------------------------------------
// <copyright file="DecisionTests.cs" company="LY.COM Enterprises">
// * Copyright (C) 2016 同程网络科技股份有限公司 版权所有。
// * author  : tzq24955
// * FileName: DecisionTests.cs
// * history : created by tzq24955 2016/8/31 10:05:23 
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RuleEngine.Decisions.Tests
{
    [TestClass()]
    public class DecisionTests
    {
        #region strings
        /// <summary>
        /// Confirm we can write a new value to a double
        /// </summary>
        [TestMethod()]
        public void double1()
        {
            //init variables
            string modelname = "1.xml";
            bool changed = false;
            XmlNode model1 = null;
            XmlNode model2 = null;
            RuleEngine.Evidence.Fact f1 = new RuleEngine.Evidence.Fact("f1", 1, 2d, typeof(double));
            RuleEngine.Evidence.Fact f2 = new RuleEngine.Evidence.Fact("f2", 1, 4d, typeof(double));
            #region delegates
            f1.Changed += delegate (object source, ChangedArgs args)
            {
                changed = true;
            };
            f1.ModelLookup += delegate (object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f1.EvidenceLookup += delegate (object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            #region delegates
            f2.Changed += delegate (object source, ChangedArgs args)
            {
                changed = true;
            };
            f2.ModelLookup += delegate (object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f2.EvidenceLookup += delegate (object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f1.IsEvaluatable = true;
            f2.IsEvaluatable = true;
            f1.Evaluate();
            f2.Evaluate();

            RuleEngine.Evidence.Actions.ActionExpression f = new RuleEngine.Evidence.Actions.ActionExpression("a1", "f1", "3", 1);
            #region delegates
            f.Changed += delegate (object source, ChangedArgs args)
            {
                changed = true;
            };
            f.ModelLookup += delegate (object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f.EvidenceLookup += delegate (object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f.IsEvaluatable = true;

            //init model
            changed = false;
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname);
            model1 = doc.DocumentElement;
            f.Evaluate();

            Assert.AreEqual(true, changed);
            Assert.IsTrue(f1.Value is double);
            Assert.AreEqual(3, (double)f1.Value);
        }
        /// <summary>
        /// Confirm we can read from one fact and write its value to another.
        /// </summary>
        [TestMethod()]
        public void double2()
        {
            //init variables
            string modelname = "1.xml";
            bool changed = false;
            XmlNode model1 = null;
            XmlNode model2 = null;
            RuleEngine.Evidence.Fact f1 = new RuleEngine.Evidence.Fact("f1", 1, 2d, typeof(double));
            RuleEngine.Evidence.Fact f2 = new RuleEngine.Evidence.Fact("f2", 1, 4d, typeof(double));
            #region delegates
            f1.Changed += delegate (object source, ChangedArgs args)
            {
                changed = true;
            };
            f1.ModelLookup += delegate (object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f1.EvidenceLookup += delegate (object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            #region delegates
            f2.Changed += delegate (object source, ChangedArgs args)
            {
                changed = true;
            };
            f2.ModelLookup += delegate (object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f2.EvidenceLookup += delegate (object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f1.IsEvaluatable = true;
            f2.IsEvaluatable = true;
            f1.Evaluate();
            f2.Evaluate();

            RuleEngine.Evidence.Actions.ActionExpression f = new RuleEngine.Evidence.Actions.ActionExpression("a1", "f1", "f2", 1);
            #region delegates
            f.Changed += delegate (object source, ChangedArgs args)
            {
                changed = true;
            };
            f.ModelLookup += delegate (object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f.EvidenceLookup += delegate (object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f.IsEvaluatable = true;

            //init model
            changed = false;
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname);
            model1 = doc.DocumentElement;
            f.Evaluate();

            Assert.AreEqual(true, changed);
            Assert.IsTrue(f1.Value is double);
            Assert.AreEqual(4, (double)f1.Value);
        }
        /// <summary>
        /// Confirm we can read from one fact, add one, and write its value to another.
        /// </summary>
        [TestMethod()]
        public void double3()
        {
            //init variables
            string modelname = "1.xml";
            bool changed = false;
            XmlNode model1 = null;
            XmlNode model2 = null;
            RuleEngine.Evidence.Fact f1 = new RuleEngine.Evidence.Fact("f1", 1, 2d, typeof(double));
            RuleEngine.Evidence.Fact f2 = new RuleEngine.Evidence.Fact("f2", 1, 4d, typeof(double));
            #region delegates
            f1.Changed += delegate (object source, ChangedArgs args)
            {
                changed = true;
            };
            f1.ModelLookup += delegate (object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f1.EvidenceLookup += delegate (object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            #region delegates
            f2.Changed += delegate (object source, ChangedArgs args)
            {
                changed = true;
            };
            f2.ModelLookup += delegate (object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f2.EvidenceLookup += delegate (object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f1.IsEvaluatable = true;
            f2.IsEvaluatable = true;
            f1.Evaluate();
            f2.Evaluate();

            RuleEngine.Evidence.Actions.ActionExpression f = new RuleEngine.Evidence.Actions.ActionExpression("a1", "f1", "f2+1", 1);
            #region delegates
            f.Changed += delegate (object source, ChangedArgs args)
            {
                changed = true;
            };
            f.ModelLookup += delegate (object source, ModelLookupArgs args)
            {
                if (args.Key == "1.xml")
                    return model1;
                else if (args.Key == "2.xml")
                    return model2;
                else
                    throw new Exception("Couldnt find model: " + ((ModelLookupArgs)args).Key);
            };
            f.EvidenceLookup += delegate (object source, EvidenceLookupArgs args)
            {
                if (args.Key == "f1")
                {
                    return f1;
                }
                else if (args.Key == "f2")
                {
                    return f2;
                }
                else
                    throw new Exception("Unknown evidence");
            };
            #endregion
            f.IsEvaluatable = true;

            //init model
            changed = false;
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Fact\" + modelname);
            model1 = doc.DocumentElement;
            f.Evaluate();

            Assert.AreEqual(true, changed);
            Assert.IsTrue(f1.Value is double);
            Assert.AreEqual(5, (double)f1.Value);
        }
        #endregion
    }
}