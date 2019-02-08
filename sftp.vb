Imports WinSCP

Module sftp

    Sub main2()


        Try
            ' Setup session options
            Dim sessionOptions As New SessionOptions
            With sessionOptions
                .Protocol = Protocol.Sftp
                .HostName = "secureftp.torque.eu"
                .UserName = "ftptrutex"
                .Password = "xkazW09H"
                .SshHostKeyFingerprint = "ssh-rsa 2048 53:a0:ba:88:57:32:c8:7b:33:ac:6d:4a:e5:35:23:e2"
            End With

            Using session As New Session
                ' Connect
                session.Open(sessionOptions)

                ' Upload files
                Dim transferOptions As New TransferOptions
                transferOptions.TransferMode = TransferMode.Binary


                Dim transferResult As TransferOperationResult
                transferResult =
                    session.GetFiles("/test/out/*.csv", "E:\priority\system\3pl\in\", False, transferOptions)

                'transferResult =
                '    session.PutFiles("d:\toupload\*", "/home/user/", False, transferOptions)

                ' Throw on any error
                transferResult.Check()

                ' Print results
                For Each transfer In transferResult.Transfers
                    Console.WriteLine("Download of {0} succeeded", transfer.FileName)
                Next

            End Using


        Catch e As Exception
            Console.WriteLine("Error: {0}", e)

        End Try

    End Sub

End Module
