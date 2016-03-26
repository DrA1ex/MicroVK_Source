Public Class PhotosWrapPanel
    Inherits WrapPanel

    Protected Overrides Function MeasureOverride(constraint As Size) As Size
        Return constraint
    End Function

    Protected Overrides Function ArrangeOverride(finalSize As Size) As Size

        Select case Children.Count
            case 0
            Case 1
                Children(0).Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height))
            case else
                Children(0).Arrange(new Rect(0, 0, finalSize.Width/2, finalSize.Height))
                Children(1).Arrange(new Rect(finalSize.Width/2, 0, finalSize.Width/2, finalSize.Height))
        End Select
        Return finalSize
    End Function
End Class
