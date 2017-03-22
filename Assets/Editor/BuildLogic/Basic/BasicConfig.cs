using System;
using System.Globalization;
using Assets.Scripts.Build;
using UnityEditor;
using UniRx;
using UnityEngine;
using Target = UnityEditor.BuildTarget;


namespace Assets.Editor.BuildLogic.Basic
{


    enum BuildTarget
    {
        iOS = Target.iOS,
        Android = Target.Android,
        WebGL = Target.WebGL,
        Win64 = Target.StandaloneWindows64,
        OSX = Target.StandaloneOSXUniversal,
        Other = -1
    }

    class BasicConfig : ConfigBase
    {
        public readonly ConfigValue<string> ProductName = new ConfigValue<string>();


        public ConfigValue<string> BundleVersion =new ConfigValue<string>();
        public ConfigValue<string> BundleIdentifier = new ConfigValue<string>();
        public ConfigValue<string> XcodeTeamId = new ConfigValue<string>();
        public ConfigValue<string> ProvisioningProfile = new ConfigValue<string>();
        public ConfigValue<string> CodeSignIdentity = new ConfigValue<string>();
        public ConfigValue<bool> iPhoneScriptCallOptimization = new ConfigValue<bool>();


        public ConfigValue<Branch> Branch = new ConfigValue<Branch>();
        public ConfigValue<BuildEnvironment> Environment = new ConfigValue<BuildEnvironment>();

        public ConfigValue<bool> DevelopmentBuild = new ConfigValue<bool>();

        public ConfigValue<int> BuildNumber = new ConfigValue<int>();

        public ConfigValue<BuildTarget> BuildTarget = new ConfigValue<BuildTarget>();

        public ConfigValue<bool> EnableDebugDirectory = new ConfigValue<bool>(true);

        public override BuildOptions BuildOptions
        {
            get
            {
                var debugFlag = DevelopmentBuild.Value
                    ? BuildOptions.AllowDebugging | BuildOptions.Development
                    : BuildOptions.None;
                return base.BuildOptions | debugFlag;
            }
        }

        /// <summary>
        /// 設定の変更が適用されたときのバインディングをします
        /// </summary>
        protected override void Bind()
        {
            base.Bind();
            ProductName.Subscribe(n => PlayerSettings.productName = n);

            BundleVersion.Subscribe(bv => PlayerSettings.bundleVersion = bv);
            BundleIdentifier.Subscribe(bi => PlayerSettings.bundleIdentifier = bi);
            BuildNumber.Subscribe (bn => PlayerSettings.iOS.buildNumber = (PlayerSettings.Android.bundleVersionCode = bn).ToString());

            iPhoneScriptCallOptimization.Subscribe(
                isco =>
                    PlayerSettings.iOS.scriptCallOptimization =
                        isco ? ScriptCallOptimizationLevel.FastButNoExceptions : ScriptCallOptimizationLevel.SlowAndSafe);




            ConfigureAsObservable().Subscribe(_ =>
            {
                
                var bi = ScriptableObject.CreateInstance<BuildInfomation>();
                bi.Build.Date = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                bi.Build.Number = BuildNumber.Value.ToString();
                bi.Build.Branch = Branch.Value;
                bi.Build.Environment = Environment.Value;
                bi.Build.IsDevelopment = DevelopmentBuild.Value;

                bi.Save();

                var ebi = ScriptableObject.CreateInstance<EditorBuildInfomation>();
                ebi.TeamId = XcodeTeamId.Value;
                ebi.CodeSignIdentity = CodeSignIdentity.Value;
                ebi.ProvisioningProfile = ProvisioningProfile.Value;
                ebi.Save();
            });

        }

        /// <summary>
        /// 現在の設定をロードして自身のインスタンスを変更します
        /// </summary>
        public override ConfigBase Load()
        {
            ProductName.Value = PlayerSettings.productName;

            BundleVersion.Value = PlayerSettings.bundleVersion;
            BundleIdentifier.Value = PlayerSettings.bundleIdentifier;
            BuildNumber.Value = PlayerSettings.Android.bundleVersionCode; //同じはずなので

            BuildTarget.Value = (BuildTarget)EditorUserBuildSettings.activeBuildTarget;

            return base.Load();
        }

        public override void OnBuild()
        {
            base.OnBuild();
            if ((int) EditorUserBuildSettings.activeBuildTarget != (int) this.BuildTarget.Value)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget((Target) BuildTarget.Value);
            }

        }
    }
}
