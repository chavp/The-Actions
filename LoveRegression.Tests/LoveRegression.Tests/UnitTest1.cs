using CsvHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace LoveRegression.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // https://www.kaggle.com/c/titanic/data

            var titanicDic = new DataDictionary();

            //titanicDic.Variables = new List<Variable>
            //{
            //    new Variable
            //    {
            //        Name = "passengerId",
            //        Type = VariableType.Label,
            //        Definition = "Passenger Id",
            //    },
            //    new Variable
            //    {
            //        Name = "name",
            //        Type = VariableType.Label,
            //        Definition = "Passenger name",
            //    },
            //    new Variable
            //    {
            //        Name = "survival",
            //        Type = VariableType.Bit,
            //        Definition = "Survival",
            //    },
            //    new Variable
            //    {
            //        Name = "pclass",
            //        Type = VariableType.Label,
            //        Definition = "Ticket class",
            //    },
            //    new Variable
            //    {
            //        Name = "sex",
            //        Type = VariableType.Label,
            //        Definition = "Sex",
            //    },
            //    new Variable
            //    {
            //        Name = "Age",
            //        Type = VariableType.Number,
            //        Definition = "Age in years",
            //    },
            //    new Variable
            //    {
            //        Name = "sibsp",
            //        Type = VariableType.Number,
            //        Definition = "# of siblings / spouses aboard the Titanic",
            //    },
            //    new Variable
            //    {
            //        Name = "parch",
            //        Type = VariableType.Number,
            //        Definition = "# of parents / children aboard the Titanic",
            //    },
            //    new Variable
            //    {
            //        Name = "ticket",
            //        Type = VariableType.Label,
            //        Definition = "Ticket number",
            //    },
            //    new Variable
            //    {
            //        Name = "fare",
            //        Type = VariableType.Label,
            //        Definition = "Passenger fare",
            //    },
            //    new Variable
            //    {
            //        Name = "cabin",
            //        Type = VariableType.Label,
            //        Definition = "Cabin number",
            //    },
            //    new Variable
            //    {
            //        Name = "embarked",
            //        Type = VariableType.Label,
            //        Definition = "Port of Embarkation",
            //    },
            //};

            // load data set
            var dataSet = new DataSet();
            using (var reader = new StreamReader(@"E:\projects\The-Actions\titanic\train.csv"))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<dynamic>();
                foreach (var rec in records)
                {
                    foreach (var item in rec)
                    {
                        var key = item.Key;
                        var val = item.Value;
                        if (!dataSet.Values.ContainsKey(key))
                        {
                            dataSet.Values.Add(key, new List<string>());
                        }
                        dataSet.Values[key].Add(val);
                    }
                }
            }

            var theDic = dataSet.BuildDataDict();

            foreach (var item in dataSet.Values)
            {
                var limit = Math.Ceiling(0.1 * item.Value.Count);
                var randomValues = item.Value.OrderBy(x => Guid.NewGuid()).Take((int)limit);
                var dataString = string.Join("", randomValues);
                var numberRegex = @"^-?[0-9][0-9,\.]+$";
                var bitRegex = @"^[0-1]+$";
                if (Regex.Match(dataString, numberRegex).Success)
                {
                    var var = (from x in theDic.Variables
                              where x.Name == item.Key
                              select x).Single();
                    if (Regex.Match(dataString, bitRegex).Success)
                    {
                        var.Type = VariableType.Bit;
                        continue;
                    }
                    var.Type = VariableType.Number;
                }
            }

            foreach (var item in theDic.Variables)
            {
                Console.WriteLine($"{item.Name} - {item.Type}");
            }
        }
    }

    public class DataDictionary
    {
        public List<Variable> Variables { get; set; } = new List<Variable>();
    }

    public class DataSet
    {
        public Dictionary<string, List<string>> Values { get; set; } = new Dictionary<string, List<string>>();

        public DataDictionary BuildDataDict()
        {
            var dic = new DataDictionary();
            foreach (var key in Values.Keys)
            {
                dic.Variables.Add(new Variable
                {
                    Name = key
                });
            }
            return dic;
        }
    }

    // https://en.wikipedia.org/wiki/Regression_analysis
    public class Variable
    {
        public string Name { get; set; }
        public string Definition { get; set; }
        public VariableType Type { get; set; } = VariableType.Label;
    }

    public enum VariableType
    {
        Number, Label, DateTime, Bit
    }
}
