namespace Lushi.PaperProducts.Model.Table
{
    public static class EnumColumnTypeDisplayModel
    {
        public enum Value
        {
            /// <summary>
            /// 常规
            /// </summary>
            Conventional = 0,

            /// <summary>
            /// 轮播
            /// </summary>
            Carousel = 1,

            /// <summary>
            /// 平铺
            /// </summary>
            Tile = 2,

            /// <summary>
            /// 滚动
            /// </summary>
            Roll = 3,
        }

        public enum Name
        {
            常规 = 0,
            轮播 = 1,
            平铺 = 2,
            滚动 = 3,
        }
    }
}
