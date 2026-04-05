# 2レイヤー・レトロ減色パイプライン（RetDis）仕様 v0.1

## 1. 概要

RetDis は、入力画像をレトロゲーム風の見た目へ変換するクラスライブラリである。

変換結果は、以下の 2 レイヤー構造を前提とする。

- 輪郭レイヤー: 元解像度で保持する黒主体の線情報
- カラーレイヤー: 半解像度を基準にした色面情報

最終表示では、半解像度のカラーレイヤーを 2x2 実画素へ展開し、その上に輪郭レイヤーを重ねる。

本仕様で特に重視する点は次のとおり。

- レトロ感のある色面表現
- 顔パーツ、文字、建物輪郭などの保持
- 自動変換後に手修正しやすい内部表現
- ADV 用途を想定した十分に実用的な変換速度

## 2. プロジェクト構成

- `RetDis`: 変換本体を実装するクラスライブラリ
- `RetDisWin`: 表示、保存、パラメータ調整などの UI を担当するアプリケーション

## 3. 入力仕様

### 3.1 入力形式

- フルカラー画像
- 推奨形式: PNG、JPG
- 推奨色形式: RGBA 8bit/channel
- 入力サイズ: 任意

### 3.2 奇数サイズ画像

半解像度化を前提とするため、入力幅または入力高さが奇数の場合は警告対象とする。

初版では、奇数サイズ画像に対して以下の処理を行う。

- 右端の奇数 1 列を切り捨てる
- 下端の奇数 1 行を切り捨てる
- UI 上では、奇数サイズが切り捨てられたことを明示する

## 4. 出力仕様

### 4.1 出力種別

ファイル出力は次の 2 種を基本とする。

- 輪郭合成済画像: PNG
- 輪郭合成前画像: PSD

### 4.2 PNG 出力

- 元解像度
- 2x2 ディザ展開済み
- 輪郭合成済み

### 4.3 PSD 出力

PSD 出力は、少なくとも以下の編集用情報を保持できる構造を目指す。

- 輪郭レイヤー
- カラーレイヤー
- 必要に応じた補助情報

初版では、PSD 出力形式の詳細実装は後段でもよいが、内部データ構造は PSD 出力へ拡張しやすい形にしておく。

## 5. 2レイヤー構造

### 5.1 カラーレイヤー

- 元画像の 1/2 解像度を基準に生成する
- 各セルは最大 2 色と 1 つのディザパターンで表現する
- 最終表示時に各セルを 2x2 実画素へ展開する

### 5.2 輪郭レイヤー

- 元解像度で保持する
- 初版は黒または透明の 2 値でよい
- 将来的には濃灰などの多値化を許容する

## 6. カラーレイヤー詳細

### 6.1 解像度

- 出力幅 = `floor(inputWidth / 2)`
- 出力高さ = `floor(inputHeight / 2)`

### 6.2 目標色

各セルは、対応する 2x2 領域の平均色を基本の目標色とする。

ただし、輪郭レイヤーで覆われる比率が高いセルでは、色誤差の重みを下げてよい。

### 6.3 色空間

色誤差評価は Lab 色空間を推奨する。

初版では RGB ベースの簡易実装でもよいが、後から Lab へ差し替えやすい構造にする。

## 7. パレット仕様

### 7.1 基本方針

パレットは画像ごとに最適化する。固定共通パレットは前提としない。

### 7.2 色数

パレットサイズは内部的には可変対応とする。  
ただし、UI の初期値は 64 色とする。

初版で最も重視する構成は以下。

- 64 色パレット
- 2x2 ディザ
- 輪郭保持

### 7.3 初期生成方法

初期パレット生成方法は以下のいずれかでよい。

- Median Cut
- K-Means
- Popularity
- Octree

初版推奨:

1. 半解像度画像から代表色を抽出する
2. K-Means で所定色数へクラスタリングする
3. 頻度の低い色の一部をハイライト寄り、または深黒寄りの色に寄せる余地を残す

## 8. セル表現仕様

各半解像度セルは次の構造を持つ。

```csharp
public struct ColorCell
{
    public byte ColorA;
    public byte ColorB;
    public byte PatternId;
    public byte Confidence;
}
```

### 8.1 意味

- `ColorA`: 使用色 1
- `ColorB`: 使用色 2
- `PatternId`: 2x2 内での配置ルール
- `Confidence`: 自動決定の信頼度

### 8.2 単色セル

以下のいずれかの場合は、単色セルとして扱う。

- `ColorA == ColorB`
- `PatternId == SolidA`

## 9. ディザパターン仕様

初版では以下の 5 種でよい。

- `SolidA`: A 100%
- `Mix25`: A 75% + B 25%
- `Mix50`: A 50% + B 50%
- `Mix75`: A 25% + B 75%
- `SolidB`: B 100%

### 9.1 2x2 配置例

#### SolidA
```text
A A
A A
```

#### Mix25
```text
A A
A B
```

#### Mix50
```text
A B
B A
```

#### Mix75
```text
A B
B B
```

#### SolidB
```text
B B
B B
```

初版では位相固定でよい。複数位相対応は将来拡張とする。

## 10. 輪郭レイヤー仕様

### 10.1 目的

以下を元解像度のまま保持する。

- 黒線
- 暗い細線
- 顔パーツ
- 文字
- メカや建物の輪郭
- 高コントラスト境界

### 10.2 抽出条件

輪郭候補画素は、以下のいずれかを満たすものとする。

#### 条件 A: 暗色線

- `max(R, G, B) <= darkThreshold`
- かつ `saturation <= lowSaturationThreshold`

#### 条件 B: 高コントラスト境界

- Sobel などの輝度勾配強度が `edgeThreshold` 以上
- かつ局所的に暗い側へ属する

#### 条件 C: 細線優先

- 1〜2px 幅の連続線として検出される

### 10.3 初期値

- `darkThreshold = 48`
- `lowSaturationThreshold = 24`
- `edgeThreshold = 32`

### 10.4 後処理

輪郭レイヤーには以下の後処理を適用してよい。

- 小ノイズ除去
- 細線連結
- 孤立点除去
- 1px 膨張または収縮

## 11. セル候補生成仕様

各セルについて候補集合を生成する。

```csharp
public struct CellCandidate
{
    public byte ColorA;
    public byte ColorB;
    public byte PatternId;
    public float BaseError;
}
```

### 11.1 候補生成手順

各セルの目標色 `targetColor` に対し、以下を行う。

1. パレット中の近い単色を上位 N 個取得する
2. パレット中の近い色ペアを探索する
3. 各ペアについて 25 / 50 / 75 の混合見え色を評価する
4. `targetColor` との誤差が小さいものを候補として採用する

### 11.2 推奨候補数

- `CandidateCount = 16`

## 12. 画像全体最適化仕様

初版では ICM による全体最適化を採用する。

### 12.1 エネルギー関数

```text
E = Σ D_i(s_i) + λ Σ S_ij(s_i, s_j) + η Σ O_i(s_i) + γ Σ R_i(s_i)
```

### 12.2 項目

- `D_i`: 単体色誤差
- `S_ij`: 隣接セルとの整合
- `O_i`: 輪郭保護
- `R_i`: レトロ感維持のための罰則

### 12.3 初期重み例

- `WeightColorError = 1.0`
- `WeightNeighbor = 0.20`
- `WeightOutline = 0.60`
- `WeightRetro = 0.12`

### 12.4 反復回数

- `ICMIterations = 6`

## 13. Confidence 定義

`ColorCell.Confidence` は、1 位候補と 2 位候補の誤差差をもとに 0..255 へ正規化した値とする。

意図は次のとおり。

- 1 位と 2 位の差が大きいほど自信が高い
- 1 位と 2 位の差が小さいほど手修正候補になりやすい

初版では線形正規化でよい。詳細なスケーリング係数は実装パラメータとして切り出してよい。

## 14. 2x2 展開仕様

各半解像度セルを、`PatternId` に従って 2x2 実画素へ変換する。

```csharp
Color ResolvePixel(ColorCell cell, int subX, int subY);
```

- `subX` = 0 or 1
- `subY` = 0 or 1

展開結果は元解像度のカラー画像となる。

## 15. 輪郭合成仕様

### 15.1 合成順

1. カラーレイヤーを元解像度へ展開する
2. 輪郭レイヤーを上に重ねる

### 15.2 合成ルール

初版は単純上書きでよい。

```text
if outline(x, y) == black:
    output(x, y) = black
else:
    output(x, y) = colorLayer(x, y)
```

## 16. 主要パラメータ

```csharp
public sealed class RetroQuantizeParameters
{
    public int PaletteSize = 64;
    public int DownsampleFactor = 2;
    public int CandidateCount = 16;
    public int ICMIterations = 6;

    public int DarkThreshold = 48;
    public int LowSaturationThreshold = 24;
    public int EdgeThreshold = 32;

    public float WeightColorError = 1.0f;
    public float WeightNeighbor = 0.20f;
    public float WeightOutline = 0.60f;
    public float WeightRetro = 0.12f;

    public bool EnablePaletteRefine = true;
    public int PaletteRefineIterations = 2;

    public bool PreserveOutline = true;
    public bool UseLabColor = true;
}
```

## 17. モジュール分割案

実装は以下のモジュールへ分離する。

- `InputPreprocessor`: 入力画像読込、正規化、奇数サイズ切り落とし
- `OutlineExtractor`: 輪郭抽出、輪郭マスク生成
- `HalfResBuilder`: カラー用半解像度画像生成
- `PaletteBuilder`: 初期パレット生成、再学習
- `CandidateBuilder`: 各セル候補生成
- `CellOptimizer`: ICM による全体最適化
- `DitherExpander`: 2x2 展開
- `LayerComposer`: 輪郭合成
- `CacheManager`: キャッシュ管理
- `ExportPipeline`: PNG / PSD 出力

## 18. 最低限の C# インターフェース案

```csharp
public interface IRetroQuantizer
{
    RetroQuantizeResult Process(Bitmap input, RetroQuantizeParameters parameters);
}
```

```csharp
public sealed class RetroQuantizeResult
{
    public Color[] Palette { get; init; }
    public ColorCell[,] Cells { get; init; }
    public byte[,] OutlineMask { get; init; }
    public Bitmap ExpandedColorLayer { get; init; }
    public Bitmap FinalComposite { get; init; }
}
```

## 19. 処理フロー擬似コード

```csharp
Process(input, parameters):
    normalized = Preprocess(input)

    outlineMask = ExtractOutline(normalized, parameters)

    halfRes = BuildHalfRes(normalized, outlineMask, parameters)

    palette = BuildInitialPalette(halfRes, parameters)

    for refine in 0 .. PaletteRefineIterations:
        candidates = BuildCandidates(halfRes, palette, outlineMask, parameters)
        cells = OptimizeCells(candidates, halfRes, outlineMask, parameters)

        if !EnablePaletteRefine:
            break

        palette = RefinePalette(cells, halfRes, palette, parameters)

    expandedColor = ExpandCells(cells, palette, parameters)

    finalImage = Composite(expandedColor, outlineMask, parameters)

    return result
```

## 20. 初版の実装優先順位

### 優先度 A

- 2 レイヤー構造
- 輪郭抽出
- 半解像度化
- パレット生成
- 2x2 ディザ展開
- 輪郭合成

### 優先度 B

- 候補生成
- ICM 最適化
- パレット再学習

### 優先度 C

- キャッシュ
- リアルタイム再生成
- UI 支援情報
- PSD 出力の拡張

## 21. 実装メモ

- 最初から完璧な全体最適を狙いすぎないこと
- まずは見た目で気持ちいい結果を早く出すこと
- 輪郭レイヤーの効きが強いため、カラー側は多少荒くても成立しやすい
- 品質で迷ったら、まず輪郭保持を優先すること
- 初版では、2 レイヤー構造と 64 色前後 + 2x2 ディザ + 輪郭保持の骨格成立を最優先とする
