using UnityEditor;
using UnityEngine;

namespace Assets.Editor.BuildLogic
{
    class WindowBase<T> : EditorWindow where T : ConfigBase,new()
    {
        protected T Config;
        protected T PreviousConfig;

        public virtual bool CanBuild{ get { return Config.EqualsValues(PreviousConfig); } }

        protected virtual void OnEnable()
        {
            Config = (T) (new T().Load());
            PreviousConfig = Config.Copy();
        }

        public virtual void OnGUI()
        {

            if (PreviousConfig == null) PreviousConfig = Config.Copy();

            var isEqual = Config.EqualsValues(PreviousConfig);
            if (!isEqual)
            {
                EditorGUILayout.LabelField("※ビルド設定が保存されていません。");
                
            }
            if (GUILayout.Button("設定保存"))
            {
                Config.Configure();
                PreviousConfig = Config.Copy();
            }
            EditorGUILayout.Separator();
            if (isEqual)
            {
                if (GUILayout.Button("ビルド"))
                {
                    //ビルド
                    var config = new T();
                    config.Load();

                    ClientBuilder.Build(config,"build");
                }
            }

        }

    }
}
