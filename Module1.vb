Imports System.Data.SqlClient
Imports System.IO

Module Module1

    Public cn As New SqlConnection("Integrated Security=true;Initial Catalog=wlnd;Server=walrus\PRI")
    Public cn2 As New SqlConnection("Integrated Security=true;Initial Catalog=wlnd;Server=walrus\PRI")

    Sub Main()

        cn.Open()
        cn2.Open()

        Using sku As New OutboundSKU()
            Using sw As New StreamWriter(Path.Combine("d:\", sku.FileStr))
                Using r As SqlDataReader = sku.cmd.ExecuteReader()

                    Dim m As Dictionary(Of Integer, Integer) = sku.CreateMap(r)

                    While r.Read
                        sku.write(sw, m, r)

                    End While

                End Using

            End Using

        End Using

        Using PO As New OutboundPO()
            Dim sl As Dictionary(Of Integer, Integer) = Nothing

            Using sw As New StreamWriter(Path.Combine("d:\", PO.FileStr))
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

    End Sub

End Module
