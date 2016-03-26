Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI.Presentation


Namespace WPFGrowlNotification
    Public Class Notification
        Inherits NotifyPropertyChanged
        Property Type As Integer
        Private m_message As String


        Public Property Message() As String
            Get
                Return m_message
            End Get


            Set(value As String)
                If m_message = value Then
                    Return
                End If
                m_message = value
                OnPropertyChanged("Message")
            End Set
        End Property

        Private m_content As UIElement

        Public Property Content As UIElement
            Get
                Return m_content
            End Get
            Set(value As UIElement)
                m_content = value
                OnPropertyChanged("Content")
            End Set
        End Property

        Private m_id As Integer

        Public Property Id() As Integer
            Get
                Return m_id
            End Get


            Set(value As Integer)
                If m_id = value Then
                    Return
                End If
                m_id = value
                OnPropertyChanged("Id")
            End Set
        End Property


        Private m_imageUrl As String

        Public Property ImageUrl() As String
            Get
                Return m_imageUrl
            End Get


            Set(value As String)
                If m_imageUrl = value Then
                    Return
                End If
                m_imageUrl = value
                OnPropertyChanged("ImageUrl")
            End Set
        End Property


        Private m_title As String

        Public Property Title() As String
            Get
                Return m_title
            End Get


            Set(value As String)
                If m_title = value Then
                    Return
                End If
                m_title = value
                OnPropertyChanged("Title")
            End Set
        End Property

        Private m_mytag As String

        Public Property MyTag() As String
            Get
                Return m_mytag
            End Get


            Set(value As String)
                If m_mytag = value Then
                    Return
                End If
                m_mytag = value
                OnPropertyChanged("MyTag")
            End Set
        End Property

        Public Property IsPinned As Boolean
            Get
                Return _isPinned
            End Get
            Set(value As Boolean)
                _isPinned = value
                OnPropertyChanged(NameOf(IsPinned))
            End Set
        End Property

        Private _isPinned As Boolean
    End Class


    Public Class Notifications
        Inherits ObservableCollection(Of Notification)
    End Class
End Namespace


