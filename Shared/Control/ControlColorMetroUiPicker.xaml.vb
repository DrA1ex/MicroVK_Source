Public Class ControlColorMetroUiPicker
    Inherits UserControl
    Property TypeNotyfication As Integer = 0

    Private Sub button_Click(sender As Object, e As RoutedEventArgs)
        Popup1.IsOpen = False
        Popup1.IsOpen = True
    End Sub

    Private Sub ListBox1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If ListBox1.SelectedIndex >= 0 Then
            Me.Background = New SolidColorBrush(CType(ListBox1.SelectedItem, System.Windows.Media.Color))
        End If
        If Me.Background IsNot Nothing Then
            Dim c As Color? = TryCast(Me.Background, SolidColorBrush)?.Color
            Me.Background = New SolidColorBrush(Color.FromArgb(CByte(Slider1.Value*255),
                                                               c.Value.R,
                                                               c.Value.G,
                                                               c.Value.B))
        End If
    End Sub

    Private Sub RangeBase_OnValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Me.Background IsNot Nothing Then
            Dim c As Color? = TryCast(Me.Background, SolidColorBrush)?.Color
            Me.Background = New SolidColorBrush(Color.FromArgb(CByte(Slider1.Value*255),
                                                               c.Value.R,
                                                               c.Value.G,
                                                               c.Value.B))
        End If
    End Sub
End Class
