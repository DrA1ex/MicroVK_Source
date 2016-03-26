Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json
Imports Un4seen.Bass.AddOn.Fx

Namespace OtherLib
    Public Class MicroVkAccount
        Property full_name As String
        Property photo As String
        Property AccessToken As String
        Property id As String
    End Class

    Class SettingSystem
        Private Shared _appConfigPath As String
        Public Shared RecentSmiles As New ObservableCollection(Of String)
        Public Shared RecentSmilesProducts As types.Product
        Public Shared LastRadio As New List(Of types.audio)
        Public Shared MicroVkAccounts As New ObservableCollection(Of MicroVkAccount)

        Public Shared BassFXSettings As New MyBassSetting
        Public Const RecentSmileFileName = "RecentSmiles_0.0.0.245"


        Shared Async Function LoadSettingsAsync (Of T As {New})(name As String) As Task(Of T)


            Dim localAppData As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Dim userFilePath As String = Path.Combine(Path.Combine(localAppData, "MicroVK"), "Settings")

            If (Not Directory.Exists(userFilePath)) Then
                Directory.CreateDirectory(userFilePath)
            End If

            _appConfigPath = userFilePath
            Dim path1 = Path.Combine(_appConfigPath, name & ".config.json")
            If (File.Exists(path1)) Then
                Dim json = Await (New WebClient().DownloadStringTaskAsync(path1))
                Return Await MyJsonConvert.DeserializeObjectAsync (Of T)(json)
            End If
            Return New T
        End Function

        Function LoadSettings (Of T)(name As String) As T
            _appConfigPath = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)
            Dim path1 = Path.Combine(_appConfigPath, name & ".config.json")
            If (File.Exists(path1)) Then
                Dim json = (New WebClient().DownloadString(path1))
                Return JsonConvert.DeserializeObject (Of T)(json)
            End If
            Return Nothing
        End Function

        Shared Async Sub SaveSettings(name As String, value As Object)
            Dim localAppData As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Dim userFilePath As String = Path.Combine(Path.Combine(localAppData, "MicroVK"), "Settings")
            Try
                If (Not Directory.Exists(userFilePath)) Then
                    Directory.CreateDirectory(userFilePath)
                End If

                _appConfigPath = userFilePath
                Dim path1 = Path.Combine(_appConfigPath, name & ".config.json")
                File.WriteAllText(Path.Combine(_appConfigPath, name & ".config.json"),
                                  Await MyJsonConvert.SerializeObjectAsync(value),
                                  Encoding.UTF8)
            Catch ex As Exception

            End Try
        End Sub

        Public Shared Sub SmilePopup1_Closed(sender As Object, e As EventArgs)
            SaveSettings(RecentSmileFileName, RecentSmiles)
        End Sub

        Public Shared Sub SmilePopup1_Loaded(sender As Object, e As RoutedEventArgs)
            CType(sender, ListBox).Visibility = Visibility.Collapsed
            CType(sender, ListBox).Visibility = Visibility.Visible
        End Sub


        Public Class MyBassSetting
            Property IsEcho As Boolean = False
            Property IsDistortion As Boolean = False
            Property IsRotate As Boolean
            Property Echo As New BASS_BFX_ECHO4 With {.lChannel = BASSFXChan.BASS_BFX_CHANALL}
            Property Distortion As New BASS_BFX_DISTORTION With {.lChannel = BASSFXChan.BASS_BFX_CHANALL}
            Property Rotate As New BASS_BFX_ROTATE With {.lChannel = BASSFXChan.BASS_BFX_CHANALL}
        End Class

        Public Shared Function AES_Encrypt(ByVal input As String, ByVal pass As String) As String
            Dim AES As New System.Security.Cryptography.RijndaelManaged
            Dim Hash_AES As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim encrypted As String
            Try
                Dim hash(31) As Byte
                Dim temp As Byte() = Hash_AES.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(pass))
                Array.Copy(temp, 0, hash, 0, 16)
                Array.Copy(temp, 0, hash, 15, 16)
                AES.Key = hash
                AES.Mode = System.Security.Cryptography.CipherMode.ECB
                Dim desEncrypter As System.Security.Cryptography.ICryptoTransform = AES.CreateEncryptor
                Dim buffer As Byte() = System.Text.ASCIIEncoding.ASCII.GetBytes(input)
                encrypted = Convert.ToBase64String(desEncrypter.TransformFinalBlock(buffer, 0, buffer.Length))
                Return encrypted
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Shared Function AES_Decrypt(ByVal input As String, ByVal pass As String) As String
            Dim aes As New System.Security.Cryptography.RijndaelManaged
            Dim hashAes As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim decrypted As String
            Try
                Dim hash(31) As Byte
                Dim temp As Byte() = hashAes.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(pass))
                Array.Copy(temp, 0, hash, 0, 16)
                Array.Copy(temp, 0, hash, 15, 16)
                aes.Key = hash
                aes.Mode = System.Security.Cryptography.CipherMode.ECB
                Dim desDecrypter As System.Security.Cryptography.ICryptoTransform = aes.CreateDecryptor
                Dim buffer As Byte() = Convert.FromBase64String(input)
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(desDecrypter.TransformFinalBlock(buffer,
                                                                                                       0,
                                                                                                       buffer.Length))
                Return decrypted
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Shared Async Function UpdateMicroVkAccounts() As Task
            If MicroVkAccounts?.Count = 0 Then _
                MicroVkAccounts =
                    Await _
                        SettingSystem.LoadSettingsAsync (Of ObservableCollection(Of MicroVkAccount))("MicroVkAccounts")
        End Function

        Public Shared Async Function UpdateRecentSmiles() As Task
            If RecentSmiles?.Count = 0 Then
                RecentSmiles =
                    Await _
                        SettingSystem.LoadSettingsAsync (Of ObservableCollection(Of String))(
                            SettingSystem.RecentSmileFileName)
            End If
        End Function
    End Class
End Namespace