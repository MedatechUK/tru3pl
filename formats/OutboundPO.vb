Imports System.Data.SqlClient

Public Class OutboundPO : Inherits Upload

    Public Overrides ReadOnly Property FileName As String
        Get
            Return "PO"
        End Get
    End Property

    Public Overrides ReadOnly Property cmd As SqlCommand
        Get
            Return New SqlCommand(
                        "SELECT * from v3pl_po", cn
                    )
        End Get
    End Property

    Sub New()
        With Me
            .Add("Record_Type", 0)
            .Add("Merge_Action", 1)
            .Add("Pre_Advice_Id", 2)
            .Add("Site_Id", 3)
            .Add("Supplier_Id", 4)
            .Add("Status", 5)
            .Add("Bookref_Id", 6)
            .Add("Due_Dstamp", 7)
            .Add("Notes", 8)
            .Add("Owner_Id", 9)
            .Add("Contact", 10)
            .Add("Contact_Phone", 11)
            .Add("Contact_Fax", 12)
            .Add("Contact_Email", 13)
            .Add("Name", 14)
            .Add("Address1", 15)
            .Add("Address2", 16)
            .Add("Town", 17)
            .Add("County", 18)
            .Add("Postcode", 19)
            .Add("Country", 20)
            .Add("Pre_Advice_Type", 21)
            .Add("Sampling_Type", 22)
            .Add("Return_Flag", 23)
            .Add("Returned_Order_Id", 24)
            .Add("Collection_Reqd", 25)
            .Add("Consignment", 26)
            .Add("Load_Sequence", 27)
            .Add("Email_Confirm", 28)
            .Add("Client_Id", 29)
            .Add("Disallow_Merge_Rules", 30)
            .Add("User_Def_Type_1", 31)
            .Add("User_Def_Type_2", 32)
            .Add("User_Def_Type_3", 33)
            .Add("User_Def_Type_4", 34)
            .Add("User_Def_Type_5", 35)
            .Add("User_Def_Type_6", 36)
            .Add("User_Def_Type_7", 37)
            .Add("User_Def_Type_8", 38)
            .Add("User_Def_Chk_1", 39)
            .Add("User_Def_Chk_2", 40)
            .Add("User_Def_Chk_3", 41)
            .Add("User_Def_Chk_4", 42)
            .Add("User_Def_Date_1", 43)
            .Add("User_Def_Date_2", 44)
            .Add("User_Def_Date_3", 45)
            .Add("User_Def_Date_4", 46)
            .Add("User_Def_Num_1", 47)
            .Add("User_Def_Num_2", 48)
            .Add("User_Def_Num_3", 49)
            .Add("User_Def_Num_4", 50)
            .Add("User_Def_Note_1", 51)
            .Add("User_Def_Note_2", 52)
            .Add("Time_Zone_Name", 53)
            .Add("Disallow_Replens", 54)
            .Add("Client_Group", 55)
            .Add("Supplier_Reference", 56)
            .Add("Carrier_Name", 57)
            .Add("Carrier_Reference", 58)
            .Add("Tod", 59)
            .Add("Tod_Place", 60)
            .Add("Mode_Of_Transport", 61)
            .Add("Vat_Number", 62)
            .Add("Yard_Container_Type", 63)
            .Add("Yard_Container_Id", 64)
            .Add("Ce_Consignment_Id", 65)
            .Add("Collective_Mode", 66)
            .Add("Contact_Mobile", 67)
            .Add("Master_Pre_Advice", 68)
            .Add("Ce_Invoice_Number", 69)
            .Add("Nls_Calendar", 70)


        End With

    End Sub
End Class
