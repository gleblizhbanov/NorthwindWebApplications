CREATE PROCEDURE UpdateProductCategory (
	@categoryId int,
	@categoryName nvarchar(15),
	@description ntext,
	@picture image
)
AS
BEGIN
	UPDATE dbo.Categories
	SET CategoryName = @categoryName, Description = @description, Picture = @picture
	WHERE CategoryID = @categoryId
	SELECT @@ROWCOUNT
END;