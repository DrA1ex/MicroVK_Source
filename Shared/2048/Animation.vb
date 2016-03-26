Imports System.Windows.Media.Animation

Namespace _2048
    Class Animation
        Public Shared Function CreatePropertyPath(property_path As String) As PropertyPath
            Return New PropertyPath(property_path)
        End Function

        Public Shared Function CreateDoubleAnimation(from1 As System.Nullable(Of Double),
                                                     to1 As System.Nullable(Of Double),
                                                     duration_ticks As Long) As DoubleAnimation
            Dim animation = New DoubleAnimation()
            animation.From = From1
            animation.To = To1
            animation.Duration = New Duration(New TimeSpan(duration_ticks))
            Return animation
        End Function
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
