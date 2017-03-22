/**
 * このクラスの動作原理
 * Configに設定されているプロパティはすべてConfigValueまたはそれを継承したものだとして作成している
 * GetValues()によって自身のクラス内のすべてのConfigValueを取得してそれを比較やコピーに利用している
 * 
 * この実装はゲームで利用するにはあまりに低速なため利用することはできないが、ビルドに利用するにはConfig側にコピーなどのロジックを書かずに済む
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEditor;

namespace Assets.Editor.BuildLogic
{
    
    public class ConfigBase
    {
        /// <summary>
        /// この設定で利用されるBuildOptionsを取得します。
        /// </summary>
        public virtual BuildOptions BuildOptions
        {
            get
            {
                var b = BuildOptions.None;
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) return b | BuildOptions.Il2CPP | BuildOptions.SymlinkLibraries;
                return b;
            }
        }

        public ConfigBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Bind();
        }

        /// <summary>
        /// ConfigのObservableをBindするときに利用してください
        /// </summary>
        protected virtual void Bind()
        {

        }

        /// <summary>
        /// Configを現在のビルド設定にします。これは継承して実装してください。
        /// </summary>
        public virtual ConfigBase Load()
        {
            return this;
        }

        /// <summary>
        /// ビルドの直前に呼び出されます
        /// </summary>
        public virtual void OnBuild()
        {
            
        }


        /// <summary>
        /// 宣言されている全てのConfigValueに対してConfigureを呼び出します
        /// </summary>
        public void Configure()
        {
            foreach (var cv in GetValues().Values)
            {
                cv.Configure();
            }

            if(configureSubject != null)
                configureSubject.OnNext(Unit.Default);
        }

        private Subject<Unit> configureSubject;
        public IObservable<Unit> ConfigureAsObservable()
        {
            return configureSubject ?? (configureSubject = new Subject<Unit>());
        }

        /// <summary>
        /// 全てのConfigValueを辞書で返します（キーはプロパティ名）
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,ConfigValue> GetValues()
        {
            var myType = GetType();

            return myType.GetFields()
                .Select(f => new {n = f.Name, cv = f.GetValue(this) as ConfigValue})
                .Where(v => v.cv != null).ToDictionary(v => v.n, v => v.cv);

        }

        /// <summary>
        /// 値がすべて同じかを検証します。
        /// インスタンスは比較しません。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool EqualsValues(ConfigBase other)
        {
            if (other == null) return false;
            if(GetType() != other.GetType()) throw new InvalidOperationException("型がちがいます");
            try
            {
                var ovs = other.GetValues();
                foreach (var gv in GetValues())
                {
                    var ov = ovs[gv.Key];

                    var gvv = gv.Value.GetValue();
                    var ovv = ov.GetValue();
                    if ((ovv == null) ^ (gvv == null))
                    {
                        return false;
                    }
                    if (gvv == null) return true; // 一行上の式によりどちらもnull
                    if(!gvv.Equals(ovv)){ 
                        return false;
                    }
                }
                return true;
            }
            catch (KeyNotFoundException e)
            {
                UnityEngine.Debug.LogWarning(e.Message);
                return false;
            }
            
        }

        public override string ToString()
        {

            StringBuilder sb = new StringBuilder(1024);
            foreach (var cv in GetValues())
            {
                sb.Append(cv.Key + "=" + cv.Value.GetValue() + Environment.NewLine);
            }

            return sb.ToString();
        }
    }

    public static class Config
    {
        public static T Copy<T>(this T origin) where T : ConfigBase, new()
        {
            var cp = new T();
            var ovs = origin.GetValues();
            var cvs = cp.GetValues();

            foreach (var cv in cvs)
            {
                var ov = ovs[cv.Key];
                cv.Value.SetValue(ov.GetValue());
            }
            return cp;
        }
    }
}
