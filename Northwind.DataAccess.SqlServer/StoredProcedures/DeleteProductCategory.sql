CREATE PROCEDURE DeleteProductCategory (
	@categoryId int
)
AS
BEGIN
	DELETE FROM dbo.Categories
	WHERE CategoryID = @categoryId
	SELECT @@ROWCOUNT
END;