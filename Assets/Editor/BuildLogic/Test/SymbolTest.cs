using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEditor;

namespace Assets.Editor.BuildLogic.Test
{
    [TestFixture]
    class SymbolTest
    {
        string startSymbols;

        [SetUp]
        public void SetUp()
        {
            startSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        }

        [TearDown]
        public void TearDown()
        {
            Symbol.Set(startSymbols.Split(';'));
        }

        [Test]
        public void SetTest()
        {
            Symbol.Set("A","B");
            AssertSymbol("A", "B");
            Symbol.Set();
            AssertSymbol();

        }

        [Test]
        public void AddTest()
        {
            Symbol.Set();
            Symbol.Add("A");
            AssertSymbol("A");
            Symbol.Add("B");
            AssertSymbol("A","B");
            Symbol.Add("C","D");
            AssertSymbol("A","B","C","D");

            Symbol.Add("A","B","E");
            AssertSymbol("A", "B", "C", "D","E");
        }

        [Test]
        public void RemoveTest()
        {
            Symbol.Set("A", "B", "C", "D");
            Symbol.Remove("C", "D");
            AssertSymbol("A", "B"); 
            Symbol.Remove("A");
            AssertSymbol("B");
            Symbol.Remove("D");
            AssertSymbol("B");
            Symbol.Add("A");
            AssertSymbol("B","A");
            Symbol.Remove("B","C","D");
            AssertSymbol("A");
            Symbol.Remove("A");
            AssertSymbol();
        }

        void AssertSymbol(params string[] symbols)
        {
            Assert.AreEqual(string.Join(";", symbols),
                PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));
        }
    }
}
