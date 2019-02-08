Imports System.Data.SqlClient

Public Class OutboundPOItems : Inherits Upload

    Private _ord As Integer
    Sub New(ORD As Integer)
        _ord = ORD
        With Me
            .Add("Record_Type", 0)
            .Add("Merge_Action", 1)
            .Add("Pre_Advice_Id", 2)
            .Add("Line_Id", 3)
            .Add("Sku_Id", 4)
            .Add("Config_Id", 5)
            .Add("Batch_Id", 6)
            .Add("Expiry_Dstamp", 7)
            .Add("Pallet_Config", 8)
            .Add("Condition_Id", 9)
            .Add("Qty_Due", 10)
            .Add("Lock_Code", 11)
            .Add("Tag_Id", 12)
            .Add("Origin_Id", 13)
            .Add("Notes", 14)
            .Add("Manuf_Dstamp", 15)
            .Add("Client_Id", 16)
            .Add("Disallow_Merge_Rules", 17)
            .Add("User_Def_Type_1", 18)
            .Add("User_Def_Type_2", 19)
            .Add("User_Def_Type_3", 20)
            .Add("User_Def_Type_4", 21)
            .Add("User_Def_Type_5", 22)
            .Add("User_Def_Type_6", 23)
            .Add("User_Def_Type_7", 24)
            .Add("User_Def_Type_8", 25)
            .Add("User_Def_Chk_1", 26)
            .Add("User_Def_Chk_2", 27)
            .Add("User_Def_Chk_3", 28)
            .Add("User_Def_Chk_4", 29)
            .Add("User_Def_Date_1", 30)
            .Add("User_Def_Date_2", 31)
            .Add("User_Def_Date_3", 32)
            .Add("User_Def_Date_4", 33)
            .Add("User_Def_Num_1", 34)
            .Add("User_Def_Num_2", 35)
            .Add("User_Def_Num_3", 36)
            .Add("User_Def_Num_4", 37)
            .Add("User_Def_Note_1", 38)
            .Add("User_Def_Note_2", 39)
            .Add("Time_Zone_Name", 40)
            .Add("Spec_Code", 41)
            .Add("Client_Group", 42)
            .Add("Tracking_Level", 43)
            .Add("Host_Pre_Advice_Id", 44)
            .Add("Host_Line_Id", 45)
            .Add("Qty_Due_Tolerance", 46)
            .Add("Ce_Coo", 47)
            .Add("Owner_Id", 48)
            .Add("Ce_Consignment_Id", 49)
            .Add("Collective_Mode", 50)
            .Add("Ce_Under_Bond", 51)
            .Add("Ce_Link", 52)
            .Add("Product_Price", 53)
            .Add("Product_Currency", 54)
            .Add("Ce_Invoice_Number", 55)
            .Add("Nls_Calendar", 56)
        End With
    End Sub

    Public Overrides ReadOnly Property cmd As SqlCommand
        Get
            Dim ret = New SqlCommand(
                String.Format("SELECT * from v3pl_poi({0})", _ord), cn2
            )
            ret.CommandTimeout = 500
            Return ret
        End Get

    End Property

End Class
