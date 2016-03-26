Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace Api
    Class Stats
        Shared Async Sub TrackVisitor()
            Await MicroVkApi.GetResponse (Of JToken)("stats.trackVisitor")
        End Sub

        Async Sub TrackEvents(e As StatsEvent)
            Dim fdf = JsonConvert.SerializeObject(e).ToString()
            Dim a =
                    Await _
                    MicroVkApi.GetResponse (Of JToken)("stats.trackVisitor",
                                                       {{"events", JsonConvert.SerializeObject(e).ToString()}})
        End Sub

        Class StatsEvent
            Property e As String
            Property audio_id As String
            Property source As String
        End Class

        Structure StatsEventString
            Const open_user = "open_user"
            Const open_group = "open_group"
            Const view_post = "view_post"
            Const video_play = "video_play"
            Const audio_play = "audio_play"
            Const games_visit = "games_visit"
            Const games_action = "games_action"
            Const menu_click = "menu_click"
            Const user_action = "user_action"
            Const show_user_rec = "show_user_rec"
        End Structure

        Structure AudioPlaySourceString
            Const wall_user = "wall_user"
            Const wall_group = "wall_group"
            Const news = "news"
            Const audios_user = "audios_user"
            Const audios_group = "audios_group"
            Const search = "search"
            Const comments = "comments"
            Const messages = "messages"
            Const status = ""
        End Structure
    End Class
End NameSpace