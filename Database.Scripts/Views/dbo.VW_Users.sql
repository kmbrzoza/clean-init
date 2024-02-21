CREATE VIEW [dbo].[VW_Users]
AS
SELECT 
	u.Id,
	u.Sub,
	u.Email,
	u.RoleId
FROM
	[dbo].[User] u
