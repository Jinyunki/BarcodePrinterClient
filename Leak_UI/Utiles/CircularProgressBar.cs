using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Leak_UI.Utiles
{
    /// <summary>
    /// 환형 진행바
    /// </summary>
    public class CircularProgressBar : Shape
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Dependency Property
        ////////////////////////////////////////////////////////////////////////////////////////// Static
        //////////////////////////////////////////////////////////////////////////////// Public

        #region 값 속성 - ValueProperty

        /// <summary>
        /// 값 속성
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
        (
            "Value",
            typeof(double),
            typeof(CircularProgressBar),
            new FrameworkPropertyMetadata
            (
                0.0,
                FrameworkPropertyMetadataOptions.AffectsRender,
                null,
                new CoerceValueCallback(ValuePropertyCoerceValueCallback)
            )
        );

        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////// Property
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 값 - Value

        /// <summary>
        /// 값
        /// </summary>
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////// Protected

        #region 정의 지오메트리 - DefiningGeometry

        /// <summary>
        /// 정의 지오메트리
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                double startAngle = 90.0;
                double endAngle = 90.0 - ((Value / 100.0) * 360.0);

                double maximumWidth = Math.Max(0.0, RenderSize.Width - StrokeThickness);
                double maximumHeight = Math.Max(0.0, RenderSize.Height - StrokeThickness);

                double xStart = maximumWidth / 2.0 * Math.Cos(startAngle * Math.PI / 180.0);
                double yStart = maximumHeight / 2.0 * Math.Sin(startAngle * Math.PI / 180.0);

                double xEnd = maximumWidth / 2.0 * Math.Cos(endAngle * Math.PI / 180.0);
                double yEnd = maximumHeight / 2.0 * Math.Sin(endAngle * Math.PI / 180.0);

                StreamGeometry streamGeometry = new StreamGeometry();

                using (StreamGeometryContext context = streamGeometry.Open())
                {
                    context.BeginFigure
                    (
                        new Point
                        (
                            (RenderSize.Width / 2.0) + xStart,
                            (RenderSize.Height / 2.0) - yStart
                        ),
                        true,
                        false
                    );

                    context.ArcTo
                    (
                        new Point
                        (
                            (RenderSize.Width / 2.0) + xEnd,
                            (RenderSize.Height / 2.0) - yEnd
                        ),
                        new Size(maximumWidth / 2.0, maximumHeight / 2),
                        0.0,
                        (startAngle - endAngle) > 180,
                        SweepDirection.Clockwise,
                        true,
                        false
                    );
                }

                return streamGeometry;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Constructor
        ////////////////////////////////////////////////////////////////////////////////////////// Static

        #region 생성자 - CircularProgressBar()

        /// <summary>
        /// 생성자
        /// </summary>
        static CircularProgressBar()
        {
            StrokeThicknessProperty.OverrideMetadata
            (
                typeof(CircularProgressBar),
                new FrameworkPropertyMetadata(10.0)
            );

            Brush brush = new SolidColorBrush(Color.FromArgb(255, 6, 176, 37));

            brush.Freeze();

            StrokeProperty.OverrideMetadata
            (
                typeof(CircularProgressBar),
                new FrameworkPropertyMetadata(brush)
            );

            FillProperty.OverrideMetadata
            (
                typeof(CircularProgressBar),
                new FrameworkPropertyMetadata(Brushes.Transparent)
            );
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Static
        //////////////////////////////////////////////////////////////////////////////// Private

        #region 값 속성 값 강제 콜백 처리하기 - ValuePropertyCoerceValueCallback(d, value)

        /// <summary>
        /// 값 속성 값 강제 콜백 처리하기
        /// </summary>
        /// <param name="d">의존 객체</param>
        /// <param name="value">값</param>
        /// <returns>처리 결과</returns>
        private static object ValuePropertyCoerceValueCallback(DependencyObject d, object value)
        {
            double valueDouble = (double)value;

            valueDouble = Math.Min(valueDouble, 99.999);
            valueDouble = Math.Max(valueDouble, 0.0);

            return valueDouble;
        }

        #endregion
    }
}
