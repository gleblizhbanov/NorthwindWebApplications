CREATE PROCEDURE DeleteEmployee (
	@employeeId int
)
AS
BEGIN
	DELETE FROM dbo.Employees
	WHERE EmployeeID = @employeeId
	SELECT @@ROWCOUNT
END;