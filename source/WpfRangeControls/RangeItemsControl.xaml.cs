namespace WpfRangeControls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    public class RangeItemsControl : ItemsControl
    {
        public static DependencyProperty OrientationProperty =
            RangePanel.OrientationProperty.AddOwner(typeof(RangeItemsControl));

        public static DependencyProperty MinimumProperty =
            RangePanel.MinimumProperty.AddOwner(typeof(RangeItemsControl));

        public static DependencyProperty MaximumProperty =
            RangePanel.MaximumProperty.AddOwner(typeof(RangeItemsControl));

        static RangeItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeItemsControl),
                                                     new FrameworkPropertyMetadata(typeof(RangeItemsControl)));
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Minimum restricts the minimum value of range
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Minimum restricts the minimum value of range
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
    }
}
