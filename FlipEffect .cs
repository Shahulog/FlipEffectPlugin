using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace FlipEffects
{
    /// <summary>
    /// 映像エフェクト
    /// 映像エフェクトには必ず[VideoEffect]属性を設定してください。
    /// </summary>
    [VideoEffect("反復反転", new[] { "アニメーション" }, new string[] { },IsAviUtlSupported =false)]
    internal class FlipEffect : VideoEffectBase
    {
        /// <summary>
        /// エフェクトの名前
        /// </summary>
        public override string Label => "反復反転";

        /// <summary>
        /// アイテム編集エリアに表示するエフェクトの設定項目。
        /// [Display]と[AnimationSlider]等のアイテム編集コントロール属性の2つを設定する必要があります。
        /// [AnimationSlider]以外のアイテム編集コントロール属性の一覧はSamplePropertyEditorsプロジェクトを参照してください。
        /// </summary>


        [Display(GroupName = "繰り返し", Name = "左右反転", Description = "左右反転")]
        [ToggleSlider]
        public bool IsSideFlip { get => isSideFlip; set => Set(ref isSideFlip, value); }
        bool isSideFlip = false;

        [Display(GroupName = "繰り返し", Name = "上下反転", Description = "上下反転")]
        [ToggleSlider]
        public bool IsWarpFlip { get => isWarpFlip; set => Set(ref isWarpFlip, value); }
        bool isWarpFlip = false;

        [Display(GroupName = "繰り返し", Name = "間隔", Description = "繰り返す間隔")]
        [AnimationSlider("F2", "秒", 0, 1)]
        public Animation Interval { get; } = new Animation(0.30, 0, 10000);
        /// <summary>
        /// ExoFilterを作成する
        /// </summary>
        /// <param name="keyFrameIndex">キーフレーム番号</param>
        /// <param name="exoOutputDescription">exo出力に必要な各種パラメーター</param>
        /// <returns></returns>
        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            var fps = exoOutputDescription.VideoInfo.FPS;
            return new[]
            {
                $"_name=ぼかし\r\n" +
                $"_disable={(IsEnabled ?1:0)}\r\n" +
                $"縦横比=0.0\r\n" +
                $"光の強さ=0\r\n" +
                $"サイズ固定=0\r\n",
            };
        }

        /// <summary>
        /// 映像エフェクトを作成する
        /// </summary>
        /// <param name="devices">デバイス</param>
        /// <returns>映像エフェクト</returns>
        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new FlipEffectProcessor(devices, this);
        }

        /// <summary>
        /// クラス内のIAnimatableの一覧を取得する
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IAnimatable> GetAnimatables() => new IAnimatable[] {  Interval };
    }
}