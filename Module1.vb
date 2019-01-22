Imports System.Data.SqlClient
Imports System.IO

Module Module1

    Public cn As New SqlConnection("Integrated Security=true;Initial Catalog=wlnd;Server=walrus\PRI")

    Sub Main()

        cn.Open()

        Using sku As New OutboundSKU()
            Using sw As New StreamWriter(sku.FileStr)
                Using r As SqlDataReader = sku.cmd.ExecuteReader()

                    sku.CreateMap(r)

                    While r.Read
                        sku.write(sw, r)

                    End While

                End Using

            End Using

        End Using

        Using PO As New OutboundPO()
            Using sw As New StreamWriter(PO.FileStr)
                Using r As SqlDataReader = PO.cmd.ExecuteReader()

                    PO.CreateMap(r)

                    While r.Read
                        PO.write(sw, r)

                    End While

                End Using

            End Using

        End Using

    End Sub

End Module
