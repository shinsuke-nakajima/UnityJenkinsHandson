using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Build
{
    /// <summary>
    /// ビルドの方式です
    /// </summary>
    [Flags]
    public enum BuildMode
    {
        Development = 0,
        Release = 1
    }
}
