CREATE PROCEDURE SelectProductsByName (
	@productsNames nvarchar(255)
)
AS
BEGIN
	SELECT product.ProductID, product.ProductName, product.SupplierID, product.CategoryID, product.QuantityPerUnit, product.UnitPrice, product.UnitsInStock, product.UnitsOnOrder, product.ReorderLevel, product.Discontinued
	FROM dbo.Products
	AS product
	WHERE product.ProductName IN (@productsNames)
	ORDER BY product.ProductID
END;