USE BookstoreDB;

Select * from WishLists;
drop table WishLists;

CREATE TABLE WishLists (
    wishListId INT PRIMARY KEY IDENTITY,
    userId INT NOT NULL FOREIGN KEY REFERENCES Users(userId),
    bookId INT NOT NULL FOREIGN KEY REFERENCES Books(bookId),
	IsDeleted BIT DEFAULT 0,
    createdAt DATETIME DEFAULT GETDATE(),
    updatedAt DATETIME DEFAULT GETDATE()
);

ALTER PROCEDURE usp_add_item_to_wishlist
    @userId INT,
    @bookId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if the book exists
        IF NOT EXISTS (SELECT 1 FROM Books WHERE bookId = @bookId AND IsDeleted = 0)
        BEGIN
            THROW 50001, 'Invalid bookId. Book not found.', 1;
        END

        -- Check if the user exists
        IF NOT EXISTS (SELECT 1 FROM Users WHERE userId = @userId AND IsDeleted = 0)
        BEGIN
            THROW 50002, 'Invalid userId. User not found.', 1;
        END

        -- Check if the item already exists in the wishlist and is not deleted
        IF EXISTS (SELECT 1 FROM WishLists WHERE userId = @userId AND bookId = @bookId AND IsDeleted = 0)
        BEGIN
            THROW 50003, 'Item already exists in the wishlist.', 1;
        END

        -- Check if the item already exists in the wishlist but is deleted
        IF EXISTS (SELECT 1 FROM WishLists WHERE userId = @userId AND bookId = @bookId AND IsDeleted = 1)
        BEGIN
            -- Update the IsDeleted flag to 0 and update the updatedAt timestamp
            UPDATE WishLists
            SET IsDeleted = 0, updatedAt = GETDATE(), createdAt = GETDATE()
            WHERE userId = @userId AND bookId = @bookId AND IsDeleted = 1;

            -- Return the existing wishListId
            DECLARE @wishListId INT;
            SELECT @wishListId = wishListId FROM WishLists WHERE userId = @userId AND bookId = @bookId;
            SELECT @wishListId AS wishListId;

            RETURN;
        END

        -- Insert a new item into the wishlist
        INSERT INTO WishLists (userId, bookId, createdAt, updatedAt)
        VALUES (@userId, @bookId, GETDATE(), GETDATE());

        -- Return the newly generated wishListId
        DECLARE @newWishListId INT;
        SELECT @newWishListId = SCOPE_IDENTITY();
        SELECT @newWishListId AS wishListId;
    END TRY
    BEGIN CATCH
        -- Handle errors
        DECLARE @errorMessage NVARCHAR(4000);
        DECLARE @errorSeverity INT;
        DECLARE @errorState INT;

        SELECT @errorMessage = ERROR_MESSAGE(), @errorSeverity = ERROR_SEVERITY(), @errorState = ERROR_STATE();
        RAISERROR (@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;

--get specfic users wishlists
Create PROCEDURE usp_get_user_wishlist_details
    @userId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if the user exists
        IF NOT EXISTS (SELECT 1 FROM Users WHERE userId = @userId AND IsDeleted = 0)
        BEGIN
            THROW 50001, 'Invalid userId. User not found.', 1;
        END

        -- Fetch the wishlist details for the user
        SELECT 
            w.wishListId,
            w.userId,
            w.bookId,
            b.Title AS bookTitle,
            b.Author AS bookAuthor,
            b.Price AS bookPrice,
            w.createdAt,
            w.updatedAt
        FROM 
            WishLists w
        INNER JOIN 
            Books b ON w.bookId = b.bookId
        WHERE 
            w.userId = @userId AND w.IsDeleted = 0 AND b.IsDeleted = 0;

        -- Check if the user has items in the wishlist
        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50002, 'No items found in the wishlist for the specified user.', 1;
        END
    END TRY
    BEGIN CATCH
        -- Handle errors
        DECLARE @errorMessage NVARCHAR(4000);
        DECLARE @errorSeverity INT;
        DECLARE @errorState INT;

        SELECT @errorMessage = ERROR_MESSAGE(), @errorSeverity = ERROR_SEVERITY(), @errorState = ERROR_STATE();
        RAISERROR (@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;


--get all users wishlists
Create PROCEDURE usp_get_all_users_wishlist_details
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Fetch the wishlist details for all users
        SELECT 
            w.wishListId,
            w.userId,
            w.bookId,
            b.Title AS bookTitle,
            b.Author AS bookAuthor,
            b.Price AS bookPrice,
            w.createdAt,
            w.updatedAt
        FROM 
            WishLists w
        INNER JOIN 
            Books b ON w.bookId = b.bookId
        WHERE 
            w.IsDeleted = 0 AND b.IsDeleted = 0
        ORDER BY 
            w.userId;
    END TRY
    BEGIN CATCH
        -- Handle errors
        DECLARE @errorMessage NVARCHAR(4000);
        DECLARE @errorSeverity INT;
        DECLARE @errorState INT;

        SELECT @errorMessage = ERROR_MESSAGE(), @errorSeverity = ERROR_SEVERITY(), @errorState = ERROR_STATE();
        RAISERROR (@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;

--remove item from wish list
ALTER PROCEDURE usp_remove_item_from_wishlist
    @userId INT,
    @bookId INT
AS
BEGIN
    --SET NOCOUNT ON;

    BEGIN TRY
        -- Check if the item exists in the wishlist and is not already deleted
        IF NOT EXISTS (SELECT 1 FROM WishLists WHERE userId = @userId AND bookId = @bookId AND IsDeleted = 0)
        BEGIN
            THROW 50004, 'Item not found in the wishlist or already removed.', 1;
        END

        -- Update the IsDeleted flag to 1
        UPDATE WishLists
        SET IsDeleted = 1, updatedAt = GETDATE()
        WHERE userId = @userId AND bookId = @bookId AND IsDeleted = 0;
    END TRY
    BEGIN CATCH
        -- Handle errors
        DECLARE @errorMessage NVARCHAR(4000);
        DECLARE @errorSeverity INT;
        DECLARE @errorState INT;

        SELECT @errorMessage = ERROR_MESSAGE(), @errorSeverity = ERROR_SEVERITY(), @errorState = ERROR_STATE();
        RAISERROR (@errorMessage, @errorSeverity, @errorState);
    END CATCH
END;
