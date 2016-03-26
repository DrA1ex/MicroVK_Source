Imports FirstFloor.ModernUI.Presentation
Imports FirstFloor.ModernUI.Windows.Controls

Public Class MyTabItem
    Inherits NotifyPropertyChanged
    Private _title As String
    Public ForwardMessages As String
    Public FlowDocument As FlowDocument
    Public CaretPosition As TextPointer
    Private _isChat1 As Boolean

    Property Title() As String
        Get
            Return _title
        End Get
        Set
            _title = Value
            OnPropertyChanged("Title")
        End Set
    End Property

    Public Property Description As String

    Public Property Photo As String

    Public Property RawTitle As String

    Public Property Unread As String

    Property Status As Boolean

    Public Property ToolTip As String

    Public Property IsSelected As Boolean

    Public ReadOnly Property IsChat As Boolean
        Get
            Return Description.GetParametr("chat_id").Length > 0
        End Get
    End Property

    Public Property Content As Uri
    Public Property Frame As ModernFrame
    Property FloatWindows As ModernWindow
End Class
