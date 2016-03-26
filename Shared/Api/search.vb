

Namespace Api
    Class Search
        Private Shared _isSearch As Boolean
        Private Shared _tempSearch As List(Of types.SearchHints)

        Shared Async Function GetHints(q As String,
                                       Optional search_global As String = "1",
                                       Optional filters As String = "") As Task(Of List(Of types.SearchHints))
            If Not _isSearch Then
                _isSearch = True
                Dim temp =
                        Await _
                        MicroVkApi.GetResponse (Of types.ExecuteSearchGetHints)("execute.search_getHints",
                                                                                {{"q", q}, {"limit", "50"},
                                                                                 {"search_global", search_global},
                                                                                 {"filters", filters}})
                If temp IsNot Nothing Then
                    _tempSearch = temp.hints
                    Dim users = temp.users
                    For Each Hints In _tempSearch
                        If Hints.type = "profile" Then
                            Dim u = users.FirstOrDefault(Function(profile) profile.id = Hints.profile.id)
                            If u IsNot Nothing Then
                                Hints.profile.photo_50 = u.photo_50
                            End If
                        End If
                    Next
                End If
                _isSearch = False
            End If
            Return _tempSearch
        End Function
    End Class
End Namespace