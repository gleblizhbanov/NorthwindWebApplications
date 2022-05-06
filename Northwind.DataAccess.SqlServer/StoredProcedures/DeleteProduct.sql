CREATE PROCEDURE DeleteProduct (
	@productId int
)
AS
BEGIN
	DELETE FROM dbo.Products
	WHERE ProductID = @productId
	SELECT @@ROWCOUNT
END;