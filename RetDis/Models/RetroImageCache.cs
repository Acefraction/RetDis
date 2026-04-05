using System;

namespace RetDis
{
    /// <summary>
    /// 同じ画像と設定の再利用を想定したキャッシュ単位。
    /// </summary>
    public sealed class RetroImageCache
    {
        /// <summary>
        /// 入力画像を識別するためのキー。
        /// </summary>
        public string SourceKey { get; set; }

        /// <summary>
        /// キャッシュ内容を最後に更新した UTC 時刻。
        /// </summary>
        public DateTime UpdatedAtUtc { get; set; }

        /// <summary>
        /// この結果を生成したときのパラメータ。
        /// </summary>
        public RetroQuantizeParameters Parameters { get; set; }

        /// <summary>
        /// 実際の変換結果本体。
        /// </summary>
        public RetroQuantizeResult Result { get; set; }
    }
}
