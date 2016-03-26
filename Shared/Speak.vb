Imports System.Speech.Synthesis
Imports MicroVK.WPFGrowlNotification

Class Speak
    Public Shared SpeechSynthesizer1 As New SpeechSynthesizer
    Public Shared TypingDictionary As New Dictionary(Of String, DateTime)

    Private Shared ReadOnly BufferText As New List(Of String)

    Private Shared _tempbool As Boolean = False

    Public Shared Sub Speak(text As String)
        If Not My.Settings.IsSpeak Then
            Return
        End If
        Try

            If SpeechSynthesizer1.State = SynthesizerState.Speaking Then
                BufferText.Add(text)
            Else
                SpeechSynthesizer1.Rate = My.Settings.SpeakRate
                SpeechSynthesizer1.Volume = My.Settings.SpeakVolume
                SpeechSynthesizer1.SpeakAsync(text)
                'If SpeechSynthesizer1.Voice.Name <> my.Settings.Voice AndAlso my.Settings.Voice <> ""
                '    SpeechSynthesizer1.SelectVoice(my.Settings.Voice)
                'end if
                If Not _tempbool Then
                    AddHandler SpeechSynthesizer1.SpeakCompleted, AddressOf SpeechSynthesizer1_SpeakCompleted
                    _tempbool = True
                End If
            End If
        Catch ex As Exception
            OtherApi.GrowlNotifiactions1.AddNotification(New Notification With {
                                                            .Content =
                                                            New ControllNotificationBBCode With {
                                                            .DataContext =
                                                            New types.Notifycation _
                                                            With {.title = "MicroVK.SpeechSynthesizer",
                                                            .text = ex.Message}}})
            My.Settings.IsSpeak = False
        End Try
    End Sub

    Private Shared Sub SpeechSynthesizer1_SpeakCompleted(sender As Object, e As SpeakCompletedEventArgs)
        If BufferText.Count > 0 Then
            Speak(BufferText(0))
            BufferText.RemoveAt(0)
        End If
    End Sub
End Class
