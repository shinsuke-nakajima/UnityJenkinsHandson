using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace Assets.Editor.BuildLogic
{
    static class Symbol
    {
        private static IEnumerable<string> currentSymbols;

        private static readonly BuildTargetGroup[] buildTargetGroup = new[]
        {
            BuildTargetGroup.Android, 
            BuildTargetGroup.Standalone, 
            BuildTargetGroup.iOS, 
        };

        public static string CurrentSymbols
        {
            get { return String.Join(";", currentSymbols.ToArray()); }
        }

        static Symbol()
        {
            currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(';');
        }

        private static void SaveSymbol()
        {
            var symbols = CurrentSymbols;
            foreach (var target in buildTargetGroup)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, symbols);
            }
            
        }



        public static void Add(params string[] symbols)
        {
            currentSymbols = currentSymbols.Concat(symbols).Distinct().ToArray();
            SaveSymbol();
        }

        public static void Remove(params string[] symbols)
        {
            currentSymbols = currentSymbols.Except(symbols).ToArray();

            SaveSymbol();
        }

        public static void Set(params string[] symbols)
        {
            currentSymbols = symbols;
            SaveSymbol();
        }

        /// <summary>
        /// onがtrueなら追加falseなら削除
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="on"></param>
        public static void Toggle(string symbol, bool on)
        {
            if (on) Add(symbol);
            else Remove(symbol);
        }
    }
}
