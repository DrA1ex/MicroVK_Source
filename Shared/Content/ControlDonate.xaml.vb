Public Class ControlDonate
    Private Sub ButtonBase_OnClick(sender As Object, e As RoutedEventArgs)
        OtherApi.ProcessStart(
            "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=kryeker%40yandex%2eru&lc=RU&item_name=MicroVK&no_note=0&cn=%d0%94%d0%be%d0%b1%d0%b0%d0%b2%d0%b8%d1%82%d1%8c%20%d1%81%d0%bf%d0%b5%d1%86%d0%b8%d0%b0%d0%bb%d1%8c%d0%bd%d1%8b%d0%b5%20%d0%b8%d0%bd%d1%81%d1%82%d1%80%d1%83%d0%ba%d1%86%d0%b8%d0%b8%20%d0%b4%d0%bb%d1%8f%20%d0%bf%d1%80%d0%be%d0%b4%d0%b0%d0%b2%d1%86%d0%b0%3a&no_shipping=1&rm=1&return=https%3a%2f%2fvk%2ecom%2fmicrovk&cancel_return=https%3a%2f%2fvk%2ecom%2fmicrovk&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted")
    End Sub

    Private Sub ButtonBase2_OnClick(sender As Object, e As RoutedEventArgs)
        OtherApi.ProcessStart("https://money.yandex.ru/to/410011579847720")
    End Sub
End Class
