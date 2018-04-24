Imports System.Windows.Threading

Class MainWindow

    Dim GameLoop As New DispatcherTimer

    'Brick Properties
    Private BRICK_W As Integer = 100
    Private BRICK_H As Integer = 25
    Private BRICK_COLS As Integer = 4
    Private BRICK_ROWS As Integer = 4
    Private BRICK_GAP As Integer = 1

    'Paddle Properties
    Dim PADDLE As New Rectangle()
    Dim PADDLE_BUFFER As Integer = 15
    Const PADDLE_DISTANCE_FROM_BOTTOM As Double = 50
    Dim CENTER_OF_PADDLE As Double
    Private PADDLE_SPEED As Double = 5
    Dim PADDLE_TRANSLATE As New TranslateTransform(0, 0)
    Private MOVE_LEFT As Boolean
    Private MOVE_RIGHT As Boolean

    'Ball Properties
    Private BALL As New Ellipse()
    Private BALL_BUFFER As Double = 5
    Private BALL_TRANSLATE As New TranslateTransform(0, 0)
    Private BALL_SPEED_Y As Double = 5
    Private BALL_SPEED_X As Double = 5

    'Wall Properties
    Private WALL_TOP As Double = 0
    Private WALL_LEFT As Double = 0
    Private WALL_RIGHT As Double
    Private WALL_BOTTOM As Double

    Sub New()
        InitializeComponent()

        GameLoop.Interval = TimeSpan.FromMilliseconds(1)
        AddHandler GameLoop.Tick, AddressOf UpdateLoop

        DrawBricks()
        DrawPaddle()
        DrawBall()
        SETWALLS()
        GameLoop.Start()
    End Sub

    Private Sub UpdateLoop(sender As Object, e As EventArgs)
        MovePaddle()
        MoveBall()
        Check_Collision()
    End Sub


    Private Sub Check_Collision()
        'Retrieve the coordinates of the ball
        Dim pt As Point = New Point(BALL_TRANSLATE.X, BALL_TRANSLATE.Y)
        VisualTreeHelper.HitTest(MainCanvas, Nothing, New HitTestResultCallback(AddressOf MyHitTestResult), New PointHitTestParameters(pt))

        If BALL_TRANSLATE.Y < WALL_TOP And BALL_SPEED_Y < 0 Then
            BALL_SPEED_Y *= -1
        End If

        If BALL_TRANSLATE.X <= WALL_LEFT And BALL_SPEED_X < 0 Then
            BALL_SPEED_X *= -1
        End If

        If BALL_TRANSLATE.X >= WALL_RIGHT And BALL_SPEED_X > 0 Then
            BALL_SPEED_X *= -1
        End If

        If BALL_TRANSLATE.Y > WALL_BOTTOM Then
            BALL_TRANSLATE.Y = MainCanvas.Height / 2
            BALL_TRANSLATE.X = MainCanvas.Width / 2
        End If
    End Sub

    Public Function MyHitTestResult(ByVal result As HitTestResult) As HitTestResultBehavior
        If result.VisualHit.GetType() Is GetType(Rectangle) Then
            If (CType(result.VisualHit, Rectangle)).Tag = "brick" Then
                MainCanvas.Children.Remove(CType(result.VisualHit, Rectangle))
                BALL_SPEED_Y *= -1
            Else
                'The ball hit the paddle
                BALL_SPEED_Y *= -1

                'Check the distance of the ball from the center of the paddle
                Dim centerOfPaddleX As Double = PADDLE_TRANSLATE.X + CENTER_OF_PADDLE
                Dim ballDistFromPaddleCenterX As Double = BALL_TRANSLATE.X - centerOfPaddleX
                'find the andlge the ball will move as it hits the paddle
                BALL_SPEED_X = ballDistFromPaddleCenterX * 0.08
            End If
        End If
        Return HitTestResultBehavior.Continue
    End Function

    Private Sub DrawBricks()
        For row = 0 To BRICK_ROWS
            For column = 0 To BRICK_COLS
                Dim BRICK As New Rectangle()
                BRICK.Height = BRICK_H - BRICK_GAP
                BRICK.Width = BRICK_W - BRICK_GAP
                BRICK.Fill = Brushes.AliceBlue
                BRICK.StrokeThickness = 2
                BRICK.RenderTransform = New TranslateTransform(BRICK_W * column, BRICK_H * row)
                BRICK.Tag = "brick"
                MainCanvas.Children.Add(BRICK)
            Next
        Next
    End Sub

    Private Sub DrawBall()
        With BALL
            .Fill = Brushes.Red
            .StrokeThickness = 2
            .Stroke = Brushes.Black
            .Width = 20
            .Height = 20
            .RenderTransform = BALL_TRANSLATE
        End With

        BALL_TRANSLATE.X = (MainCanvas.Width / 2)
        BALL_TRANSLATE.Y = (MainCanvas.Height / 2)
        MainCanvas.Children.Add(BALL)
    End Sub

    Private Sub DrawPaddle()
        With PADDLE
            .Fill = Brushes.LightBlue
            .Stroke = Brushes.Black
            .StrokeThickness = 2
            .Width = 124
            .Height = 20
            .RenderTransform = PADDLE_TRANSLATE
            CENTER_OF_PADDLE = .Width / 2
        End With

        PADDLE_TRANSLATE.X = (MainCanvas.Width - PADDLE.Width - PADDLE_BUFFER)
        PADDLE_TRANSLATE.Y = MainCanvas.Height - PADDLE.Height - (PADDLE_DISTANCE_FROM_BOTTOM)
        MainCanvas.Children.Add(PADDLE)
    End Sub

    Private Sub MovePaddle()
        If MOVE_LEFT Then
            PADDLE_TRANSLATE.X -= PADDLE_SPEED
        End If
        If MOVE_RIGHT Then
            PADDLE_TRANSLATE.X += PADDLE_SPEED
        End If
        PADDLE.RenderTransform = PADDLE_TRANSLATE

    End Sub

    Private Sub MoveBall()
        BALL_TRANSLATE.X += BALL_SPEED_X
        BALL_TRANSLATE.Y += BALL_SPEED_Y
        BALL.RenderTransform = BALL_TRANSLATE

    End Sub

    Private Sub SETWALLS()
        WALL_RIGHT = MainCanvas.Width - (BALL.Width + BALL_BUFFER)
        WALL_TOP += BALL.Height
        WALL_BOTTOM = MainCanvas.Height
    End Sub

    Private Sub MyWindow_KeyDown(sender As Object, e As KeyEventArgs) Handles MyWindow.KeyDown
        Select Case e.Key
            Case Key.A
                MOVE_LEFT = True
            Case Key.Left
                MOVE_LEFT = True
            Case Key.Right
                MOVE_RIGHT = True
            Case Key.D
                MOVE_RIGHT = True
            Case Key.Escape
                Me.Close()
        End Select
    End Sub

    Private Sub MyWindow_KeyUp(sender As Object, e As KeyEventArgs) Handles MyWindow.KeyUp
        Select Case e.Key
            Case Key.A
                MOVE_LEFT = False
            Case Key.Left
                MOVE_LEFT = False
            Case Key.Right
                MOVE_RIGHT = False
            Case Key.D
                MOVE_RIGHT = False
            Case Key.Escape
                Me.Close()
        End Select
    End Sub
End Class
