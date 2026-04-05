namespace RetDis
{
    /// <summary>
    /// 1 セルを 2x2 実画素へ展開するときの配置パターン。
    /// </summary>
    public enum DitherPattern : byte
    {
        /// <summary>
        /// A 色のみを使うベタ塗り。
        /// </summary>
        SolidA = 0,

        /// <summary>
        /// A を 75%、B を 25% 使う。
        /// </summary>
        Mix25 = 1,

        /// <summary>
        /// A と B を半々で使う。
        /// </summary>
        Mix50 = 2,

        /// <summary>
        /// A を 25%、B を 75% 使う。
        /// </summary>
        Mix75 = 3,

        /// <summary>
        /// B 色のみを使うベタ塗り。
        /// </summary>
        SolidB = 4,
    }
}
