using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MicroVK.CSharp.Controls
{
    [DefaultProperty("Orientation")] public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo,
        IPanelKeyboardHelper
    {
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight",
            typeof (double), typeof (VirtualizingWrapPanel),
            new PropertyMetadata((object) 100.0,
                new PropertyChangedCallback(VirtualizingWrapPanel.OnAppearancePropertyChanged)));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation",
            typeof (Orientation), typeof (VirtualizingWrapPanel),
            new PropertyMetadata((object) Orientation.Horizontal,
                new PropertyChangedCallback(VirtualizingWrapPanel.OnAppearancePropertyChanged)));


        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth",
            typeof (double), typeof (VirtualizingWrapPanel),
            new PropertyMetadata((object) 100.0,
                new PropertyChangedCallback(VirtualizingWrapPanel.OnAppearancePropertyChanged)));


        public static readonly DependencyProperty ScrollStepProperty = DependencyProperty.Register("ScrollStep",
            typeof (double), typeof (VirtualizingWrapPanel),
            new PropertyMetadata((object) 10.0,
                new PropertyChangedCallback(VirtualizingWrapPanel.OnAppearancePropertyChanged)));

        private bool canHorizontallyScroll;
        private bool canVerticallyScroll;
        private Size contentExtent = new Size(0.0, 0.0);
        private Point contentOffset = new Point();
        private int itemsCount;
        private int previousItemCount;
        private ScrollViewer scrollOwner;
        private Size viewport = new Size(0.0, 0.0);


        public double ItemHeight
        {
            get { return (double) this.GetValue(VirtualizingWrapPanel.ItemHeightProperty); }
            set { this.SetValue(VirtualizingWrapPanel.ItemHeightProperty, (object) value); }
        }


        public double ItemWidth
        {
            get { return (double) this.GetValue(VirtualizingWrapPanel.ItemWidthProperty); }
            set { this.SetValue(VirtualizingWrapPanel.ItemWidthProperty, (object) value); }
        }


        public Orientation Orientation
        {
            get { return (Orientation) this.GetValue(VirtualizingWrapPanel.OrientationProperty); }
            set { this.SetValue(VirtualizingWrapPanel.OrientationProperty, (object) value); }
        }


        public double ScrollStep
        {
            get { return (double) this.GetValue(VirtualizingWrapPanel.ScrollStepProperty); }
            set { this.SetValue(VirtualizingWrapPanel.ScrollStepProperty, (object) value); }
        }

        IPanelHelper IPanelKeyboardHelper.PanelHelper { get; set; }

        Point IPanelKeyboardHelper.GetOffsets(int index)
        {
            FrameworkElement containerInViewport1 = this.GetFirstContainerInViewport();
            FrameworkElement containerInViewport2 = this.GetLastContainerInViewport();
            if (containerInViewport1 != null && containerInViewport2 != null)
            {
                int num1 =
                    ((ItemContainerGenerator) this.ItemContainerGenerator).IndexFromContainer(
                        (DependencyObject) containerInViewport1);
                int num2 =
                    ((ItemContainerGenerator) this.ItemContainerGenerator).IndexFromContainer(
                        (DependencyObject) containerInViewport2);
                if (index >= num1 && index <= num2)
                    return new Point(this.HorizontalOffset, this.VerticalOffset);
            }
            int num = index/this.GetVerticalChildrenCountPerRow(this.viewport);
            double y = (double) num*this.ItemHeight;
            double x = (double) num*this.ItemWidth;
            Point point = new Point(x, y);
            if (y + this.ItemHeight > this.VerticalOffset + this.ViewportHeight)
                point.Y = y - this.ViewportHeight + this.ItemHeight;
            if (y + this.ItemWidth > this.HorizontalOffset + this.ViewportWidth)
                point.X = x - this.ViewportWidth + this.ItemWidth;
            return point;
        }

        int IPanelKeyboardHelper.GetPageUpIndex(int fromIndex)
        {
            FrameworkElement containerInViewport1 = this.GetFirstContainerInViewport();
            FrameworkElement containerInViewport2 = this.GetLastContainerInViewport();
            if (containerInViewport1 != null && containerInViewport2 != null)
            {
                int num =
                    ((ItemContainerGenerator) this.ItemContainerGenerator).IndexFromContainer(
                        (DependencyObject) containerInViewport1);
                ((ItemContainerGenerator) this.ItemContainerGenerator).IndexFromContainer(
                    (DependencyObject) containerInViewport2);
                if (num != fromIndex)
                    return num;
            }
            int childrenCountPerRow1 = this.GetHorizontalChildrenCountPerRow(this.viewport);
            int childrenCountPerRow2 = this.GetVerticalChildrenCountPerRow(this.viewport);
            int num1 = fromIndex - childrenCountPerRow1*childrenCountPerRow2;
            if (num1 >= 0)
                return num1;
            return 0;
        }

        int IPanelKeyboardHelper.GetPageDownIndex(int fromIndex)
        {
            FrameworkElement containerInViewport1 = this.GetFirstContainerInViewport();
            FrameworkElement containerInViewport2 = this.GetLastContainerInViewport();
            if (containerInViewport1 != null && containerInViewport2 != null)
            {
                ((ItemContainerGenerator) this.ItemContainerGenerator).IndexFromContainer(
                    (DependencyObject) containerInViewport1);
                int num =
                    ((ItemContainerGenerator) this.ItemContainerGenerator).IndexFromContainer(
                        (DependencyObject) containerInViewport2);
                if (num != fromIndex)
                    return num;
            }
            int childrenCountPerRow1 = this.GetHorizontalChildrenCountPerRow(this.viewport);
            int childrenCountPerRow2 = this.GetVerticalChildrenCountPerRow(this.viewport);
            int num1 = fromIndex + childrenCountPerRow1*childrenCountPerRow2;
            if (num1 <= this.itemsCount - 1)
                return num1;
            return this.itemsCount - 1;
        }

        double IPanelKeyboardHelper.GetVerticalOffsetForTouch()
        {
            return this.VerticalOffset;
        }

        double IPanelKeyboardHelper.GetHorizontalOffsetForTouch()
        {
            return this.HorizontalOffset;
        }


        public bool CanHorizontallyScroll
        {
            get { return this.canHorizontallyScroll; }
            set
            {
                if (this.canHorizontallyScroll == value)
                    return;
                this.canHorizontallyScroll = value;
                this.InvalidateMeasure();
            }
        }


        public bool CanVerticallyScroll
        {
            get { return this.canVerticallyScroll; }
            set
            {
                if (this.canVerticallyScroll == value)
                    return;
                this.canVerticallyScroll = value;
                this.InvalidateMeasure();
            }
        }

        public ScrollViewer ScrollOwner
        {
            get { return this.scrollOwner; }
            set { this.scrollOwner = value; }
        }


        public double VerticalOffset
        {
            get { return this.contentOffset.Y; }
        }


        public double ViewportHeight
        {
            get { return this.viewport.Height; }
        }

        public double ViewportWidth
        {
            get { return this.viewport.Width; }
        }


        public double ExtentHeight
        {
            get { return this.contentExtent.Height; }
        }

        public double ExtentWidth
        {
            get { return this.contentExtent.Width; }
        }

        public double HorizontalOffset
        {
            get { return this.contentOffset.X; }
        }

        public void LineDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.ScrollStep);
        }

        public void LineLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ScrollStep);
        }

        public void LineRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ScrollStep);
        }

        public void LineUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.ScrollStep);
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            this.MakeVisible(visual as UIElement);
            return rectangle;
        }

        public void MouseWheelDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.ScrollStep);
        }

        public void MouseWheelLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ScrollStep);
        }

        public void MouseWheelRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ScrollStep);
        }

        public void MouseWheelUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.ScrollStep);
        }

        public void PageDown()
        {
            this.SetVerticalOffset(this.VerticalOffset + this.ViewportHeight);
        }

        public void PageLeft()
        {
            this.SetHorizontalOffset(this.HorizontalOffset - this.ViewportHeight);
        }

        public void PageRight()
        {
            this.SetHorizontalOffset(this.HorizontalOffset + this.ViewportHeight);
        }

        public void PageUp()
        {
            this.SetVerticalOffset(this.VerticalOffset - this.viewport.Height);
        }

        public void SetVerticalOffset(double offset)
        {
            if (offset < 0.0 || this.ViewportHeight >= this.ExtentHeight)
                offset = 0.0;
            else if (offset + this.ViewportHeight >= this.ExtentHeight)
                offset = this.ExtentHeight - this.ViewportHeight;
            this.contentOffset.Y = offset;
            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();
            this.InvalidateMeasure();
        }

        public void SetHorizontalOffset(double offset)
        {
            if (offset < 0.0 || this.ViewportWidth >= this.ExtentWidth)
                offset = 0.0;
            else if (offset + this.ViewportWidth >= this.ExtentWidth)
                offset = this.ExtentWidth - this.ViewportWidth;
            this.contentOffset.X = offset;
            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();
            this.InvalidateMeasure();
        }

        internal void PageLast()
        {
            this.contentOffset.Y = this.ExtentHeight;
            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();
            this.InvalidateMeasure();
        }

        internal void PageFirst()
        {
            this.contentOffset.Y = 0.0;
            if (this.ScrollOwner != null)
                this.ScrollOwner.InvalidateScrollInfo();
            this.InvalidateMeasure();
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    if (args.Position.Index < 0 || args.Position.Index >= this.InternalChildren.Count)
                        break;
                    this.RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ItemsControl itemsOwner = ItemsControl.GetItemsOwner((DependencyObject) this);
                    if (itemsOwner == null)
                        break;
                    if (this.previousItemCount != itemsOwner.Items.Count)
                    {
                        if (this.Orientation == Orientation.Horizontal)
                            this.SetVerticalOffset(0.0);
                        else
                            this.SetHorizontalOffset(0.0);
                    }
                    this.previousItemCount = itemsOwner.Items.Count;
                    break;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            int firstVisibleItemIndex;
            int lastVisibleItemIndex;
            if (this.Orientation == Orientation.Horizontal)
                this.GetVerticalVisibleRange(out firstVisibleItemIndex, out lastVisibleItemIndex);
            else
                this.GetHorizontalVisibleRange(out firstVisibleItemIndex, out lastVisibleItemIndex);
            UIElementCollection children = this.Children;
            IItemContainerGenerator containerGenerator = this.ItemContainerGenerator;
            if (containerGenerator != null)
            {
                GeneratorPosition position = containerGenerator.GeneratorPositionFromIndex(firstVisibleItemIndex);
                int index = position.Offset == 0 ? position.Index : position.Index + 1;
                if (index == -1)
                    this.RefreshOffset();
                using (containerGenerator.StartAt(position, GeneratorDirection.Forward, true))
                {
                    int num = firstVisibleItemIndex;
                    while (num <= lastVisibleItemIndex)
                    {
                        bool isNewlyRealized;
                        UIElement child = containerGenerator.GenerateNext(out isNewlyRealized) as UIElement;
                        if (isNewlyRealized)
                        {
                            if (index >= children.Count)
                                this.AddInternalChild(child);
                            else
                                this.InsertInternalChild(index, child);
                            containerGenerator.PrepareItemContainer((DependencyObject) child);
                        }
                        if (child != null)
                            child.Measure(new Size(this.ItemWidth, this.ItemHeight));
                        ++num;
                        ++index;
                    }
                }
                this.CleanUpChildren(firstVisibleItemIndex, lastVisibleItemIndex);
            }
            Size availableSize1 = availableSize;
            if (double.IsPositiveInfinity(availableSize.Width))
                availableSize1 = new Size(this.GetExtent(availableSize1, this.itemsCount).Width, availableSize1.Height);
            if (double.IsPositiveInfinity(availableSize.Height))
                availableSize1 = new Size(availableSize1.Width, this.GetExtent(availableSize1, this.itemsCount).Height);
            return availableSize1;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            bool isHorizontal = this.Orientation == Orientation.Horizontal;
            this.InvalidateScrollInfo(finalSize);
            int num = 0;
            foreach (object obj in this.Children)
                this.ArrangeChild(isHorizontal, finalSize, num++, obj as UIElement);
            return finalSize;
        }

        private static void OnAppearancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uiElement = d as UIElement;
            if (uiElement == null)
                return;
            uiElement.InvalidateMeasure();
        }

        private void MakeVisible(UIElement element)
        {
            ItemContainerGenerator generatorForPanel =
                this.ItemContainerGenerator.GetItemContainerGeneratorForPanel((Panel) this);
            if (element == null || generatorForPanel == null)
                return;
            for (int index = generatorForPanel.IndexFromContainer((DependencyObject) element);
                index == -1;
                index = generatorForPanel.IndexFromContainer((DependencyObject) element))
                element = ParentOfTypeExtensions.ParentOfType<UIElement>((DependencyObject) element);
            ScrollViewer scrollViewer = ParentOfTypeExtensions.ParentOfType<ScrollViewer>((DependencyObject) element);
            if (scrollViewer == null)
                return;
            Rect rect =
                element.TransformToVisual((Visual) scrollViewer)
                    .TransformBounds(new Rect(new Point(0.0, 0.0), element.RenderSize));
            if (this.Orientation == Orientation.Horizontal)
            {
                if (rect.Bottom > this.ViewportHeight)
                {
                    this.SetVerticalOffset(this.contentOffset.Y + rect.Bottom - this.ViewportHeight);
                }
                else
                {
                    if (rect.Top >= 0.0)
                        return;
                    this.SetVerticalOffset(this.contentOffset.Y + rect.Top);
                }
            }
            else if (rect.Right > this.ViewportWidth)
            {
                this.SetHorizontalOffset(this.contentOffset.X + rect.Right - this.ViewportWidth);
            }
            else
            {
                if (rect.Left >= 0.0)
                    return;
                this.SetHorizontalOffset(this.contentOffset.X + rect.Left);
            }
        }

        private void GetVerticalVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            int childrenCountPerRow = this.GetVerticalChildrenCountPerRow(this.contentExtent);
            firstVisibleItemIndex = (int) Math.Floor(this.VerticalOffset/this.ItemHeight)*childrenCountPerRow;
            lastVisibleItemIndex = !double.IsInfinity(this.ViewportHeight)
                ? (int) Math.Ceiling((this.VerticalOffset + this.ViewportHeight)/this.ItemHeight)*childrenCountPerRow -
                  1
                : this.itemsCount - 1;
            this.AdjustVisibleRange(ref firstVisibleItemIndex, ref lastVisibleItemIndex);
        }

        private void GetHorizontalVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            int childrenCountPerRow = this.GetHorizontalChildrenCountPerRow(this.contentExtent);
            firstVisibleItemIndex = (int) Math.Floor(this.HorizontalOffset/this.ItemWidth)*childrenCountPerRow;
            lastVisibleItemIndex = !double.IsInfinity(this.ViewportWidth)
                ? (int) Math.Ceiling((this.HorizontalOffset + this.ViewportWidth)/this.ItemWidth)*childrenCountPerRow -
                  1
                : this.itemsCount;
            this.AdjustVisibleRange(ref firstVisibleItemIndex, ref lastVisibleItemIndex);
        }

        private void AdjustVisibleRange(ref int firstVisibleItemIndex, ref int lastVisibleItemIndex)
        {
            --firstVisibleItemIndex;
            ++lastVisibleItemIndex;
            ItemsControl itemsOwner = ItemsControl.GetItemsOwner((DependencyObject) this);
            if (itemsOwner == null)
                return;
            if (firstVisibleItemIndex < 0)
                firstVisibleItemIndex = 0;
            if (lastVisibleItemIndex < itemsOwner.Items.Count)
                return;
            lastVisibleItemIndex = itemsOwner.Items.Count - 1;
        }

        private void CleanUpChildren(int minIndex, int maxIndex)
        {
            UIElementCollection children = this.Children;
            IItemContainerGenerator containerGenerator = this.ItemContainerGenerator;
            for (int index = children.Count - 1; index >= 0; --index)
            {
                GeneratorPosition position = new GeneratorPosition(index, 0);
                int num = containerGenerator.IndexFromGeneratorPosition(position);
                if (num < minIndex || num > maxIndex)
                {
                    containerGenerator.Remove(position, 1);
                    this.RemoveInternalChildRange(index, 1);
                }
            }
        }

        private void ArrangeChild(bool isHorizontal, Size finalSize, int index, UIElement child)
        {
            if (child == null)
                return;
            int num1 = isHorizontal
                ? this.GetVerticalChildrenCountPerRow(finalSize)
                : this.GetHorizontalChildrenCountPerRow(finalSize);
            int num2 = this.ItemContainerGenerator.IndexFromGeneratorPosition(new GeneratorPosition(index, 0));
            int num3 = isHorizontal ? num2/num1 : num2%num1;
            Rect finalRect = new Rect((isHorizontal ? (double) (num2%num1) : (double) (num2/num1))*this.ItemWidth,
                (double) num3*this.ItemHeight, this.ItemWidth, this.ItemHeight);
            if (isHorizontal)
                finalRect.Y -= this.VerticalOffset;
            else
                finalRect.X -= this.HorizontalOffset;
            child.Arrange(finalRect);
        }

        private void InvalidateScrollInfo(Size availableSize)
        {
            ItemsControl itemsOwner = ItemsControl.GetItemsOwner((DependencyObject) this);
            if (itemsOwner == null)
                return;
            this.itemsCount = itemsOwner.Items.Count;
            Size extent = this.GetExtent(availableSize, this.itemsCount);
            if (extent != this.contentExtent)
            {
                this.contentExtent = extent;
                this.RefreshOffset();
            }
            if (double.IsPositiveInfinity(availableSize.Width) || double.IsPositiveInfinity(availableSize.Height) ||
                !(availableSize != this.viewport))
                return;
            this.viewport = availableSize;
            this.InvalidateScrollOwner();
            this.RefreshOffset();
        }

        private void RefreshOffset()
        {
            if (this.Orientation == Orientation.Horizontal)
                this.SetVerticalOffset(this.VerticalOffset);
            else
                this.SetHorizontalOffset(this.HorizontalOffset);
        }

        private void InvalidateScrollOwner()
        {
            if (this.ScrollOwner == null)
                return;
            this.ScrollOwner.InvalidateScrollInfo();
        }

        private Size GetExtent(Size availableSize, int itemCount)
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                int childrenCountPerRow = this.GetVerticalChildrenCountPerRow(availableSize);
                return new Size((double) childrenCountPerRow*this.ItemWidth,
                    this.ItemHeight*Math.Ceiling((double) itemCount/(double) childrenCountPerRow));
            }
            int childrenCountPerRow1 = this.GetHorizontalChildrenCountPerRow(availableSize);
            return new Size(this.ItemWidth*Math.Ceiling((double) itemCount/(double) childrenCountPerRow1),
                (double) childrenCountPerRow1*this.ItemHeight);
        }

        private int GetVerticalChildrenCountPerRow(Size availableSize)
        {
            return availableSize.Width != double.PositiveInfinity
                ? Math.Max(1, (int) Math.Floor(availableSize.Width/this.ItemWidth))
                : this.Children.Count;
        }

        private int GetHorizontalChildrenCountPerRow(Size availableSize)
        {
            return availableSize.Height != double.PositiveInfinity
                ? Math.Max(1, (int) Math.Floor(availableSize.Height/this.ItemHeight))
                : this.Children.Count;
        }

        private bool IsInTheViewport(FrameworkElement item)
        {
            if (item == null)
                return false;
            Rect layoutSlot = ((IPanelKeyboardHelper) this).PanelHelper.GetLayoutSlot(item);
            if (layoutSlot.Y >= 0.0 && layoutSlot.Height + layoutSlot.Y <= this.ViewportHeight && layoutSlot.X >= 0.0)
                return layoutSlot.Width + layoutSlot.X <= this.ViewportWidth;
            return false;
        }

        private FrameworkElement GetFirstContainerInViewport()
        {
            return
                Enumerable.FirstOrDefault<FrameworkElement>(
                    Enumerable.Cast<FrameworkElement>((IEnumerable) this.Children),
                    (Func<FrameworkElement, bool>) (item => this.IsInTheViewport(item)));
        }

        private FrameworkElement GetLastContainerInViewport()
        {
            return
                Enumerable.LastOrDefault<FrameworkElement>(
                    Enumerable.Cast<FrameworkElement>((IEnumerable) this.Children),
                    (Func<FrameworkElement, bool>) (item => this.IsInTheViewport(item)));
        }
    }
}