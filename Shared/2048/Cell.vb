Namespace _2048.Model
    Public Class Coordinate
        Public Property X As Integer

        Public Property Y As Integer

        Public Sub New(X As Integer, Y As Integer)
            Me.X = X
            Me.Y = Y
        End Sub
    End Class

    Public Class Cell
        Public Property Value As Integer

        Public Property WasMerged As Boolean

        Public Property WasCreated As Boolean

        Public Property Position As Coordinate

        Public Property X() As Integer
            Get
                Return Position.X
            End Get
            Set
                Position.X = value
            End Set
        End Property

        Public Property Y() As Integer
            Get
                Return Position.Y
            End Get
            Set
                Position.Y = value
            End Set
        End Property

        Public Property PreviousPosition As Coordinate

        Public Sub New(X As Integer, Y As Integer)
            Me.Position = New Coordinate(X, Y)
        End Sub

        Public Function IsEmpty() As Boolean
            Return Value = 0
        End Function
    End Class
End Namespace
