Imports System.Collections.ObjectModel
Imports Newtonsoft.Json.Linq

Namespace Api
    Class Audio
        Shared Async Function [Get](Optional offset As String = "") As Task(Of ObservableCollection(Of types.audio))
            Return _
                Await _
                    MicroVkApi.GetResponse (Of ObservableCollection(Of types.audio))("audio.get@items",
                                                                                     {{"offset", offset},
                                                                                      {"count", "100"}})
        End Function

        Shared Async Function GetLyrics(lyrics_id As String) As Task(Of String)
            Dim a = Await MicroVkApi.GetResponse (Of JToken)("audio.getLyrics", {{"lyrics_id", lyrics_id}})
            If a Is Nothing Then Return ""
            Return a("text").ToString()
        End Function

        Shared Async Function Search(q As String, Optional offset As String = "") _
            As Task(Of ObservableCollection(Of types.audio))
            Return _
                Await _
                    MicroVkApi.GetResponse (Of ObservableCollection(Of types.audio))("audio.search@items",
                                                                                     {{"q", q}, {"auto_complete", "1"},
                                                                                      {"offset", offset}})
        End Function

        Shared Async Function Add(audioId As String, ownerId As String) As Task(Of JToken)
            Return Await MicroVkApi.GetResponse (Of JToken)("audio.add", {{"audio_id", audioId}, {"owner_id", ownerId}})
        End Function

        Shared Async Function GetPopular(Optional genre_id As String = "",
                                         Optional only_eng As String = "",
                                         Optional offset As String = "") _
            As Task(Of ObservableCollection(Of types.audio))
            Return _
                Await _
                    MicroVkApi.GetResponse (Of ObservableCollection(Of types.audio))("audio.getPopular",
                                                                                     {{"genre_id", genre_id},
                                                                                      {"only_eng", only_eng},
                                                                                      {"offset", offset}})
        End Function

        Shared Async Function SetBroadcast(audio As String) As Task(Of JToken)
            Return Await MicroVkApi.GetResponse (Of JToken)("audio.setBroadcast", {{"audio", audio}})
        End Function

        Shared Async Function GetRecommendations(Optional offset As String = "",
                                                 Optional target_audio As String = "",
                                                 Optional shuffle As String = "") _
            As Task(Of ObservableCollection(Of types.audio))
            Return _
                Await _
                    MicroVkApi.GetResponse (Of ObservableCollection(Of types.audio))("audio.getRecommendations@items",
                                                                                     {{"offset", offset},
                                                                                      {"target_audio", target_audio},
                                                                                      {"shuffle", shuffle}})
        End Function
    End Class
End NameSpace