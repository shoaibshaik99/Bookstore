Use BookstoreDB;

select * from Addresses;

CREATE TABLE Addresses (
    AddressId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    Street NVARCHAR(255) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    ZipCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    AddressType NVARCHAR(50) NOT NULL,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);


--									Add Adderess
CREATE PROCEDURE usp_insert_address
    @UserId INT,
    @Street NVARCHAR(255),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @ZipCode NVARCHAR(20),
    @Country NVARCHAR(100),
    @AddressType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Addresses (UserId, Street, City, State, ZipCode, Country, AddressType, IsDeleted, CreatedAt, UpdatedAt)
        VALUES (@UserId, @Street, @City, @State, @ZipCode, @Country, @AddressType, 0, GETDATE(), GETDATE());

        SELECT SCOPE_IDENTITY() AS AddressId;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(), 
            @ErrorSeverity = ERROR_SEVERITY(), 
            @ErrorState = ERROR_STATE();
            
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

--										Fetch an users' addresses
CREATE PROCEDURE usp_get_addresses_by_userid
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT * FROM Addresses WHERE UserId = @UserId AND IsDeleted = 0;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(), 
            @ErrorSeverity = ERROR_SEVERITY(), 
            @ErrorState = ERROR_STATE();
            
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

--										CREATE PROCEDURE usp_update_address
CREATE PROCEDURE usp_update_address
    @AddressId INT,
    @Street NVARCHAR(255),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @ZipCode NVARCHAR(20),
    @Country NVARCHAR(100),
    @AddressType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Addresses
        SET Street = @Street, City = @City, State = @State, ZipCode = @ZipCode, Country = @Country, AddressType = @AddressType, UpdatedAt = GETDATE()
        WHERE AddressId = @AddressId;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(), 
            @ErrorSeverity = ERROR_SEVERITY(), 
            @ErrorState = ERROR_STATE();
            
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

--												Delete Address
CREATE PROCEDURE usp_delete_address
    @AddressId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Addresses
        SET IsDeleted = 1, UpdatedAt = GETDATE()
        WHERE AddressId = @AddressId;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(), 
            @ErrorSeverity = ERROR_SEVERITY(), 
            @ErrorState = ERROR_STATE();
            
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
