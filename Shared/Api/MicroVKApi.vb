
Imports JetBrains.Annotations
Imports MicroVK.CSharp
Imports MicroVK.CSharp.Controls

Namespace Api
    Class MicroVkApi
        <CanBeNull>
        Shared Async Function GetResponse (Of T)(method As String, Optional args(,) As String = Nothing) As Task(Of T)
            Dim p = ""
            Dim q As String
            Dim path = ""
            If args IsNot Nothing Then
                For i = 0 To args.GetLength(0) - 1
                    If Not args(i, 1).IsNullOrEmpty() Then
                        If Not String.IsNullOrEmpty(args(i, 0)) Then
                            p = String.Format("{0}={1}&", p + args(i, 0), args(i, 1))
                        Else
                            p = String.Format("{0}&", p + args(i, 1))
                        End If
                    End If
                Next
            End If

            If method.IndexOf("@"c) > 0 Then
                q = method.Split("@"c)(0)
                path = method.Split("@"c)(1)
            Else
                q = method
            End If
            Dim response = Await OtherApi.VkPost(q, p)
            If Not IsNothing(response) AndAlso response.ToString() <> "" Then
                If path = "" Then
                    Return Await MyJsonConvert.DeserializeObjectAsync (Of T)((response).ToString)
                Else
                    Return Await MyJsonConvert.DeserializeObjectAsync (Of T)((response)(path).ToString)
                End If
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace