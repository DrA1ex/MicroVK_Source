
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation

Public Class ControlDialogOld
    Implements IContent

    Public Async Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
        ControlDialogListBox1.SendControl = SendControl1
        SendControl1.SmileListBox.Visibility = Visibility.Collapsed
        SendControl1.DialogListBox = ControlDialogListBox1.ListBox1
        SendControl1.FloatDialogTopMenuItem.Visibility = Visibility.Visible
        ControlDialogListBox1.OnFragmentNavigation(New FragmentNavigationEventArgs() With {.Fragment = e.Fragment})

        Await NetHelper.Delay(100)

        Keyboard.Focus(SendControl1.TextBox1)
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub
End Class