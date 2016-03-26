using System.Collections;
using System.Windows;

namespace MicroVK.CSharp.Controls
{
    internal interface IPanelHelper
    {
        IList Children { get; }

        double Width { get; }

        double Height { get; }

        Size DesiredSizeAt(int index);

        Rect GetLayoutSlot(FrameworkElement item);
    }
}