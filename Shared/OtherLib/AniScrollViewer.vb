
Imports System.Windows
Imports System.Windows.Controls


Public Class AniScrollViewer
    Inherits ScrollViewer
    'Register a DependencyProperty which has a onChange callback
    Public Shared _
        CurrentVerticalOffsetProperty As DependencyProperty = DependencyProperty.Register("CurrentVerticalOffset",
                                                                                          GetType(Double),
                                                                                          GetType(AniScrollViewer),
                                                                                          New PropertyMetadata(
                                                                                              New _
                                                                                                                  PropertyChangedCallback(
                                                                                                                      AddressOf _
                                                                                                                                             OnVerticalChanged)))

    Public Shared _
        CurrentHorizontalOffsetProperty As DependencyProperty =
            DependencyProperty.Register("CurrentHorizontalOffsetOffset",
                                        GetType(Double),
                                        GetType(AniScrollViewer),
                                        New PropertyMetadata(New PropertyChangedCallback(AddressOf OnHorizontalChanged)))

    'When the DependencyProperty is changed change the vertical offset, thus 'animating' the scrollViewer
    Private Shared Sub OnVerticalChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim viewer As AniScrollViewer = TryCast(d, AniScrollViewer)
        viewer.ScrollToVerticalOffset(CDbl(e.NewValue))
    End Sub

    'When the DependencyProperty is changed change the vertical offset, thus 'animating' the scrollViewer
    Private Shared Sub OnHorizontalChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim viewer As AniScrollViewer = TryCast(d, AniScrollViewer)
        viewer.ScrollToHorizontalOffset(CDbl(e.NewValue))
    End Sub


    Public Property CurrentHorizontalOffset() As Double
        Get
            Return CDbl(Me.GetValue(CurrentHorizontalOffsetProperty))
        End Get
        Set(value As Double)
            Me.SetValue(CurrentHorizontalOffsetProperty, value)
        End Set
    End Property

    Public Property CurrentVerticalOffset() As Double
        Get
            Return CDbl(Me.GetValue(CurrentVerticalOffsetProperty))
        End Get
        Set(value As Double)
            Me.SetValue(CurrentVerticalOffsetProperty, value)
        End Set
    End Property


    Private Shared Sub OnPreviewMouseWheelScrolled(sender As Object, e As MouseWheelEventArgs)
        Dim scrollHost As DependencyObject = TryCast(sender, DependencyObject)

        Dim scrollSpeed As Double = 2

        Dim scrollViewer As ScrollViewer = TryCast(OtherApi.GetScrollViewer(scrollHost), ScrollViewer)

        If scrollViewer IsNot Nothing Then
            Dim offset As Double = scrollViewer.VerticalOffset - (e.Delta*scrollSpeed/6)
            If offset < 0 Then
                scrollViewer.ScrollToVerticalOffset(0)
            ElseIf offset > scrollViewer.ExtentHeight Then
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight)
            Else
                scrollViewer.ScrollToVerticalOffset(offset)
            End If

            e.Handled = True
        Else
            Throw _
                New NotSupportedException(
                    "ScrollSpeed Attached Property is not attached to an element containing a ScrollViewer.")
        End If
    End Sub
End Class
