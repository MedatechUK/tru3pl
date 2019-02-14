Imports System.Data.SqlClient
Imports System.IO
Imports WinSCP

Module Module1

    Private cnstr As String = "Integrated Security=true;Initial Catalog=tru;Server=localhost\PRI"

    Public cn As New SqlConnection(cnstr)
    Public cn2 As New SqlConnection(cnstr)
    Public cn3 As New SqlConnection(cnstr)

    Public args As clArg
    Public transferOptions As TransferOptions
    Public sessionOptions As SessionOptions

    Sub Main(arg() As String)

        transferOptions = New TransferOptions
        transferOptions.TransferMode = TransferMode.Binary

        sessionOptions = New SessionOptions
        With sessionOptions
            .Protocol = Protocol.Sftp
            .HostName = "secureftp.torque.eu"
            .UserName = "ftptrutex"
            .Password = "xkazW09H"
            .SshHostKeyFingerprint = "ssh-rsa 2048 53:a0:ba:88:57:32:c8:7b:33:ac:6d:4a:e5:35:23:e2"
        End With

        args = New clArg(arg)

        Dim basedir As DirectoryInfo = Nothing
        Dim indir As DirectoryInfo = Nothing
        Dim outdir As DirectoryInfo = Nothing
        Dim insave As DirectoryInfo = Nothing
        Dim outsave As DirectoryInfo = Nothing

        For Each k As String In args.Keys
            Select Case k.ToLower
                Case "?", "help"
                    args.syntax()
                    End

                Case "dir"
                    basedir = New DirectoryInfo(args(k))

            End Select

        Next

        If Not basedir.EXISTS Then
            Console.WriteLine("Missing or invalid output location.")
            args.syntax()
            End

        Else
            With New DirectoryInfo(Path.Combine(basedir.FullName, "in"))
                If Not .Exists Then .Create()
                indir = New DirectoryInfo(.FullName)
                With New DirectoryInfo(Path.Combine(.FullName, "save"))
                    If Not .Exists Then .Create()
                    insave = New DirectoryInfo(.FullName)
                End With
            End With
            With New DirectoryInfo(Path.Combine(basedir.FullName, "out"))
                If Not .Exists Then .Create()
                outdir = New DirectoryInfo(.FullName)
                With New DirectoryInfo(Path.Combine(.FullName, "save"))
                    If Not .Exists Then .Create()
                    outsave = New DirectoryInfo(.FullName)
                End With
            End With
        End If

        Try
            cn.Open()
            cn2.Open()
            cn3.Open()

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            End

        End Try

        If args.Keys.Contains("sku") Then

            Using sku As New OutboundSKU()
                Using r As SqlDataReader = sku.cmd.ExecuteReader()
                    If r.HasRows Then
                        Using sw As New StreamWriter(Path.Combine(outdir.FullName, sku.FileStr))
                            Dim m As Dictionary(Of Integer, Integer) = sku.CreateMap(r)
                            While r.Read
                                sku.write(sw, m, r)
                                sku.update(r(0)).ExecuteNonQuery()

                            End While

                        End Using

                    End If

                End Using

            End Using

        End If

        If args.Keys.Contains("po") Then

            Using PO As New OutboundPO()
                Dim sl As Dictionary(Of Integer, Integer) = Nothing
                Using r As SqlDataReader = PO.cmd.ExecuteReader()
                    If r.HasRows Then
                        Using sw As New StreamWriter(Path.Combine(outdir.FullName, PO.FileStr))
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
                                            POi.update(q(0)).ExecuteNonQuery()

                                        End While

                                    End Using

                                End Using

                            End While

                        End Using

                    End If

                End Using

            End Using

        End If

        If args.Keys.Contains("so") Then

            Using SO As New OutboundSO()
                Dim sl As Dictionary(Of Integer, Integer) = Nothing
                Using r As SqlDataReader = SO.cmd.ExecuteReader()
                    If r.HasRows Then
                        Using sw As New StreamWriter(Path.Combine(outdir.FullName, SO.FileStr))
                            Dim m As Dictionary(Of Integer, Integer) = SO.CreateMap(r)

                            While r.Read
                                SO.write(sw, m, r)

                                Using SOi As New OutboundSOItems(r(0), r(1))
                                    Using q As SqlDataReader = SOi.cmd.ExecuteReader()
                                        If sl Is Nothing Then
                                            sl = SOi.CreateMap(q)
                                        End If

                                        While q.Read
                                            SOi.write(sw, sl, q)
                                            SOi.update(q(0)).ExecuteNonQuery()

                                        End While

                                    End Using

                                End Using

                            End While

                        End Using

                    End If

                End Using

            End Using

        End If

        If args.Keys.Contains("ftp") Then

            Dim transferResult As TransferOperationResult
            Using session As New Session

                ' Connect
                session.Open(sessionOptions)

                ' Send outbound files
                Try
                    transferResult = session.PutFiles(
                        String.Format("{0}\*.txt", outdir.FullName),
                        "/test/in/",
                        False,
                        transferOptions
                    )

                    ' Throw on any error
                    transferResult.Check()

                    ' Move to save folder
                    For Each transfer In transferResult.Transfers
                        File.Move(
                            transfer.FileName,
                            Path.Combine(
                                outsave.FullName,
                                New FileInfo(transfer.FileName).Name
                            )
                        )

                    Next

                Catch ex As Exception
                    Console.Write(ex.Message)

                End Try

                ' Get inbound files
                For Each orph As FileInfo In indir.GetFiles("*.csv")
                    Try
                        Dim moveto As New FileInfo(
                            Path.Combine(
                                insave.FullName,
                                orph.Name
                            )
                        )
                        If moveto.Exists Then
                            orph.Delete()

                        Else
                            Dim imp As New Import
                            imp.import(orph)

                            ' Move to save folder
                            File.Move(
                                orph.FullName,
                                moveto.FullName
                            )

                        End If

                    Catch ex As Exception
                        Console.Write(ex.Message)

                    End Try

                Next

                Try
                    transferResult = session.GetFiles(
                        "/test/out/*.csv",
                        String.Format(
                            "{0}\",
                            indir.FullName
                        ),
                        False,
                        transferOptions
                   )

                    ' Throw on any error
                    transferResult.Check()

                    For Each transfer In transferResult.Transfers

                        Try
                            Dim FN As New FileInfo(
                                Path.Combine(
                                    indir.FullName,
                                    Split(transfer.FileName, "/").Last
                                )
                            )
                            Dim moveto As New FileInfo(
                                Path.Combine(
                                    insave.FullName,
                                    Split(transfer.FileName, "/").Last
                                )
                            )

                            If moveto.Exists Then
                                FN.Delete()

                            Else
                                Dim imp As New Import
                                imp.import(FN)

                                ' Move to save folder
                                File.Move(
                                    FN.FullName,
                                    moveto.FullName
                                )

                            End If

                        Catch ex As Exception
                            Console.Write(ex.Message)

                        End Try

                    Next

                Catch ex As Exception
                    Console.Write(ex.Message)

                End Try

            End Using

        End If

    End Sub

End Module
