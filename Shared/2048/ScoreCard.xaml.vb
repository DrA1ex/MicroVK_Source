Namespace _2048
    Public NotInheritable Partial Class ScoreCard
        Private _score As Integer

        Public Property Score() As Integer
            Get
                Return _score
            End Get
            Set
                If _score <> value Then
                    _score = value
                    ScoreTextBlock.Text = Value.ToString()
                End If
            End Set
        End Property

        Private _title As String

        Public Property Title() As String
            Get
                Return _title
            End Get
            Set
                If _title <> value Then
                    _title = value
                    'TitleTextBlock.text = Value
                End If
            End Set
        End Property

        Public Sub New()
            Me.InitializeComponent()
        End Sub
    End Class
End Namespace
