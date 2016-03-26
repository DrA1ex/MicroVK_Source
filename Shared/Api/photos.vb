Imports System.Collections.ObjectModel

Namespace Api
    Class Photos
        Async Function [Get](param As String) As Task(Of ObservableCollection(Of types.photo))
            Return _
                Await _
                    MicroVkApi.GetResponse (Of ObservableCollection(Of types.photo))("photos.get@items", {{"", param}})
        End Function

        Async Function GetById(photos As String) As Task(Of types.photo())
            Return Await MicroVkApi.GetResponse (Of types.photo())("photos.getById", {{"photos", photos}})
        End Function

        Shared Async Function SaveWallPhoto(a As types.ResponseUploadServer, group_id As String) _
            As Task(Of types.photo())
            Return _
                Await _
                    MicroVkApi.GetResponse (Of types.photo())("photos.saveWallPhoto",
                                                              {{"server", a.server}, {"photo", a.photo},
                                                               {"hash", a.hash}, {"group_id", group_id}})
        End Function

        Shared Async Function GetWallUploadServer() As Task(Of types.UploadServer)
            Return Await MicroVkApi.GetResponse (Of types.UploadServer)("photos.getWallUploadServer")
        End Function

        Shared Async Function GetMessagesUploadServer() As Task(Of types.UploadServer)
            Return Await MicroVkApi.GetResponse (Of types.UploadServer)("photos.getMessagesUploadServer")
        End Function

        Shared Async Function SaveMessagesPhoto(a As types.ResponseUploadServer) As Task(Of types.photo())
            Return _
                Await _
                    MicroVkApi.GetResponse (Of types.photo())("photos.saveMessagesPhoto",
                                                              {{"server", a.server}, {"photo", a.photo},
                                                               {"hash", a.hash}})
        End Function
    End Class
End Namespace