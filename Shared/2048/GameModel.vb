Imports System.Collections.Generic
Imports System.Linq

Namespace _2048.Model
    Public Enum MoveDirection
        Up
        Down
        Left
        Right
    End Enum

    Public Class GameModel
        Public Property Score As Integer

        Public Property RowCount As Integer

        Public Property ColumnCount As Integer

        Public Property Cells As Cell()()

        Public Function CellsIterator() As IEnumerable(Of Cell)
            dim result As new List(Of Cell)
            For x = 0 To ColumnCount - 1
                For y = 0 To RowCount - 1
                    result.Add(Cells(x)(y))
                Next
            Next
            Return result
        End Function

        Public Sub New(RowCount As Integer, ColumnCount As Integer)
            Me.RowCount = RowCount
            Me.ColumnCount = ColumnCount

            Me.Reset()
        End Sub

        Public Function PerformMove(Direction As MoveDirection) As Boolean
            If PackAndMerge(Direction) Then
                Dim newTile = GetRandomEmptyTile()

                If newTile IsNot Nothing Then
                    ' TODO move this to its own testable method
                    Cells(newTile.Item1)(newTile.Item2).Value = GetRandomStartingNumber()
                    Cells(newTile.Item1)(newTile.Item2).WasCreated = True
                    Return True
                    ' Game over?
                Else
                End If
            End If
            Return False
        End Function


        Private Function PackAndMerge(Direction As MoveDirection) As Boolean
            Dim changed = False
            If Direction = MoveDirection.Up Then
                ' For each column
                For x = 0 To ColumnCount - 1
                    ' Look at tiles in the column from bottom to top
                    For y = 1 To RowCount - 1
                        If Cells(x)(y).IsEmpty() Then
                            Continue For
                        End If

                        Dim destinationY = y
                        While _
                            destinationY - 1 >= 0 AndAlso
                            (Cells(x)(destinationY - 1).IsEmpty() OrElse
                             (Cells(x)(destinationY - 1).Value = Cells(x)(y).Value AndAlso
                              Not Cells(x)(destinationY - 1).WasMerged))
                            destinationY -= 1
                        End While

                        If destinationY <> y Then
                            MergeCells(Cells(x)(y), Cells(x)(destinationY))
                            changed = True
                        End If
                    Next
                Next
            ElseIf Direction = MoveDirection.Down Then
                ' For each column
                For x = 0 To ColumnCount - 1
                    ' Look at tiles in the column from bottom to top
                    For y = RowCount - 2 To 0 Step - 1
                        If Cells(x)(y).IsEmpty() Then
                            Continue For
                        End If

                        Dim destinationY = y
                        While _
                            destinationY + 1 < RowCount AndAlso
                            (Cells(x)(destinationY + 1).IsEmpty() OrElse
                             (Cells(x)(destinationY + 1).Value = Cells(x)(y).Value AndAlso
                              Not Cells(x)(destinationY + 1).WasMerged))
                            destinationY += 1
                        End While

                        If destinationY <> y Then
                            MergeCells(Cells(x)(y), Cells(x)(destinationY))
                            changed = True
                        End If
                    Next
                Next
            ElseIf Direction = MoveDirection.Left Then
                For y = 0 To RowCount - 1
                    ' Look at tiles in the column from bottom to top
                    For x = 1 To ColumnCount - 1
                        If Cells(x)(y).IsEmpty() Then
                            Continue For
                        End If

                        Dim destinationX = x
                        While _
                            destinationX - 1 >= 0 AndAlso
                            (Cells(destinationX - 1)(y).IsEmpty() OrElse
                             (Cells(destinationX - 1)(y).Value = Cells(x)(y).Value AndAlso
                              Not Cells(destinationX - 1)(y).WasMerged))
                            destinationX -= 1
                        End While

                        If destinationX <> x Then
                            MergeCells(Cells(x)(y), Cells(destinationX)(y))
                            changed = True
                        End If
                    Next
                Next
            ElseIf Direction = MoveDirection.Right Then
                For y = 0 To RowCount - 1
                    ' Look at tiles in the column from bottom to top
                    For x = ColumnCount - 2 To 0 Step - 1
                        If Cells(x)(y).IsEmpty() Then
                            Continue For
                        End If

                        Dim destinationX = x
                        While _
                            destinationX + 1 < ColumnCount AndAlso
                            (Cells(destinationX + 1)(y).IsEmpty() OrElse
                             (Cells(destinationX + 1)(y).Value = Cells(x)(y).Value AndAlso
                              Not Cells(destinationX + 1)(y).WasMerged))
                            destinationX += 1
                        End While

                        If destinationX <> x Then
                            MergeCells(Cells(x)(y), Cells(destinationX)(y))
                            changed = True
                        End If
                    Next
                Next
            End If
            Return changed
        End Function

        Private Sub MergeCells(SourceCell As Cell, DestinationCell As Cell)
            ' Assumes that an appropriate merge CAN definitely be done.
            If Not (SourceCell.X = DestinationCell.X Xor SourceCell.Y = DestinationCell.Y) Then
                Throw New InvalidOperationException("Cells to be merged must share either a row or column but not both")
            End If

            If DestinationCell.IsEmpty() Then
                ' This is the last available empty cell so take it!
                DestinationCell.Value = SourceCell.Value
                DestinationCell.PreviousPosition =
                    If(SourceCell.PreviousPosition, New Coordinate(SourceCell.X, SourceCell.Y))
                SourceCell.Value = 0
                SourceCell.PreviousPosition = Nothing
            Else
                If DestinationCell.WasMerged Then
                    Throw New InvalidOperationException("Destination cell has already been merged")
                ElseIf SourceCell.Value <> DestinationCell.Value Then
                    Throw New InvalidOperationException("Source and destination cells must have the same value")
                End If

                ' The next available cell has the same value and hasn't yet
                ' been merged, so lets merge them!
                DestinationCell.Value *= 2
                DestinationCell.WasMerged = True
                DestinationCell.PreviousPosition =
                    If(SourceCell.PreviousPosition, New Coordinate(SourceCell.X, SourceCell.Y))
                SourceCell.Value = 0
                SourceCell.PreviousPosition = Nothing

                ' Update the score
                Score += DestinationCell.Value
            End If
        End Sub

        Public Sub Reset()
            Me.Score = 0

            Cells = New Cell(Me.ColumnCount - 1)() {}
            For i As Integer = 0 To Me.ColumnCount - 1
                Cells(i) = New Cell(Me.RowCount - 1) {}
            Next

            For y As Integer = 0 To Me.RowCount - 1
                For x As Integer = 0 To Me.ColumnCount - 1
                    Cells(x)(y) = New Cell(x, y)
                Next
            Next
        End Sub

        Private ReadOnly rnd As New Random()

        Private Function GetRandomEmptyTile() As Tuple(Of Integer, Integer)
            Dim emptyIndices =
                    CellsIterator().Where(function(cell) cell.IsEmpty()).Select(
                        function(cell) New Tuple(Of Integer, Integer)(cell.X, cell.Y)).ToList()

            If emptyIndices.Count = 0 Then
                Return Nothing
            End If

            Dim [next] = rnd.[Next](0, emptyIndices.Count - 1)
            Return emptyIndices([next])
        End Function

        Private Function GetRandomStartingNumber() As Integer
            Return If(rnd.NextDouble() < 0.9, 2, 4)
        End Function
    End Class
End Namespace
