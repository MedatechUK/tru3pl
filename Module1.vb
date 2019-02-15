Imports System.Data.SqlClient
Imports System.IO
Imports WinSCP

Module Module1

    Public cn As SqlConnection
    Public cn2 As SqlConnection
    Public cn3 As SqlConnection

    Public args As clArg
    Public transferOptions As TransferOptions
    Public sessionOptions As SessionOptions

    Public remoteFolder As String = "test"
    Public basedir As DirectoryInfo = Nothing

    Sub Main(arg() As String)

#Region "Handle command line."

        args = New clArg(arg)
        For Each k As String In args.Keys
            Select Case k.ToLower
                Case "?", "help"
                    args.syntax()
                    End

                Case "dir"
                    basedir = New DirectoryInfo(args(k))

                Case "live"
                    remoteFolder = "live"

            End Select

        Next

#End Region

#Region "Check Directory structure"

        Dim indir As DirectoryInfo = Nothing
        Dim outdir As DirectoryInfo = Nothing
        Dim insave As DirectoryInfo = Nothing
        Dim outsave As DirectoryInfo = Nothing

        If basedir Is Nothing Then
            Console.WriteLine("Missing -dir.")
            args.syntax()
            args.wait()


        ElseIf Not basedir.Exists Then
            Console.WriteLine(String.Format("Invalid -dir [{0}].", basedir.FullName))
            args.wait()

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

#End Region

#Region "Open database connections."

        Try
            cn = New SqlConnection(My.Settings.cnstr)
            cn.Open()

            cn2 = New SqlConnection(My.Settings.cnstr)
            cn2.Open()

            cn3 = New SqlConnection(My.Settings.cnstr)
            cn3.Open()

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            args.wait()

        End Try

#End Region

#Region "Out"

        If args.Keys.Contains("sku") Or args.Keys.Contains("all") Then

            Using sku As New OutboundSKU()
                Using r As SqlDataReader = sku.cmd.ExecuteReader()
                    If r.HasRows Then
                        Console.WriteLine("OK")
                        sku.ProgressBar()

                        Using sw As New StreamWriter(Path.Combine(outdir.FullName, sku.FileStr))
                            Dim m As Dictionary(Of Integer, Integer) = sku.CreateMap(r)
                            While r.Read
                                sku.write(sw, m, r)
                                sku.update(r(0)).ExecuteNonQuery()

                            End While

                        End Using

                    Else
                        Console.WriteLine("No Rows.")

                    End If

                End Using

            End Using

        End If

        If args.Keys.Contains("po") Or args.Keys.Contains("all") Then

            Using PO As New OutboundPO()
                Dim sl As Dictionary(Of Integer, Integer) = Nothing
                Using r As SqlDataReader = PO.cmd.ExecuteReader()
                    If r.HasRows Then
                        Console.WriteLine("OK")
                        PO.ProgressBar()

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

                    Else
                        Console.WriteLine("No Rows.")

                    End If

                End Using

            End Using

        End If

        If args.Keys.Contains("so") Or args.Keys.Contains("all") Then

            Using SO As New OutboundSO()
                Dim sl As Dictionary(Of Integer, Integer) = Nothing
                Using r As SqlDataReader = SO.cmd.ExecuteReader()
                    If r.HasRows Then
                        Console.WriteLine("OK")
                        SO.ProgressBar()

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

                    Else
                        Console.WriteLine("No Rows.")

                    End If

                End Using

            End Using

        End If

#End Region

#Region "In"

        If args.Keys.Contains("ftp") Then

#Region "FTP settings."

            transferOptions = New TransferOptions
            transferOptions.TransferMode = TransferMode.Binary
            sessionOptions = New SessionOptions
            With sessionOptions
                .Protocol = Protocol.Sftp
                .HostName = My.Settings.HostName
                .UserName = My.Settings.UserName
                .Password = My.Settings.Password
                .SshHostKeyFingerprint = My.Settings.SshHostKeyFingerprint
            End With

#End Region

            Dim transferResult As TransferOperationResult
            Using session As New Session

                ' Connect
                Console.WriteLine("")
                Console.Write(String.Format("Opening SFTP connection to {0} ... ", sessionOptions.HostName))

                Try
                    session.Open(sessionOptions)
                    Console.WriteLine("OK")

                Catch ex As Exception
                    Console.WriteLine("FAILED")
                    Console.WriteLine(ex.Message)
                    args.wait()

                End Try

                Select Case args("ftp").ToLower
                    Case "", "out"

                        Console.WriteLine("")

                        ' Send outbound files                        
                        If outdir.GetFiles("*.txt").Count > 0 Then
                            Console.Write(
                                String.Format(
                                    "Sending {0} files...",
                                    outdir.GetFiles("*.txt").Count.ToString
                                )
                            )

                            Try
                                transferResult = session.PutFiles(
                                    String.Format(
                                        "{0}\*.txt",
                                        outdir.FullName
                                    ),
                                    String.Format(
                                        "/{0}/in/",
                                        remoteFolder
                                    ),
                                    False,
                                    transferOptions
                                )

                                ' Throw on any error
                                transferResult.Check()
                                Console.WriteLine("OK")

                                ' Move to save folder
                                For Each transfer In transferResult.Transfers
                                    Console.WriteLine(
                                        String.Format(
                                            " Uploaded file {0}.",
                                            Split(transfer.FileName, "\").Last
                                        )
                                    )
                                    File.Move(
                                        transfer.FileName,
                                        Path.Combine(
                                            outsave.FullName,
                                            New FileInfo(transfer.FileName).Name
                                        )
                                    )

                                Next

                            Catch ex As Exception
                                Console.WriteLine("FAILED")
                                Console.WriteLine(ex.Message)

                            End Try

                        Else
                            Console.WriteLine("No files to upload.")

                        End If

                End Select

                Select Case args("ftp").ToLower
                    Case "", "in"

                        Console.WriteLine("")

                        ' Check for orphan files
                        If indir.GetFiles("*.csv").Count > 0 Then
                            Console.WriteLine(
                                String.Format(
                                    "{0} Orphan files found in [{1}].",
                                    indir.GetFiles("*.csv").Count.ToString,
                                    insave.FullName
                                )
                            )
                            For Each orph As FileInfo In indir.GetFiles("*.csv")

                                Try
                                    Dim moveto As New FileInfo(
                                        Path.Combine(
                                            insave.FullName,
                                            orph.Name
                                        )
                                    )
                                    If moveto.Exists Then
                                        Console.WriteLine(
                                            String.Format(
                                                "{0} exists in save folder. Deleting.",
                                                orph.Name
                                            )
                                        )
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

                        End If

                        ' Get inbound files
                        Try
                            Console.Write("Getting files ... ")
                            transferResult = session.GetFiles(
                                String.Format(
                                    "/{0}/out/*.csv",
                                    remoteFolder
                                ),
                                String.Format(
                                    "{0}\",
                                    indir.FullName
                                ),
                                String.Compare(remoteFolder, "LIVE", True) = 0,
                                transferOptions
                           )

                            ' Throw on any error
                            transferResult.Check()

                            Console.WriteLine("OK")
                            Console.WriteLine(
                                String.Format(
                                    " {0} file(s) downloaded.",
                                    transferResult.Transfers.Count
                                )
                            )
                            Console.WriteLine("")

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
                                        Console.WriteLine(
                                            String.Format(
                                                "{0} exists in save folder. Deleting.",
                                                moveto.Name
                                            )
                                        )
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
                            Console.WriteLine("FAILURE")
                            Console.WriteLine(ex.Message)

                        End Try

                End Select

            End Using

        End If

#End Region

        args.wait()

    End Sub

End Module
