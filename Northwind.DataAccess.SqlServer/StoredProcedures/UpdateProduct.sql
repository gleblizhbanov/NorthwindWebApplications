CREATE PROCEDURE UpdateProduct (
	@productId int,
	@productName nvarchar(40),
	@supplierId int,
	@categoryId int,
	@quantityPerUnit nvarchar(20),
	@unitPrice money,
	@unitsInStock smallint,
	@unitsOnOrder smallint,
	@reorderLevel smallint,
	@discontinued bit
)
AS
BEGIN
	UPDATE dbo.Products 
	SET ProductName = @productName, SupplierID = @supplierId, CategoryID = @categoryId, QuantityPerUnit = @quantityPerUnit, UnitPrice = @unitPrice, UnitsInStock = @unitsInStock, UnitsOnOrder = @unitsOnOrder, ReorderLevel = @reorderLevel, Discontinued = @discontinued
	WHERE ProductID = @productId
	SELECT @@ROWCOUNT
END;