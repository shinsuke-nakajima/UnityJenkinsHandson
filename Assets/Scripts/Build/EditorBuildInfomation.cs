using System;
using System.IO;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.Build
{
    /// <summary>
    /// エディタでのみ使用するビルド設定
    /// </summary>
    public class EditorBuildInfomation : ScriptableObject
    {

        public string TeamId;
        public string ProvisioningProfile;
        public string CodeSignIdentity;


        private static EditorBuildInfomation instance;

        public static EditorBuildInfomation Instance
        {
            //パスは場合によって変えてください
            get { return (instance
#if UNITY_EDITOR
                ?? (instance = UnityEditor.AssetDatabase.LoadAssetAtPath<EditorBuildInfomation>(Path.Combine(SaveDir, SavePath)))
#endif
                ) ?? (instance = CreateInstance<EditorBuildInfomation>()); }
        }

        const string SavePath = "EditorBuildInfomation.asset";
        const string SaveDir = "Assets/";

        /// <summary>
        /// 情報を保存します　
        /// </summary>
#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        public void Save(){
            
            if (!Directory.Exists (SaveDir)) {
                UnityEditor.AssetDatabase.CreateFolder (Path.GetDirectoryName(SaveDir),Path.GetFileName(SaveDir));
            }
            UnityEditor.AssetDatabase.CreateAsset(this,Path.Combine(SaveDir, SavePath));
            instance = this;
        }
#endif

    }
}

