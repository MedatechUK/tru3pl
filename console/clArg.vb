Public Class clArg
    Inherits Dictionary(Of String, String)

    Private Enum eMode
        Switch
        Param
    End Enum

    Sub New(ByVal Args As String())

        Console.WriteLine("")

        Dim i As Integer = 0
        Dim m As eMode = eMode.Switch
        Dim thisSwitch As String = ""

        If Args.Length = 0 Then Exit Sub

        Do
            Select Case Args(i).Substring(0, 1)
                Case "-", "/"
                    Add(Args(i).Substring(1).ToLower, "")
                    thisSwitch = Args(i).Substring(1).ToLower
                    m = eMode.Param
                Case Else
                    Select Case m
                        Case eMode.Param
                            Me(thisSwitch.ToLower) = Args(i)
                            thisSwitch = ""
                            m = eMode.Switch

                        Case eMode.Switch
                            Add(Args(i).ToLower, "")

                    End Select

            End Select
            i += 1
        Loop Until i = Args.Count

    End Sub

    Public Sub syntax()
        Console.Write(My.Resources.syntax)
        Console.WriteLine("")

    End Sub

    Sub wait()
        If args.Keys.Contains("w") Then
            Console.WriteLine("")
            Console.WriteLine("Finished. Press any key.")
            Console.ReadKey()

        End If
        End
    End Sub

End Class
