Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MicroVK.Api

Public Class ContentUserWall
    Implements IContent
    Private user_id As String
    Private ReadOnly _postCollection As New ObservableCollection(Of types.wall_post)

    Public Async Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
        user_id = e.Fragment.GetParametr("user_id")
        Dim posts = Await wall.Get(user_id)
        _postCollection.Clear()
        Dim profileDictionary As Dictionary(Of Integer, types.profile)
        Dim groupDictionary As Dictionary(Of Integer, types.group)
        profileDictionary = posts.profiles.ToDictionary(Function(profile) profile.id)
        groupDictionary = posts.groups.ToDictionary(Function(group) - group.id)
        If posts IsNot Nothing Then
            posts.items.ToList().ForEach(Sub(p)
                If (p.from_id > 0) Then
                    p.from_name = profileDictionary(p.from_id).full_name
                Else
                    p.from_name = groupDictionary(p.from_id).name
                                            End If
                If (p.owner_id > 0) Then
                    p.owner_name = profileDictionary(p.owner_id).full_name
                Else
                    p.owner_name = groupDictionary(p.owner_id).name
                                            End If
                If p.copy_history IsNot Nothing AndAlso p.copy_history.Length > 0 Then
                    p.copy_history = New types.wall_post() {p.copy_history(0)}
                    If (p.copy_history(0).from_id > 0) Then
                        p.copy_history(0).from_name = profileDictionary(p.copy_history(0).from_id).full_name
                    Else
                        p.copy_history(0).from_name = groupDictionary(p.copy_history(0).from_id).name
                                            End If
                    If (p.copy_history(0).owner_id > 0) Then
                        p.copy_history(0).owner_name = profileDictionary(p.copy_history(0).owner_id).full_name
                    Else
                        p.copy_history(0).owner_name = groupDictionary(p.copy_history(0).owner_id).name
                                            End If
                    p.copy_history(0).IsCopyHistory = True
                                            End If
                _postCollection.Add(p)
                                            End Sub)
        End If
        ListBox1.ItemsSource = _postCollection
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub
End Class
