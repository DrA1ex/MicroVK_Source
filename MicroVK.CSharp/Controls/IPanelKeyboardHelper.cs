using System.Windows;

namespace MicroVK.CSharp.Controls
{
    internal interface IPanelKeyboardHelper
    {
        IPanelHelper PanelHelper { get; set; }

        Point GetOffsets(int index);

        int GetPageUpIndex(int fromIndex);

        int GetPageDownIndex(int fromIndex);

        double GetVerticalOffsetForTouch();

        double GetHorizontalOffsetForTouch();
    }
}