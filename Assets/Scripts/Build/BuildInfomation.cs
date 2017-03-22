using System;
using System.Linq;
using UnityEngine;
using System.Diagnostics;
using System.IO;

namespace Assets.Scripts.Build
{
    /// <summary>
    /// ビルド時の情報をまとめています
    /// </summary>
    
    public class BuildInfomation : ScriptableObject
    {

        const string SavePath = "Info/BuildInfomation";
        private static BuildInfomation instance;

        public static BuildInfomation Instance
        {
            //パスは場合によって変えてください
            get { return (instance ??
                (instance = Resources.Load<BuildInfomation>(SavePath))) ??
                (instance = CreateInstance<BuildInfomation>()); }
        }


        /// <summary>
        /// ビルド時の状態を示します
        /// </summary>
        public BuildSituation Build = new BuildSituation();

        /// <summary>
        /// 情報を保存します　
        /// </summary>
#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        public void Save(){
            UnityEditor.AssetDatabase.CreateAsset(this,Path.Combine("Assets/Resources",SavePath+ ".asset"));
        }
#endif

        /// <summary>
        /// ビルド時の状況を表します
        /// </summary>
        [Serializable]
        public class BuildSituation
        {
            /// <summary>
            /// ビルドしたブランチ
            /// </summary>
            public Branch Branch = Branch.Master;
            
            /// <summary>
            /// ビルドタイプ
            /// </summary>
            public BuildEnvironment Environment = BuildEnvironment.Development;

            public bool IsDevelopment;
            /// <summary>
            /// ビルド番号
            /// </summary>
            public string Number = "private"; // Jenkins Build番号
            /// <summary>
            /// ビルド日付
            /// </summary>
            public string Date = ""; // Jenkins Build時の日付 ( xx/xx ) のみ



            public override string ToString()
            {
                var day = string.IsNullOrEmpty(Date) ? DateTime.Today : DateTime.Parse(Date); 
                var date = day.ToString("MM/dd");

                //substringだと3文字以下に対応できない
                return new string(Branch.ToString().Take(3).ToArray()) + "-" + Environment.ToString().Substring(0,3) + "-" + Number + "-" + date;
            }
        }
    }
}
