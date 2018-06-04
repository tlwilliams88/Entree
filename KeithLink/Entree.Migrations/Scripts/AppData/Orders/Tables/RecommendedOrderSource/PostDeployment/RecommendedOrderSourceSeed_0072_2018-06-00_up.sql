INSERT 
	INTO [Orders].[RecommendedOrderSource] 
		([OrderSource], [Description]) 
	VALUES 
		('CartIQ', 'Recommended to order because of customer type')
INSERT 
	INTO [Orders].[RecommendedOrderSource] 
		([OrderSource], [Description]) 
	VALUES 
		('SalesIQ-Growth', 'Recommended for growth order because of past sales'),
		('SalesIQ-Recovery', 'Recommended for sales reocvery order because of past sales')
