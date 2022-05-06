CREATE PROCEDURE SelectProductCategoriesByName (
	@categoriesNames nvarchar(255)
)
AS
BEGIN
	SELECT category.CategoryID, category.CategoryName, category.Description, category.Picture
	FROM dbo.Categories
	AS category
	WHERE category.CategoryName IN (@categoriesNames)
	ORDER BY category.CategoryID
END;