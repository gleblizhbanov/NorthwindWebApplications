CREATE PROCEDURE InsertProductCategory (
	@categoryName nvarchar(15),
	@description ntext,
	@piture image
)
AS
BEGIN
	INSERT INTO dbo.Categories (CategoryName, Description, Picture)
	OUTPUT Inserted.CategoryID
	VALUES (@categoryName, @description, @piture)
END;