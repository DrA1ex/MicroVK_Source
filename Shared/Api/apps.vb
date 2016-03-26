Namespace Api
    Public Class Apps
        Shared Async Function [Get](Optional app_id As String = "") As Task(Of types.app_list)
            Return Await MicroVkApi.GetResponse (Of types.app_list)("apps.get", {{"app_id", app_id}})
        End Function
    End Class
End Namespace