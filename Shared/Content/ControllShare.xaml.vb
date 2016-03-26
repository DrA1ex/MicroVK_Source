' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation

Public NotInheritable Class ControllShare
    Inherits UserControl
    Implements IContent
    Public Shared ScreenshotPath As String
    Public Shared CustomText As String

    Public Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
        Dim text = If(CustomText.IsNullOrEmpty, My.Resources.microvkRecommend, CustomText)
        SendControl1.SendSettingButton.Visibility = Visibility.Collapsed
        SendControl1.TextBox1.Document.Blocks.Clear()
        SendControl1.TextBox1.Document.Blocks.Add(New Paragraph(New Run(text)))
        SendControl1.TextBox1.SpellCheck.IsEnabled = False
        ScreenShotImage.Source = TryCast(New ImageSourceConverter().ConvertFromString(ScreenshotPath), ImageSource)
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub ButtonBase_OnClick(sender As Object, e As RoutedEventArgs)
        If AddScreenshotCheckBox.IsChecked Then
            SendControl1.AddAttachmentFromPath({ScreenshotPath})
        Else
            SendControl1.Button_Click_1(Nothing, Nothing)
        End If
    End Sub

    Private Sub ControllShare_OnInitialized(sender As Object, e As EventArgs)
        AddHandler SendControl1.OnSend, Sub()
            OtherApi.MyWindow1.Flyout1.IsOpen = False
                                        End Sub
    End Sub

    Private Sub ButtonShare_OnClick(sender As Object, e As RoutedEventArgs)
        SendControl1.SendMessages()
    End Sub
End Class
