using System;
using NUnit.Framework;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Threading;

using RuleEngine;
using RuleEngine.Evidence;
using RuleEngine.Evidence.EvidenceValue;

namespace UnitTests
{
    [TestFixture]
    public class Xml
    {
        #region internal
        private string modelname;
        private XmlNode model;
        private bool changed;

        public void Changed(object source, ChangedArgs args)
        {
            changed = true;
        }
        public XmlNode ModelLookup(object source, ModelLookupArgs args)
        {
            return model;
        }
        public IEvidence EvidenceLookup(object source, EvidenceLookupArgs args)
        {
            return null;
        }
        public Xml()
        {
        }
        #endregion
        #region string
        /// <summary>
        /// Confirm we can read the first name as text element
        /// </summary>
        [Test]
        public void string1()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load( AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname );
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual("Joe", x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as text element
        /// </summary>
        [Test]
        public void string2()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();
            
            //change the value and see if it was updated
            changed = false;
            x.Value = "Bob";

            Assert.AreEqual("Bob", x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can read the first name as element into string
        /// </summary>
        [Test]
        public void string3()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual("Joe", x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as element from string
        /// </summary>
        [Test]
        public void string4()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = "Bob";

            Assert.AreEqual("Bob", x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can read as attributes into string
        /// </summary>
        [Test]
        public void string5()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/@optional", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual("true", x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write as attrributes from string
        /// </summary>
        [Test]
        public void string6()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/@optional", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = "false";

            Assert.AreEqual("false", x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can read as attributes into string
        /// </summary>
        [Test]
        public void string7()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/attribute::node()[name(.)='optional']", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual("true", x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write as attrributes from string
        /// </summary>
        [Test]
        public void string8()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/attribute::node()[name(.)='optional']", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();
 
            //change the value and see if it was updated
            changed = false;
            x.Value = "false";

            Assert.AreEqual("false", x.Value);
            Assert.AreEqual(true, changed);
        }
        #endregion
        #region double
        /// <summary>
        /// Confirm we can read the first name as text element
        /// </summary>
        [Test]
        public void double1()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/text()", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual(456, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as text element
        /// </summary>
        [Test]
        public void double2()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/text()", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = 999;

            Assert.AreEqual(999, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can read the first name as element into double
        /// </summary>
        [Test]
        public void double3()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual(456, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as element from double
        /// </summary>
        [Test]
        public void double4()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();
 
            //change the value and see if it was updated
            changed = false;
            x.Value = 111;

            Assert.AreEqual(111, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can read as attributes into double
        /// </summary>
        [Test]
        public void double5()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/@id", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual(123, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write as attrributes from double
        /// </summary>
        [Test]
        public void double6()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/@id", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = 234;

            Assert.AreEqual(234, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can read as attributes into double
        /// </summary>
        [Test]
        public void double7()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/attribute::node()[name(.)='id']", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual(123, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write as attrributes from double
        /// </summary>
        [Test]
        public void double8()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/pin/attribute::node()[name(.)='id']", typeof(double), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = 555;

            Assert.AreEqual(555, x.Value);
            Assert.AreEqual(true, changed);
        }
        #endregion
        #region boolean
        /// <summary>
        /// Confirm we can read the first name as text element
        /// </summary>
        [Test]
        public void boolean1()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/attribute::node()[name(.)='optional']", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual(true, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as text element
        /// </summary>
        [Test]
        public void boolean2()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/attribute::node()[name(.)='optional']", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = false;

            Assert.AreEqual(false, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can read the first name as element into double
        /// </summary>
        [Test]
        public void boolean3()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/dis", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual(true, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as element from double
        /// </summary>
        [Test]
        public void boolean4()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/dis", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = false;

            Assert.AreEqual(false, x.Value);
            Assert.AreEqual(true, changed);
        }


        /// <summary>
        /// Confirm we can read the first name as attribute into boolean
        /// </summary>
        [Test]
        public void boolean5()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/@optional", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual(true, x.Value);
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we can write the first name as attribute from boolean
        /// </summary>
        [Test]
        public void boolean6()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address/@optional", typeof(bool), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = false;

            Assert.AreEqual(false, x.Value);
            Assert.AreEqual(true, changed);
        }
        #endregion
        #region xmlnode
        /// <summary>
        /// Confirm we can read the first name as node
        /// </summary>
        [Test]
        public void node1()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address", typeof(XmlNode), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual("address", ((XmlNode)x.Value).Name );
            Assert.AreEqual(true, changed);
        }

        /// <summary>
        /// Confirm we cant write the first name as node
        /// </summary>
        [Test]
        [NUnit.Framework.ExpectedException(typeof(Exception))]
        public void node2()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/address", typeof(XmlNode), modelname);
            RuleEngine.Evidence.EvidenceValue.Xml y = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/dis", typeof(XmlNode), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            y.Changed += Changed;
            y.ModelLookup += ModelLookup;
            x.Evaluate();
            y.Evaluate();

            //change the value and see if it was updated
            changed = false;
            x.Value = y.Value;

            Assert.AreEqual("dis", ((XmlNode)x.Value).Name );
            Assert.AreEqual(true, changed);
        }
        #endregion
        #region model updates
        /// <summary>
        /// Confirm no change event was sent if the field was not updated
        /// </summary>
        [Test]
        public void modelupdate1()
        {
            //init variables
            modelname = "xml.xml";
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;

            //init model
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            x.Evaluate();

            Assert.AreEqual(true, changed);
            Assert.AreEqual("Joe", (string)x.Value);

            //change the model, dont update this xpath expression.
            changed = false;
            model["person"]["address"].InnerText = "bob";
            x.Evaluate();

            Assert.AreEqual(false, changed);
            Assert.AreEqual("Joe", (string)x.Value);
        }
        /// <summary>
        /// Confirm change event was sent if the field was updated
        /// </summary>
        [Test]
        public void modelupdate2()
        {
            //init variables
            modelname = "xml.xml";
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;

            //init model
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            x.Evaluate();

            Assert.AreEqual(true, changed);
            Assert.AreEqual("Joe", (string)x.Value);

            //change the model, update this xpath expression.
            changed = false;
            model["person"]["firstname"].InnerText = "bob";
            x.Evaluate();

            Assert.AreEqual(true, changed);
            Assert.AreEqual("bob", (string)x.Value);
        }
        /// <summary>
        /// Confirm that when a model changes due to xml.value being set that other xml objects can get the new value too
        /// </summary>
        [Test]
        public void modelUpdate3()
        {
            //init variables
            modelname = "xml.xml";
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;

            RuleEngine.Evidence.EvidenceValue.Xml y = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            y.Changed += Changed;
            y.ModelLookup += ModelLookup;

            //init model
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            x.Evaluate();
            y.Evaluate();
            changed = false;

            x.Value = "JOEJOE";

            Assert.IsTrue((string)x.Value == "JOEJOE");
            Assert.IsTrue((string)y.Value != "JOEJOE");

            if (changed)
            {
                x.Evaluate();
                y.Evaluate();
            }

            Assert.IsTrue((string)x.Value == "JOEJOE");
            Assert.IsTrue((string)y.Value == "JOEJOE");
        }
        #endregion
        #region clone
        [Test]
        public void clone1()
        {
            modelname = "xml.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Xml\" + modelname);
            model = doc.DocumentElement;
            changed = false;

            RuleEngine.Evidence.EvidenceValue.Xml x = new RuleEngine.Evidence.EvidenceValue.Xml("/root/person/firstname/text()", typeof(string), modelname);
            x.Changed += Changed;
            x.ModelLookup += ModelLookup;
            x.Evaluate();

            Assert.AreEqual("Joe", x.Value);
            Assert.AreEqual(true, changed);
            
            //clone
            RuleEngine.Evidence.EvidenceValue.Xml y = (RuleEngine.Evidence.EvidenceValue.Xml)x.Clone();
            y.Changed += Changed;
            y.ModelLookup += ModelLookup;

            //change the model, update this xpath expression.
            changed = false;
            model["person"]["firstname"].InnerText = "bob";
            y.Evaluate();

            Assert.AreEqual(true, changed);
            Assert.AreEqual("bob", (string)y.Value);
        }
        #endregion
    }
}
