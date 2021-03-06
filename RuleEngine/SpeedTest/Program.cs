using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Threading;

using RuleEngine.Evidence.Actions;
using RuleEngine.Evidence.EvidenceValue;
using RuleEngine.Evidence;
using RuleEngine.Compiler;
using RuleEngine;

namespace SpeedTest
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class Performance
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // IndividualFactTest();
            //FactTest();
            //ClonedFactTest();
            ExampleTest();
            Console.Write("Finished.. Press Return.");
            Console.Read();
        }

        private static void IndividualFactTest()
        {
            DateTime end;
            DateTime start;
            TimeSpan diff;
            Xml x;
            int total;

            ROM rom = new ROM();

            //set up out facts
            Fact F1 = new Fact("F1", 1, new Naked("1", typeof(double)), typeof(double));
            Fact F2 = new Fact("F2", 1, new Naked("1", typeof(double)), typeof(double));
            Fact F3 = new Fact("F3", 1, new Naked("2", typeof(double)), typeof(double));
            rom.AddEvidence(F1);
            rom.AddEvidence(F2);
            rom.AddEvidence(F3);

            //set up our assignments
            ActionExpression A1 = new ActionExpression("1", "F1", "2", 2);
            ActionExpression A2 = new ActionExpression("2", "F2", "2", 2);
            ActionExpression A3 = new ActionExpression("3", "F2", "2", 2);
            rom.AddEvidence(A1);
            rom.AddEvidence(A2);
            rom.AddEvidence(A3);
            List<EvidenceSpecifier> listA = new List<EvidenceSpecifier>();
            listA.Add(new EvidenceSpecifier(true, "1"));
            listA.Add(new EvidenceSpecifier(true, "2"));
            listA.Add(new EvidenceSpecifier(true, "3"));

            List<EvidenceSpecifier> listB = new List<EvidenceSpecifier>();
            listB.Add(new EvidenceSpecifier(true, "1"));
            listB.Add(new EvidenceSpecifier(true, "2"));
            listB.Add(new EvidenceSpecifier(true, "3"));

            List<EvidenceSpecifier> listC = new List<EvidenceSpecifier>();
            listC.Add(new EvidenceSpecifier(true, "1"));
            listC.Add(new EvidenceSpecifier(true, "2"));
            listC.Add(new EvidenceSpecifier(true, "3"));

            //create a rule
            Rule R1 = new Rule("R1", "1==2", listA, 500, true);
            Rule R2 = new Rule("R2", "1==1", listB, 500, true);
            Rule R3 = new Rule("R3", "R1>R2", listC, 500, true);
            rom.AddEvidence(R1);
            rom.AddEvidence(R2);
            rom.AddEvidence(R3);
            rom.Evaluate();


            Console.WriteLine("Starting Test:" + DateTime.Now);
            total = 50000;
            start = DateTime.Now;
            for (int counter = 0; counter < total; counter++)
            {
                //cause rules to evaluate
                rom.Evaluate();
            }


            end = DateTime.Now;
            diff = end - start;
            Console.WriteLine("Total ms: " + diff.TotalMilliseconds);

            Console.WriteLine("qps /s: " + diff.TotalMilliseconds / total * 1000);

            Console.WriteLine("milliseconds per rule: " + (diff.TotalMilliseconds / (total * 8d))); //eight rules per run
        }

        private static void FactTest()
        {
            DateTime end;
            DateTime start;
            TimeSpan diff;

            Console.WriteLine("Loading and Compiling ruleset: " + DateTime.Now);

            //rules
            XmlDocument rules = new XmlDocument();
            string directory = AppDomain.CurrentDomain.BaseDirectory + "ChainedRules.xml";
            rules.Load(directory);
            ROM rom = Compiler.Compile(rules);

            //model
            XmlDocument model = new XmlDocument();
            model.LoadXml("<a><result1/><result2/></a>");
            rom.AddModel("bob", model);

            //set default values for rom
            Console.WriteLine("Starting Test:" + DateTime.Now);
            int total = 5000;
            start = DateTime.Now;
            for (int counter = 0; counter < total; counter++)
            {
                //cause all rules to evaluate
                rom.Evaluate();
            }
            end = DateTime.Now;
            diff = end - start;
            Console.WriteLine("Total ms: " + diff.TotalMilliseconds);
            Console.WriteLine("milliseconds per ruleset: " + ((TimeSpan)(end - start)).TotalMilliseconds / (total * 10d));
        }


        private static void ExampleTest()
        {
            //规则
            XmlDocument rules = new XmlDocument();
            string directory = AppDomain.CurrentDomain.BaseDirectory + @"\RuleExample.xml";//规则链条
            rules.Load(directory);
            ROM rom = Compiler.Compile(rules);

            //模型
            XmlDocument model = new XmlDocument();
            model.LoadXml("<a><number1>10</number1><number2>1</number2><result2/></a>");
            rom.AddModel("bob", model);

            Console.WriteLine("Starting Test:" + DateTime.Now);
            //解析全部规则计算
            rom.Evaluate();
        }
    }
}
