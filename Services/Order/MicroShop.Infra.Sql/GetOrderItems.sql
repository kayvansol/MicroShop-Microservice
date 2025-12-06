
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

alter PROCEDURE GetOrderItems
	@OrderId int = 281
AS
BEGIN
	
	SET NOCOUNT ON;

	SELECT        sales.order_items.item_id itemId, production.products.product_name productName, sales.order_items.quantity quantity
					, sales.order_items.price, sales.order_items.discount
FROM            sales.order_items INNER JOIN
                         production.products ON sales.order_items.product_id = production.products.product_id
						 where sales.order_items.order_id = @OrderId

END
GO
