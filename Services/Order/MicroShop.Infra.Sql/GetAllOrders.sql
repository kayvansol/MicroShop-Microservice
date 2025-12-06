USE [MicroShop]
GO
/****** Object:  StoredProcedure [dbo].[GetAllOrders]    Script Date: 12/7/2025 2:26:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetAllOrders]
	
AS
BEGIN
	
	SET NOCOUNT ON;

SELECT        sales.orders.order_id OrderId, sales.customers.first_name + ' ' + sales.customers.last_name CustomerName,
				case(sales.orders.order_status) when 0 then N'در انتظار پرداخت' when 1 then N'عدم موجودی' when 2 then N'پرداخت‌ شده' 
				when 3 then N'پرداخت ناموفق' when 4 then N'در حال پردازش'  when 5 then N'آماده ارسال' end as OrderStatus,       
				sales.orders.order_date OrderDate, sales.orders.required_date RequiredDate, sales.orders.shipped_date ShippedDate
FROM            sales.orders INNER JOIN
                         sales.customers ON sales.orders.customer_id = sales.customers.customer_id

END
