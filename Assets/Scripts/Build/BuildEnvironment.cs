using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Build
{
    /// <summary>
    /// ビルド環境
    /// </summary>
    public enum BuildEnvironment
    {
        /// <summary>
        /// ローカル
        /// </summary>
        Local,
        /// <summary>
        /// dev
        /// </summary>
        Development,
        /// <summary>
        /// staging
        /// </summary>
        Staging,
        /// <summary>
        /// 審査環境
        /// </summary>
        Inspection,
        /// <summary>
        /// 本番環境
        /// </summary>
        Production
    }
}
