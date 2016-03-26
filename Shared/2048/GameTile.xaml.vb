Imports System.Windows.Media.Animation


Namespace _2048
    Partial Public NotInheritable Class GameTile
        Private ReadOnly _
            _backColors As Color() =
                {Color.FromArgb(255, 159, 192, 255), Color.FromArgb(255, 168, 255, 99),
                 Color.FromArgb(255, 255, 255, 104), Color.FromArgb(255, 255, 221, 140),
                 Color.FromArgb(255, 255, 170, 63), Color.FromArgb(255, 255, 223, 50), Color.FromArgb(255, 206, 255, 86),
                 Color.FromArgb(255, 118, 255, 138), Color.FromArgb(255, 89, 118, 255),
                 Color.FromArgb(255, 205, 96, 255), Color.FromArgb(255, 255, 78, 59)}

        Private ReadOnly _
            _foreColors As Color() =
                {Color.FromArgb(&HFF, &H77, &H6E, &H65), Color.FromArgb(&HFF, &H77, &H6E, &H65),
                 Color.FromArgb(&HFF, &H77, &H6E, &H65), Color.FromArgb(&HFF, &H77, &H6E, &H65),
                 Color.FromArgb(255, 255, 255, 255), Color.FromArgb(&HFF, &H77, &H6E, &H65),
                 Color.FromArgb(&HFF, &H77, &H6E, &H65), Color.FromArgb(&HFF, &H77, &H6E, &H65),
                 Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 255, 255, 255),
                 Color.FromArgb(255, 255, 255, 255)}

        Private _value As Integer

        Public Property Value() As Integer
            Get
                Return _value
            End Get
            Set
                _value = Value
                _textBlock.Text = If(_value > 0, _value.ToString(), "")
                TileBorder.Background = New SolidColorBrush(If _
                                                               (Value > 0,
                                                                _backColors(CInt(Math.Log(_value, 2)) - 1),
                                                                Color.FromArgb(&HFF, &HBB, &HAB, &HB0)))
                _textBlock.Foreground = If _
                    (Value > 0, New SolidColorBrush(_foreColors(CInt(Math.Log(_value, 2)) - 1)), _textBlock.Foreground)

                If _value > 0 Then
                    Dim index = CInt(Math.Log(_value, 2)) - 1
                    If index < OtherApi.Game20481.FriendList.Count Then
                        _textBlock.Text = ""
                        DataContext = OtherApi.Game20481.FriendList(index)
                    Else
                        DataContext = Nothing
                    End If
                Else
                    DataContext = Nothing
                End If
                OtherApi.Game20481.UpdateMaxValue(value, TryCast(DataContext, types.user))
            End Set
        End Property

        Private x As Integer
        Private y As Integer

        Public Sub New(x As Integer, y As Integer, Optional TransparentBorder As Boolean = False)
            Me.InitializeComponent()

            If TransparentBorder Then
                ContentBorder.BorderBrush = New SolidColorBrush(Colors.Transparent)
            End If

            Me.x = x
            Me.y = y
        End Sub

        Private ReadOnly _newTileStoryboard As New Storyboard()

        Public Sub BeginNewTileAnimation()
            Dim scaleXAnimation = Animation.CreateDoubleAnimation(0.1, 1.0, 1200000)
            Dim scaleYAnimation = Animation.CreateDoubleAnimation(0.1, 1.0, 1200000)

            Storyboard.SetTarget(scaleXAnimation, TileBorder)
            Storyboard.SetTargetName(scaleXAnimation, "AnimatedScaleTransform")
            Storyboard.SetTargetProperty(scaleXAnimation,
                                         Animation.CreatePropertyPath(
                                             "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"))

            '((TransformGroup)RenderTransform).Children

            Storyboard.SetTarget(scaleYAnimation, TileBorder)
            Storyboard.SetTargetName(scaleYAnimation, "AnimatedScaleTransform")
            Storyboard.SetTargetProperty(scaleYAnimation,
                                         Animation.CreatePropertyPath(
                                             "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"))

            _newTileStoryboard.Children.Clear()
            _newTileStoryboard.Children.Add(scaleXAnimation)
            _newTileStoryboard.Children.Add(scaleYAnimation)
            _newTileStoryboard.Begin()
        End Sub

        Private ReadOnly _doubledStoryboard As New Storyboard()

        Public Sub BeginDoubledAnimation()
            Dim scaleXAnimation = Animation.CreateDoubleAnimation(1.0, 1.2, 1200000)
            scaleXAnimation.AutoReverse = True

            Dim scaleYAnimation = Animation.CreateDoubleAnimation(1.0, 1.2, 1200000)
            scaleYAnimation.AutoReverse = True

            Storyboard.SetTarget(scaleXAnimation, TileBorder)
            Storyboard.SetTargetName(scaleXAnimation, "AnimatedScaleTransform")
            Storyboard.SetTargetProperty(scaleXAnimation,
                                         Animation.CreatePropertyPath(
                                             "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"))

            Storyboard.SetTarget(scaleYAnimation, TileBorder)
            Storyboard.SetTargetName(scaleYAnimation, "AnimatedScaleTransform")
            Storyboard.SetTargetProperty(scaleYAnimation,
                                         Animation.CreatePropertyPath(
                                             "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"))

            _doubledStoryboard.Children.Clear()
            _doubledStoryboard.Children.Add(scaleXAnimation)
            _doubledStoryboard.Children.Add(scaleYAnimation)
            AddHandler _doubledStoryboard.Completed, Sub(sender, o)
                SetValue(Canvas.ZIndexProperty, 0)
                                                     End Sub
            _doubledStoryboard.Begin()
        End Sub

        Public Sub StopAnimations()
            _newTileStoryboard.[Stop]()
            _doubledStoryboard.[Stop]()
            SetValue(Canvas.ZIndexProperty, 0)
        End Sub
    End Class
End Namespace

