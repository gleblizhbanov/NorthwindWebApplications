CREATE PROCEDURE InsertEmployee (
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
	INSERT INTO dbo.Employees (LastName, FirstName, Title, TitleOfCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, HomePhone, Extension, Photo, Notes, ReportsTo, PhotoPath) 
	OUTPUT Inserted.EmployeeID
	VALUES (@lastName, @firstName, @title, @titleOfCourtesy, @birthDate, @hireDate, @address, @city, @region, @postalCode, @country, @homePhone, @extension, @photo, @notes, @reportsTo, @photoPath)
END;