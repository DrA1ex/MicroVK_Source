
Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI.Windows.Controls
Imports MahApps.Metro.Controls
Imports MicroVK.Api
Imports MicroVK.OtherLib


Namespace Command
    Class PlayAudio
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            If IsNothing(OtherApi.BassPlayer1) Then OtherApi.BassPlayer1 = New WindowPlayer
            OtherApi.BassPlayer1.Play(CType(parameter, types.attachment).audio)
            OtherApi.BassPlayer1.Playlist1 = Nothing
            OtherApi.BassPlayer1.PlaylistTecIndex1 = 0
        End Sub
    End Class

    Class Feedback
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            If Not IsNothing(parameter) Then
                OtherApi.ProcessStart("http://vk.com/kryeker")
            Else
                OtherApi.MyWindow1.ContentSource = New Uri("Pages/PageMessage.xaml#user_id=64974451",
                                                           UriKind.RelativeOrAbsolute)
            End If
        End Sub
    End Class

    Class ShowWindowCommand
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            OtherApi.MyWindow1.Show()
            If OtherApi.MyWindow1.WindowState = WindowState.Minimized Then _
                OtherApi.MyWindow1.WindowState = WindowState.Normal
            OtherApi.MyWindow1.Visibility = Visibility.Visible
            OtherApi.MyWindow1.Activate()
        End Sub
    End Class

    Class MinimizeWindowCommand
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            OtherApi.MyWindow1.WindowState = WindowState.Minimized
        End Sub
    End Class

    Class RestoreWindowCommand
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            OtherApi.MyWindow1.WindowState = WindowState.Normal
        End Sub
    End Class

    Class MaximizeWindowCommand
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            OtherApi.MyWindow1.WindowState = WindowState.Maximized
        End Sub
    End Class

    Class CloseWindowCommand
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            OtherApi.MyWindow1.Close()
        End Sub
    End Class

    Class OpenSourceIdCommand
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return CInt(parameter) > 0
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            OtherApi.MyWindow1.ContentSource = New Uri("Pages/PageUserInfo.xaml#user_id=" & parameter.ToString,
                                                       UriKind.RelativeOrAbsolute)
        End Sub
    End Class

    Class OpenSourceIdCommandInN
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            OtherApi.MyWindow1.ContentSource = New Uri("Pages/PageUserInfo.xaml#user_id=" & parameter.ToString,
                                                       UriKind.RelativeOrAbsolute)
            OtherApi.MyWindow1.Show()
            If (OtherApi.MyWindow1.WindowState = WindowState.Minimized) Then _
                OtherApi.MyWindow1.WindowState = WindowState.Normal
            OtherApi.MyWindow1.Visibility = Visibility.Visible
            OtherApi.MyWindow1.Activate()
        End Sub
    End Class

    Class LikeCommand
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Async Sub Execute(parameter As Object) Implements ICommand.Execute

            Dim p = TryCast(parameter, types.wall_post)
            If p Is Nothing Then Exit Sub
            p.likes.user_likes = Not p.likes.user_likes
            If p.likes.user_likes Then
                p.likes.count = Await likes.Add(p.post_type, p.id.ToString(), p.from_id.ToString)
            Else
                p.likes.count = Await likes.Delete(p.post_type, p.id.ToString(), p.from_id.ToString)
            End If
        End Sub
    End Class

    Class OpenUrlCommand
        Implements ICommand

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

        Public Async Sub Execute(parameter As Object) Implements ICommand.Execute
            Await OtherApi.GoToURL(parameter.ToString)
        End Sub
    End Class

    Class CloseDialogTab
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim t = CType(parameter, MyTabItem)
            Dim user_id = t.Description.GetParametr("user_id")
            Dim chat_id = t.Description.GetParametr("chat_id")
            OtherApi.DialogsTabControl.Items.Remove(t)

            If user_id.Length > 0 Then
                messages.MessagesDictionary.Remove(user_id)
            Else
                messages.MessagesDictionary.Remove("c" & chat_id)
            End If
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class CloseAllDialogTab
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            OtherApi.DialogsTabControl.Items.Clear()
            messages.MessagesDictionary.Clear()
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class OpenFloatWindows
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Async Sub Execute(parameter As Object) Implements ICommand.Execute

            Dim t = CType(parameter, MyTabItem)
            If t Is Nothing Then Exit Sub
            Dim user_id = t.Description.GetParametr("user_id")
            Dim chat_id = t.Description.GetParametr("chat_id")

            OtherApi.DialogsTabControl.Items.Remove(t)
            t.Content = New Uri(t.Content.OriginalString.Replace("ListBox.xaml", ".xaml"), UriKind.RelativeOrAbsolute)
            t.Frame = New ModernFrame() With {.Source = t.Content}
            Dim w = New FloatDialogWindow() With {.Title = t.Title, .Width = 250, .Height = 400, .MinWidth = 200,
                    .DataContext = t}
            t.FloatWindows = w

            AddHandler w.SizeChanged, Sub(sender As Object, e As SizeChangedEventArgs)
                Dim mw = CType(sender, ModernWindow)
                If mw.WindowState = WindowState.Maximized Then
                    mw.WindowState = WindowState.Normal
                End If
                                      End Sub
            AddHandler w.Activated, Sub(sender, args)
                OtherApi.MyWindow1.TaskbarItemInfo.ProgressValue = 0
                TrayIconManager.SetUnreadTaskBarEvent(0)
                                    End Sub

            w.Show()
#If NEt46 Then
            Await Task.Delay(100)
#Else
            Await TaskEx.Delay(100)
#End If
            w.Activate()
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class CloseFloatWindows
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim t = CType(parameter, MyTabItem)
            Dim user_id = t.Description.GetParametr("user_id")
            Dim chat_id = t.Description.GetParametr("chat_id")

            t.FloatWindows.Close()

            If user_id.Length > 0 Then
                messages.MessagesDictionary.Remove(user_id)
            Else
                messages.MessagesDictionary.Remove("c" & chat_id)
            End If
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class PlayVideoCommand
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim video = TryCast(parameter, types.video)
            If video Is Nothing Then Return
            If OtherApi.VideoPlayer1 Is Nothing Then
                OtherApi.VideoPlayer1 = New VideoPlayer()
            End If
            OtherApi.VideoPlayer1.Play(video)
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class OpenSourceFromSearchDialog
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim s = CType(parameter, types.SearchDialogsItem)
            OtherApi.MyWindow1.ContentSource = New Uri("Pages/PageUserInfo.xaml#user_id=" & s.id,
                                                       UriKind.RelativeOrAbsolute)
            OtherApi.MyWindow1.Flyout1.IsOpen = False
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class OpenChatInfoComand
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim chat_id = CInt(parameter)
            OtherApi.MyWindow1.Flyout1.IsOpen = False
            OtherApi.MyWindow1.ShowFlyout(Position.Right,
                                          "Content/ContentChatInfo.xaml#chat_id=" & chat_id,
                                          My.Resources.chat_users)
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class OpenDialogCommand
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            OtherApi.MyWindow1.ContentSource =
                New Uri("Pages/PageMessage.xaml#" & "user_id=" & parameter.ToString() & "&rnd=" & Rnd(),
                        UriKind.RelativeOrAbsolute)
            OtherApi.MyWindow1.Flyout1.IsOpen = False
            If (OtherApi.MyWindow1.WindowState = WindowState.Minimized) Then
                OtherApi.MyWindow1.WindowState = WindowState.Normal
            End If
            OtherApi.MyWindow1.Show()
            OtherApi.MyWindow1.Visibility = Visibility.Visible
            OtherApi.MyWindow1.Activate()
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class SaveDocCommand
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim d = CType(parameter, types.doc)
            OtherApi.ProcessStart(Trim(d.url))
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class ShowAudioRecomendationsCommand
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim a = TryCast(parameter, types.audio)
            If a Is Nothing Then Exit Sub
            OtherApi.MyWindow1.ContentSource = New Uri("/Pages/PageAudio.xaml#target_audio=" & a.owner_id & "_" & a.id,
                                                       UriKind.RelativeOrAbsolute)
            If OtherApi.MyWindow1.WindowState = WindowState.Minimized Then _
                OtherApi.MyWindow1.WindowState = WindowState.Normal

            OtherApi.MyWindow1.Show()
            OtherApi.MyWindow1.Visibility = Visibility.Visible
            OtherApi.MyWindow1.Activate()
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class AddAudioToMyReccordCommand
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Async Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim b = TryCast(parameter, types.audio)
            If b Is Nothing Then Exit Sub
            Await audio.Add(b.id.ToString(), b.owner_id.ToString())
            OtherApi.ShowMicroVkNot(b.artist & "-" & b.title, My.Resources.addToYourAudio)
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class ShowLyricsCommand
        Implements icommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim a = TryCast(parameter, types.audio)
            If a Is Nothing Then Exit Sub
            OtherApi.MyWindow1.ShowFlyout(Position.Left,
                                          "Content/ControlViewLyrics.xaml#lyrics_id=" & a.lyrics_id,
                                          My.Resources.Text)
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Dim a = TryCast(parameter, types.audio)
            If a Is Nothing Then Return False
            Return a.lyrics_id > 0
        End Function
    End Class

    Class ShowViewerCommand
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
#If Not DEBUG Then
            Exit Sub
#End If
            If TypeOf parameter Is types.wall_post Then
                Dim w = TryCast(parameter, types.wall_post)
                PhotoViewer.Attachments = New ObservableCollection(Of Object)({w})
                OtherApi.MyWindow1.ModernFrame1.Source = Nothing
                OtherApi.MyWindow1.ShowFlyout(Position.Right,
                                              "Content/Flyouts/PhotoViewer.xaml",
                                              OtherApi.AttachmentToString("wall") & " (1/" & 1 & ")",
                                              True)
            Else
            End If
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class ShowMessagesViewCommand
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim msgs = TryCast(parameter, List(Of types.message))
            If msgs Is Nothing Then Exit Sub
            MessagesView.messages = msgs
            OtherApi.MyWindow1.ModernFrame1.Source = Nothing
            OtherApi.MyWindow1.ShowFlyout(Position.Left,
                                          "Content/Flyouts/MessagesView.xaml",
                                          String.Format("{0}({1})", My.Resources.Messages, msgs.Count),
                                          False)
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return True
        End Function
    End Class

    Class LoginCommand
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim p as OtherLib.MicroVkAccount = TryCast(parameter, MicroVkAccount)
            My.Settings.AccessToken = SettingSystem.AES_Decrypt(p.AccessToken, p.id)
            System.Windows.Forms.Application.Restart()
            My.Application.Shutdown()
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            return true
        End Function
    End Class

    Class DelAccountCommand
        Implements ICommand

        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            Dim p = TryCast(parameter, MicroVkAccount)
            dim a = SettingSystem.MicroVkAccounts.FirstOrDefault(function(account) account.id = p.id)
            if a IsNot nothing
                SettingSystem.MicroVkAccounts.Remove(a)
                SettingSystem.SaveSettings("MicroVkAccounts", SettingSystem.MicroVkAccounts)
            End If
        End Sub

        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            return true
        End Function
    End Class
End Namespace
