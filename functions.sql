USE [wlnd]
GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_PO]    Script Date: 24/01/2019 11:50:41 ******/
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
	SELECT        
		dbo.PORDERS.ORD, 
		dbo.SUPPLIERS.SUPNAME AS "Supplier_Id", 
		dbo.PORDERS.ORDNAME AS "Bookref_Id"

	FROM            dbo.PORDERS INNER JOIN
							 dbo.SUPPLIERS ON dbo.PORDERS.SUP = dbo.SUPPLIERS.SUP
	WHERE        (dbo.PORDERS.ORD > 0)
)

GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_SKU]    Script Date: 24/01/2019 11:50:41 ******/
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
	SELECT        
		PARTNAME AS "Sku_Id", 
		PARTDES AS "Description"

	FROM            dbo.PART
	WHERE        (PART > 0)
)

GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_SO]    Script Date: 24/01/2019 11:50:41 ******/
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
	SELECT        
		ORD, 
		ORDNAME AS "Order_Id"

	FROM            dbo.ORDERS
	WHERE        (ORD > 0)
)

GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_poi]    Script Date: 24/01/2019 11:50:41 ******/
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
	-- Add the SELECT statement with parameter references here
	SELECT TQUANT as "Qty_Due"
	from PORDERITEMS
)

GO
/****** Object:  UserDefinedFunction [dbo].[v3pl_soi]    Script Date: 24/01/2019 11:50:41 ******/
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
	@ORD int
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT 
		PARTNAME as "Sku_Id"
	from ORDERITEMS join PART on ORDERITEMS.PART = PART.PART
	where @ORD =  ORDERITEMS.ORD
)

GO
