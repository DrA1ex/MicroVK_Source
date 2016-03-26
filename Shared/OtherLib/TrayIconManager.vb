Imports System.Drawing
Imports System.Windows.Media.Imaging
Imports Hardcodet.Wpf.TaskbarNotification
Imports System.Windows.Interop

Class TrayIconManager
    Public Shared isNetworError As Boolean = False
    Public Shared TaskbarIcon1 As TaskbarIcon
    Public Shared UnreadLocalEvent As Integer

    Public Shared Sub SetOnlineIcon(invisible As Boolean)
        If invisible Then
        Else
            Exit Sub
            Dim bi As New BitmapImage
            bi.BeginInit()
            bi.UriSource = New Uri("images/Invisible.ico", UriKind.RelativeOrAbsolute)
            bi.EndInit()
            TaskbarIcon1.IconSource = bi
        End If
    End Sub

    Private Shared trayIconBitmap As Bitmap
    Private Shared iconBitmap As Bitmap

    Public Shared Sub SetUnreadEvent(count As Integer)
        UnreadLocalEvent = count
        Dim c As Graphics
        trayIconBitmap = New Bitmap(16, 16)

        c = Graphics.FromImage(trayIconBitmap)

        c.DrawIcon(
            New Icon(
                Application.GetResourceStream(New Uri("images/MicroVKTrayIcon.ico", UriKind.RelativeOrAbsolute)).Stream),
            0,
            0)
        Dim countString = If(count > 99, "∞", count.ToString())
        If count > 0 Then

            c.Clear(Color.FromArgb(255, &HFA, &H68, &H0))

            Dim f = New StringFormat
            f.Alignment = StringAlignment.Center
            f.Trimming = StringTrimming.None
            c.DrawString(countString,
                         New Font(OtherApi.MyWindow1.FontFamily.ToString(), 8.0),
                         New SolidBrush(Color.FromArgb(255, 255, 255)),
                         New Rectangle(0, 0, 16, 16),
                         f)
        End If
        TaskbarIcon1.Icon = Icon.FromHandle(trayIconBitmap.GetHicon())
    End Sub

    Public Shared Sub SetUnreadTaskBarEvent(count As Integer)
        If count = 0 AndAlso UnreadLocalEvent = 0 Then
            Exit Sub
        End If
        SetUnreadEvent(count)
        UnreadLocalEvent = count
        Dim c As Graphics
        iconBitmap = New Bitmap(32, 32)
        c = Graphics.FromImage(iconBitmap)
        Dim countString = If(count > 99, "∞", count.ToString())
        c.DrawIcon(New Icon(Application.GetResourceStream(New Uri("23_32.ico", UriKind.RelativeOrAbsolute)).Stream),
                   New Rectangle(0, 0, 32, 32))
        If count > 0 Then
            ' c.FillEllipse()
            c.Clear(Color.FromArgb(My.Settings.AccentColor.R, My.Settings.AccentColor.G, My.Settings.AccentColor.B))
            c.FillRectangle(New SolidBrush(Color.FromArgb(255, &HFA, &H68, &H0)), New Rectangle(0, 0, 32, 32))
            Dim f = New StringFormat
            f.Alignment = StringAlignment.Center
            f.LineAlignment = StringAlignment.Center
            c.DrawString(countString,
                         New Font(OtherApi.MyWindow1.FontFamily.ToString(), 16.0),
                         New SolidBrush(Color.FromArgb(255, 255, 255)),
                         New RectangleF(0, 0, 32, 32),
                         f)
        End If
        OtherApi.MyWindow1.Icon = Imaging.CreateBitmapSourceFromHIcon(iconBitmap.GetHicon(),
                                                                      Int32Rect.Empty,
                                                                      BitmapSizeOptions.FromEmptyOptions())
    End Sub

    Friend Shared Sub ChangeNetworkStatus(isError As Boolean, Optional text As String = "")
        If isError = isNetworError Then
            Exit Sub
        End If
        Dim c As Graphics
        Dim iconBitmap As Bitmap
        iconBitmap = New Bitmap(16, 16)
        isNetworError = isError
        c = Graphics.FromImage(iconBitmap)

        c.DrawIcon(
            New Icon(
                Application.GetResourceStream(New Uri("images/MicroVKTrayIcon.ico", UriKind.RelativeOrAbsolute)).Stream),
            0,
            0)
        If isError Then
            c.Clear(Color.FromArgb(255, &HDD, &H0, &H0))
            Dim f = New StringFormat
            f.Alignment = StringAlignment.Center
            f.Trimming = StringTrimming.None
        End If
        TaskbarIcon1.Icon = Icon.FromHandle(iconBitmap.GetHicon())
        TaskbarIcon1.ToolTipText = text
    End Sub
End Class