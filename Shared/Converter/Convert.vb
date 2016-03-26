Imports System.Globalization
Imports FirstFloor.ModernUI
Imports MahApps.Metro.Controls
Imports MicroVK.Api
Imports MicroVK.OtherLib

Namespace Converter
    Class DateConvert
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            If value IsNot Nothing AndAlso value.ToString = "no" Then
                Return Nothing
            End If
#If Not XP Then
            Dim a = DateTimeOffset.FromUnixTimeSeconds(CInt(value)).DateTime.ToLocalTime()
#Else
            Dim a = TimeZone.CurrentTimeZone.ToLocalTime(New DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(CInt(value)))
#End If
            Return a.ToString()
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class DateSmallConvert
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            If value IsNot Nothing AndAlso value.ToString = "no" Then
                Return Nothing
            Else
#If Not XP Then
                Dim a = DateTimeOffset.FromUnixTimeSeconds(CInt(value)).DateTime.ToLocalTime()
#Else
                Dim a = TimeZone.CurrentTimeZone.ToLocalTime(New DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(CInt(value)))
#End If
                If a.Date = Now.Date Then
                    Return a.ToLongTimeString()
                Else
                    Return a.ToShortDateString()
                End If
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class


    Class DialogGetName
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Dim a = CType(value, types.message)
            If a.chat_id = 0 Then
                If Not Lists.ProfilesDictionary.ContainsKey(a.user_id) Then
                    Return If(CBool(parameter), My.Resources.unknown.ToUpper(), My.Resources.unknown)
                End If
                If CBool(parameter) Then
                    Return _
                        (Lists.ProfilesDictionary(a.user_id).first_name & " " &
                         Lists.ProfilesDictionary(a.user_id).last_name).ToUpper
                Else
                    Return _
                        (Lists.ProfilesDictionary(a.user_id).first_name & " " &
                         Lists.ProfilesDictionary(a.user_id).last_name)
                End If

            Else
                If CBool(parameter) Then
                    Return a.title.ToUpper
                Else
                    Return a.title
                End If
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class GetFirstName
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Dim a = CInt(value)
            If Lists.ProfilesDictionary.ContainsKey(a) Then
                Return Lists.ProfilesDictionary(a).first_name
            Else
                Return My.Resources.unknown
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Public Class ConverterNameGet
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert

            Dim a = CInt(value)
            If a > 0 Then
                If Lists.ProfilesDictionary.ContainsKey(a) Then
                    Return Lists.ProfilesDictionary(a).first_name + " " + Lists.ProfilesDictionary(a).last_name
                Else
                    Return My.Resources.unknown
                End If
            Else
                If Lists.GroupsDictionary.ContainsKey(Math.Abs(a)) Then
                    Return Lists.GroupsDictionary(Math.Abs(a)).name
                Else
                    Return My.Resources.unknown
                End If
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class


    Public Class ConverterGetPhoto
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            If Lists.ProfilesDictionary.ContainsKey(CInt(value)) Then
                Return Lists.ProfilesDictionary(CInt(value)).photo
            Else
                Return Nothing
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Public Class ConverterGetPhotoInDialog
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Dim m = CType(value, types.message)
            Dim result = "https://vk.com/images/camera_50.png"
            If IsNothing(m) Then
                Return result
            End If
            If (m.out) Then
                If Not IsNothing(OtherApi.MyUser) Then
                    If OtherApi.MyUser.photo.Length > 0 Then
                        result = OtherApi.MyUser.photo
                    Else
                        Return result
                    End If
                Else
                    Return result
                End If
            Else
                If Lists.ProfilesDictionary.ContainsKey(m.user_id) Then
                    result = Lists.ProfilesDictionary(m.user_id).photo
                End If
            End If
            Return result
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Public Class ConvertGetMyPhoto
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Return If(IsNothing(OtherApi.MyUser), Nothing, OtherApi.MyUser.photo)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Public Class ConvartReadState
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Return If(IsNothing(value), Nothing, If(CType(value, types.message).read_state, "#00000000", "#55000000"))
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class NumberTo4
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Return CDbl(value)/4
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class PhotoSizeToUrl
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            If value Is Nothing Then Return Nothing
            Dim a = CType(value, types.attachment), url As String = ""
            If parameter?.ToString = "item" Then
                If a.type = "photo" Then
                    If a.Width <= 75 Then
                        url = a.photo.photo_75
                    ElseIf a.Width <= 130 Then
                        url = a.photo.photo_130
                    ElseIf a.Width <= 604 Then
                        url = a.photo.photo_604
                    End If
                ElseIf a.type = "video" Then
                    If a.Width <= 130 Then
                        url = a.video.photo_130
                    ElseIf a.Width <= 320 Then
                        url = a.video.photo_320
                    ElseIf a.Width <= 640 Then
                        url = If(IsNothing(a.video.photo_640), a.video.photo_320, a.video.photo_640)
                    End If
                ElseIf a.type = "doc" Then
                    If a.Width <= 100 Then
                        url = If(IsNothing(a.doc.photo_100), "", a.doc.photo_100)
                    Else
                        url = If(IsNothing(a.doc.photo_130), "", a.doc.photo_130)
                    End If
                End If
            Else
                If a.type = "photo" Then
                    url = a.photo.photo_604
                ElseIf a.type = "video" Then
                    url = If(IsNothing(a.video.photo_640), a.video.photo_320, a.video.photo_640)
                ElseIf a.type = "doc" Then
                    url = If(IsNothing(a.doc.photo_130), "", a.doc.photo_130)
                End If
            End If
            Return url
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class


    Class ActualSizeToPhotoUrl
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IMultiValueConverter.Convert

            Dim url = ""
            Dim fv = TryCast(values(0), FlipView)
            Dim attachment = TryCast(fv?.SelectedItem, types.attachment)
            Try
                Dim p = fv.PointToScreen(New Point(fv.ActualWidth, fv.ActualHeight)) - fv.PointToScreen(New Point())

                Select Case attachment?.type
                    Case "photo"
                        url = attachment.photo.UpdatePhotoFromSize(New Size(p.X, p.Y))
                End Select
                Return New ImageSourceConverter().ConvertFromString(url)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) _
            As Object() Implements IMultiValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class LikeColorConvert
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Return If _
                (CBool(value),
                 OtherApi.MyWindow1.FindResource("Accent"),
                 OtherApi.MyWindow1.FindResource("LinkButtonText"))
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class MultiParameterConvert
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IMultiValueConverter.Convert
            Return values.ToList
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) _
            As Object() Implements IMultiValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class MyVisibilityMultiConverter
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IMultiValueConverter.Convert

            Dim b = values.Aggregate(Function(o, o1) CBool(o) AndAlso CBool(o1))
            Return New MyVisibilityConverter().Convert(b, targetType, parameter, culture)
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) _
            As Object() Implements IMultiValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class MyVisibilityConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Dim result As Visibility
            If IsNothing(value) Then
                result = Visibility.Collapsed
            ElseIf value.ToString.Length = 0 Then
                result = Visibility.Collapsed
            ElseIf TypeOf value Is types.wall_post Then
                'Dim p = CType(value, types.wall_post)
                'If Not IsNothing(p.copy_history) Then
                '    If p.copy_history.text.Length = 0 Then
                '        result = Visibility.Collapsed
                '    Else
                '        result = Visibility.Visible
                '    End If
                'Else
                '    result = Visibility.Collapsed
                'End If
            ElseIf TypeOf value Is Integer AndAlso CInt(value) = 0 Then
                result = Visibility.Collapsed
            ElseIf TypeOf value Is Boolean AndAlso Not CBool(value) Then
                result = Visibility.Collapsed
            Else
                result = Visibility.Visible
            End If
            If parameter IsNot Nothing AndAlso parameter.ToString = "inverse" Then
                If result = Visibility.Visible Then
                    result = Visibility.Collapsed
                Else
                    result = Visibility.Visible
                End If
            End If
            Return result
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class PhotoToAttachments
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Dim a = New List(Of types.attachment)
            For Each i In CType(value, List(Of types.photo))
                a.Add(New types.attachment With {.type = "photo",
                         .photo = i})
            Next
            Dim s = New types.SetDockAttachments() With {.attachments = a}
            Return s
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class TypeToColor
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Dim type = CInt(value)
            If type = 0 Then Return "#FF2A3345"
            Dim c =
                    CType(
                        ColorConverter.ConvertFromString(
                            My.Settings.PropertyValues("NColor" & type).PropertyValue.ToString()),
                        Color)
            Return New SolidColorBrush(Color.FromArgb(255, c.R, c.G, c.B))
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class TypeToTransparent
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert

            Dim a = CInt(value)
            If value Is Nothing OrElse a = 0 Then Return 1
            Dim s = My.Settings.PropertyValues("NColor" & a).PropertyValue.ToString()
            Dim c = CType(ColorConverter.ConvertFromString(s), Color)
            Return c.A/255
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class GetSticker
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IMultiValueConverter.Convert
            If IsNothing(values(1)) Then
                Return Nothing
            Else
                Return New ImageSourceConverter().ConvertFromString(values(1).ToString & values(0).ToString & "/64.png")
            End If
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) _
            As Object() Implements IMultiValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class GetStickerThumb
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Return CType(value, types.Product).base_url & "/thumb_22.png"
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class SmileToImagePath
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Return String.Format("pack://application:,,,/MicroVK.Smiles;component/Smile/{0}.png", value.ToString)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class ListBoxToProduct
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IMultiValueConverter.Convert
            Dim pr = CType(values(0), types.Product)
            If (TypeOf values(1) Is ListBox) Then
                pr.ListBox1 = CType(values(1), ListBox)
                If pr.type = "RecentSmiles" AndAlso pr.ListBox1.ItemsSource Is Nothing Then
                    pr.ListBox1.ItemsSource = SettingSystem.RecentSmiles
                    AddHandler pr.ListBox1.Loaded, AddressOf SettingSystem.SmilePopup1_Loaded
                End If
                AddHandler pr.ListBox1.SelectionChanged, AddressOf OtherApi.ListBox1_SelectionChanged
            End If

            If IsNothing(pr.stickers) Then
                Return Nothing
            Else
                Return pr.stickers.base_url
            End If
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) _
            As Object() Implements IMultiValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class NotyficationTypeToString
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Select Case CInt(value)
                Case 3
                    Return My.Resources.NotifycationConvert3
                Case 4
                    Return My.Resources.NotifycationConvert4
                Case 8
                    Return My.Resources.NotifycationConvert8
                Case 9
                    Return My.Resources.NotifycationConvert9
                Case 61
                    Return My.Resources.NotifycationConvert61
                Case 62
                    Return My.Resources.NotifycationConvert62
                Case Else
                    Return My.Resources.unknown
            End Select
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class NotBooleanConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Return Not CBool(value)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class HideTypingTextBlock
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IMultiValueConverter.Convert
            If IsNothing(values(0)) Then
                Return ""
            End If
            Dim s = values(0).ToString()
            If s = "end" AndAlso Not IsNothing(values(1)) Then
                Dim m = TryCast(values(1), types.message)
                Dim user_id = m.user_id
                If _
                    Messages.MessagesDictionary.ContainsKey(user_id.ToString()) AndAlso
                    Not IsNothing(Messages.MessagesDictionary(user_id.ToString).items) Then
                    For j = Messages.MessagesDictionary(user_id.ToString).items.Count - 1 To _
                        Messages.MessagesDictionary(user_id.ToString).items.Count - 5 Step - 1
                        If j < Messages.MessagesDictionary(user_id.ToString).items.Count AndAlso j > 0 Then
                            If Messages.MessagesDictionary(user_id.ToString).items(j).IsActive Then
                                Messages.MessagesDictionary(user_id.ToString).items.RemoveAt(j)
                            End If
                        End If
                    Next
                End If
                Return ""
            Else
                Return " " & My.Resources.is_typing & s
            End If
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) _
            As Object() Implements IMultiValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class DataToStringLastActivity
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            If ModernUIHelper.IsInDesignMode Then
                Return Now
            End If
            If value IsNot Nothing Then
                Dim info = CType(value, types.GetUserInfo)
                Dim s As String
                If Not info.lastActivity.online Then
                    s = OtherApi.SexToString(My.Resources.LastActyvity1, If(info.user Is Nothing, 0, info.user.sex))
                Else
                    s = My.Resources.Online & ", " & My.Resources.LastActivity
                End If
                Return s & " " & OtherApi.DataToString(info.lastActivity.time)
            End If
            Return Nothing
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class UserIdToMargin
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            If OtherApi.MyWindow1?.ContentSource?.ToString Like "*Pages/PageUserInfo.xaml*" Then
                Return New Thickness(- 34, 0, 0, - 20)
            Else
                Return New Thickness(- 245, 0, 0, - 20)
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class SmallMessageVisiblityOut
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            If CBool(value) = CBool(parameter) Then
                Return Visibility.Visible
            Else
                Return Visibility.Hidden
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class SmallMessageFontColor
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IMultiValueConverter.Convert
            Dim out = CBool(values(0))
            If My.Settings.Theme = 0 Then
                If out Then
                    Return _
                        New SolidColorBrush(CType(ColorConverter.ConvertFromString(My.Settings.dsFontColorOut), Color))
                Else
                    Return New SolidColorBrush(CType(ColorConverter.ConvertFromString(My.Settings.dsFontColor), Color))
                End If
            Else
                If out Then
                    Return _
                        New SolidColorBrush(CType(ColorConverter.ConvertFromString(My.Settings.dsFontColorOutDark),
                                                  Color))
                Else
                    Return _
                        New SolidColorBrush(CType(ColorConverter.ConvertFromString(My.Settings.dsFontColorDark), Color))
                End If
            End If
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) _
            As Object() Implements IMultiValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class SmallMessageColorOut
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IMultiValueConverter.Convert
            Dim out = CBool(values(0))
            If My.Settings.Theme = 0 Then
                If out Then
                    Return New SolidColorBrush(CType(ColorConverter.ConvertFromString(My.Settings.dsColorOut), Color))
                Else
                    Return New SolidColorBrush(CType(ColorConverter.ConvertFromString(My.Settings.dsColor), Color))
                End If
            Else
                If out Then
                    Return _
                        New SolidColorBrush(CType(ColorConverter.ConvertFromString(My.Settings.dsColorOutDark), Color))
                Else
                    Return New SolidColorBrush(CType(ColorConverter.ConvertFromString(My.Settings.dsColorDark), Color))
                End If
            End If
        End Function


        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) _
            As Object() Implements IMultiValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class FloatDialogUnreadLabelVisiblity
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            If value.ToString() = "" Then
                value = "0"
            End If
            Dim unread = CInt(value)
            If (unread > 0) Then
                Return Visibility.Visible
            Else
                Return Visibility.Collapsed
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class DurationToTime
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Dim duration = CType(value, Integer)
            Dim ts = TimeSpan.FromSeconds(duration)
            Return If(ts.TotalMinutes >= 60, ts.ToString("hh\:mm\:ss"), ts.ToString("mm\:ss"))
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class GetMessageBodyForDialogListConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Dim m = TryCast(value, types.message)
            Dim tempBody = ""
            If m IsNot Nothing Then
                If m.body?.Length > 0 Then
                    tempBody = m.body
                End If
                If m.fwd_messages?.Count > 0 Then
                    tempBody = tempBody & vbNewLine &
                               String.Format("[{0} ({1})]", My.Resources.Messages, m.fwd_messages.Count)
                End If
                If m.attachments?.Count > 0 Then
                    If m.attachments.Count = 1 Then
                        tempBody = tempBody & vbNewLine &
                                   String.Format("[{0}]", OtherApi.AttachmentToString(m.attachments(0).type))
                    Else
                        tempBody = tempBody & vbNewLine &
                                   String.Format("[{0} ({1})]", My.Resources.attachments, m.attachments.Count)
                    End If
                End If
            Else
                tempBody = tempBody & vbNewLine & String.Format("[{0}]", My.Resources.unknown)
            End If
            Return tempBody.Trim
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class MessageOutToHorizontalAlignment
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Return If(CBool(value), HorizontalAlignment.Right, HorizontalAlignment.Left)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class UserOnlineToGeometryConvert
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IMultiValueConverter.Convert
            Dim u = TryCast(values(0), types.profile)
            Dim p = TryCast(values(1), Path)
            If u Is Nothing Then Return Nothing

            If u.online Then
                If u.online_mobile Then
                    p.Height = 12
                    Return TryCast(OtherApi.MyWindow1.FindResource("cellphone_basic"), Geometry)
                Else
                    p.Height = 5
                    Return TryCast(OtherApi.MyWindow1.FindResource("record"), Geometry)
                End If
            Else
                Return Nothing
            End If
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) _
            As Object() Implements IMultiValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class BoolToDialogTabsBorderThickness
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Return If _
                (CBool(value),
                 New Thickness(0, 0, 0, 0),
                 If(My.Settings.IsTabBottom, New Thickness(0, 0, 0, 2), New Thickness(0, 2, 0, 0)))
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class

    Class BoolToDockConvert
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.Convert
            Return If(CBool(value), Dock.Bottom, Dock.Top)
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) _
            As Object Implements IValueConverter.ConvertBack
            Throw New NotSupportedException
        End Function
    End Class
End Namespace