CREATE PROCEDURE UpdateEmployee (
	@employeeId int,
	@lastName nvarchar(20),
	@firstName nvarchar(10),
	@title nvarchar(30),
	@titleOfCourtesy nvarchar(25),
	@birthDate datetime,
	@hireDate datetime,
	@address nvarchar(60),
	@city nvarchar(15),
	@region nvarchar(15),
	@postalCode nvarchar(10),
	@country nvarchar(15),
	@homePhone nvarchar(24),
	@extension nvarchar(4),
	@photo image,
	@notes ntext,
	@reportsTo int,
	@photoPath nvarchar(255)
)
AS
BEGIN
	UPDATE dbo.Employees
	SET LastName = @lastName, FirstName = @firstName, Title = @title, TitleOfCourtesy = @titleOfCourtesy, BirthDate = @birthDate, HireDate = @hireDate, Address = @address, City = @city, Region = @region, PostalCode = @postalCode, Country = @country, HomePhone = @homePhone, Extension = @extension, Photo = @photo, Notes = @notes, ReportsTo = @reportsTo, PhotoPath = @photoPath
	WHERE EmployeeID = @employeeId
	SELECT @@ROWCOUNT
END;