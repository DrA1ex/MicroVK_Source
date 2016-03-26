Imports System.IO
Imports FirstFloor.ModernUI.Windows.Controls
Imports Microsoft.Win32

Public Class ControlSettingSystem
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        If _
            ModernDialog.ShowMessage(My.Resources.confirmation_reset, My.Resources.confirmation, MessageBoxButton.YesNo) =
            MessageBoxResult.Yes Then
            My.Settings.Reset()
            My.Settings.Save()
            Forms.Application.Restart()
            My.Application.Shutdown()
        End If
    End Sub

    Private Sub RunAsStartupCheckBox_OnClick(sender As Object, e As RoutedEventArgs)
        Dim p = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                          "MicroVK"))
        If Not Directory.Exists(p) Then
            Directory.CreateDirectory(p)
        End If
        Dim linkPath = Path.Combine(p, "MicroVK.appref-ms")
        If Not File.Exists(linkPath) Then
            File.Copy("MicroVK.appref-ms", linkPath)
        End If
        Dim key As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True)
        If My.Settings.RunAsStartup Then
            key.SetValue("MicroVK", linkPath)
        Else
            key.DeleteValue("MicroVK", True)
        End If
    End Sub
End Class
