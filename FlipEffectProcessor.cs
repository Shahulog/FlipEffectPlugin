using Microsoft.VisualBasic;
using System.Windows;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace FlipEffects
{
    internal class FlipEffectProcessor : IVideoEffectProcessor
    {
        readonly FlipEffect item;
       // readonly Vortice.Direct2D1.Effects.AffineTransform2D affineTransform2DEffect;
        readonly Vortice.Direct2D1.Effects.Transform3D transform3DEffect;
        

        public ID2D1Image Output { get; }

        double interval;
        bool isFlip = false;
        bool isSideFlip = true;
        bool isWarpFlip = false;
        double FlipedAngle = 180.0;
        double NormalAngle = 0.0;
        List<System.Numerics.Matrix4x4> matrices = new();

        public FlipEffectProcessor(IGraphicsDevicesAndContext devices, FlipEffect item)
        {
            this.item = item;
            transform3DEffect = new Vortice.Direct2D1.Effects.Transform3D(devices.DeviceContext);
            
            //affineTransform2DEffect = new Vortice.Direct2D1.Effects.AffineTransform2D(devices.DeviceContext);
            //            blurEffect = new Vortice.Direct2D1.Effects.GaussianBlur(devices.DeviceContext);
            Output = transform3DEffect.Output;//EffectからgetしたOutputは必ずDisposeする。Effect側ではDisposeされない。
        }

        /// <summary>
        /// エフェクトに入力する映像を設定する
        /// </summary>
        /// <param name="input"></param>
        public void SetInput(ID2D1Image input)
        {
            transform3DEffect.SetInput(0, input, true);
        }

        /// <summary>
        /// エフェクトに入力する映像をクリアする
        /// </summary>
        public void ClearInput()
        {
            transform3DEffect.SetInput(0, null, true);
        }
        private System.Numerics.Matrix4x4 RotXbyRadian(double radian)
        {
            return new System.Numerics.Matrix4x4(
                 1, 0, 0, 0,
                 0, (float)Math.Cos(radian), (float)-Math.Sin(radian), 0,
                 0, (float)Math.Sin(radian), (float)Math.Cos(radian), 0,
                 0, 0, 0, 1

                );
        }
        private System.Numerics.Matrix4x4 RotYbyRadian(double radian)
        {
            return new System.Numerics.Matrix4x4(
                 (float)Math.Cos(radian), 0, (float)-Math.Sin(radian), 0,
                 0, 1, 0, 0,
                 (float)Math.Sin(radian), 0, (float)Math.Cos(radian), 0,
                 0, 0, 0, 1

                );
        }

        private System.Numerics.Matrix4x4 RotXbyAngle(double  angle)
        {
           
            return RotXbyRadian(Math.PI * angle / 180);
        }
        private System.Numerics.Matrix4x4 RotYbyAngle(double angle)
        {

            return RotYbyRadian(Math.PI * angle / 180);
        }
       
        private System.Numerics.Matrix4x4 ProductMatrix4x4(IList<System.Numerics.Matrix4x4> matrices)
        {
            if (matrices.Count == 0) return new System.Numerics.Matrix4x4();
            else if (matrices.Count == 1) return matrices.First();
            else
            {
                System.Numerics.Matrix4x4 result = matrices.First();
                foreach (var item in matrices.Skip(1))
                {
                    result = result * item;
                }
                return result;
            }
        }

        /// <summary>
        /// エフェクトを更新する
        /// </summary>
        /// <param name="effectDescription">エフェクトの描画に必要な各種設定項目</param>
        /// <returns>描画関連の設定項目</returns>
        public DrawDescription Update(EffectDescription effectDescription)
        {
            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;
            //double routateX = item.RoutateX.GetValue(frame, length, fps);
            double interval = item.Interval.GetValue(frame, length, fps);
            bool isSideFlip = item.IsSideFlip;
            bool isWarpFlip = item.IsWarpFlip;


            bool CheckIsFlip()
            {
                var seconds = (float)frame / (float)fps;
                    if (((seconds < interval) || ((int)Math.Floor(seconds / interval) % 2 == 0)))
                    {
                        isFlip = false;
                    }
                    else
                    {
                        isFlip = true;
                    }
                return isFlip;
            }
          
            if (CheckIsFlip())
            {
                matrices.Clear();
                if (isSideFlip)
                {
                    matrices.Add(RotYbyAngle(FlipedAngle));
                }
                else
                {
                    matrices.Add(RotYbyAngle(NormalAngle));
                }
                if (isWarpFlip)
                {
                    matrices.Add(RotXbyAngle(FlipedAngle));
                   
                }
                else
                {
                    matrices.Add(RotXbyAngle(NormalAngle));
                   
                }

                transform3DEffect.TransformMatrix = ProductMatrix4x4(matrices);
            }
            else
            {
                transform3DEffect.TransformMatrix = RotXbyAngle(NormalAngle) * RotYbyAngle(NormalAngle);

            }
            
               



            this.interval = interval;
            this.isSideFlip = isSideFlip;
            this.isWarpFlip = isWarpFlip;

            return effectDescription.DrawDescription;
        }



        public void Dispose()
        {
            transform3DEffect.SetInput(0, null, true);//Inputは必ずnullに戻す。
            Output.Dispose();//EffectからgetしたOutputは必ずDisposeする。Effect側ではDisposeされない。
            transform3DEffect.Dispose();
        }
    }
}