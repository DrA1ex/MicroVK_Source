Imports System.Windows.Controls.Primitives

Public Class MicroVkWrapPanel
    Inherits VirtualizingPanel
    Implements IScrollInfo
    Property ItemWidth As Double

    Private contentExtent As New Size(0, 0)
    Private viewport As New Size(0, 0)
    Private contentOffset As New Point
    Private offsetDelta As Double
    Private ScrollStep As Integer = 30
    Private itemsCount As Integer
    Public Property CanVerticallyScroll As Boolean Implements IScrollInfo.CanVerticallyScroll

    Public Property CanHorizontallyScroll As Boolean Implements IScrollInfo.CanHorizontallyScroll

    Public ReadOnly Property ExtentWidth As Double Implements IScrollInfo.ExtentWidth
        Get
            Return contentExtent.Width
        End Get
    End Property

    Public ReadOnly Property ExtentHeight As Double Implements IScrollInfo.ExtentHeight
        Get
            Return contentExtent.Height
        End Get
    End Property

    Public ReadOnly Property ViewportWidth As Double Implements IScrollInfo.ViewportWidth
        Get
            Return viewport.Width
        End Get
    End Property

    Public ReadOnly Property ViewportHeight As Double Implements IScrollInfo.ViewportHeight
        Get
            Return viewport.Height
        End Get
    End Property

    Public ReadOnly Property HorizontalOffset As Double Implements IScrollInfo.HorizontalOffset
        Get
            Return contentOffset.X
        End Get
    End Property

    Public ReadOnly Property VerticalOffset As Double Implements IScrollInfo.VerticalOffset
        Get
            Return contentOffset.Y
        End Get
    End Property

    Public Property ScrollOwner As ScrollViewer Implements IScrollInfo.ScrollOwner

    Private TempItemWidth As Double
    Private columns() As Double
    Private childrenCountPerRow As Integer
    Private _childDictionaty As New Dictionary(Of Integer, MyItem)
    Private firstVisibleItemIndex As Integer
    Private lastVisibleItemIndex As Integer

    Public Class MyItem
        Property Col As Integer
        Property Index As Integer
        Property ItemRect As Rect
        Property Child As UIElement
    End Class

    Protected Overrides Function MeasureOverride(availableSize As Windows.Size) As Windows.Size
        Dim children = Me.Children
        Dim containerGenerator = Me.ItemContainerGenerator
        Dim tempChildrenCountPerRow = CType(Math.Max(1, Math.Floor(availableSize.Width/ItemWidth)), Integer)
        If tempChildrenCountPerRow <> childrenCountPerRow Then
            firstVisibleItemIndex = 0
            _childDictionaty.Clear()
            childrenCountPerRow = tempChildrenCountPerRow
        End If

        TempItemWidth = ItemWidth 'availableSize.Width / childrenCountPerRow
        columns = New Double(childrenCountPerRow - 1) {}
        Dim i = firstVisibleItemIndex
        Debug.Print(i.ToString())
        If containerGenerator IsNot Nothing Then
            Dim position = containerGenerator.GeneratorPositionFromIndex(firstVisibleItemIndex)

            Dim index = If(position.Offset = 0, position.Index, position.Index + 1)
            Using containerGenerator.StartAt(position, GeneratorDirection.Forward, True)
                Dim child As UIElement
                Do
                    Dim isNewlyRealized As Boolean
                    child = TryCast(containerGenerator.GenerateNext(isNewlyRealized), UIElement)
                    If isNewlyRealized Then
                        If index >= children.Count Or True Then
                            Me.AddInternalChild(child)
                        Else
                            Me.InsertInternalChild(index, child)
                        End If
                        containerGenerator.PrepareItemContainer(child)

                    End If
                    If child IsNot Nothing Then

                        Dim minCol = Array.IndexOf(columns, columns.Min())
                        child.Measure(New Size(ItemWidth, availableSize.Height))
                        AddChildToDictionary(index,
                                             New MyItem With {.Col = minCol,
                                                .Child = child,
                                                .ItemRect =
                                                New Rect(minCol*ItemWidth,
                                                         columns(minCol),
                                                         ItemWidth,
                                                         child.DesiredSize.Height)})
                        columns(minCol) = columns(minCol) + child.DesiredSize.Height
                        If columns(minCol) < VerticalOffset Then
                            firstVisibleItemIndex = index + 1
                        End If
                    Else
                        Exit Do
                    End If
                    lastVisibleItemIndex = index
                    index += 1
                Loop While columns.Min < VerticalOffset + ViewportHeight
            End Using
        End If

        Me.CleanUpInVisibleChildren()
        Return New Size(availableSize.Width, ScrollOwner.ViewportHeight)
    End Function

    Protected Overrides Function ArrangeOverride(finalSize As Windows.Size) As Windows.Size
        InvalidateScrollInfo(finalSize)
        columns = New Double(childrenCountPerRow - 1) {}
        Dim children As UIElementCollection = Me.Children
        Dim containerGenerator As IItemContainerGenerator = Me.ItemContainerGenerator
        For index As Integer = children.Count - 1 To 0 Step - 1
            Dim position As New GeneratorPosition(index, 0)
            Dim num As Integer = containerGenerator.IndexFromGeneratorPosition(position)
            If _childDictionaty.ContainsKey(num) Then
                Dim item = _childDictionaty(num)
                Dim r = item.ItemRect
                item.Child.Arrange(New Rect(r.X, r.Y - VerticalOffset, r.Width, r.Height))
            End If
        Next
        Return finalSize
    End Function

    Sub CleanUpInVisibleChildren()
        Dim children As UIElementCollection = Me.Children
        Dim containerGenerator As IItemContainerGenerator = Me.ItemContainerGenerator
        For index As Integer = children.Count - 1 To 0 Step - 1
            Dim position As New GeneratorPosition(index, 0)
            Dim num As Integer = containerGenerator.IndexFromGeneratorPosition(position)
            If num > lastVisibleItemIndex OrElse num < firstVisibleItemIndex Then
                containerGenerator.Remove(position, 1)
                Me.RemoveInternalChildRange(index, 1)
            End If
        Next
    End Sub


    Sub AddChildToDictionary(i As Integer, myItem As MyItem)
        If _childDictionaty.ContainsKey(i) Then
            _childDictionaty(i) = myItem
        Else
            _childDictionaty.Add(i, myItem)
        End If
    End Sub

    Sub RemoveChildFromDictionary(i As Integer)
        If _childDictionaty.ContainsKey(i) Then
            _childDictionaty.Remove(i)
        End If
    End Sub

    Public Sub LineUp() Implements IScrollInfo.LineUp
        SetVerticalOffset(VerticalOffset - ScrollStep)
    End Sub

    Public Sub LineDown() Implements IScrollInfo.LineDown
        SetVerticalOffset(VerticalOffset + ScrollStep)
    End Sub

    Public Sub LineLeft() Implements IScrollInfo.LineLeft
        SetHorizontalOffset(HorizontalOffset - ScrollStep)
    End Sub

    Public Sub LineRight() Implements IScrollInfo.LineRight
        SetHorizontalOffset(HorizontalOffset + ScrollStep)
    End Sub

    Public Sub PageUp() Implements IScrollInfo.PageUp
        Throw New NotImplementedException()
    End Sub

    Public Sub PageDown() Implements IScrollInfo.PageDown
        Throw New NotImplementedException()
    End Sub

    Public Sub PageLeft() Implements IScrollInfo.PageLeft
        Throw New NotImplementedException()
    End Sub

    Public Sub PageRight() Implements IScrollInfo.PageRight
        Throw New NotImplementedException()
    End Sub

    Public Sub MouseWheelUp() Implements IScrollInfo.MouseWheelUp
        LineUp()
    End Sub

    Public Sub MouseWheelDown() Implements IScrollInfo.MouseWheelDown
        LineDown()
    End Sub

    Public Sub MouseWheelLeft() Implements IScrollInfo.MouseWheelLeft
        LineLeft()
    End Sub

    Public Sub MouseWheelRight() Implements IScrollInfo.MouseWheelRight
        LineRight()
    End Sub

    Public Sub SetHorizontalOffset(offset As Double) Implements IScrollInfo.SetHorizontalOffset
        Throw New NotImplementedException()
    End Sub

    Public Sub SetVerticalOffset(offset As Double) Implements IScrollInfo.SetVerticalOffset
        If offset < 0 OrElse ViewportHeight >= ExtentHeight Then
            offset = 0
        ElseIf offset + ViewportHeight >= ExtentHeight Then
            offset = ExtentHeight - ViewportHeight
        End If
        If Me.contentOffset.Y <> offset Then
            offsetDelta = Me.contentOffset.Y - offset
            Me.contentOffset.Y = offset
        End If

        If ScrollOwner IsNot Nothing Then
            ScrollOwner.InvalidateScrollInfo()
        End If
        Me.InvalidateMeasure()
    End Sub

    Public Function MakeVisible(visual As Visual, rectangle As Rect) As Rect Implements IScrollInfo.MakeVisible
        'Throw New NotImplementedException()
    End Function

    Sub InvalidateScrollInfo(availableSize As Size)
        Dim itemsOwner = ItemsControl.GetItemsOwner(Me)
        If itemsOwner Is Nothing Then Return
        itemsCount = itemsOwner.Items.Count
        Dim extent = New Size(childrenCountPerRow*ItemWidth, Math.Floor(itemsCount/childrenCountPerRow)*ItemWidth)
        If extent <> contentExtent Then
            contentExtent = extent
            RefreshOffset()
        End If
        If Not (availableSize <> viewport) Then
            Return
        End If
        viewport = availableSize
        InvalidateScrollOwner()
        RefreshOffset()
    End Sub

    Sub RefreshOffset()
        Me.SetVerticalOffset(Me.VerticalOffset)
    End Sub

    Sub InvalidateScrollOwner()
        If ScrollOwner IsNot Nothing Then
            ScrollOwner.InvalidateScrollInfo()
        End If
    End Sub
End Class
