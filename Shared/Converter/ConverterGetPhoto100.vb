Public Class ConverterGetPhoto100
    Implements IValueConverter

    Public Function Convert(value As Object,
                            targetType As Type,
                            parameter As Object,
                            culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        'Return ProfilesList1(CInt(value)).photo_100
        Return Nothing
    End Function

    Public Function ConvertBack(value As Object,
                                targetType As Type,
                                parameter As Object,
                                culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return Nothing
    End Function
End Class
