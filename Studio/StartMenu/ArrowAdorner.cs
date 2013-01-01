using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Advent.VmcStudio.StartMenu
{
    internal class ArrowAdorner : Adorner
    {
        // Fields
        private readonly Arrow child;

        // Methods
        public ArrowAdorner(UIElement adornedElement, int height, Brush brush)
            : base(adornedElement)
        {
            Arrow arrow;
            arrow = new Arrow
            {
                X1 = adornedElement.DesiredSize.Width / 2.0,
                // TODO I'm assuming this is like this it was
                // X2 = arrow.X1
                X2 = adornedElement.DesiredSize.Width / 2.0,
                Y1 = -adornedElement.DesiredSize.Height / 4.0,
                Y2 = height,
                HeadHeight = 10.0,
                HeadWidth = 10.0,
                Stroke = brush,
                StrokeThickness = 6.0
            };
            this.child = arrow;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(this.child.DesiredSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            return new GeneralTransformGroup { Children = { base.GetDesiredTransform(transform), new TranslateTransform(0.0, -this.child.Y1 - this.child.Y2) } };
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.child;
        }

        protected override Size MeasureOverride(Size finalSize)
        {
            this.child.Measure(finalSize);
            return this.child.DesiredSize;
        }

        // Properties
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
    }


}
