namespace RetDis
{
    /// <summary>
    /// レトロ減色処理全体の主要パラメータをまとめた設定オブジェクト。
    /// </summary>
    public sealed class RetroQuantizeParameters
    {
        // パレット、候補数、反復回数など、処理の骨格を決める値。
        public int PaletteSize { get; set; } = 64;

        public int DownsampleFactor { get; set; } = 2;

        public int CandidateCount { get; set; } = 16;

        public int ICMIterations { get; set; } = 6;

        // 輪郭抽出まわりのしきい値。
        public int DarkThreshold { get; set; } = 48;

        public int LowSaturationThreshold { get; set; } = 24;

        public int EdgeThreshold { get; set; } = 32;

        // 全体最適化で使う重み。
        public float WeightColorError { get; set; } = 1.0f;

        public float WeightNeighbor { get; set; } = 0.20f;

        public float WeightOutline { get; set; } = 0.60f;

        public float WeightRetro { get; set; } = 0.12f;

        // 初版では簡易実装でもよいが、後から精度を上げられるようにフラグは残す。
        public bool EnablePaletteRefine { get; set; } = true;

        public int PaletteRefineIterations { get; set; } = 2;

        public bool PreserveOutline { get; set; } = true;

        public bool UseLabColor { get; set; } = true;

        /// <summary>
        /// UI やキャッシュ側で安全に使い回せるように値コピーを返す。
        /// </summary>
        public RetroQuantizeParameters Clone()
        {
            return new RetroQuantizeParameters
            {
                PaletteSize = PaletteSize,
                DownsampleFactor = DownsampleFactor,
                CandidateCount = CandidateCount,
                ICMIterations = ICMIterations,
                DarkThreshold = DarkThreshold,
                LowSaturationThreshold = LowSaturationThreshold,
                EdgeThreshold = EdgeThreshold,
                WeightColorError = WeightColorError,
                WeightNeighbor = WeightNeighbor,
                WeightOutline = WeightOutline,
                WeightRetro = WeightRetro,
                EnablePaletteRefine = EnablePaletteRefine,
                PaletteRefineIterations = PaletteRefineIterations,
                PreserveOutline = PreserveOutline,
                UseLabColor = UseLabColor,
            };
        }
    }
}
