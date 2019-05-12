Imports System.Data.SqlClient
Imports System.IO
Imports System.Net.Mail
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

        args = New clArg(arg)
        Try

            Throw New Exception("Testing email notification")

#Region "Handle command line."

            For Each k As String In args.Keys
                Select Case k.ToLower
                    Case "?", "help"
                        args.syntax()
                        args.wait()

                    Case "dir"
                        basedir = New DirectoryInfo(args(k))

                    Case "live"
                        remoteFolder = "live"

                End Select

            Next

            If args.Keys.Contains("all") Then
                With args.Keys
                    If Not .Contains("so") Then args.Add("so", "")
                    If Not .Contains("po") Then args.Add("po", "")
                    If Not .Contains("sku") Then args.Add("sku", "")

                End With
            End If

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
                args.line("Opening {0} database", remoteFolder.ToUpper)

                Select Case remoteFolder.ToLower
                    Case "live"
                        cn = New SqlConnection(My.Settings.cnstrLive)
                        cn.Open()

                        cn2 = New SqlConnection(My.Settings.cnstrLive)
                        cn2.Open()

                        cn3 = New SqlConnection(My.Settings.cnstrLive)
                        cn3.Open()

                    Case Else
                        cn = New SqlConnection(My.Settings.cnstr)
                        cn.Open()

                        cn2 = New SqlConnection(My.Settings.cnstr)
                        cn2.Open()

                        cn3 = New SqlConnection(My.Settings.cnstr)
                        cn3.Open()

                End Select

                args.Colourise(ConsoleColor.Green, "OK")
                Console.WriteLine()

                args.line("Increment Run Number")
                Try
                    Dim inc As New SqlCommand("exec [dbo].[sp_Inc3pl]", cn)
                    Dim val As New SqlCommand("select [dbo].[sp_3plInc]()", cn2)

                    inc.ExecuteNonQuery()
                    args.Colourise(ConsoleColor.Green, val.ExecuteScalar)


                Catch ex As Exception
                    With args
                        .Colourise(ConsoleColor.Red, "FAILURE")
                        .Log(ex.Message)
                        .wait()

                    End With

                End Try


            Catch ex As Exception
                With args
                    .Colourise(ConsoleColor.Red, "FAILURE")
                    .Log(ex.Message)
                    .wait()

                End With

            End Try

#End Region

#Region "Out"

            Dim ftpOnly As Boolean = True

            If args.Keys.Contains("sku") Then
                ftpOnly = False

                Using sku As New OutboundSKU()
                    Using r As SqlDataReader = sku.cmd.ExecuteReader()
                        If r.HasRows Then
                            args.Colourise(ConsoleColor.Green, "OK")
                            sku.ProgressBar()

                            Using sw As New StreamWriter(Path.Combine(outdir.FullName, sku.FileStr))
                                Dim m As Dictionary(Of Integer, Integer) = sku.CreateMap(r)
                                While r.Read
                                    sku.write(sw, m, r)
                                    If Not args.Keys.Contains("csv") Then sku.update(r(0)).ExecuteNonQuery()

                                End While

                            End Using

                        Else
                            args.Colourise(ConsoleColor.Red, "No Rows.")

                        End If

                    End Using

                End Using

            End If

            If args.Keys.Contains("po") Then
                ftpOnly = False

                Using PO As New OutboundPO()
                    Dim sl As Dictionary(Of Integer, Integer) = Nothing
                    Using r As SqlDataReader = PO.cmd(args("po")).ExecuteReader()
                        If r.HasRows Then
                            args.Colourise(ConsoleColor.Green, "OK")
                            PO.ProgressBar(args("po"))

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
                                                If Not args.Keys.Contains("csv") Then POi.update(q(0)).ExecuteNonQuery()

                                            End While

                                        End Using

                                    End Using

                                End While

                            End Using

                        Else
                            args.Colourise(ConsoleColor.Red, "No Rows.")

                        End If

                    End Using

                End Using

            End If

            If args.Keys.Contains("so") Then
                ftpOnly = False

                Using SO As New OutboundSO()
                    Dim sl As Dictionary(Of Integer, Integer) = Nothing
                    Using r As SqlDataReader = SO.cmd(args("so")).ExecuteReader()
                        If r.HasRows Then
                            args.Colourise(ConsoleColor.Green, "OK")
                            SO.ProgressBar(args("so"))

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
                                                If Not args.Keys.Contains("csv") Then
                                                    SOi.update(q(0)).ExecuteNonQuery()

                                                End If
                                                SOi.write(sw, sl, q)

                                            End While

                                        End Using

                                    End Using

                                End While

                            End Using

                        Else
                            args.Colourise(ConsoleColor.Red, "No Rows.")

                        End If

                    End Using

                End Using

                If Not args.Keys.Contains("csv") Then
                    Dim clean As New SqlCommand("exec dbo.[3plMarkSent] 'SO'", cn)
                    clean.ExecuteNonQuery()

                End If

            End If

            If args.Keys.Contains("wt") Then
                ftpOnly = False

                If args("wt").Length = 0 Then
                    Console.WriteLine("Missing -wt [#document].")
                    args.wait()

                End If

                Using WT As New OutboundWT(args("wt"))
                    Dim sl As Dictionary(Of Integer, Integer) = Nothing
                    Using r As SqlDataReader = WT.cmd.ExecuteReader()
                        If r.HasRows Then
                            args.Colourise(ConsoleColor.Green, "OK")
                            WT.ProgressBar()

                            Using sw As New StreamWriter(Path.Combine(outdir.FullName, WT.FileStr))
                                Dim m As Dictionary(Of Integer, Integer) = WT.CreateMap(r)

                                While r.Read
                                    WT.write(sw, m, r)

                                    Using WTi As New OutboundWTItems(r(0))
                                        Using q As SqlDataReader = WTi.cmd.ExecuteReader()
                                            If sl Is Nothing Then
                                                sl = WTi.CreateMap(q)
                                            End If

                                            While q.Read
                                                WTi.write(sw, sl, q)

                                            End While

                                        End Using

                                    End Using

                                End While

                            End Using

                        Else
                            args.Colourise(ConsoleColor.Red, "No Rows.")

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

                    If Not ftpOnly Then Console.WriteLine("")

                    Try
                        args.line(
                            "Opening {0}",
                            sessionOptions.HostName
                        )

                        ' Connect
                        session.Open(sessionOptions)
                        args.Colourise(ConsoleColor.Green, "OK")

                    Catch ex As Exception
                        args.Colourise(ConsoleColor.Red, "FAILED")
                        Console.WriteLine(ex.Message)
                        args.Log(ex.Message)
                        args.wait()

                    End Try

                    Select Case args("ftp").ToLower
                        Case "", "out"

                            Console.WriteLine("")

                            ' Send outbound files                        
                            If outdir.GetFiles("*.txt").Count > 0 Then
                                args.line(
                                    "Sending {0} files",
                                    outdir.GetFiles("*.txt").Count.ToString
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
                                    args.Colourise(ConsoleColor.Green, "OK")

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
                                    args.Colourise(ConsoleColor.Red, "FAILED")
                                    Console.WriteLine(ex.Message)
                                    args.Log(ex.Message)
                                    args.wait()

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
                                            Console.Write(
                                                String.Format(
                                                    " {0} exists in save folder. ",
                                                    moveto.Name
                                                )
                                            )
                                            args.Colourise(ConsoleColor.Red, "Deleting.")
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
                                        Console.WriteLine(ex.Message)
                                        args.Log(ex.Message)
                                        args.wait()

                                    End Try

                                Next

                            End If

                            ' Get inbound files
                            Try
                                args.line("Getting files")
                                transferResult = session.GetFiles(
                                    String.Format(
                                        "/{0}/out/*.csv",
                                        remoteFolder
                                    ),
                                    String.Format(
                                        "{0}\",
                                        indir.FullName
                                    ),
                                    String.Compare(remoteFolder, "LIVE", True) = 0 _
                                    Or args.Keys.Contains("del"),
                                    transferOptions
                               )

                                ' Throw on any error
                                transferResult.Check()
                                args.Colourise(ConsoleColor.Green, "OK")

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
                                            Console.Write(
                                                String.Format(
                                                    "{0} exists in save folder. ",
                                                    moveto.Name
                                                )
                                            )
                                            args.Colourise(ConsoleColor.Red, "Deleting.")
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
                                        Console.WriteLine(ex.Message)
                                        args.Log(ex.Message)

                                    End Try

                                Next

                            Catch ex As Exception
                                args.Colourise(ConsoleColor.Red, "FAILURE")
                                Console.WriteLine(ex.Message)
                                args.Log(ex.Message)
                                args.wait()

                            End Try

                    End Select

                End Using

            End If

#End Region

        Catch ex As Exception
            Console.WriteLine(ex.Message)
            args.Log(ex.Message)

            Dim erMail As New MailMessage("3pl@trutex.com", "hbradley@trutex.com")
            With erMail
                With .CC
                    .Add("wbriggs@trutex.com")
                    '.Add("si@medatechuk.com")
                End With
                .Subject = "3pl runtime error."
                .Body = ex.Message

                Using c As New SmtpClient("mail.trutex.com")
                    c.Send(erMail)

                End Using
            End With

        Finally
            args.wait()

        End Try

    End Sub

End Module
