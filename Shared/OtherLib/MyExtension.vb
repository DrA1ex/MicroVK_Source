Imports System.Runtime.CompilerServices
Imports System.Windows.Controls.Primitives
Imports FirstFloor.ModernUI.Windows.Media

Module MyExtension
    <Extension()>
    Public Function IsNullOrEmpty(ByVal s As String) As Boolean
        Return String.IsNullOrEmpty(s)
    End Function

    <Extension()>
    Public Function GetParametr(ByVal fragment As String, ByVal name As String) As String
        If String.IsNullOrEmpty(fragment) Then Return ""
        Dim s = If(fragment.IndexOf("&", StringComparison.Ordinal) >= 0, "&"c, ","c)
        If InStr(fragment, name & "=") > 0 Then
            Dim a = Split(fragment, name & "=")(1)
            Return a.Split({s})(0)
        Else
            Return ""
        End If
    End Function

    <Extension()>
    Public Function GetSafeValue (Of TKey, TValue)(dictionary As Dictionary(Of TKey, TValue), key As TKey) As TValue
        Dim result As TValue = Nothing
        dictionary.TryGetValue(key, result)
        Return result
    End Function

    <Extension()>
    Public Function ToInt(ByVal a As String) As Integer
        Return CInt(Val(a))
    End Function

    <Extension()>
    Public Function AddRandomParametr(ByVal str As String) As String
        Return str & "&rnd=" & Math.Round(Rnd(), 10)
    End Function

    <Extension()>
    Public Function GetSafeValue (Of T)(ByVal arr As T(), index As Integer) As T
        Dim result As T = Nothing
        If index >= 0 AndAlso index < arr.Length Then
            result = CType(arr.GetValue(index), T)
        End If
        Return result
    End Function

    <Extension()>
    Public Function GetVerticalOffset(list As ListBox, item As Object) As Double
        Dim container As UIElement = CType(CType(list, ItemsControl).ItemContainerGenerator.ContainerFromItem(item),
                                           UIElement)
        If IsNothing(container) Then Return 0

        Dim presenter = container.Ancestors().OfType (Of ScrollContentPresenter).FirstOrDefault()
        If IsNothing(presenter) Then Return 0

        Dim scrollInfo = If _
                (Not presenter.CanContentScroll,
                 presenter,
                 If _
                    (TryCast(presenter.Content, IScrollInfo),
                     If(TryCast(FirstVisualChild(TryCast(presenter.Content, ItemsPresenter)), IScrollInfo), presenter)))
        Dim size As Size = container.RenderSize
        Dim center As Point =
                container.TransformToVisual(DirectCast(scrollInfo, UIElement)).Transform(New Point(size.Width/2, 0))
        center.Y += scrollInfo.VerticalOffset
        center.X += scrollInfo.HorizontalOffset
        If TypeOf scrollInfo Is StackPanel OrElse TypeOf scrollInfo Is VirtualizingStackPanel Then
            Dim logicalCenter As Double =
                    CType(list, ItemsControl).ItemContainerGenerator.IndexFromContainer(container) + 0.5
            Dim orientation1 As Orientation = If _
                    (TypeOf scrollInfo Is StackPanel,
                     DirectCast(scrollInfo, StackPanel).Orientation,
                     DirectCast(scrollInfo, VirtualizingStackPanel).Orientation)
            If orientation1 = Orientation.Horizontal Then
                center.X = logicalCenter
            Else
                center.Y = logicalCenter
            End If
        End If
        Return _
            container.TransformToAncestor(DirectCast(scrollInfo, Visual)).Transform(
                New Point(container.RenderSize.Width/2, 0)).Y + scrollInfo.VerticalOffset
    End Function

    Private Function FirstVisualChild(visual As Visual) As DependencyObject
        If visual Is Nothing Then
            Return Nothing
        End If
        If VisualTreeHelper.GetChildrenCount(visual) = 0 Then
            Return Nothing
        End If
        Return VisualTreeHelper.GetChild(visual, 0)
    End Function
End Module
