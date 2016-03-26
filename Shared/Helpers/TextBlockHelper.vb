
Imports System.IO
Imports System.Text

Public Class TextBlockHelper
    Public Shared ReadOnly _
        RawTextProperty As DependencyProperty = DependencyProperty.RegisterAttached("RawText",
                                                                                    GetType(String),
                                                                                    GetType(TextBlockHelper),
                                                                                    New PropertyMetadata(
                                                                                        New PropertyChangedCallback(
                                                                                            AddressOf OnRawTextChanged)))

    Public Shared ReadOnly _
        HasLinkProperty As DependencyProperty = DependencyProperty.RegisterAttached("HasLink",
                                                                                    GetType(Boolean),
                                                                                    GetType(TextBlockHelper),
                                                                                    New PropertyMetadata(True))

    Public Shared Function GetHasLink(ByVal element As DependencyObject) As boolean
        Return DirectCast(element.GetValue(HasLinkProperty), boolean)
    End Function

    Public Shared Sub SetHasLink(ByVal element As DependencyObject, ByVal value As boolean)
        element.SetValue(HasLinkProperty, value)
    End Sub


    Private Shared Sub OnRawTextChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim textBlock1 = TryCast(d, TextBlock)
        If GetHasLink(textBlock1) Then
            SetHasLink(textBlock1, False)
            Dim str = e.NewValue.ToString()

            'Dim temp1 As String = ""
            'For Each i In str.Split
            '    temp1 += "http://vk.com/images/emoji/" &
            '        BitConverter.ToString(Encoding.GetEncoding(1201).GetBytes(i)).Replace("-", "") & ".png" & vbNewLine
            'Next
            'Dim fg As New BitmapImage(New Uri("pack://application:,,,/MicroVK.Smiles;component/Smile/23F0.png", UriKind.RelativeOrAbsolute))
            Dim g = OtherApi.Regex1.Matches(str)
            If g.Count = 0 Then
                textBlock1.Text = str
                Exit Sub
            End If

            textBlock1.Inlines.Clear()
            Dim pos = 0, countSmile = 0
            Const maxsmile As Integer = 100
            'dim smileNamet = BitConverter.ToString(Encoding.GetEncoding(12001).GetBytes(g(0).Value)).Replace("-",
            '"")
            For i = 0 To g.Count - 1
                Dim h = str.Substring(pos, g(i).Index - pos)
                If Not g(i).Value(0) = "h"c Then
                    If countSmile < maxsmile Then
                        textBlock1.Inlines.Add(h)
                        Dim smileName =
                                BitConverter.ToString(Encoding.GetEncoding(12001).GetBytes(g(i).Value)).Replace("-", "")
                        If smileName.Length = 16 Then
                            smileName = smileName.Substring(0, 8).TrimStart("0"c) & "-" &
                                        smileName.Substring(8, 8).TrimStart("0"c)
                        End If

                        Try
                            textBlock1.Inlines.Add(New Image With {
                                                      .Source =
                                                      New BitmapImage(
                                                          New Uri(
                                                              "pack://application:,,,/MicroVK.Smiles;component/Smile/" &
                                                              smileName.TrimStart("0"c) & ".png")),
                                                      .Height = 16,
                                                      .Margin = New Thickness(2, 4, 1, - 4),
                                                      .Stretch = Stretch.Uniform})
#If DEBUG Then
                            If False Then
                                '1f60e
                                Dim p = "C:\Users\Kryeker\OneDrive\Workspaces\MicroVK\MicroVK.Smiles\Smile\"
                                Dim p2 = "E:\Downloads\Архивы\twemoji-gh-pages\twemoji-gh-pages\16x16\"
                                Dim a =
                                        BitConverter.ToString(Encoding.GetEncoding(12001).GetBytes(g(i).Value)).Replace(
                                            "-",
                                            "").TrimStart({"0"c})
                                Dim a1 =
                                        BitConverter.ToString(Encoding.GetEncoding(1201).GetBytes(g(i).Value)).Replace(
                                            "-",
                                            "")
                                If File.Exists(p2 & a & ".png") Then
                                    FileSystem.FileCopy(p2 & a & ".png", p & a1 & ".png")
                                End If

                                'For Each m In Encoding.GetEncodings()
                                '    Debug.Print(String.Format("{0} {1} {2}", BitConverter.ToString(Encoding.GetEncoding(m.CodePage).GetBytes(g(i).Value)), m.CodePage, m.DisplayName))
                                'Next
                            End If
#End If
                            countSmile += 1
                        Catch ex As Exception
                            textBlock1.Inlines.Add(g(i).Value)
                        End Try
                    Else
                        textBlock1.Inlines.Add(h + g(i).Value)
                    End If
                Else
                    textBlock1.Inlines.Add(h)
#If Not XP Then
                    Dim hl = New Hyperlink(New Run(Net.WebUtility.UrlDecode(g(i).Value)))
#Else
                        Dim hl = New Hyperlink(New Run(Uri.EscapeUriString(g(i).Value)))
#End If
                    hl.Style = TryCast(textBlock1.FindResource("TextBlockHyperlink"), Style)
                    textBlock1.Inlines.Add(hl)
                    AddHandler hl.Click, AddressOf Hyperlink_Click
                End If
                pos = g(i).Index + g(i).Length
            Next
            textBlock1.Inlines.Add(str.Substring(pos))
        Else
            Dim gfgfg = 7
        End If
        SetHasLink(textBlock1, False)
    End Sub

    Private Shared Async Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)
        Dim r = TryCast(sender, Hyperlink)
        If r Is Nothing Then
            Return
        End If
        Await OtherApi.GoToURL(r.Inlines.OfType (Of Run).FirstOrDefault.Text)
    End Sub

    <AttachedPropertyBrowsableForType(GetType(TextBlock))>
    Public Shared Function GetRawText(ByVal element As DependencyObject) As String
        Return DirectCast(element.GetValue(RawTextProperty), String)
    End Function

    Public Shared Sub SetRawText(ByVal element As DependencyObject, ByVal value As String)
        element.SetValue(RawTextProperty, value)
    End Sub
End Class
