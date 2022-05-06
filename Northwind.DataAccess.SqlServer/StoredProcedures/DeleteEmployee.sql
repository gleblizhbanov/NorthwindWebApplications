CREATE PROCEDURE DeleteEmployee
	@employeeId int
AS
	DELETE FROM dbo.Employees
	WHERE EmployeeID = @employeeId 
	SELECT @@ROWCOUNT