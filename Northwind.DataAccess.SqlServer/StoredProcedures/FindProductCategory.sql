CREATE PROCEDURE FindProductCategory (
	@categoryId int
)
AS
BEGIN
	SELECT category.CategoryID, category.CategoryName, category.Description, category.Picture
	FROM dbo.Categories
	AS category
	WHERE category.CategoryID = @categoryId
END;