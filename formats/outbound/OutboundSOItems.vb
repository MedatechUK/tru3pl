Imports System.Data.SqlClient

Public Class OutboundSOItems : Inherits Upload

    Private _ord As Integer
    Private _duedate As Integer

    Sub New(ORD As Integer, duedate As Integer)
        _ord = ORD
        _duedate = duedate
        With Me
            .Add("Record_Type", 0)
            .Add("Merge_Action", 1)
            .Add("Order_Id", 2)
            .Add("Line_Id", 3)
            .Add("Sku_Id", 4)
            .Add("Config_Id", 5)
            .Add("Tracking_Level", 6)
            .Add("Batch_Id", 7)
            .Add("Batch_Mixing", 8)
            .Add("Condition_Id", 9)
            .Add("Lock_Code", 10)
            .Add("Qty_Ordered", 11)
            .Add("Allocate", 12)
            .Add("Back_Ordered", 13)
            .Add("Deallocate", 14)
            .Add("Kit_Split", 15)
            .Add("Origin_Id", 16)
            .Add("Notes", 17)
            .Add("Customer_Sku_Id", 18)
            .Add("Shelf_Life_Days", 19)
            .Add("Shelf_Life_Percent", 20)
            .Add("Client_Id", 21)
            .Add("Disallow_Merge_Rules", 22)
            .Add("User_Def_Type_1", 23)
            .Add("User_Def_Type_2", 24)
            .Add("User_Def_Type_3", 25)
            .Add("User_Def_Type_4", 26)
            .Add("User_Def_Type_5", 27)
            .Add("User_Def_Type_6", 28)
            .Add("User_Def_Type_7", 29)
            .Add("User_Def_Type_8", 30)
            .Add("User_Def_Chk_1", 31)
            .Add("User_Def_Chk_2", 32)
            .Add("User_Def_Chk_3", 33)
            .Add("User_Def_Chk_4", 34)
            .Add("User_Def_Date_1", 35)
            .Add("User_Def_Date_2", 36)
            .Add("User_Def_Date_3", 37)
            .Add("User_Def_Date_4", 38)
            .Add("User_Def_Num_1", 39)
            .Add("User_Def_Num_2", 40)
            .Add("User_Def_Num_3", 41)
            .Add("User_Def_Num_4", 42)
            .Add("User_Def_Note_1", 43)
            .Add("User_Def_Note_2", 44)
            .Add("Soh_Id", 45)
            .Add("Line_Value", 46)
            .Add("Time_Zone_Name", 47)
            .Add("Spec_Code", 48)
            .Add("Rule_Id", 49)
            .Add("Client_Group", 50)
            .Add("Task_Per_Each", 51)
            .Add("Use_Pick_To_Grid", 52)
            .Add("Ignore_Weight_Capture", 53)
            .Add("Host_Order_Id", 54)
            .Add("Host_Line_Id", 55)
            .Add("Stage_Route_Id", 56)
            .Add("Min_Qty_Ordered", 57)
            .Add("Max_Qty_Ordered", 58)
            .Add("Expected_Value", 59)
            .Add("Expected_Volume", 60)
            .Add("Expected_Weight", 61)
            .Add("Customer_Sku_Desc1", 62)
            .Add("Customer_Sku_Desc2", 63)
            .Add("Purchase_Order", 64)
            .Add("Product_Price", 65)
            .Add("Product_Currency", 66)
            .Add("Documentation_Unit", 67)
            .Add("Extended_Price", 68)
            .Add("Tax_1", 69)
            .Add("Tax_2", 70)
            .Add("Documentation_Text_1", 71)
            .Add("Serial_Number", 72)
            .Add("Owner_Id", 73)
            .Add("Collective_Mode", 74)
            .Add("Ce_Receipt_Type", 75)
            .Add("Ce_Coo", 76)
            .Add("Kit_Plan_Id", 77)
            .Add("Location_Id", 78)
            .Add("Nls_Calendar", 79)
        End With
    End Sub

    Public Overrides ReadOnly Property cmd As SqlCommand
        Get
            Dim ret = New SqlCommand(
                String.Format("SELECT * from v3pl_soi({0}, {1})", _ord, _duedate), cn2
            )
            ret.CommandTimeout = 500
            Return ret
        End Get
    End Property

    Public Overrides ReadOnly Property update(ParamArray keys() As Integer) As SqlCommand
        Get
            Dim ret = New SqlCommand(
                String.Format(
                    "UPDATE ORDERITEMS SET ZTRX_3PLSENT = 'Y' WHERE ORD= {0} AND ORDI= {1}",
                    _ord,
                    keys(0)
                ), cn3
            )
            ret.CommandTimeout = 500
            Return ret
        End Get

    End Property

End Class
