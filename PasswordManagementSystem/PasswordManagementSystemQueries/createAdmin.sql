INSERT INTO Company (CompanyName, Domain)
VALUES ('next4biz', 'next4biz.com');

DECLARE @CompanyId INT = (SELECT CompanyId FROM Company WHERE CompanyName = 'next4biz');

INSERT INTO [User] (CompanyId, Name, Email, Password)
VALUES (@CompanyId, 'Kamil', 'admin@next4biz.com', '123');

INSERT INTO Role (RoleName, CompanyId)
VALUES ('Admin', @CompanyId);

DECLARE @UserId INT = (SELECT UserId FROM [User] WHERE Email = 'admin@next4biz.com');
DECLARE @RoleId INT = (SELECT RoleId FROM Role WHERE RoleName = 'Admin' AND CompanyId = @CompanyId);

INSERT INTO UserRole (UserId, RoleId)
VALUES (@UserId, @RoleId);

