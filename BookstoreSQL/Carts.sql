USE BookstoreDB;

Select * from Carts;
drop table Carts;

CREATE TABLE Carts (
    cartItemId INT PRIMARY KEY IDENTITY,
    userId INT NOT NULL FOREIGN KEY REFERENCES Users(userId),
    bookId INT NOT NULL FOREIGN KEY REFERENCES Books(BookId),
    quantity INT DEFAULT 1 CHECK (quantity >= 1),
    unitPrice INT NOT NULL,
    createdAt DATETIME DEFAULT GETDATE(),
    updatedAt DATETIME DEFAULT GETDATE()
);

--procedure to fetch a specific user's cart details
ALTER PROCEDURE usp_add_item_to_cart
    @userId INT,
    @bookId INT,
    @quantity INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @unitPrice INT;
        DECLARE @availableQuantity INT;

        -- Fetch the unit price and available quantity from the Books table
        SELECT @unitPrice = Price, @availableQuantity = Quantity
        FROM Books
        WHERE BookId = @bookId;

        IF @unitPrice IS NULL
        BEGIN
            THROW 50001, 'Invalid bookId. Book not found.', 1;
        END																		

        -- Check if the item already exists in the cart
        DECLARE @existingCartItemId INT;
        SELECT @existingCartItemId = cartItemId
        FROM Carts
        WHERE userId = @userId AND bookId = @bookId;

        IF @existingCartItemId IS NOT NULL
        BEGIN
            THROW 50003, 'Item already exists in the cart, try updating.', 1;
        END

        -- Validate the requested quantity against the available quantity
        IF @quantity > @availableQuantity
        BEGIN
            THROW 50002, 'Requested quantity exceeds available stock.', 1;
        END

        -- Insert a new item if it does not exist
        INSERT INTO Carts (userId, bookId, quantity, unitPrice, createdAt, updatedAt)
        VALUES (@userId, @bookId, @quantity, @unitPrice, GETDATE(), GETDATE());
		
		Declare @cartItemId int;
		SELECT @cartItemId=SCOPE_IDENTITY(); -- Return the newly generated cartItemId
		--return @cartItemId; -- Return the newly generated cartItemId
		Select cartItemId,unitPrice from Carts where cartItemId=@cartItemId

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
--usp_add_item_to_cart 2, 1, 3

--procedure to fetch a specific user's cart details
Alter PROCEDURE usp_get_user_cart_details
    @userId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if the user exists
        IF NOT EXISTS (SELECT 1 FROM Users WHERE userId = @userId)
        BEGIN
            THROW 50001, 'Invalid userId. User not found.', 1;
        END

        -- Fetch the cart details for the user
        SELECT 
            c.cartItemId,
            c.userId,
            c.bookId,
            c.quantity,
            c.unitPrice AS bookPrice,
            b.Title AS bookTitle,
            b.Author AS bookAuthor,
            c.createdAt,
            c.updatedAt
        FROM 
            Carts c
        INNER JOIN 
            Books b ON c.bookId = b.BookId
        WHERE 
            c.userId = @userId;

        -- Check if the user has items in the cart
        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50002, 'No items found in the cart for the specified user.', 1;
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

-- Get all users' cart details
CREATE PROCEDURE usp_get_all_users_cart_details
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            c.cartItemId,
            c.userId,
            c.bookId,
            c.quantity,
            c.unitPrice AS bookPrice, 
            b.Title AS bookTitle,
            b.Author AS bookAuthor,
            c.createdAt,
            c.updatedAt
        FROM 
            Carts c
        INNER JOIN 
            Books b ON c.bookId = b.BookId
        ORDER BY 
            c.userId;

        -- Check if any rows were returned
        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50002, 'No items found in the carts.', 1;
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

--procedure to update the cart items
ALTER PROCEDURE usp_update_cart_item
    @userId INT,
    @cartItemId INT,
    @newQuantity INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if the item exists in the cart for the user
        IF NOT EXISTS (SELECT 1 FROM Carts WHERE cartItemId = @cartItemId AND userId = @userId)
        BEGIN
            THROW 50001, 'Cart item not found for the specified user.', 1;
        END

        -- Fetch the available quantity of the book
        DECLARE @availableQuantity INT;
        SELECT @availableQuantity = Quantity
        FROM Books
        WHERE BookId = (SELECT bookId FROM Carts WHERE cartItemId = @cartItemId);

        -- Validate the new quantity against the available quantity
        IF @newQuantity > @availableQuantity
        BEGIN
            THROW 50002, 'Requested quantity exceeds available stock.', 1;
        END

        -- Update the cart item quantity
        UPDATE Carts
        SET quantity = @newQuantity,
            updatedAt = GETDATE()
        WHERE cartItemId = @cartItemId;

        -- Return the updated cart item details
        SELECT 
            cartItemId,
            userId,
            bookId,
            quantity,
            unitPrice AS bookPrice
        FROM Carts
        WHERE cartItemId = @cartItemId;
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

--Remove item from cart
Alter PROCEDURE usp_remove_cart_item
    @userId INT,
    @cartItemId INT
AS
BEGIN
    --SET NOCOUNT ON;

    BEGIN TRY
        -- Check if the cart item exists and belongs to the user
        IF NOT EXISTS (SELECT 1 FROM Carts WHERE cartItemId = @cartItemId AND userId = @userId)
        BEGIN
            THROW 50001, 'Invalid cartItemId or user does not own the cart item.', 1;
        END

        -- Delete the cart item
        DELETE FROM Carts WHERE cartItemId = @cartItemId;

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


