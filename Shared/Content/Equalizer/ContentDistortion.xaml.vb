' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports MicroVK.OtherLib
Imports Un4seen.Bass.AddOn.Fx

Public NotInheritable Class ContentDistortion
    Inherits UserControl

    Private Sub UpdateBass()
        SettingSystem.BassFXSettings.IsDistortion = CType(DistortionCheckBox.IsChecked, Boolean)
        SettingSystem.BassFXSettings.Distortion.fDrive = CType(DistortionDrive1.Value, Single)
        SettingSystem.BassFXSettings.Distortion.fDryMix = CType(DistortionDry1.Value, Single)
        SettingSystem.BassFXSettings.Distortion.fWetMix = CType(DistortionWet1.Value, Single)
        SettingSystem.BassFXSettings.Distortion.fFeedback = CType(DistortionFeedback1.Value, Single)
        SettingSystem.BassFXSettings.Distortion.fVolume = CType(DistortionVolume1.Value, Single)

        OtherApi.BassPlayer1?.UpdateFX()
    End Sub

    Private Sub DistortionCheckBox_OnClick(sender As Object, e As RoutedEventArgs)
        SettingSystem.BassFXSettings.IsDistortion = CType(DistortionCheckBox.IsChecked, Boolean)
        UpdateBass()
        Dim a = New BASS_BFX_DISTORTION
        a.Preset_HardDistortion()
    End Sub

    Private Sub DistortionVeryHardRadioButton_OnClick(sender As Object, e As RoutedEventArgs)
        DistortionDrive1.Value = 5
        DistortionDry1.Value = 0
        DistortionWet1.Value = 1
        DistortionFeedback1.Value = 0.1
        DistortionVolume1.Value = 1
        UpdateBass()
    End Sub

    Private Sub DistortionHardRadioButton_OnClick(sender As Object, e As RoutedEventArgs)
        DistortionDrive1.Value = 1
        DistortionDry1.Value = 0
        DistortionWet1.Value = 1
        DistortionFeedback1.Value = 0
        DistortionVolume1.Value = 1
        UpdateBass()
    End Sub

    Private Sub DistortionMediumRadioButton_OnClick(sender As Object, e As RoutedEventArgs)
        DistortionDrive1.Value = 0.2
        DistortionDry1.Value = 1
        DistortionWet1.Value = 1
        DistortionFeedback1.Value = 0.1
        DistortionVolume1.Value = 1
        UpdateBass()
    End Sub
End Class
