Public Class NetHelper
    Public Shared Async Function Delay(dueTime As Integer) As Task
#If NET46 Then
       Await Task.Delay(dueTime)
#Else
        Await TaskEx.Delay(dueTime)
#End If
    End Function
End Class
