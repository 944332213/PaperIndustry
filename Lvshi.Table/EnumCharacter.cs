using System;

namespace Lvshi.PaperProducts.Model.Table
{
    public static class EnumCharacter
    {
        [Flags]
        public enum Value
        {
            Index = 1,
            Recommend = 2,
        }

        [Flags]
        public enum Name
        {
            首页 = 1,
            推荐 = 2,
        }
    }
}
