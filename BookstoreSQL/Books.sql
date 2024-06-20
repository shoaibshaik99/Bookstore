use BookstoreDB;

CREATE TABLE Books (
    BookId INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(MAX) NOT NULL,
    Author NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Rating DECIMAL(2,1) DEFAULT 0,
    RatingCount INT DEFAULT 0,
    OriginalPrice INT NOT NULL,
    DiscountPercentage INT NOT NULL,
    Price AS CAST(OriginalPrice * (1 - DiscountPercentage / 100.0) AS INT) PERSISTED NOT NULL,
    Quantity INT NOT NULL,
    Image NVARCHAR(MAX),
    IsDeleted BIT DEFAULT 0,
	deletedAt datetime null

    CONSTRAINT CHK_Rating CHECK (Rating BETWEEN 0 AND 5),
    CONSTRAINT CHK_Discount CHECK (DiscountPercentage BETWEEN 0 AND 100),
    CONSTRAINT CHK_Price CHECK (Price <= OriginalPrice)
);

--ALTER TABLE Books
--ADD DeletedAt DATETIME NULL;

--Add Book
ALTER PROCEDURE usp_add_book
    @Title NVARCHAR(MAX),
    @Author NVARCHAR(MAX),
    @Description NVARCHAR(MAX),
    @OriginalPrice INT,
    @DiscountPercentage INT,
    @Quantity INT,
    @Image NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validate input parameters
        IF @Title IS NULL OR @Author IS NULL OR @Description IS NULL OR @OriginalPrice IS NULL OR @DiscountPercentage IS NULL OR @Quantity IS NULL
        BEGIN
            THROW 50001, 'All mandatory parameters (Title, Author, Description, OriginalPrice, DiscountPercentage, Quantity) must be provided.', 1;
        END

        IF @DiscountPercentage < 0 OR @DiscountPercentage > 100
        BEGIN
            THROW 50003, 'DiscountPercentage must be between 0 and 100.', 1;
        END

        IF @Quantity < 0
        BEGIN
            THROW 50004, 'Quantity cannot be negative.', 1;
        END

        -- Insert new book
        INSERT INTO Books (Title, Author, Description, OriginalPrice, DiscountPercentage, Quantity, Image)
        VALUES (@Title, @Author, @Description, @OriginalPrice, @DiscountPercentage, @Quantity, @Image);

        SELECT SCOPE_IDENTITY() as BookId; -- Return the newly generated BookId
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(MAX) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        THROW @ErrorSeverity, @ErrorMessage, @ErrorState;
        RETURN -1; -- Return error code
    END CATCH
END

-- Procedure to retrieve a book by ID
alter procedure usp_get_book_by_id
    @BookId int
as
begin
    set nocount on;
    begin try
        select BookId, Title, Author, Description, Rating, RatingCount, OriginalPrice, DiscountPercentage, Price, Quantity, Image
        from Books
        where BookId = @BookId and IsDeleted = 0;
    end try
    begin catch
        select 
            error_number() as error_number,
            error_message() as error_message;
    end catch;
end;
go
--usp_get_book_by_id 1

-- Procedure to fetch all books
create procedure usp_get_all_books
as
begin
    set nocount on;
    begin try
        select BookId, Title, Author, Description, Rating, RatingCount, OriginalPrice, DiscountPercentage, Price, Quantity, Image
        from Books
		where IsDeleted = 0;;
    end try
    begin catch
        select 
            error_number() as error_number,
            error_message() as error_message;
    end catch;
end;
go

-- Procedure to update a book
alter procedure usp_update_book
    @bookId int,
    @title nvarchar(max),
    @author nvarchar(max),
    @description nvarchar(max),
    @originalPrice int,
    @discountPercentage int,
    @quantity int,
    @image nvarchar(max)
as
begin
    --set nocount on;
    
    begin try
        begin transaction;
        
        update books
        set 
            title = @title,
            author = @author,
            description = @description,
            originalPrice = @originalPrice,
            discountPercentage = @discountPercentage,
            quantity = @quantity,
            image = @image
        where 
            bookId = @bookId;
        
        if @@rowcount = 0
        begin
            -- if no rows were affected, it means the bookId does not exist
            select 'no rows affected, book not found' as message;
            rollback transaction;
        end
        else
        begin
            -- if rows were affected, commit the transaction
            select 'update successful' as message;
            commit transaction;
        end
    end try
    begin catch
        -- handle errors
        if @@trancount > 0
        begin
            rollback transaction;
        end
        
        -- return the error information
        select 
            error_number() as errorNumber,
            error_message() as errorMessage;
    end catch;
end;
go

-- Procedure to soft delete a book
Alter PROCEDURE usp_delete_book
    @BookId INT
AS
BEGIN
    -- Set nocount on;
    BEGIN TRY
        UPDATE Books
        SET IsDeleted = 1,
            DeletedAt = GETDATE()
        WHERE BookId = @BookId;
        
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 'No rows affected' AS Message;
        END
        ELSE
        BEGIN
            SELECT 'Deleted book (soft delete)' AS Message;
        END
    END TRY
    BEGIN CATCH
        SELECT 
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH;
END;
GO

usp_delete_book 3

UPDATE Books
SET IsDeleted = 0
WHERE BookId=3;