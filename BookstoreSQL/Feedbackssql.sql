Use BookstoreDB;

CREATE TABLE Feedbacks (
    FeedbackId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    BookId INT NOT NULL FOREIGN KEY REFERENCES Books(BookId),
    Rating INT NOT NULL,
    Review NVARCHAR(MAX) NOT NULL,
    ReviewedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT chk_feedback_rating CHECK (Rating BETWEEN 1 AND 5)
);

select * from feedbacks;

drop table feedbacks
--											Give Feedback
Alter PROCEDURE usp_give_feedback
    @UserId INT,
    @BookId INT,
    @Rating INT,
    @Review NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
		--Ensuring that a user can give only one review per book
		if exists (select 1 from Feedbacks where UserId=@UserId and BookId =@BookId)
		begin
			THROW 50001, 'Review already exists , try updating review instead.', 1;
		end
        INSERT INTO Feedbacks (UserId, BookId, Rating, Review, ReviewedAt, UpdatedAt)
        VALUES (@UserId, @BookId, @Rating, @Review, GETDATE(), GETDATE());

        SELECT * FROM Feedbacks WHERE FeedbackId = SCOPE_IDENTITY();
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

--											Fetch All feedbacks
CREATE PROCEDURE usp_view_all_feedbacks
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT * FROM Feedbacks;
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

--										Fetch feedbacks of a book
CREATE PROCEDURE usp_view_feedbacks_for_book
    @BookId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT * FROM Feedbacks WHERE BookId = @BookId;
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

--											Edit Feedback
CREATE PROCEDURE usp_edit_feedback
    @UserId INT,
    @FeedbackId INT,
    @Rating INT,
    @Review NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE Feedbacks
        SET Rating = @Rating, 
            Review = @Review, 
            UpdatedAt = GETDATE()
        WHERE FeedbackId = @FeedbackId AND UserId = @UserId;

        SELECT * FROM Feedbacks WHERE FeedbackId = @FeedbackId;
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

--												Delete Feedback
Alter PROCEDURE usp_delete_feedback
    @UserId INT,
    @FeedbackId INT
AS
BEGIN
    BEGIN TRY
        DELETE From Feedbacks
        WHERE FeedbackId = @FeedbackId AND UserId = @UserId;
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

usp_give_feedback 1,1,4,'Review'

usp_delete_feedback 1,1
  
select * from feedbacks;