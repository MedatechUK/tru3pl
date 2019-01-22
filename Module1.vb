Imports System.Data.SqlClient
Imports System.IO

Module Module1

    Private cnstring As String = "Integrated Security=true;Initial Catalog=wlnd;Server=walrus\PRI"

    Sub Main()
        Using cn As New SqlConnection(cnstring)
            cn.Open()
            Using cmd As New SqlCommand(
                        "SELECT * from v3pl_part",
                        cn
                    )
                Using r As SqlDataReader = cmd.ExecuteReader

                    Using sku As New OutboundSKU(r)
                        While r.Read
                            sku.write(r)

                        End While

                    End Using

                End Using

            End Using

        End Using

    End Sub

End Module
