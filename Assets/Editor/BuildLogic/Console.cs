using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Editor.BuildLogic.Basic;
using UnityEditor;

namespace Assets.Editor.BuildLogic
{
    public static class Console
    {

        public static void BuildWithConfigure()
        {
            Configure();
            Build();
        }

        public static void Configure()
        {
            var console = new Console<BasicConfig>();
            var args = GetArguments();
            var config = console.Load(args);
            config.Configure();
            //プラットフォームの変更
            EditorUserBuildSettings.SwitchActiveBuildTarget((UnityEditor.BuildTarget) config.BuildTarget.Value);

            UnityEngine.Debug.Log("Config" + Environment.NewLine + config);
        }

        public static void Build()
        {
            string name;
            if (!GetArguments().TryGetValue("name", out name)) name = "build";
            ClientBuilder.Build(new BasicConfig().Load(), name);
        }

        static Dictionary<string,string> GetArguments()
        {
            var d = new Dictionary<string,string>();
            var args = Environment.GetCommandLineArgs();

            for (var i = 0; i < args.Length - 1; i++)
            {
                if(args[i][0] != '-') continue;
                d[args[i].Substring(1)] = args[i + 1];
            }

            string file;
            if (d.TryGetValue("file", out file) || d.TryGetValue("f", out file))
            {
                var pt = File.ReadAllLines(file);

                //#をコメントとして =,区切りのパラメタリストを受け取って処理する
                var fd = pt.Where(l => !Regex.IsMatch(l, @"^\s*#"))
                .Select(p => p.Split('=', ','))
                .Where(pkv => pkv.Length >= 2)
                .ToDictionary(pkv => pkv[0], pkv => pkv[1]);
                //重複キーによるエラーを回避する
                foreach (var kv in fd)
                {
                    d[kv.Key] = kv.Value;
                }
            }

            return d;
        }
    }

    class Console<T> where T : ConfigBase,new ()
    {

        /// <summary>
        /// コマンドラインからの設定を反映します
        /// </summary>
        /// <param name="args"></param>
        public void Configure(Dictionary<string, string> args)
        {
            Load(args).Configure();
        }

        public T Load(Dictionary<string, string> args)
        {
            var config = new T();
            config.Load();
            var ps = config.GetValues().ToDictionary(kv => kv.Key.ToLower(), kv => kv.Value);
            var keys = ps.Keys;
            foreach (var arg in args)
            {
                var argkey = arg.Key.ToLower();
                try
                {
                    var dkey = keys.Where(k => k.StartsWith(argkey)).ToArray();
                    if (dkey.Length >= 2)
                    {
                        UnityEngine.Debug.LogError("duplicate key = " + string.Join(",", dkey));
                    }
                    var key = dkey.FirstOrDefault();
                    if (key != null)
                    {
                        SetValue(ps[key], arg.Value);
                    }
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogError(argkey + "error");
                    UnityEngine.Debug.LogException(e);
                }

            }

            return config;
        }

        void SetValue(ConfigValue configValue, string value)
        {
            var t = configValue.GetValueType();
            //列挙対
            if (t.IsEnum)
            {
                try
                {
                    configValue.SetValue(Enum.Parse(t, value, true));
                    return;

                }
                catch
                {
                    UnityEngine.Debug.LogError(t + " = " + value + "is invalid.");
                    throw;
                }
            }

            if (t == typeof(string)) configValue.SetValue(value);
            if (t == typeof(int)) configValue.SetValue(int.Parse(value));
            if (t == typeof(long)) configValue.SetValue(long.Parse(value));
            if (t == typeof(double)) configValue.SetValue(double.Parse(value));
            if (t == typeof(float)) configValue.SetValue(float.Parse(value));

            var trues = new[] {"true", "t", "1", "on", "yes", "y", "ok"};
            if (t == typeof(bool)) configValue.SetValue(trues.Contains(value.ToLower()));
        }
    }
}
