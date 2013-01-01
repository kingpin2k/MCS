using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Advent.VmcStudio.StartMenu
{
    public class PanelLayoutAnimator
    {
        public static readonly DependencyProperty IsAnimationEnabledProperty = DependencyProperty.RegisterAttached("IsAnimationEnabled", typeof(bool), typeof(PanelLayoutAnimator), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(PanelLayoutAnimator.OnIsAnimationEnabledInvalidated)));
        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.RegisterAttached("AnimationDuration", typeof(TimeSpan), typeof(PanelLayoutAnimator), new PropertyMetadata((object)TimeSpan.FromMilliseconds(200.0)));
        private static readonly DependencyProperty SavedArrangePositionProperty = DependencyProperty.RegisterAttached("SavedArrangePosition", typeof(Point), typeof(PanelLayoutAnimator));
        private static readonly DependencyProperty AttachedAnimatorProperty = DependencyProperty.RegisterAttached("AttachedAnimator", typeof(PanelLayoutAnimator), typeof(PanelLayoutAnimator));
        private Panel _panel;

        static PanelLayoutAnimator()
        {
        }

        public PanelLayoutAnimator(Panel panelToAnimate)
        {
            this._panel = panelToAnimate;
            this._panel.LayoutUpdated += new EventHandler(this.PanelLayoutUpdated);
        }

        public static void SetIsAnimationEnabled(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(PanelLayoutAnimator.IsAnimationEnabledProperty, enabled);
        }

        public static void SetAnimationDuration(DependencyObject dependencyObject, TimeSpan duration)
        {
            dependencyObject.SetValue(PanelLayoutAnimator.AnimationDurationProperty, (object)duration);
        }

        public void Detach()
        {
            if (this._panel == null)
                return;
            this._panel.LayoutUpdated -= new EventHandler(this.PanelLayoutUpdated);
            this._panel = (Panel)null;
        }

        private void PanelLayoutUpdated(object sender, EventArgs e)
        {
            bool flag = false;
            foreach (UIElement uiElement in this._panel.Children)
            {
                Point point1 = uiElement.TransformToAncestor((Visual)this._panel).Transform(new Point());
                Transform renderTransform = uiElement.RenderTransform;
                Point point2 = point1;
                if (renderTransform != null)
                    point2 = renderTransform.Inverse.Transform(point2);
                if (uiElement.ReadLocalValue(PanelLayoutAnimator.SavedArrangePositionProperty) != DependencyProperty.UnsetValue)
                {
                    Point point3 = (Point)uiElement.GetValue(PanelLayoutAnimator.SavedArrangePositionProperty);
                    if (!this.AreReallyClose(point3, point2))
                    {
                        Point point4 = renderTransform.Transform(point3);
                        TranslateTransform translateTransform = new TranslateTransform();
                        uiElement.RenderTransform = (Transform)translateTransform;
                        DoubleAnimation doubleAnimation1 = PanelLayoutAnimator.MakeAnimation(point4.X - point2.X, (TimeSpan)uiElement.GetValue(PanelLayoutAnimator.AnimationDurationProperty));
                        DoubleAnimation doubleAnimation2 = PanelLayoutAnimator.MakeAnimation(point4.Y - point2.Y, (TimeSpan)uiElement.GetValue(PanelLayoutAnimator.AnimationDurationProperty));
                        if (!flag)
                        {
                            doubleAnimation1.CurrentTimeInvalidated += new EventHandler(this.AnimationUpdated);
                            flag = true;
                        }
                        translateTransform.BeginAnimation(TranslateTransform.XProperty, (AnimationTimeline)doubleAnimation1);
                        translateTransform.BeginAnimation(TranslateTransform.YProperty, (AnimationTimeline)doubleAnimation2);
                    }
                }
                uiElement.SetValue(PanelLayoutAnimator.SavedArrangePositionProperty, (object)point2);
            }
        }

        private void AnimationUpdated(object sender, EventArgs e)
        {
            AdornerLayer.GetAdornerLayer((Visual)this._panel).InvalidateArrange();
        }

        private static void OnIsAnimationEnabledInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Panel panelToAnimate = dependencyObject as Panel;
            if (panelToAnimate == null)
                return;
            if ((bool)e.NewValue)
            {
                if (panelToAnimate.ReadLocalValue(PanelLayoutAnimator.AttachedAnimatorProperty) != DependencyProperty.UnsetValue)
                    return;
                PanelLayoutAnimator panelLayoutAnimator = new PanelLayoutAnimator(panelToAnimate);
                panelToAnimate.SetValue(PanelLayoutAnimator.AttachedAnimatorProperty, (object)panelLayoutAnimator);
            }
            else
            {
                if (panelToAnimate.ReadLocalValue(PanelLayoutAnimator.AttachedAnimatorProperty) == DependencyProperty.UnsetValue)
                    return;
                ((PanelLayoutAnimator)panelToAnimate.ReadLocalValue(PanelLayoutAnimator.AttachedAnimatorProperty)).Detach();
                panelToAnimate.SetValue(PanelLayoutAnimator.AttachedAnimatorProperty, DependencyProperty.UnsetValue);
            }
        }

        private bool AreReallyClose(Point p1, Point p2)
        {
            if (Math.Abs(p1.X - p2.X) < 0.001)
                return Math.Abs(p1.Y - p2.Y) < 0.001;
            else
                return false;
        }

        private static DoubleAnimation MakeAnimation(double start, TimeSpan duration)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation(start, 0.0, new Duration(duration));
            doubleAnimation.AccelerationRatio = 0.2;
            return doubleAnimation;
        }
    }
}
