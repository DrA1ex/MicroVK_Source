Imports Newtonsoft.Json

Public Class MyJsonConvert
    Public Shared Function DeserializeObjectAsync (Of T)(value As String, settings As JsonSerializerSettings) _
        As Task(Of T)
        Return Task.Factory.StartNew(Function() Newtonsoft.Json.JsonConvert.DeserializeObject (Of T)(value, settings))
    End Function

    Public Shared Function DeserializeObjectAsync (Of T)(value As String) As Task(Of T)
        Return DeserializeObjectAsync (Of T)(value, Nothing)
    End Function

    Public Shared Function DeserializeObjectAsync(value As String) As Task(Of Object)
        Return DeserializeObjectAsync(value, Nothing, Nothing)
    End Function

    Public Shared Function DeserializeObjectAsync(value As String, type As Type, settings As JsonSerializerSettings) _
        As Task(Of Object)
        Return Task.Factory.StartNew(Function() Newtonsoft.Json.JsonConvert.DeserializeObject(value, type, settings))
    End Function

    Public Shared Function SerializeObjectAsync(value As Object) As Task(Of String)
        Return Task.Factory.StartNew(Function() Newtonsoft.Json.JsonConvert.SerializeObject(value, Formatting.None))
    End Function
End Class
