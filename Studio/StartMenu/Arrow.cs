using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Advent.VmcStudio.StartMenu
{
    public sealed class Arrow : Shape
    {
        // Fields
        public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register("HeadHeight", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register("HeadWidth", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        // Methods
        private void InternalDrawArrowGeometry(StreamGeometryContext context)
        {
            double a = Math.Atan2(this.Y1 - this.Y2, this.X1 - this.X2);
            double num2 = Math.Sin(a);
            double num3 = Math.Cos(a);
            Point startPoint = new Point(this.X1, this.Y1);
            Point point = new Point(this.X2, this.Y2);
            Point point3 = new Point(this.X2 + ((this.HeadWidth * num3) - (this.HeadHeight * num2)), this.Y2 + ((this.HeadWidth * num2) + (this.HeadHeight * num3)));
            Point point4 = new Point(this.X2 + ((this.HeadWidth * num3) + (this.HeadHeight * num2)), this.Y2 - ((this.HeadHeight * num3) - (this.HeadWidth * num2)));
            context.BeginFigure(startPoint, true, false);
            context.LineTo(point, true, true);
            context.LineTo(point3, true, true);
            context.LineTo(point, true, true);
            context.LineTo(point4, true, true);
        }

        // Properties
        protected override Geometry DefiningGeometry
        {
            get
            {
                StreamGeometry geometry = new StreamGeometry
                {
                    FillRule = FillRule.EvenOdd
                };
                using (StreamGeometryContext context = geometry.Open())
                {
                    this.InternalDrawArrowGeometry(context);
                }
                geometry.Freeze();
                return geometry;
            }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeadHeight
        {
            get
            {
                return (double)base.GetValue(HeadHeightProperty);
            }
            set
            {
                base.SetValue(HeadHeightProperty, value);
            }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeadWidth
        {
            get
            {
                return (double)base.GetValue(HeadWidthProperty);
            }
            set
            {
                base.SetValue(HeadWidthProperty, value);
            }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double X1
        {
            get
            {
                return (double)base.GetValue(X1Property);
            }
            set
            {
                base.SetValue(X1Property, value);
            }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double X2
        {
            get
            {
                return (double)base.GetValue(X2Property);
            }
            set
            {
                base.SetValue(X2Property, value);
            }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y1
        {
            get
            {
                return (double)base.GetValue(Y1Property);
            }
            set
            {
                base.SetValue(Y1Property, value);
            }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y2
        {
            get
            {
                return (double)base.GetValue(Y2Property);
            }
            set
            {
                base.SetValue(Y2Property, value);
            }
        }
    }


}
