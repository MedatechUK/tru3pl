Imports System.IO

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
            Console.Write("Finished. Press any key.")
            Console.CursorVisible = False
            Console.ReadKey()
            Console.WriteLine("")
            Console.CursorVisible = True

        End If
        End
    End Sub

    Sub Colourise(colour As ConsoleColor, str As String, ParamArray param() As String)

        Dim last As ConsoleColor = Console.ForegroundColor
        Console.ForegroundColor = colour
        Console.WriteLine(
                String.Format(
                str,
                param
            )
        )
        Console.ForegroundColor = last

    End Sub

    Sub line(str As String, ParamArray param() As String)
        Console.Write(String.Format(str, param))
        Console.Write(
            String.Format(
                " {0} ",
                New String("."c, 40 - Console.CursorLeft)
            )
        )
    End Sub

#Region "Logging"

    Private Function LogFolder() As DirectoryInfo
        Return New DirectoryInfo(
            Path.Combine(
                Environment.GetEnvironmentVariable("SystemRoot"),
                String.Format(
                    "logs\{0}\{1}",
                    System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.NAME,
                    Now.ToString("yyyy-MM")
                )
            )
        )

    End Function

    Private Function currentlog() As FileInfo
        With LogFolder()
            If Not .Exists Then .Create()
            Return New FileInfo(
                Path.Combine(
                    .FullName,
                    String.Format(
                        "{0}.txt",
                        Now.ToString("yyMMdd")
                    )
                )
            )

        End With

    End Function

    Public Sub Log(ByVal str, ByVal ParamArray args())
        'Console.WriteLine("{0}> {1}", Format(Now, "HH:mm:ss"), String.Format(str, args))
        Using log As New StreamWriter(currentlog.FullName, True)
            log.WriteLine("{0}> {1}", Format(Now, "HH:mm:ss"), String.Format(str, args))
        End Using

    End Sub

#End Region

End Class
