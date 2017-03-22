using UniRx;
using UnityEditor;

namespace Assets.Editor.BuildLogic
{
    static class ReactiveEditorGUILayout
    {
        public static void TextField(this ReactiveProperty<string> target,string label)
        {
            target.Value = EditorGUILayout.TextField(label, target.Value);
        }

    }
    
}
