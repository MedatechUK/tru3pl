
GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_CPROFTYPE]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[v3pl_CPROFTYPE] ()
RETURNS 
@tbl TABLE 
(
	-- Add the column definitions for the TABLE variable here
	CPROFTYPE int
)
AS
BEGIN
	-- Fill the table variable with the rows for your result set
	
	insert into @tbl
	select 0
	union all
	select CPROFTYPE from CPROFTYPES where ZTR_3PLSEND = 'Y'
	RETURN 
END

GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_CustCheck]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Si
-- Create date: 06/02/19
-- Description:	Filter Customer form 3pl SO
-- =============================================
CREATE FUNCTION [dbo].[v3pl_CustCheck]
(
	-- Add the parameters for the function here
	@CUST BIGINT
)
RETURNS bit
AS
BEGIN
	return 1

END

GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_OrdType]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Si
-- Create date: 08/02/19
-- Description:	3PL order type
-- =============================================
CREATE FUNCTION [dbo].[v3pl_OrdType]
(
	-- Add the parameters for the function here
	@CUST BIGINT
)
RETURNS VARCHAR(5)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar varchar(5)
	DECLARE @CUSTNAME VARCHAR(25)
	DECLARE @CUSTTYPE VARCHAR(25)
	
	SELECT        
		@CUSTNAME = dbo.CUSTOMERS.CUSTNAME, 
		@CUSTTYPE = dbo.CTYPE.CTYPECODE
	
	FROM
		dbo.CUSTOMERS INNER JOIN
        dbo.CTYPE ON dbo.CUSTOMERS.CTYPE = dbo.CTYPE.CTYPE
	
	WHERE CUSTOMERS.CUST = @CUST

	if exists(
		select * from CUSTOMERS
		where 0=0
			and CUSTNAME in ('NRT0003073','418062','NRT0003222')
			and CUST = @CUST
		)
		begin
			set @ResultVar = 'TTWHO'

		END

	ELSE
		BEGIN
			SELECT @ResultVar =		
				case 
					when (@CUSTTYPE = 'TD') then 'TTWEB'
					when (@CUSTTYPE = 'RS') then 'TTOUT'
					when (@CUSTTYPE = 'DS') then 'TTOUT'
					when (@CUSTTYPE = 'RF') then 'TTEXP'
					when (@CUSTTYPE = 'DF') then 'TTEXP'
					else ''
				end		

		END
		
	-- Return the result of the function
	RETURN @ResultVar

END

GO

/****** Object:  View [dbo].[v3pl_3plSKU]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v3pl_3plSKU]
AS
SELECT        dbo.PART.PART
FROM            dbo.PARTSPEC INNER JOIN
                         dbo.PART ON dbo.PARTSPEC.PART = dbo.PART.PART INNER JOIN
                         dbo.PARTSTATS ON dbo.PART.PARTSTAT = dbo.PARTSTATS.PARTSTAT
WHERE        (0 = 0) AND (dbo.PART.PART > 0) AND (dbo.PARTSTATS.SELLFLAG = 'Y') AND (dbo.PARTSPEC.SPEC14 IN
                             (SELECT        RGNAME
                               FROM            dbo.ZTRX_3PLSENDRG
                               WHERE        (SEND = 'Y'))) AND (dbo.PARTSTATS.INACTIVEFLAG <> 'Y')

GO
/****** Object:  View [dbo].[v_3pl_PorderItems]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_3pl_PorderItems]
AS
SELECT dbo.PORDERITEMS.ORDI, 
       dbo.IMPFILES.IMPFILE, 
       dbo.IMPFILES.IMPFNUM, 
       dbo.PORDERITEMS.DUEDATE, 
       dbo.SUPPLIERS.SUPDES, 
       dbo.SUPPLIERS.ADDRESS, 
       dbo.SUPPLIERS.EMG_ADDRESSB, 
       dbo.SUPPLIERS.EMG_ADDRESSC, 
       dbo.COUNTRIES.COUNTRYCODE, 
       dbo.PORDERITEMSA.ZTRX_EXPECTEDQUANT / 1000 AS ZTRX_EXPECTEDQUANT, 
       dbo.PART.PARTNAME, 
       dbo.PORDERITEMS.KLINE, 
       dbo.SUPPLIERS.SUPNAME, 
       dbo.PORDERS.ORDNAME 
FROM   dbo.COUNTRIES 
       INNER JOIN dbo.SUPPLIERS 
                  INNER JOIN dbo.PORDERITEMSA 
                             INNER JOIN dbo.IMPFILES 
                                     ON dbo.PORDERITEMSA.ZTRX_IMPFILE = 
                                        dbo.IMPFILES.IMPFILE 
                             INNER JOIN dbo.PORDISTATUSES 
                                     ON dbo.PORDERITEMSA.PORDISTATUS = 
                                        dbo.PORDISTATUSES.PORDISTATUS 
                             INNER JOIN dbo.PORDERS 
                                        INNER JOIN dbo.PORDERITEMS 
                                                ON dbo.PORDERS.ORD = 
                                                   dbo.PORDERITEMS.ORD 
                                        INNER JOIN dbo.PART 
                                                ON dbo.PORDERITEMS.PART = 
                                                   dbo.PART.PART 
                                     ON dbo.PORDERITEMSA.ORDI = 
                                        dbo.PORDERITEMS.ORDI 
                          ON dbo.SUPPLIERS.SUP = dbo.PORDERS.SUP 
                  INNER JOIN dbo.PORDSTATS 
                          ON dbo.PORDERS.PORDSTAT = dbo.PORDSTATS.PORDSTAT 
               ON dbo.COUNTRIES.COUNTRY = dbo.SUPPLIERS.COUNTRY 
WHERE  ( 0 = 0 ) 
       AND ( dbo.PORDISTATUSES.PORDISTATUSDES IN ( 'In Transit' ) ) 
       AND ( dbo.PORDERS.CLOSED = '' ) 
       AND ( dbo.PORDERS.ORD > 0 ) 
       AND ( dbo.PORDISTATUSES.PORDISTATUSDES IN ( 'In Transit' ) ) 
       AND ( dbo.PORDERS.CLOSED = '' ) 
       AND ( dbo.IMPFILES.IMPFILE > 0 ) 
       AND ( dbo.PORDERITEMSA.ZTRX_EXPECTEDQUANT > 0 ) 
       AND ( dbo.PART.PART IN (SELECT PART 
                               FROM   dbo.v3pl_3plSKU AS v3pl_SKU_1) ) 
       AND ( dbo.PORDERITEMS.ZTRX_3PLSENT <> 'Y' ) 

GO
/****** Object:  View [dbo].[v3pl_CarrigePart]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v3pl_CarrigePart]
AS
SELECT        dbo.PART.PART, dbo.PART.PARTNAME
FROM            dbo.PART INNER JOIN
                         dbo.PARTSPEC ON dbo.PART.PART = dbo.PARTSPEC.PART
WHERE        (dbo.PARTSPEC.SPEC14 = 'CARR')

GO
/****** Object:  View [dbo].[v3pl_ServiceType]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v3pl_ServiceType]
AS
SELECT dbo.ORDERITEMS.ORD, 
       CASE 
         WHEN dbo.ORDERITEMS.PART = 2336 THEN 'Std' 
         WHEN dbo.ORDERITEMS.PART = 327355 THEN 'Std' 
         WHEN dbo.ORDERITEMS.PART = 108639 THEN 'Std' 
         WHEN dbo.ORDERITEMS.PART = 761124 THEN 'Express' 
         WHEN dbo.ORDERITEMS.PART = 108642 THEN 'Express' 
         WHEN dbo.ORDERITEMS.PART = 761123 THEN 'Express' 
         WHEN dbo.ORDERITEMS.PART = 763995 THEN 'Saturday' 
         WHEN dbo.ORDERITEMS.PART = 327406 THEN 'Saturday' 
         ELSE 'Std' 
       END AS Service_Level 
FROM   dbo.ORDERITEMS 
       INNER JOIN dbo.ORDERS 
               ON dbo.ORDERITEMS.ORD = dbo.ORDERS.ORD 
WHERE  ( dbo.ORDERITEMS.PART IN (SELECT PART 
                                 FROM   dbo.v3pl_CarrigePart) ) 
       AND ( dbo.ORDERS.CLOSED = '' ) 

GO
/****** Object:  View [dbo].[v_3pl_SorderItems]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[v_3pl_SorderItems]
AS
SELECT TOP (100) PERCENT dbo.ORDERS.ORDNAME, 
                         dbo.ORDERS.CUST, 
                         dbo.CUSTOMERS.CUSTNAME               AS Customer_ID, 
                         dbo.CUSTOMERS.CUSTDES                AS Name, 
                         dbo.ORDERS.ORD, 
                         dbo.ORDERITEMS.ORDI, 
                         dbo.ORDERITEMS.KLINE, 
                         dbo.ORDERS.CURDATE, 
                         dbo.ORDERITEMS.DUEDATE, 
                         dbo.PART.PARTNAME, 
                         dbo.ORDERITEMS.TBALANCE / 1000       AS QUANT, 
                         CASE 
                           WHEN ( dbo.ORDERS.DESTCODE = 0 ) THEN 
                           CUSTOMERS.ADDRESS 
                           ELSE DESTCODES.ADDRESS 
                         END                                  AS Address1, 
                         CASE 
                           WHEN ( dbo.ORDERS.DESTCODE = 0 ) THEN 
                           CUSTOMERS.EMG_ADDRESSB 
                           ELSE DESTCODES.ADDRESS2 
                         END                                  AS Address2, 
                         CASE 
                           WHEN ( dbo.ORDERS.DESTCODE = 0 ) THEN 
                           CUSTOMERS.EMG_ADDRESSC 
                           ELSE DESTCODES.ADDRESS3 
                         END                                  AS Town, 
                         CASE 
                           WHEN ( dbo.ORDERS.DESTCODE = 0 ) THEN CUSTOMERS.STATE 
                           ELSE DESTCODES.STATE 
                         END                                  AS County, 
                         CASE 
                           WHEN ( dbo.ORDERS.DESTCODE = 0 ) THEN CUSTOMERS.ZIP 
                           ELSE DESTCODES.ZIP 
                         END                                  AS Postcode, 
                         CASE 
                           WHEN ( dbo.ORDERS.DESTCODE = 0 ) THEN 
                           CUSTOMERS.COUNTRY 
                           ELSE DESTCODES.COUNTRY 
                         END                                  AS COUNTRY, 
                         dbo.ORDERS.DETAILS                   AS User_Def_Type_2 
                         , 
                         dbo.ORDERS.ORDNAME 
                         AS User_Def_Type_1, 
                         dbo.ORDERS.REFERENCE, 
                         dbo.PHONEBOOK.NAME                   AS Contact, 
                         dbo.PHONEBOOK.EMAIL                  AS Contact_Email, 
                         CASE 
                           WHEN ( Len(dbo.PHONEBOOK.PHONENUM) = 0 ) THEN 
                           CUSTOMERS.PHONE 
                           ELSE dbo.PHONEBOOK.PHONENUM 
                         END                                  AS Contact_Phone, 
                         dbo.v3pl_OrdType(dbo.CUSTOMERS.CUST) AS Order_Type, 
                         dbo.v3pl_ServiceType.Service_Level, 
                         dbo.CURRENCIES.CODE                  AS 
                         Product_Currency, 
                         dbo.CUSTPART.CUSTPARTNAME            AS Customer_Sku_Id 
                         , 
                         dbo.ORDERITEMS.QPRICE 
                         AS Product_Price, 
                         dbo.ORDERITEMS.ZTRX_RESERVED 
FROM   dbo.CUSTPART 
       RIGHT OUTER JOIN dbo.DESTCODES 
                        INNER JOIN dbo.ORDERITEMS 
                                   INNER JOIN dbo.ORDERS 
                                           ON dbo.ORDERITEMS.ORD = 
                                              dbo.ORDERS.ORD 
                                   INNER JOIN dbo.ORDSTATUS 
                                           ON dbo.ORDERS.ORDSTATUS = 
                                              dbo.ORDSTATUS.ORDSTATUS 
                                   INNER JOIN dbo.PART 
                                           ON dbo.ORDERITEMS.PART = 
                                              dbo.PART.PART 
                                   INNER JOIN dbo.PARTSPEC 
                                           ON dbo.PART.PART = dbo.PARTSPEC.PART 
                                   INNER JOIN dbo.CUSTOMERS 
                                           ON dbo.ORDERS.CUST = 
                                              dbo.CUSTOMERS.CUST 
                                   INNER JOIN dbo.CUSTSTATS 
                                           ON dbo.CUSTOMERS.CUSTSTAT = 
                                              dbo.CUSTSTATS.CUSTSTAT 
                                ON dbo.DESTCODES.DESTCODE = dbo.ORDERS.DESTCODE 
                        INNER JOIN dbo.PHONEBOOK 
                                ON dbo.ORDERS.PHONE = dbo.PHONEBOOK.PHONE 
                        INNER JOIN dbo.v3pl_ServiceType 
                                ON dbo.ORDERS.ORD = dbo.v3pl_ServiceType.ORD 
                        INNER JOIN dbo.CURRENCIES 
                                ON dbo.ORDERITEMS.CURRENCY = 
                                   dbo.CURRENCIES.CURRENCY 
                        INNER JOIN dbo.v3pl_CPROFTYPE() AS v3pl_CPROFTYPE_1 
                                ON dbo.ORDERS.ORDTYPE = 
                                   v3pl_CPROFTYPE_1.CPROFTYPE 
                     ON dbo.CUSTPART.PART = dbo.ORDERITEMS.PART 
                        AND dbo.CUSTPART.CUST = dbo.CUSTOMERS.CUST 
WHERE  ( dbo.ORDERS.CLOSED = '' ) 
       AND ( dbo.ORDERS.ORD > 0 ) 
       AND ( dbo.ORDSTATUS.ORDSTATUSDES IN ( 'Confirmed' ) ) 
       AND ( dbo.ORDERITEMS.QUANT > 0 ) 
       AND ( dbo.v3pl_CustCheck(dbo.ORDERS.CUST) = 1 ) 
       AND ( dbo.CUSTSTATS.RESTRICTEDFLAG = '' ) 
       AND ( dbo.CUSTSTATS.INACTIVE = '' ) 
       AND ( dbo.CUSTSTATS.ZTRX_ONHOLDFLAG = '' ) 
       AND ( dbo.CUSTSTATS.STATDES IN ( 'Active' ) ) 
       AND ( dbo.ORDERS.CUST > 0 ) 
       AND ( Datepart(yy, dbo.MINTODATE(dbo.ORDERS.CURDATE)) > 2018 ) 
       AND ( dbo.PART.PART IN (SELECT PART 
                               FROM   dbo.v3pl_3plSKU AS v3pl_SKU_1) ) 
       AND ( dbo.ORDERITEMS.ZTRX_3PLSENT <> 'Y' ) 
       AND ( dbo.ORDERITEMS.DUEDATE - 1440 * 7 < dbo.DATETOMIN(Getdate()) ) 
       AND ( dbo.ORDERITEMS.CLOSED = '' ) 
       AND ( dbo.ORDERITEMS.TBALANCE / 1000 > 0 ) 
       AND ( dbo.ORDERITEMS.ZTRX_RESERVED = '' ) 
ORDER  BY dbo.ORDERS.CUST, 
          dbo.ORDERS.ORD, 
          dbo.ORDERITEMS.KLINE 

GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_SO]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Si
-- Create date: 22/01/19
-- Description:	3pl integration
--				Outbound SO 
-- =============================================
CREATE FUNCTION [dbo].[v3pl_SO]()
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT 
	     
		dbo.v_3pl_SorderItems.ORD, 
		dbo.v_3pl_SorderItems.DUEDATE, 
		'ODH'                                              AS Record_Type, 
		'U'                                                AS Merge_Action, 
		'Released'                                         AS Status, 
		dbo.v_3pl_SorderItems.Order_Type                   AS Ship_Dock, 
		'BD1'                                              AS From_Site_Id, 
		'TORQUE'                                           AS Owner_Id, 
		'TT'                                              AS Client_Id, 
		'TTVIS'                                           as Client_Group,
		dbo.v_3pl_SorderItems.Customer_ID				as Customer_Id, 
		dbo.v_3pl_SorderItems.Address1, 
		dbo.v_3pl_SorderItems.Address2, 
		dbo.v_3pl_SorderItems.Town, 
		dbo.v_3pl_SorderItems.County, 
		dbo.v_3pl_SorderItems.Postcode, 
		dbo.COUNTRIES.COUNTRYCODE                          AS Country, 
		dbo.v_3pl_SorderItems.User_Def_Type_1, 
		dbo.v_3pl_SorderItems.User_Def_Type_2, 
		CASE 
		WHEN ( COUNTRIES.COUNTRYCODE = 'GB' ) THEN 'N' 
		ELSE 'Y' 
		END                                                AS Export, 
		dbo.v_3pl_SorderItems.Name, 
		dbo.v_3pl_SorderItems.ORDNAME + '-' + Ltrim(Rtrim(Str(dbo.v_3pl_SorderItems.DUEDATE))) AS Order_Id, 
		dbo.v_3pl_SorderItems.Contact, 
		dbo.v_3pl_SorderItems.Contact_Phone, 
		dbo.v_3pl_SorderItems.Contact_Email, 
		dbo.v_3pl_SorderItems.REFERENCE                    AS Purchase_Order, 		 
		dbo.v_3pl_SorderItems.Service_Level as 'Dispatch_Method',
		case 
			when (dbo.v_3pl_SorderItems.Order_Type = 'TTWHO') then 'WHOLESALE'
			when (dbo.v_3pl_SorderItems.Order_Type = 'TTWEB') then 'WEB'
			when (dbo.v_3pl_SorderItems.Order_Type = 'TTOUT') then 'RETAIL'
			when (dbo.v_3pl_SorderItems.Order_Type = 'TTEXP') then 'EXPORT'
		END AS Order_Type


	FROM   dbo.v_3pl_SorderItems 
       INNER JOIN dbo.COUNTRIES 
               ON dbo.v_3pl_SorderItems.COUNTRY = dbo.COUNTRIES.COUNTRY 

	GROUP  BY dbo.v_3pl_SorderItems.ORD, 
		dbo.v_3pl_SorderItems.Customer_ID, 
		dbo.v_3pl_SorderItems.Address1, 
		dbo.v_3pl_SorderItems.Address2, 
		dbo.v_3pl_SorderItems.Town, 
		dbo.v_3pl_SorderItems.County, 
		dbo.v_3pl_SorderItems.Postcode, 
		dbo.COUNTRIES.COUNTRYCODE, 
		dbo.v_3pl_SorderItems.User_Def_Type_1, 
		dbo.v_3pl_SorderItems.User_Def_Type_2, 
		dbo.v_3pl_SorderItems.Name, 
		dbo.v_3pl_SorderItems.ORDNAME + '-' 
		+ Ltrim(Rtrim(Str(dbo.v_3pl_SorderItems.DUEDATE))), 
		dbo.v_3pl_SorderItems.DUEDATE, 
		dbo.v_3pl_SorderItems.Contact, 
		dbo.v_3pl_SorderItems.Contact_Phone, 
		dbo.v_3pl_SorderItems.Contact_Email, 
		dbo.v_3pl_SorderItems.REFERENCE, 
		dbo.v_3pl_SorderItems.Order_Type,

		dbo.v_3pl_SorderItems.Service_Level 
)
GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_poi]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Si
-- Create date: 22/01/19
-- Description:	3pl integration
--				Outbound PO Items 
-- =============================================
CREATE FUNCTION [dbo].[v3pl_poi]
(	
	-- Add the parameters for the function here
	@ORD int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT DISTINCT 	
		ORDI AS ORDI,
		IMPFNUM AS Pre_Advice_Id, 
		'PAL' AS Record_Type, 
		'A' AS Merge_Action, 
		'TORQUE' AS Owner_Id, 
		'TT' AS Client_Id, 
		'TTVIS' AS Client_Group, 
		'MD' as "Pallet_Config",
		COUNTRYCODE AS Ce_Coo, 
		SUPNAME AS Supplier_Id, 
		ORDNAME AS User_Def_Type_1, 
		KLINE AS User_Def_Num_1, 
		ZTRX_EXPECTEDQUANT AS Qty_Due, 
		PARTNAME AS Sku_Id, 
		ROW_NUMBER() over (order by KLINE) AS Line_Id

	FROM   
		[dbo].[v_3pl_PorderItems]
		
	where IMPFILE = @ORD
	
)


GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_soi]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Si
-- Create date: 22/01/19
-- Description:	3pl integration
--				Outbound SO Items 
-- =============================================
CREATE FUNCTION [dbo].[v3pl_soi]
(	
	-- Add the parameters for the function here
	@ORD bigint,
	@DUEDATE BIGINT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		ORDI as ORDI,
		'ODL'                                      AS Record_Type, 
		'U'                                        AS Merge_Action, 
		'TT'                                      AS Client_Id, 
		ORDNAME + '-' + Ltrim(Rtrim(Str(DUEDATE))) AS Order_Id, 
		PARTNAME                                   AS Sku_Id, 
		QUANT                                      AS Qty_Ordered, 
		ORDNAME                                    AS User_Def_Type_1, 
		ORDI                                       AS User_Def_Num_3, 
		Product_Currency, 
		Customer_Sku_Id, 
		Product_Price , 
		ROW_NUMBER() over (order by KLINE) AS Line_Id

	FROM   dbo.v_3pl_SorderItems 

	WHERE  ( 0 = 0 ) 
		   AND ( ORD = @ORD ) 
		   AND ( DUEDATE = @DUEDATE ) 

)


GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_PO]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Si
-- Create date: 22/01/19
-- Description:	3pl integration
--				Outbound PO 
-- =============================================
CREATE FUNCTION [dbo].[v3pl_PO]()
RETURNS TABLE 
AS
RETURN 
(
	SELECT   DISTINCT 
		IMPFILE AS "IMO", 
		IMPFNUM AS "Pre_Advice_Id", 	
			
		LTRIM(RTRIM(STR(DATEPART(yyyy, dbo.MINTODATE(min(DUEDATE)))))) +
		right('00' + LTRIM(RTRIM(STR(DATEPART(MM, dbo.MINTODATE(min(DUEDATE)))))),2) +
		right('00' + LTRIM(RTRIM(STR(DATEPART(dd, dbo.MINTODATE(min(DUEDATE)))))),2) +
		'000000' 
		AS "Due_Dstamp",

		'PAH' as "Record_Type",
		'A' AS "Merge_Action",
		'BD1' as "Site_Id",
		'TORQUE' as "Owner_Id",
		'TT' as "Client_Id",
		'TTVIS' as "Client_Group",
		SUPDES as "Name",
		ADDRESS AS "Address1",
		EMG_ADDRESSB AS "Address2",
		EMG_ADDRESSC AS "Town",
		COUNTRYCODE AS "Country"		

	FROM
		[dbo].[v_3pl_PorderItems]

	GROUP BY 
		IMPFNUM, 
		IMPFILE, 
		SUPDES, 
		ADDRESS, 
		EMG_ADDRESSB, 
		EMG_ADDRESSC, 
		COUNTRYCODE, 
        SUPNAME

)


GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_SKU]    Script Date: 20/02/2019 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Si
-- Create date: 22/01/19
-- Description:	3pl integration
--				Outbound SKU 
-- =============================================
CREATE FUNCTION [dbo].[v3pl_SKU]()
RETURNS TABLE 
AS
RETURN 
(
	SELECT dbo.PART.PART, 
		dbo.CUSTOMSITEMS.CUSTOMSITEMNAME AS 'Commodity_Code', 
		dbo.CUSTOMSITEMS.CUSTOMSITEMDES  AS 'Commodity_Desc', 
		dbo.COUNTRIES.COUNTRYCODE        AS 'Ce_Coo', 
		CASE TAXGROUPS.TAXGROUPCODE 
			WHEN 'ZZZ' THEN 'Z' 
			ELSE 'S' 
		END                              AS 'Ce_Vat_Code', 
		dbo.ZTRX_PARTWEBDATA.FABRICTYPE  AS 'User_Def_Note_1', 
		'SKU'                            AS Record_Type, 
		'U'                              AS Merge_Action, 
		'TTVIS'                          AS Client_Group, 
		'TT'                             AS Client_Id, 
		dbo.PART.BARCODE                 AS EAN, 
		dbo.PART.PARTNAME                AS Sku_Id, 
		dbo.PART.PARTDES                 AS Description, 
		dbo.PART.PRICE                   AS Each_Value, 
		'N'                              AS Expiry_Reqd, 
		'N'                              AS Split_Lowest, 
		'N'                              AS Condition_Reqd, 
		'N'                              AS Origin_Reqd, 
		'N'                              AS Serial_At_Pick, 
		'N'                              AS Serial_At_Pack, 
		'N'                              AS Kit_Sku, 
		'N'                              AS Kit_Split, 
		'N'                              AS Abc_Disable, 
		'N'                              AS Obsolete_Product, 
		'N'                              AS New_Product, 
		'N'                              AS Disallow_Upload, 
		'N'                              AS Manuf_Dstamp_Reqd, 
		'N'                              AS Manuf_Dstamp_Dflt, 
		'N'                              AS Hazmat, 
		'N'                              AS Disallow_Cross_Dock, 
		'N'                              AS User_Def_Chk_1, 
		'N'                              AS User_Def_Chk_2, 
		'N'                              AS User_Def_Chk_3, 
		'N'                              AS User_Def_Chk_4, 
		'N'                              AS Disallow_Merge_Rules, 
		'N'                              AS Pack_Despatch_Repack, 
		'N'                              AS Serial_At_Receipt, 
		'N'                              AS Serial_Valid_Merge, 
		'N'                              AS Serial_No_Reuse, 
		'N'                              AS Ce_Customs_Excise, 
		'N'                              AS Disallow_Clustering, 
		'N'                              AS Ce_Duty_Stamp, 
		'N'                              AS Capture_Weight, 
		dbo.PARTSPEC.SPEC2               AS Colour, 
		dbo.PARTSPEC.SPEC14              AS Product_Group, 
		CASE SPEC14 
			WHEN 'RG03' THEN 'Y' 
			WHEN 'RG04' THEN 'Y' 
			WHEN 'RG05' THEN 'Y' 
			ELSE 'N' 
		END                              AS Hanging_Garment, 
		dbo.PARTSPEC.SPEC1               AS 'User_Def_Note_2', 
		dbo.PARTSPEC.SPEC3               AS 'User_Def_Type_2', 
		dbo.PARTSPEC.SPEC4               AS 'User_Def_Type_3', 
		dbo.PARTSPEC.SPEC10              AS 'User_Def_Type_4', 
		dbo.PARTSPEC.SPEC2               AS 'User_Def_Type_5', 
		dbo.ZTRX_PARTWEBDATA.WEARTYPE    AS 'User_Def_Type_6' 
		FROM   dbo.COUNTRIES 
		RIGHT OUTER JOIN dbo.SUPPLIERS 
		ON dbo.COUNTRIES.COUNTRY = dbo.SUPPLIERS.COUNTRY 
		RIGHT OUTER JOIN dbo.TAXGROUPS 
		RIGHT OUTER JOIN dbo.PARTPARAM 
					ON dbo.TAXGROUPS.TAXGROUP = 
						dbo.PARTPARAM.TAXGROUP 
		LEFT OUTER JOIN dbo.CUSTOMSITEMS 
					ON dbo.PARTPARAM.CUSTOMSITEM = 
					dbo.CUSTOMSITEMS.CUSTOMSITEM 
		ON dbo.SUPPLIERS.SUP = dbo.PARTPARAM.SUP 
		RIGHT OUTER JOIN dbo.PART 
		ON dbo.PARTPARAM.PART = dbo.PART.PART 
		LEFT OUTER JOIN dbo.ZTRX_PARTWEBDATA 
		ON dbo.ZTRX_PARTWEBDATA.PART = dbo.PART.MPART 
		LEFT OUTER JOIN dbo.PARTSPEC 
		ON dbo.PART.PART = dbo.PARTSPEC.PART 
		WHERE  ( 0 = 0 ) 
		AND ( dbo.PART.PART IN (
			SELECT PART FROM dbo.v3pl_3plSKU 
		)
		AND PART.ZTRX_3PLSENT <> 'Y'
	) 
)

--SELECT * FROM [dbo].[v3pl_SKU]()
GO
