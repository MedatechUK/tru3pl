Imports System.Data.SqlClient
Imports System.IO

Module Module1

    Private cnstr As String = "Integrated Security=true;Initial Catalog=wlnd;Server=walrus\PRI"

    Public cn As New SqlConnection(cnstr)
    Public cn2 As New SqlConnection(cnstr)
    Public args As clArg

    Sub Main(arg() As String)

        args = New clArg(arg)

        Dim act As Boolean = False
        Dim out As Boolean = False

        For Each k As String In args.Keys
            Select Case k.ToLower
                Case "?", "help"
                    args.syntax()
                    End

                Case "sku", "po", "so"
                    act = True

                Case "out"
                    out = New DirectoryInfo(args(k)).Exists

            End Select

        Next

        If Not act Then
            Console.WriteLine("Missing action type.")
            args.syntax()
            End

        End If

        If Not out Then
            Console.WriteLine("Missing or invalid output location.")
            args.syntax()
            End

        End If

        cn.Open()
        cn2.Open()

        If args.Keys.Contains("sku") Then
            Using sku As New OutboundSKU()
                Using sw As New StreamWriter(Path.Combine(args("out"), sku.FileStr))
                    Using r As SqlDataReader = sku.cmd.ExecuteReader()

                        Dim m As Dictionary(Of Integer, Integer) = sku.CreateMap(r)

                        While r.Read
                            sku.write(sw, m, r)

                        End While

                    End Using

                End Using

            End Using

        End If

        If args.Keys.Contains("po") Then

            Using PO As New OutboundPO()
                Dim sl As Dictionary(Of Integer, Integer) = Nothing

                Using sw As New StreamWriter(Path.Combine(args("out"), PO.FileStr))
                    Using r As SqlDataReader = PO.cmd.ExecuteReader()

                        Dim m As Dictionary(Of Integer, Integer) = PO.CreateMap(r)

                        While r.Read
                            PO.write(sw, m, r)

                            Using POi As New OutboundPOItems(r(0))
                                Using q As SqlDataReader = POi.cmd.ExecuteReader()
                                    If sl Is Nothing Then
                                        sl = POi.CreateMap(q)
                                    End If

                                    While q.Read
                                        POi.write(sw, sl, q)

                                    End While

                                End Using

                            End Using

                        End While

                    End Using

                End Using

            End Using

        End If

        If args.Keys.Contains("so") Then

            Using SO As New OutboundSO()
                Dim sl As Dictionary(Of Integer, Integer) = Nothing

                Using sw As New StreamWriter(Path.Combine(args("out"), SO.FileStr))
                    Using r As SqlDataReader = SO.cmd.ExecuteReader()

                        Dim m As Dictionary(Of Integer, Integer) = SO.CreateMap(r)

                        While r.Read
                            SO.write(sw, m, r)

                            Using SOi As New OutboundSOItems(r(0))
                                Using q As SqlDataReader = SOi.cmd.ExecuteReader()
                                    If sl Is Nothing Then
                                        sl = SOi.CreateMap(q)
                                    End If

                                    While q.Read
                                        SOi.write(sw, sl, q)

                                    End While

                                End Using

                            End Using

                        End While

                    End Using

                End Using

            End Using

        End If

    End Sub

End Module
