Imports Microsoft.Windows.Shell

Public Class WindowChromeHelper
    Public Shared ReadOnly _
        IsHitTestVisibleInChromeProperty As DependencyProperty = DependencyProperty.Register("IsHitTestVisibleInChrome",
                                                                                             GetType(boolean),
                                                                                             GetType(Window),
                                                                                             new PropertyMetadata(
                                                                                                 Nothing))

    '<AttachedPropertyBrowsableForType(GetType(Window))>
    Public Shared Function GetIsHitTestVisibleInChrome(ByVal element As DependencyObject) As Boolean
#If NET46 Then
         Return DirectCast(element.GetValue(WindowChrome.IsHitTestVisibleInChromeProperty), Boolean)
#End If

    End Function

    Public Shared Sub SetIsHitTestVisibleInChrome(ByVal element As DependencyObject, ByVal value As Boolean)
#If NET46 Then
        element.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, value)
#End If
    End Sub
End Class
