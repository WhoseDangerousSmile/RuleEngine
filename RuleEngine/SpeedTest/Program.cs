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
            //IndividualFactTest();
            //FactTest();
            //ClonedFactTest();
            ExampleTest();
            /// 工资计算实例
            //ExecuteTaxRules();
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

            //设置变量
            Fact F1 = new Fact("a", 1, new Naked("10", typeof(double)), typeof(double));
            Fact F2 = new Fact("b", 1, new Naked("20", typeof(double)), typeof(double));
            Fact F3 = new Fact("c", 1, new Naked("30", typeof(double)), typeof(double));
            rom.AddEvidence(F1);
            rom.AddEvidence(F2);
            rom.AddEvidence(F3);

            //set up our assignments
            ActionExpression A1 = new ActionExpression("1", "A1", "a+(b/5)+c*7", 2);
            ActionExpression A2 = new ActionExpression("2", "A2", "a*2-1", 2);
            ActionExpression A3 = new ActionExpression("3", "A3", "c*2+4", 2);
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

            ////create a rule
            Rule R1 = new Rule("R1", "a*4/b*6+9-c", listA, 500, true);
            //Rule R2 = new Rule("R2", "1==1", listB, 500, true);
            //Rule R3 = new Rule("R3", "R1>R2", listC, 500, true);
            rom.AddEvidence(R1);
            //rom.AddEvidence(R2);
            //rom.AddEvidence(R3);
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


        public static void ExecuteTaxRules()
        {
            XmlDocument rules = new XmlDocument();

            string directory = AppDomain.CurrentDomain.BaseDirectory + @"\TaxCalculator.xml";

            rules.Load(directory);
            //model
            XmlDocument model = new XmlDocument();
            // 工资总额  GrossSalary
            // 房租津贴 HRA
            // Tax 税
            // NetSalary 净工资
            model.LoadXml(@"
<Employee>
<GrossSalary>700000</GrossSalary>
<HRA>50000</HRA>
<Tax></Tax>
<NetSalary></NetSalary>
</Employee>");

            ROM rom = Compiler.Compile(rules);
            rom.AddModel("Employee", model);
            rom.Evaluate();
            var tax = model["Employee"]["Tax"].InnerText;
            var NetSalary = model["Employee"]["NetSalary"].InnerText;
            var grossSalary = model["Employee"]["GrossSalary"].InnerText;
            var message = string.Format("GrossSalary:{0},Tax: {1} and Net take home salary :{2}", grossSalary, tax, NetSalary);
            Console.Write(message);
            Console.Read();
        }
    }
}
