use BookstoreDB;

Select * From Orders;
select * from Users;
select * from Books;
Select * from Carts;
Drop Table Orders;


-- Drop the Orders table if it already exists
IF OBJECT_ID('Orders', 'U') IS NOT NULL
DROP TABLE Orders;

-- Create the Orders table
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    BookId INT NOT NULL,
    Quantity INT CHECK (Quantity >= 1),
    TotalOriginalBookPrice INT NOT NULL,
    TotalFinalBookPrice INT NOT NULL,
    OrderDateTime DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (BookId) REFERENCES Books(BookId)
);

---------------------------Place Order procedure (handles direct book order as well as order from cart)-------------------
Alter PROCEDURE usp_place_order
    @UserId INT,
    @BookId INT = NULL,
    @Quantity INT = NULL,
    @IsDirectOrder BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OrderDetails TABLE (
            OrderId INT,
            UserId INT,
            BookId INT,
            Quantity INT,
            TotalOriginalBookPrice INT,
            TotalFinalBookPrice INT,
            OrderDateTime DATETIME
        );

        IF @IsDirectOrder = 1
        BEGIN
            -- Direct order
            DECLARE @AvailableQuantity INT;
            SELECT @AvailableQuantity = Quantity FROM Books WHERE BookId = @BookId AND IsDeleted = 0;

            IF @AvailableQuantity < @Quantity
            BEGIN
                RAISERROR('Not enough quantity available for BookId %d', 16, 1, @BookId);
                ROLLBACK TRANSACTION;
                RETURN;
            END

            UPDATE Books
            SET Quantity = Quantity - @Quantity
            WHERE BookId = @BookId AND IsDeleted = 0;

            INSERT INTO Orders (UserId, BookId, Quantity, TotalOriginalBookPrice, TotalFinalBookPrice, OrderDateTime, IsDeleted, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.OrderId, INSERTED.UserId, INSERTED.BookId, INSERTED.Quantity, INSERTED.TotalOriginalBookPrice, INSERTED.TotalFinalBookPrice, INSERTED.OrderDateTime
            INTO @OrderDetails
            VALUES (@UserId, @BookId, @Quantity, @Quantity * (SELECT OriginalPrice FROM Books WHERE BookId = @BookId), @Quantity * (SELECT Price FROM Books WHERE BookId = @BookId), GETDATE(), 0, GETDATE(), GETDATE());
        END
        ELSE
        BEGIN
            -- Order from cart
            DECLARE @BookIdFromCart INT, @CartQuantity INT;
            DECLARE cart_cursor CURSOR FOR 
                SELECT bookId, quantity FROM Carts WHERE userId = @UserId

            OPEN cart_cursor;
            FETCH NEXT FROM cart_cursor INTO @BookIdFromCart, @CartQuantity;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                DECLARE @AvailableQuantityCart INT;
                SELECT @AvailableQuantityCart = Quantity FROM Books WHERE BookId = @BookIdFromCart AND IsDeleted = 0;

                IF @AvailableQuantityCart < @CartQuantity
                BEGIN
                    RAISERROR('Not enough quantity available for BookId %d', 16, 1, @BookIdFromCart);
                    ROLLBACK TRANSACTION;
                    RETURN;
                END

                UPDATE Books
                SET Quantity = Quantity - @CartQuantity
                WHERE BookId = @BookIdFromCart;

                INSERT INTO Orders (UserId, BookId, Quantity, TotalOriginalBookPrice, TotalFinalBookPrice, OrderDateTime, IsDeleted, CreatedAt, UpdatedAt)
                OUTPUT INSERTED.OrderId, INSERTED.UserId, INSERTED.BookId, INSERTED.Quantity, INSERTED.TotalOriginalBookPrice, INSERTED.TotalFinalBookPrice, INSERTED.OrderDateTime
                INTO @OrderDetails
                VALUES (@UserId, @BookIdFromCart, @CartQuantity, @CartQuantity * (SELECT OriginalPrice FROM Books WHERE BookId = @BookIdFromCart), @CartQuantity * (SELECT Price FROM Books WHERE BookId = @BookIdFromCart), GETDATE(), 0, GETDATE(), GETDATE());

                FETCH NEXT FROM cart_cursor INTO @BookIdFromCart, @CartQuantity;
            END

            CLOSE cart_cursor;
            DEALLOCATE cart_cursor;

            -- Clear the cart
            DELETE FROM Carts WHERE userId = @UserId;
        END

        COMMIT TRANSACTION;

        -- Return the order details
        SELECT * FROM @OrderDetails;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

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

--checking the place order procedure
EXEC usp_place_order @UserId = 1, @BookId = 1, @Quantity = 2, @IsDirectOrder = 0;

usp_add_item_to_cart 3,1,3

EXEC usp_place_order @UserId = 3, @IsDirectOrder = 0;

select * from Orders

--						Procedure to fetch all the orders of all the users
CREATE PROCEDURE usp_get_all_orders
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            o.OrderId,
            o.UserId,
            u.fullName AS UserName,
            o.BookId,
            b.Title AS BookTitle,
            o.Quantity,
            o.TotalOriginalBookPrice,
            o.TotalFinalBookPrice,
            o.OrderDateTime,
            o.IsDeleted,
            o.CreatedAt,
            o.UpdatedAt
        FROM 
            Orders o
        INNER JOIN 
            Users u ON o.UserId = u.UserId
        INNER JOIN 
            Books b ON o.BookId = b.BookId
        WHERE 
            o.IsDeleted = 0;
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

-- Fetch all orders
EXEC usp_get_all_orders;

-- Fetch orders for a specific user with UserId = 1
EXEC usp_get_orders_by_user @UserId = 1;


--						Procedure to Fetch an users' orders

CREATE PROCEDURE usp_get_orders_by_user
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT 
            o.OrderId,
            o.UserId,
            u.fullName AS UserName,
            o.BookId,
            b.Title AS BookTitle,
            o.Quantity,
            o.TotalOriginalBookPrice,
            o.TotalFinalBookPrice,
            o.OrderDateTime,
            o.IsDeleted,
            o.CreatedAt,
            o.UpdatedAt
        FROM 
            Orders o
        INNER JOIN 
            Users u ON o.UserId = u.UserId
        INNER JOIN 
            Books b ON o.BookId = b.BookId
        WHERE 
            o.UserId = @UserId AND o.IsDeleted = 0;
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

--									Procedure to Cancel Order

CREATE PROCEDURE usp_cancel_order
    @UserId INT,
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Check if the order exists, belongs to the user, and is not already canceled
        IF EXISTS (SELECT 1 FROM Orders WHERE OrderId = @OrderId AND UserId = @UserId AND IsDeleted = 0)
        BEGIN
            -- Update the order's IsDeleted status
            UPDATE Orders
            SET IsDeleted = 1, UpdatedAt = GETDATE()
            WHERE OrderId = @OrderId;

            -- Restore the book quantities to the inventory
            DECLARE @BookId INT, @Quantity INT;
            SELECT @BookId = BookId, @Quantity = Quantity FROM Orders WHERE OrderId = @OrderId;

            UPDATE Books
            SET Quantity = Quantity + @Quantity
            WHERE BookId = @BookId AND IsDeleted = 0;

            COMMIT TRANSACTION;

            -- Return the canceled order details
            SELECT * FROM Orders WHERE OrderId = @OrderId;
        END
        ELSE
        BEGIN
            RAISERROR('Order not found, does not belong to the user, or is already canceled.', 16, 1);
            ROLLBACK TRANSACTION;
        END
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

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
