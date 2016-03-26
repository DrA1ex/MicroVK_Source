' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports MicroVK.OtherLib

Public NotInheritable Class ContentEcho
    Inherits UserControl

    Private Sub UpdateBass()
        SettingSystem.BassFXSettings.IsEcho = CType(EchoCheckBox1.IsChecked, Boolean)
        SettingSystem.BassFXSettings.Echo.fDelay = CType(DelaySlider.Value, Single)
        SettingSystem.BassFXSettings.Echo.fDryMix = CType(DrySlider1.Value, Single)
        SettingSystem.BassFXSettings.Echo.fWetMix = CType(WetSlider1.Value, Single)

        OtherApi.BassPlayer1?.UpdateFX()
    End Sub

    Private Sub DrySlider1_OnValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        UpdateBass()
    End Sub

    Private Sub WetSlider1_OnValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        UpdateBass()
    End Sub

    Private Sub DoubleKickRadioButton_OnChecked(sender As Object, e As RoutedEventArgs)
        DrySlider1.Value = 0.5
        WetSlider1.Value = 0.599
        DelaySlider.Value = 0.5
    End Sub

    Private Sub LongEchoRadioButton_OnChecked(sender As Object, e As RoutedEventArgs)
        DrySlider1.Value = 0.999
        WetSlider1.Value = 0.699
        DelaySlider.Value = 0.9
    End Sub

    Private Sub SmallEchoRadioButton_OnChecked(sender As Object, e As RoutedEventArgs)
        DrySlider1.Value = 0.999
        WetSlider1.Value = 0.999
        DelaySlider.Value = 0.2
    End Sub

    Private Sub DelaySlider_OnValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        UpdateBass()
    End Sub

    Private Sub EchoCheckBox1_OnClick(sender As Object, e As RoutedEventArgs)
        SettingSystem.BassFXSettings.IsEcho = CType(EchoCheckBox1.IsChecked, Boolean)
        UpdateBass()
    End Sub
End Class
