using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.BuildLogic.Basic
{
    class BasicWindow : WindowBase<BasicConfig>
    {
        [MenuItem("Build/ビルド設定", false, 10)]
        public static void GetWindow()
        {
            var window = EditorWindow.GetWindow<BasicWindow>();
            var p = window.position;
            window.position = new Rect(p.x, p.y, Math.Max(p.width, 400), Math.Max(p.height, 440));
        }

        public override void OnGUI()
        {
            Config.ProductName.Property.TextField("アプリ名");
            EditorGUILayout.Separator();
            Config.BundleVersion.Property.TextField("バンドルバージョン");
            Config.BundleIdentifier.Property.TextField("バンドルID");
            Config.BuildNumber.Value = EditorGUILayout.IntField ("ビルド番号", Config.BuildNumber.Value);
            base.OnGUI();
        }
    }
}
