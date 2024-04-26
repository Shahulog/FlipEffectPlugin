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
    /// �f���G�t�F�N�g
    /// �f���G�t�F�N�g�ɂ͕K��[VideoEffect]������ݒ肵�Ă��������B
    /// </summary>
    [VideoEffect("�������]", new[] { "�A�j���[�V����" }, new string[] { },IsAviUtlSupported =false)]
    internal class FlipEffect : VideoEffectBase
    {
        /// <summary>
        /// �G�t�F�N�g�̖��O
        /// </summary>
        public override string Label => "�������]";

        /// <summary>
        /// �A�C�e���ҏW�G���A�ɕ\������G�t�F�N�g�̐ݒ荀�ځB
        /// [Display]��[AnimationSlider]���̃A�C�e���ҏW�R���g���[��������2��ݒ肷��K�v������܂��B
        /// [AnimationSlider]�ȊO�̃A�C�e���ҏW�R���g���[�������̈ꗗ��SamplePropertyEditors�v���W�F�N�g���Q�Ƃ��Ă��������B
        /// </summary>


        [Display(GroupName = "�J��Ԃ�", Name = "���E���]", Description = "���E���]")]
        [ToggleSlider]
        public bool IsSideFlip { get => isSideFlip; set => Set(ref isSideFlip, value); }
        bool isSideFlip = false;

        [Display(GroupName = "�J��Ԃ�", Name = "�㉺���]", Description = "�㉺���]")]
        [ToggleSlider]
        public bool IsWarpFlip { get => isWarpFlip; set => Set(ref isWarpFlip, value); }
        bool isWarpFlip = false;

        [Display(GroupName = "�J��Ԃ�", Name = "�Ԋu", Description = "�J��Ԃ��Ԋu")]
        [AnimationSlider("F2", "�b", 0, 1)]
        public Animation Interval { get; } = new Animation(0.30, 0, 10000);
        /// <summary>
        /// ExoFilter���쐬����
        /// </summary>
        /// <param name="keyFrameIndex">�L�[�t���[���ԍ�</param>
        /// <param name="exoOutputDescription">exo�o�͂ɕK�v�Ȋe��p�����[�^�[</param>
        /// <returns></returns>
        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            var fps = exoOutputDescription.VideoInfo.FPS;
            return new[]
            {
                $"_name=�ڂ���\r\n" +
                $"_disable={(IsEnabled ?1:0)}\r\n" +
                $"�c����=0.0\r\n" +
                $"���̋���=0\r\n" +
                $"�T�C�Y�Œ�=0\r\n",
            };
        }

        /// <summary>
        /// �f���G�t�F�N�g���쐬����
        /// </summary>
        /// <param name="devices">�f�o�C�X</param>
        /// <returns>�f���G�t�F�N�g</returns>
        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new FlipEffectProcessor(devices, this);
        }

        /// <summary>
        /// �N���X����IAnimatable�̈ꗗ���擾����
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IAnimatable> GetAnimatables() => new IAnimatable[] {  Interval };
    }
}