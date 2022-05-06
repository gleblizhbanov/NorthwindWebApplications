CREATE PROCEDURE InsertProduct (
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
	INSERT INTO dbo.Products (ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued)
	OUTPUT Inserted.ProductID
	VALUES (@productName, @supplierId, @categoryId, @quantityPerUnit, @unitPrice, @unitsInStock, @unitsOnOrder, @reorderLevel, @discontinued)
END;