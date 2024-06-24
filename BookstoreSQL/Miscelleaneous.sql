--Fetch Book by Title and AUthor
CREATE PROCEDURE usp_get_book_by_Title_and_Author
    @Author NVARCHAR(MAX),
    @Title NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if input parameters are provided
        IF @Author IS NULL OR @Title IS NULL
        BEGIN
            THROW 50001, 'Author and Title parameters are required.', 1;
        END

        -- Fetch book details
        SELECT 
            BookId, 
            Title, 
            Author, 
            Description, 
            Rating, 
            RatingCount, 
            OriginalPrice, 
            DiscountPercentage, 
            Price, 
            Quantity,
            Image
        FROM 
            Books
        WHERE 
            Author = @Author AND Title = @Title AND IsDeleted = 0;

        -- Check if any rows were returned
        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50002, 'No book found with the specified Author and Title.', 1;
        END
    END TRY
    BEGIN CATCH
        -- Capture error details
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;
        DECLARE @ErrorNumber INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(), 
            @ErrorSeverity = ERROR_SEVERITY(), 
            @ErrorState = ERROR_STATE(),
            @ErrorNumber = ERROR_NUMBER();

        -- Raise the error again to be handled by the calling application
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO


usp_upsert_book 5,'Book 5','Steve Krug','Book 5 by Steve Krug', 9925, 25, 10, 'Book5.png';
select *from Books;

Alter PROCEDURE usp_upsert_book
    @BookId INT,
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
        IF EXISTS (SELECT 1 FROM Books WHERE BookId = @BookId AND IsDeleted = 0)
        BEGIN
            UPDATE Books
            SET 
                Title = @Title, 
                Author = @Author, 
                Description = @Description,
                OriginalPrice = @OriginalPrice, 
                DiscountPercentage = @DiscountPercentage,
                Quantity = @Quantity, 
                Image = @Image
            WHERE BookId = @BookId AND IsDeleted = 0;
			print 'Book is already exists, hence updated it with the provided details';
        END
        ELSE
        BEGIN
            INSERT INTO Books (Title, Author, Description, OriginalPrice, DiscountPercentage, Quantity, Image, IsDeleted)
            VALUES (@Title, @Author, @Description, @OriginalPrice, @DiscountPercentage, @Quantity, @Image, 0);
			print 'New book added';
        END

		Declare @newBookId int;
		SELECT @newBookId=SCOPE_IDENTITY();
		SELECT BookId, Price, Rating, RatingCount from Books where BookId=@BookId and IsDeleted=0;

        --SELECT BookId, Rating, RatingCount 
        --FROM Books 
        --WHERE BookId = COALESCE(@BookId, SCOPE_IDENTITY()) AND IsDeleted = 0;
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


CREATE PROCEDURE usp_get_wishlist_details
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT
            wl.wishListId,
            u.userId,
            u.fullName AS UserName,
            b.BookId,
            b.Title AS BookTitle,
            b.Author AS BookAuthor,
            b.Image AS BookImage,
            b.OriginalPrice AS OriginalPrice,
            b.Price AS FinalPrice,
            wl.createdAt,
            wl.updatedAt
        FROM
            WishLists wl
        INNER JOIN
            Users u ON wl.userId = u.userId
        INNER JOIN
            Books b ON wl.bookId = b.bookId
        WHERE
            wl.isDeleted = 0;
    END TRY
    BEGIN CATCH
        -- Handle errors
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;