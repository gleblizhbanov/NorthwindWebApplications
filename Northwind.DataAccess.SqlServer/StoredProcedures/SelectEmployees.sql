CREATE PROCEDURE SelectEmployees
	@offset int,
	@limit int
AS
	SELECT employee.EmployeeId, employee.LastName, employee.FirstName, employee.Title, employee.TitleOfCourtesy, employee.BirthDate, employee.HireDate, employee.Address, employee.City, employee.Region, employee.PostalCode, employee.Country, employee.HomePhone, employee.Extension, employee.Photo, employee.Notes, employee.ReportsTo, employee.PhotoPath
	FROM dbo.Employees
	AS employee
	ORDER BY employee.EmployeeID
	OFFSET @offset ROWS
	FETCH FIRST @limit ROWS ONLY