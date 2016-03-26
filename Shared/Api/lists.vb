Imports System.Collections.ObjectModel

Namespace Api
    Class Lists
        Public Shared ProfilesDictionary As New Dictionary(Of Integer, types.profile),
                      GroupsDictionary As New Dictionary(Of Integer, types.group),
                      NewsfeedList1 As New newsfeedlist

        Class newsfeedlist
            Property items As New ObservableCollection(Of types.wall_post)
            Property profiles As types.profile()
            Property groups As types.group()
            Property new_offset As Integer
            Property next_from As String
        End Class
    End Class
End NameSpace