using System.Drawing;

namespace RetDis
{
    /// <summary>
    /// 1 回の減色処理で得られた結果一式を保持する。
    /// </summary>
    public sealed class RetroQuantizeResult
    {
        /// <summary>
        /// 元画像の幅。奇数サイズ切り落とし前の値を保持する。
        /// </summary>
        public int OriginalWidth { get; set; }

        /// <summary>
        /// 元画像の高さ。奇数サイズ切り落とし前の値を保持する。
        /// </summary>
        public int OriginalHeight { get; set; }

        /// <summary>
        /// 実際に処理へ使った幅。必要に応じて右端 1 列が落ちる。
        /// </summary>
        public int ProcessedWidth { get; set; }

        /// <summary>
        /// 実際に処理へ使った高さ。必要に応じて下端 1 行が落ちる。
        /// </summary>
        public int ProcessedHeight { get; set; }

        /// <summary>
        /// 画像専用に生成されたパレット。
        /// </summary>
        public Color[] Palette { get; set; }

        /// <summary>
        /// 半解像度セルの配列。内部は 1 次元だが 2 次元アクセサで扱う。
        /// </summary>
        public Grid2D<ColorCell> Cells { get; set; }

        /// <summary>
        /// 元解像度の輪郭マスク。
        /// </summary>
        public Grid2D<byte> OutlineMask { get; set; }

        /// <summary>
        /// 2x2 展開済みだが、まだ輪郭合成していないカラーレイヤー。
        /// </summary>
        public Bitmap ExpandedColorLayer { get; set; }

        /// <summary>
        /// 輪郭まで合成した最終表示用画像。
        /// </summary>
        public Bitmap FinalComposite { get; set; }

        public bool WasInputTrimmed
        {
            get { return IsWidthTrimmed || IsHeightTrimmed; }
        }

        public bool IsWidthTrimmed
        {
            get { return OriginalWidth != ProcessedWidth; }
        }

        public bool IsHeightTrimmed
        {
            get { return OriginalHeight != ProcessedHeight; }
        }

        public int CellWidth
        {
            get { return Cells == null ? 0 : Cells.Width; }
        }

        public int CellHeight
        {
            get { return Cells == null ? 0 : Cells.Height; }
        }
    }
}
