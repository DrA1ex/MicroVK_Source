
Imports System.Collections.Generic
Imports System.Windows.Media.Animation
Imports MicroVK._2048.Model

Namespace _2048
    Partial Public NotInheritable Class GameGrid
        Private Const _ROWS As Integer = 4
        Private Const _COLS As Integer = 4

        Private ReadOnly _underlyingTiles As GameTile()()
        public ReadOnly _gameModel As GameModel

        Private ReadOnly _scoreCard As ScoreCard

        Public ReadOnly Property Score() As Integer
            Get
                Return _gameModel.Score
            End Get
        End Property

        Private Function GetTileSize() As Double
            Return GameCanvas.ActualWidth/_ROWS
        End Function

        Public Sub New()
            Me.InitializeComponent()

            AddHandler Me.SizeChanged, AddressOf GameGrid_SizeChanged

            _gameModel = New GameModel(_ROWS, _COLS)

            _underlyingTiles = New GameTile(_COLS - 1)() {}

            For i As Integer = 0 To _COLS - 1
                _underlyingTiles(i) = New GameTile(_ROWS - 1) {}
            Next

            For y As Integer = 0 To _ROWS - 1
                For x As Integer = 0 To _COLS - 1
                    _underlyingTiles(x)(y) = New GameTile(x, y)
                    _underlyingTiles(x)(y).SetValue(Canvas.ZIndexProperty, 0)
                    GameCanvas.Children.Add(_underlyingTiles(x)(y))
                Next
            Next

            _scoreCard = New ScoreCard()
            '_scoreCard.SetValue(Grid.RowProperty, 0)
            '_scoreCard.SetValue(Grid.ColumnProperty, 0)
            'ContentGrid.Children.Add(_scoreCard)

            '_scoreCard.Score = 0
            '_scoreCard.Title = "SCORE"

            StartGame()
        End Sub

        Private Sub GameGrid_SizeChanged(Sender As Object, Args As SizeChangedEventArgs)
            For y = 0 To _ROWS - 1
                For x = 0 To _COLS - 1
                    _underlyingTiles(x)(y).Width = GetTileSize()
                    _underlyingTiles(x)(y).Height = GetTileSize()
                    _underlyingTiles(x)(y).SetValue(Canvas.LeftProperty, x*GetTileSize())
                    _underlyingTiles(x)(y).SetValue(Canvas.TopProperty, y*GetTileSize())
                Next
            Next
        End Sub

        Private Sub LoadMap()
            Dim i = 0
            While i < 2
                Dim x as Integer = CType(Int(Rnd()*4), Integer)
                Dim y as Integer = CType(Int(Rnd()*4), Integer)
                If _gameModel.Cells(x)(y).Value = 0 Then
                    _gameModel.Cells(x)(y) = New Cell(x, y) With {.Value = 2}
                    i = i + 1
                End If
            End While
        End Sub

        Private Sub StartGame()
            LoadMap()

            'var first = new Tuple<int, int>(0, 0);//GetRandomEmptyTile();
            '            _gameModel.Cells[first.Item1][first.Item2].Value = GetRandomStartingNumber();
            '            _gameModel.Cells[first.Item1][first.Item2].WasCreated = true;
            '
            '            /*var second = GetRandomEmptyTile();
            '            _gameModel.Cells[second.Item1][second.Item2].Value = GetRandomStartingNumber();
            '            _gameModel.Cells[second.Item1][second.Item2].WasCreated = true;


            UpdateUI()

            'Window.Current.CoreWindow.KeyDown += OnKeyDown;
            'this.ManipulationStarted += OnManipulationStarted;
            'this.ManipulationDelta += OnManipulationDelta;
            'this.ManipulationMode = ManipulationModes.All;
        End Sub


        Private Sub UpdateUI()
            For Each cell In _gameModel.CellsIterator()
                _underlyingTiles(cell.X)(cell.Y).StopAnimations()
            Next

            ' Set to 0 any underlying tile where MovedFrom != null && !WasDoubled OR newValue == 0

            For Each cell In _gameModel.CellsIterator()
                If _
                    (cell.PreviousPosition IsNot Nothing AndAlso Not cell.WasMerged) OrElse cell.Value = 0 OrElse
                    cell.WasCreated Then
                    _underlyingTiles(cell.X)(cell.Y).Value = 0
                End If
            Next

            ' For each tile where MovedFrom != null
            ' Create a new temporary animation tile and animate to move to new location
            Dim storyboard__1 = New Storyboard()
            Dim tempTiles = New List(Of GameTile)()
            For y = 0 To _ROWS - 1
                For x = 0 To _COLS - 1
                    If _gameModel.Cells(x)(y).PreviousPosition IsNot Nothing Then
                        Dim tempTile = New GameTile(x, y, True)
                        tempTile.Width = GetTileSize()
                        tempTile.Height = GetTileSize()
                        tempTile.SetValue(Canvas.ZIndexProperty, 1)
                        tempTiles.Add(tempTile)
                        GameCanvas.Children.Add(tempTile)

                        tempTile.Value = CType(If _
                            (_gameModel.Cells(x)(y).WasMerged,
                             _gameModel.Cells(x)(y).Value/2,
                             _gameModel.Cells(x)(y).Value),
                                               Integer)

                        Dim from = _gameModel.Cells(x)(y).PreviousPosition.X*GetTileSize()
                        Dim [to] = x*GetTileSize()
                        Dim xAnimation = Animation.CreateDoubleAnimation(from, [to], 1200000)

                        from = _gameModel.Cells(x)(y).PreviousPosition.Y*GetTileSize()
                        [to] = y*GetTileSize()
                        Dim yAnimation = Animation.CreateDoubleAnimation(from, [to], 1200000)

                        Storyboard.SetTarget(xAnimation, tempTile)
                        Storyboard.SetTargetProperty(xAnimation, Animation.CreatePropertyPath("(Canvas.Left)"))

                        Storyboard.SetTarget(yAnimation, tempTile)
                        Storyboard.SetTargetProperty(yAnimation, Animation.CreatePropertyPath("(Canvas.Top)"))

                        storyboard__1.Children.Add(xAnimation)
                        storyboard__1.Children.Add(yAnimation)
                    End If
                Next
            Next

            AddHandler storyboard__1.Completed, Sub(sender, o)
                For y = 0 To _ROWS - 1
                    For x = 0 To _COLS - 1
                        _underlyingTiles(x)(y).Value = _gameModel.Cells(x)(y).Value
                    Next
                Next

                For Each tile In tempTiles
                    GameCanvas.Children.Remove(tile)
                Next

                For Each cell In _gameModel.CellsIterator()
                    If cell.WasCreated Then
                        _underlyingTiles(cell.X)(cell.Y).BeginNewTileAnimation()
                    ElseIf cell.WasMerged Then
                        _underlyingTiles(cell.X)(cell.Y).SetValue(Canvas.ZIndexProperty, 100)
                        _underlyingTiles(cell.X)(cell.Y).BeginDoubledAnimation()
                    End If

                    ' TODO move this to a 'ResetTurn' method in the model
                    cell.WasCreated = False
                    cell.WasMerged = False
                    cell.PreviousPosition = Nothing
                Next

                _moveInProgress = False

                ' Update the score
                _scoreCard.Score = _gameModel.Score
                                                End Sub

            storyboard__1.Begin()
        End Sub

        Private _moveInProgress As Boolean

        Public Sub HandleMove(Direction As MoveDirection)
            If _moveInProgress Then
                Return
            End If

            _moveInProgress = True

            If _gameModel.PerformMove(Direction) Then
                UpdateUI()
            Else
                _moveInProgress = False
            End If
        End Sub
    End Class
End Namespace
