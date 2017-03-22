using System;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Assets.Editor.BuildLogic
{
    public class ClientBuilder : MonoBehaviour
    {

        public static void SetAndroidKeyStore()
        {
            /* リリース時にこの設定が必要です
            PlayerSettings.Android.keystoreName = "hogehoge";
            PlayerSettings.Android.keystorePass = "hogehoge";
            PlayerSettings.Android.keyaliasName = "hogehoge";
            PlayerSettings.Android.keyaliasPass = "hogehoge";
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string[] GetBuildScenes()
        {
            return EditorBuildSettings.scenes
                .Where(s => s.enabled).Where(s => File.Exists(s.path))
                .Select(s => s.path).ToArray();
        }

        /// <summary>
        /// outputディレクトリをなければ作成します
        /// </summary>
        /// <param name="output"></param>
        private static void MakeBuildOutputDirectory(string path)
        {
            if (Directory.Exists(path)) return;
            if (File.Exists(path)) return;

            Directory.CreateDirectory(path);
        }

        public static void Build(ConfigBase config,string name)
        {
           
            //パスがターゲットによってFileだったりDirectoryだったり
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    name = name + ".apk";
                    break;
                case BuildTarget.iOS:
                    name = "xcode";
                    break;
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                    name = name + ".exe";
                    break;
                case BuildTarget.StandaloneOSXUniversal:
                    name = name + ".app";
                    break;
                case BuildTarget.WebGL:
                    name = "webgl";
                    break;
                default:
                    throw new InvalidOperationException("現在のオプションではサポートしていません。");

            }

            Build("Build/" + name, config, EditorUserBuildSettings.activeBuildTarget);
        }

        public static void Build(string outputPath, ConfigBase config, BuildTarget buildTarget)
        {
            UnityEngine.Debug.Log(buildTarget + "Building");

            MakeBuildOutputDirectory(outputPath);

            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);

            //Android用のキーの設定（パスワードがプロジェクトに保存されないためビルド時に設定）
            //ほかのターゲットでは不要だがあって困ることはない
            SetAndroidKeyStore();

            var buildOptions = config.BuildOptions;

            config.OnBuild();

            // ビルド
            // シーン、出力ファイル（フォルダ）、ターゲット、オプションを指定
            var errorMsg = BuildPipeline.BuildPlayer(GetBuildScenes(), outputPath, buildTarget, buildOptions);


            // errorMsgがない場合は成功
            if (string.IsNullOrEmpty(errorMsg))
            {
                UnityEngine.Debug.Log(buildTarget + " Build Success.");
                return;
            }

            throw new InvalidOperationException(errorMsg);
        }

    }
}