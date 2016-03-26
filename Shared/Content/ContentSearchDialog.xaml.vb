
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MicroVK.Api

Public Class ContentSearchDialog
    Implements IContent
    Private q As String

    Public Async Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
        q = e.Fragment.GetParametr("q")
        ListBox1.ItemsSource = Await messages.SearchDialogs(q, "photo_50")
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub ListBox1_OnMouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        Dim s = CType(ListBox1.SelectedItem, types.SearchDialogsItem)
        If s IsNot Nothing Then
            Dim mode = OtherApi.MyWindow1.FlyoutUrl.GetParametr("mode")
            Dim forward_messages = OtherApi.MyWindow1.FlyoutUrl.GetParametr("forward_messages")
            Select Case mode
                Case Else
                    OtherApi.MyWindow1.ContentSource =
                        New Uri(
                            "Pages/PageMessage.xaml#" & "user_id=" & s.id & "&forward_messages=" &
                            forward_messages.AddRandomParametr(),
                            UriKind.RelativeOrAbsolute)
            End Select

            OtherApi.MyWindow1.Flyout1.IsOpen = False
        End If
    End Sub
End Class
