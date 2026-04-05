namespace RetDis
{
    /// <summary>
    /// 半解像度セル 1 個ぶんの表現を保持する。
    /// </summary>
    public struct ColorCell
    {
        public ColorCell(byte colorA, byte colorB, DitherPattern pattern, byte confidence)
        {
            ColorA = colorA;
            ColorB = colorB;
            PatternId = (byte)pattern;
            Confidence = confidence;
        }

        /// <summary>
        /// 使用色 A のパレット添字。
        /// </summary>
        public byte ColorA;

        /// <summary>
        /// 使用色 B のパレット添字。
        /// </summary>
        public byte ColorB;

        /// <summary>
        /// 2x2 展開時のディザパターン ID。
        /// </summary>
        public byte PatternId;

        /// <summary>
        /// 1 位候補と 2 位候補の誤差差から求めた信頼度。
        /// </summary>
        public byte Confidence;

        /// <summary>
        /// PatternId を列挙体として扱いやすくするための補助プロパティ。
        /// </summary>
        public DitherPattern Pattern
        {
            get { return (DitherPattern)PatternId; }
            set { PatternId = (byte)value; }
        }

        /// <summary>
        /// ベタ塗りとして扱えるセルかどうかを返す。
        /// </summary>
        public bool IsSolid
        {
            get
            {
                return ColorA == ColorB
                    || Pattern == DitherPattern.SolidA
                    || Pattern == DitherPattern.SolidB;
            }
        }
    }
}
