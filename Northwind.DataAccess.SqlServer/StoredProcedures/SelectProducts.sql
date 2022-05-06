CREATE PROCEDURE SelectProducts (
	@offset int,
	@limit int
)
AS
BEGIN
	SELECT product.ProductID, product.ProductName, product.SupplierID, product.CategoryID, product.QuantityPerUnit, product.UnitPrice, product.UnitsInStock, product.UnitsOnOrder, product.ReorderLevel, product.Discontinued
	FROM dbo.Products
	AS product
	ORDER BY product.ProductID
	OFFSET @offset ROWS
	FETCH FIRST @limit ROWS ONLY
END;