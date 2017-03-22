using System;

namespace Assets.Scripts.Build
{
    /// <summary>
    /// ビルド時に参照したブランチ。サーバー選択にも使う。
    /// </summary>
    public enum Branch
    {
        /// <summary>
        /// stable
        /// </summary>
        Stable,
        /// <summary>
        /// master
        /// </summary>
        Master,
        /// <summary>
        /// feature
        /// </summary>
        Feature
    }
}

