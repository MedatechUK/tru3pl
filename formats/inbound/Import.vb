Imports System.Data.SqlClient
Imports System.IO
Imports System.Text

Public Class Import

    Private Cur As cursorloc
    Public Cols As New Dictionary(Of Integer, importCol)

    Sub New()

        With Cols
            .Add(0, New importCol("RECORD_TYPE", "CHAR"))
            .Add(1, New importCol("ACTION", "CHAR"))
            .Add(2, New importCol("CODE", "CHAR"))
            .Add(3, New importCol("ORIGINAL_QUANTITY_SI", "CHAR"))
            .Add(4, New importCol("ORIGINAL_QTY", "REAL"))
            .Add(5, New importCol("UPDATE_QUANTITY_SIGN", "CHAR"))
            .Add(6, New importCol("UPDATE_QTY", "REAL"))
            .Add(7, New importCol("DSTAMP", "DATE"))
            .Add(8, New importCol("CLIENT_ID", "CHAR"))
            .Add(9, New importCol("SKU_ID", "CHAR"))
            .Add(10, New importCol("FROM_LOC_ID", "CHAR"))
            .Add(11, New importCol("TO_LOC_ID", "CHAR"))
            .Add(12, New importCol("FINAL_LOC_ID", "CHAR"))
            .Add(13, New importCol("TAG_ID", "CHAR"))
            .Add(14, New importCol("REFERENCE_ID", "CHAR"))
            .Add(15, New importCol("LINE_ID", "INT"))
            .Add(16, New importCol("CONDITION_ID", "CHAR"))
            .Add(17, New importCol("NOTES", "CHAR"))
            .Add(18, New importCol("REASON_ID", "CHAR"))
            .Add(19, New importCol("BATCH_ID", "CHAR"))
            .Add(20, New importCol("EXPIRY_DSTAMP", "DATE"))
            .Add(21, New importCol("USER_ID", "CHAR"))
            .Add(22, New importCol("SHIFT", "CHAR"))
            .Add(23, New importCol("STATION_ID", "CHAR"))
            .Add(24, New importCol("SITE_ID", "CHAR"))
            .Add(25, New importCol("CONTAINER_ID", "CHAR"))
            .Add(26, New importCol("PALLET_ID", "CHAR"))
            .Add(27, New importCol("LIST_ID", "CHAR"))
            .Add(28, New importCol("OWNER_ID", "CHAR"))
            .Add(29, New importCol("ORIGIN_ID", "CHAR"))
            .Add(30, New importCol("WORK_GROUP", "CHAR"))
            .Add(31, New importCol("CONSIGNMENT", "CHAR"))
            .Add(32, New importCol("MANUF_DSTAMP", "DATE"))
            .Add(33, New importCol("LOCK_STATUS", "CHAR"))
            .Add(34, New importCol("QC_STATUS", "CHAR"))
            .Add(35, New importCol("SESSION_TYPE", "CHAR"))
            .Add(36, New importCol("SUMMARY_RECORD", "CHAR"))
            .Add(37, New importCol("ELAPSED_TIME", "INT"))
            .Add(38, New importCol("SUPPLIER_ID", "CHAR"))
            .Add(39, New importCol("USER_DEF_TYPE_1", "CHAR"))
            .Add(40, New importCol("USER_DEF_TYPE_2", "CHAR"))
            .Add(41, New importCol("USER_DEF_TYPE_3", "CHAR"))
            .Add(42, New importCol("USER_DEF_TYPE_4", "CHAR"))
            .Add(43, New importCol("USER_DEF_TYPE_5", "CHAR"))
            .Add(44, New importCol("USER_DEF_TYPE_6", "CHAR"))
            .Add(45, New importCol("USER_DEF_TYPE_7", "CHAR"))
            .Add(46, New importCol("USER_DEF_TYPE_8", "CHAR"))
            .Add(47, New importCol("USER_DEF_CHK_1", "CHAR"))
            .Add(48, New importCol("USER_DEF_CHK_2", "CHAR"))
            .Add(49, New importCol("USER_DEF_CHK_3", "CHAR"))
            .Add(50, New importCol("USER_DEF_CHK_4", "CHAR"))
            .Add(51, New importCol("USER_DEF_DATE_1", "DATE"))
            .Add(52, New importCol("USER_DEF_DATE_2", "DATE"))
            .Add(53, New importCol("USER_DEF_DATE_3", "DATE"))
            .Add(54, New importCol("USER_DEF_DATE_4", "DATE"))
            .Add(55, New importCol("USER_DEF_NUM_1", "REAL"))
            .Add(56, New importCol("USER_DEF_NUM_2", "REAL"))
            .Add(57, New importCol("USER_DEF_NUM_3", "REAL"))
            .Add(58, New importCol("USER_DEF_NUM_4", "REAL"))
            .Add(59, New importCol("USER_DEF_NOTE_1", "CHAR"))
            .Add(60, New importCol("USER_DEF_NOTE_2", "CHAR"))
            .Add(61, New importCol("FROM_SITE_ID", "CHAR"))
            .Add(62, New importCol("TO_SITE_ID", "CHAR"))
            .Add(63, New importCol("TIME_ZONE_NAME", "CHAR"))
            .Add(64, New importCol("JOB_ID", "CHAR"))
            .Add(65, New importCol("JOB_UNIT", "CHAR"))
            .Add(66, New importCol("MANNING", "INT"))
            .Add(67, New importCol("SPEC_CODE", "CHAR"))
            .Add(68, New importCol("CONFIG_ID", "CHAR"))
            .Add(69, New importCol("ESTIMATED_TIME", "REAL"))
            .Add(70, New importCol("TASK_CATEGORY", "INT"))
            .Add(71, New importCol("SAMPLING_TYPE", "CHAR"))
            .Add(72, New importCol("COMPLETE_DSTAMP", "DATE"))
            .Add(73, New importCol("GRN", "INT"))
            .Add(74, New importCol("GROUP_ID", "CHAR"))
            .Add(75, New importCol("UPLOADED", "CHAR"))
            .Add(76, New importCol("UPLOADED_VVIEW", "CHAR"))
            .Add(77, New importCol("UPLOADED_AB", "CHAR"))
            .Add(78, New importCol("SAP_IDOC_TYPE", "CHAR"))
            .Add(79, New importCol("SAP_TID", "CHAR"))
            .Add(80, New importCol("CE_ORIG_ROTATION_ID", "CHAR"))
            .Add(81, New importCol("CE_ROTATION_ID", "CHAR"))
            .Add(82, New importCol("CE_CONSIGNMENT_ID", "CHAR"))
            .Add(83, New importCol("CE_RECEIPT_TYPE", "CHAR"))
            .Add(84, New importCol("CE_ORIGINATOR", "CHAR"))
            .Add(85, New importCol("CE_ORIGINATOR_REFERENCE", "CHAR"))
            .Add(86, New importCol("CE_COO", "CHAR"))
            .Add(87, New importCol("CE_CWC", "CHAR"))
            .Add(88, New importCol("CE_UCR", "CHAR"))
            .Add(89, New importCol("CE_UNDER_BOND", "CHAR"))
            .Add(90, New importCol("CE_DOCUMENT_DSTAMP", "DATE"))
            .Add(91, New importCol("UPLOADED_CUSTOMS", "CHAR"))
            .Add(92, New importCol("UPLOADED_LABOR", "CHAR"))
            .Add(93, New importCol("ASN_ID", "CHAR"))
            .Add(94, New importCol("CUSTOMER_ID", "CHAR"))
            .Add(95, New importCol("PRINT_LABEL_ID", "INT"))
            .Add(96, New importCol("LOCK_CODE", "CHAR"))
            .Add(97, New importCol("SHIP_DOCK", "CHAR"))
            .Add(98, New importCol("CE_DUTY_STAMP", "CHAR"))
            .Add(99, New importCol("PALLET_GROUPED", "CHAR"))

        End With

    End Sub

    Sub import(infn As FileInfo)

        Dim ln As Integer = 0

        Console.Write(String.Format("Importing file {0} ... ", infn.Name))
        Using sr As New StreamReader(infn.FullName)
            Cur = New cursorloc(sr.BaseStream.Length)

            While Not sr.EndOfStream

                ln += 1

                Dim s As String = sr.ReadLine()
                Cur.current += (s.Length + 2)
                Dim str() As String = s.Split(",")

                Dim val As New Dictionary(Of String, String)
                For i As Integer = 0 To UBound(str)
                    If Len(str(i)) > 0 Then
                        Select Case Cols(i).Type.ToUpper
                            Case "CHAR"
                                val.Add(Cols(i).Name, String.Format("'{0}'", str(i)))

                            Case "INT", "REAL"
                                val.Add(Cols(i).Name, String.Format("{0}", str(i)))

                            Case "DATE"
                                Dim dt As New DateTime(
                                    CInt(str(i).Substring(0, 4)),
                                    CInt(str(i).Substring(4, 2)),
                                    CInt(str(i).Substring(6, 2)),
                                    CInt(str(i).Substring(8, 2)),
                                    CInt(str(i).Substring(10, 2)),
                                    CInt(str(i).Substring(12, 2))
                                )
                                val.Add(Cols(i).Name, String.Format("{0}", DateDiff(DateInterval.Minute, #1/1/1988#, dt).ToString))

                        End Select

                    End If
                Next

                Dim colstr As New StringBuilder
                Dim valstr As New StringBuilder
                For Each col As String In val.Keys
                    colstr.AppendFormat("{0}", col)
                    valstr.AppendFormat("{0}", val(col))
                    If Not (val.Keys.Last = col) Then
                        colstr.Append(", ")
                        valstr.Append(", ")
                    End If
                Next

                Dim cmd As New SqlCommand(
                    String.Format(
                        "insert into ZTRX_3PL_LOAD (FILENAME, LINE, {0}) values ('{1}', {2}, {3})",
                        colstr.ToString,
                        infn.Name,
                        ln,
                        valstr.ToString
                    ), cn
                )

                Try
                    'Console.Write(cmd.CommandText)
                    cmd.ExecuteNonQuery()

                Catch ex As Exception
                    Console.Write(ex.Message)

                End Try

            End While

            Dim loaded As New SqlCommand(
                String.Format(
                    "update ZTRX_3PL_LOAD set LOADED = 'Y' where FILENAME = '{0}'",
                    infn.Name
                ), cn
            )
            loaded.ExecuteNonQuery()

        End Using

    End Sub

End Class
