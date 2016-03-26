Imports System.IO
Imports FirstFloor.ModernUI.Windows
Imports MicroVK._2048
Imports MicroVK._2048.Model
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MahApps.Metro.Controls
Imports MicroVK.Api

Class Game2048
    Implements IContent

    Private Const _MAX_GRID_SIZE As Integer = 600

    Private _gameGrid As GameGrid

    Sub New()

        ' Этот вызов является обязательным для конструктора.
        InitializeComponent()

        ' Добавить код инициализации после вызова InitializeComponent().
    End Sub

    Private Sub Game2048_OnSizeChanged(sender As Object, e As SizeChangedEventArgs)
        If _gameGrid Is Nothing Then
            Exit Sub
        End If
        Dim gridSize = Math.Min(ActualHeight, ActualWidth)*0.9
        gridSize = Math.Min(gridSize, _MAX_GRID_SIZE)

        _gameGrid.Width = gridSize
        _gameGrid.Height = gridSize
    End Sub

    Private Sub Game2048_OnKeyDown(sender As Object, e As KeyEventArgs)
        Dim direction As System.Nullable(Of MoveDirection) = Nothing
        If e.Key = Key.Up Then
            direction = MoveDirection.Up
        ElseIf e.Key = Key.Down Then
            direction = MoveDirection.Down
        ElseIf e.Key = Key.Left Then
            direction = MoveDirection.Left
        ElseIf e.Key = Key.Right Then
            direction = MoveDirection.Right
        End If

        If direction IsNot Nothing Then
            _gameGrid?.HandleMove(direction.Value)
        End If
    End Sub


    Public FriendList As New List(Of types.user)
    Public GameLabel As String
    Private isGameLoaded As Boolean

    Private Async Sub Game2048_OnLoaded(sender As Object, e As RoutedEventArgs)
        ContentGrid.Focusable = True
        ContentGrid.Focus()
        Keyboard.Focus(ContentGrid)
        If isGameLoaded Then Exit Sub
        isGameLoaded = True
        OtherApi.Game20481 = Me
        If OtherApi.MyUser IsNot Nothing AndAlso OtherApi.MyUser.sex = 2 Then
            GameLabel = OtherApi.SexToString(My.Resources.GameLabel, OtherApi.MyUser.sex)
        Else
            GameLabel = OtherApi.SexToString(My.Resources.GameLabel, 0)
        End If
        If FriendList Is Nothing OrElse FriendList.Count = 0 Then
            ModernProgressRing1.Visibility = Visibility.Visible
            FriendList =
                (Await friends.get("", "", "50", "photo_100", "acc")).Reverse().Where(
                    Function(user) Not (user.photo_100 Like "*deactivated*" Or user.photo_100 Like "*camera*")).ToList()
            If (FriendList.Count > 12) Then
                FriendList = FriendList.Skip(FriendList.Count - 12).ToList()
            End If
            ContentGrid.Visibility = Visibility.Visible
            ModernProgressRing1.Visibility = Visibility.Collapsed
            Randomize()

            ReloadGameGrid()

        End If
    End Sub

    Sub ReloadGameGrid()

        If ContentGrid.Children.Count > 0 Then
            ContentGrid.Children.Clear()
            _gameGrid = Nothing
        End If
        If _gameGrid Is Nothing Then
            _gameGrid = New GameGrid()
        End If
        ContentGrid.Children.Add(_gameGrid)
        Dim gridSize = Math.Min(ActualHeight, ActualWidth)*0.9
        gridSize = Math.Min(gridSize, _MAX_GRID_SIZE)

        maxUser = nothing
        maxValue = 0
        Run1.Text = ""
        _gameGrid.Width = gridSize
        _gameGrid.Height = gridSize
    End Sub

    Public Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
#If Not XP Then
        Yandex.Metrica.YandexMetrica.ReportEvent("2048_play")
#End If
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub


    Private maxValue As Integer
    Private maxUser As types.user

    Public Sub UpdateMaxValue(value As Integer, u As types.user)
        If value > maxValue Then
            maxValue = value

            If u IsNot Nothing AndAlso maxValue > 2 Then
                maxUser = u
                Button1.Visibility = Visibility.Visible
                Run1.Text = String.Format(GameLabel, u.full_name & " (" & maxValue & ")", "")
                Run2.Text = _gameGrid?.Score.ToString()
            End If
        ElseIf maxUser IsNot Nothing Then
            Run1.Text = String.Format(GameLabel, maxUser.full_name & " (" & maxValue & ")", "")
            Button1.Visibility = Visibility.Visible
            Run2.Text = _gameGrid?.Score.ToString()
        Else
            Button1.Visibility = Visibility.Visible
            Run2.Text = _gameGrid?.Score.ToString()
        End If
    End Sub

    Public Sub GameOver()
    End Sub

    Private Sub ButtonBase_OnClick(sender As Object, e As RoutedEventArgs)
        Dim t = New RenderTargetBitmap(CInt(OtherApi.MyWindow1.ActualWidth),
                                       CInt(OtherApi.MyWindow1.ActualHeight),
                                       96,
                                       96,
                                       PixelFormats.Default)
        t.Render(OtherApi.MyWindow1)

        Dim file = IO.Path.GetTempFileName() & ".jpg"

        Dim jpeg = New JpegBitmapEncoder()
        jpeg.Frames.Add(BitmapFrame.Create(t))
        Using filestream = New FileStream(file, FileMode.Create)
            jpeg.Save(filestream)
        End Using

        Dim text As String
        If maxUser IsNot Nothing Then
            text = String.Format(GameLabel,
                                 "@id" & maxUser.id & "(" & maxUser.full_name & ")",
                                 _gameGrid.Score & vbNewLine + "#MicroVK_2048")
        Else
            text = _gameGrid.Score & vbNewLine + "#MicroVK_2048"
        End If

        text = text + " https://vk.com/microvk"
        ControllShare.ScreenshotPath = file
        ControllShare.CustomText = text
        OtherApi.MyWindow1.ModernFrame1.Source = Nothing
        OtherApi.MyWindow1.ShowFlyout(Position.Left, "Content/ControllShare.xaml", My.Resources.Share)
        Exit Sub
        'Using webClient As New WebClient
        '    Dim s = Await Api.photos.GetWallUploadServer()
        '    Dim p = Await webClient.UploadFileTaskAsync(s.upload_url, file)

        '    Dim response =
        '            Await _
        '            MyJsonConvert.DeserializeObjectAsync (Of types.ResponseUploadServer)(Text.Encoding.UTF8.GetString(p))
        '    Dim photo = (Await photos.SaveWallPhoto(response, "")).FirstOrDefault()
        '    If maxUser IsNot Nothing Then
        '        Dim i =
        '                Await _
        '                wall.Post(
        '                    String.Format(GameLabel,
        '                                  "@id" & maxUser.id & "(" & maxUser.full_name & ")",
        '                                  _gameGrid.Score & vbNewLine + "#MicroVK_2048"),
        '                    "photo" & photo.owner_id & "_" & photo.id)
        '    Else
        '        Dim i =
        '                Await _
        '                wall.Post(_gameGrid.Score & vbNewLine + "#MicroVK_2048",
        '                          "photo" & photo.owner_id & "_" & photo.id)
        '    End If

        'End Using
        'ModernProgressRing2.Visibility = Visibility.Collapsed
    End Sub

    Private Sub ButtonBase1_OnClick(sender As Object, e As RoutedEventArgs)
        ReloadGameGrid()
    End Sub

    Private Sub ContentGrid_OnLostKeyboardFocus(sender As Object, e As KeyboardFocusChangedEventArgs)
        ContentGrid.Focus()
        Keyboard.Focus(ContentGrid)
    End Sub

    Private Sub UserControl_IsVisibleChanged(sender As Object, e As DependencyPropertyChangedEventArgs)
        ContentGrid.Focus()
        Keyboard.Focus(ContentGrid)
    End Sub

    Private Sub Game2048_OnInitialized(sender As Object, e As EventArgs)
        AddHandler OtherApi.MyWindow1.PreviewKeyDown, AddressOf MyWindow1_PreviewKeyDown
        RemoveHandler OtherApi.MyWindow1.PreviewKeyDown, AddressOf MyWindow1_PreviewKeyDown
    End Sub

    Private Sub MyWindow1_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If OtherApi.MyWindow1.FlyoutUrl Like "*2048*" Then
            ContentGrid.Focus()
            Keyboard.Focus(ContentGrid)
        End If
    End Sub
End Class
