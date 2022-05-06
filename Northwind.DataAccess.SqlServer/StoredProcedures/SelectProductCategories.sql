CREATE PROCEDURE SelectProductCategories (
	@offset int,
	@limit int
)
AS
BEGIN
	SELECT category.CategoryID, category.CategoryName, category.Description, category.Picture
	FROM dbo.Categories
	AS category
	ORDER BY category.CategoryID
	OFFSET @offset ROWS
	FETCH FIRST @LIMIT ROWS ONLY
END;