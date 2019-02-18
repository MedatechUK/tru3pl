Public Class cursorloc

    Private Progress As Integer = -1
    Private x As Integer
    Private y As Integer
    Private _max As Integer

    Private _current As Integer = 0
    Public Property current As Integer
        Get
            Return _current
        End Get
        Set(value As Integer)
            _current = value
            If _max > 0 Then
                If CInt(current / _max * 100) > Progress Then
                    Progress = CInt(current / _max * 100)
                    Console.CursorLeft = x
                    Console.CursorTop = y
                    args.Colourise(ConsoleColor.Yellow, "{0}%", Progress.ToString)

                End If
                If current >= _max Then
                    Console.CursorLeft = x
                    Console.CursorTop = y
                    args.Colourise(ConsoleColor.Green, "Done.")
                    Console.CursorVisible = True

                End If
            End If
        End Set
    End Property

    Public Sub New(max As Integer)
        _max = max
        Console.CursorVisible = False
        x = Console.CursorLeft
        y = Console.CursorTop
        current = 0

    End Sub

End Class
