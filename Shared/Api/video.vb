

Namespace Api
    Class Video
        Shared Async Function [Get](Optional videos As String = "") As Task(Of types.video())
            Return Await MicroVkApi.GetResponse (Of types.video())("video.get@items", {{"videos", videos}})
        End Function
    End Class
End NameSpace