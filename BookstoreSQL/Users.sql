create database BookstoreDB;
go

use BookstoreDB;
go

select * from Users

create table Users (
    userId int primary key identity(1,1),
    fullName varchar(100) not null,
    email varchar(100) not null unique,
    contactNumber varchar(15),
    hashedPassword varchar(256) not null,
	isDeleted bit default 0 not null,
    deletedAt datetime null
);
go

-- Procedure to create a new user
create procedure usp_create_user
    @fullName varchar(100),
    @contactNumber varchar(15),
    @hashedPassword varchar(256),
    @Email varchar(100)
as
begin
    set nocount on;
    begin try
        if exists (select 1 from users where email = @Email)
        begin
            throw 50001, 'A user with this email already exists.', 1;
        end

        insert into users (fullName, contactNumber, hashedPassword, email)
        values (@fullName, @contactNumber, @hashedPassword, @Email);
        
        select scope_identity() as userId;  -- Return the ID of the newly created user
    end try
    begin catch
        select 
            error_number() as error_number,
            error_message() as error_message;
    end catch;
end;
go

-- Procedure to retrieve a user by ID
alter procedure usp_get_user_by_id
    @userId int
as
begin
    set nocount on;
    begin try
        select userId, fullName, contactNumber, email
        from users
        where userId = @userId;
    end try
    begin catch
        select 
            error_number() as error_number,
            error_message() as error_message;
    end catch;
end;
go

-- Procedure to retrieve a user by email
create procedure usp_get_user_by_email
    @Email varchar(100)
as
begin
    set nocount on;
    begin try
        select userId, fullName, contactNumber, email
        from users
        where email = @Email;
    end try
    begin catch
        select 
            error_number() as error_number,
            error_message() as error_message;
    end catch;
end;
go

-- Procedure to retrieve all users
create procedure usp_get_all_users
as
begin
    set nocount on;
    begin try
        select userId, fullName, contactNumber, email
        from users;
    end try
    begin catch
        select 
            error_number() as error_number,
            error_message() as error_message;
    end catch;
end;
go

-- Procedure to update a user
create procedure usp_update_user
    @userId int,
    @fullName varchar(100),
    @contactNumber varchar(15),
    @hashedPassword varchar(256),
    @Email varchar(100)
as
begin
    --set nocount on;
    
    begin try
        begin transaction;
        
        update users
        set 
            fullName = @fullName,
            contactNumber = @contactNumber,
            hashedPassword = @hashedPassword,
            email = @Email
        where 
            userId = @userId;
        
        if @@rowcount = 0
        begin
            -- If no rows were affected, it means the userId does not exist
            select 'no rows affected, user not found' as message;
            rollback transaction;
        end
        else
        begin
            -- If rows were affected, commit the transaction
            select 'update successful' as message;
            commit transaction;
        end
    end try
    begin catch
        -- Handle errors
        if @@trancount > 0
        begin
            rollback transaction;
        end
        
        -- Return the error information
        select 
            error_number() as errorNumber,
            error_message() as errorMessage;
    end catch;
end;
go

-- Procedure to soft delete a user
alter procedure usp_delete_user
    @userId int
as
begin
    --set nocount on;
    begin try
        update users
        set isDeleted = 1,
            deletedAt = getdate()
        where userId = @userId;
        
        if @@rowcount = 0
        begin
            select 'No rows affected' as message;
        end
        else
        begin
            select 'Deleted user (soft delete)' as message;
        end
    end try
    begin catch
        select 
            error_number() as error_number,
            error_message() as error_message;
    end catch;
end;
go

---- Procedure to hard delete a user
--create procedure usp_purge_user
--    @userId int
--as
--begin
--    set nocount on;
--    begin try
--        delete from users
--        where userId = @userId;
        
--        if @@rowcount = 0
--        begin
--            select 'No rows affected' as message;
--        end
--        else
--        begin
--            select 'Deleted user (hard delete)' as message;
--        end
--    end try
--    begin catch
--        select 
--            error_number() as error_number,
--            error_message() as error_message;
--    end catch;
--end;
--go



-- Procedure for user login
alter procedure usp_login_user
    @Email varchar(100),
    @HashedPassword varchar(256)
as
begin
    set nocount on;
    
    begin try
        if exists (select 1 from users where email = @Email and hashedPassword = @HashedPassword)
        begin
            select userId, fullName, email, contactNumber
            from users
            where email = @Email and hashedPassword = @HashedPassword;
        end
        else
        begin
            -- Throw an error if no user exists with the provided details
            throw 50001, 'No user exists with the provided details.', 1;
        end
    end try
    begin catch
        -- Handle errors and return error details
        select 
            error_number() as errorNumber,
            error_message() as errorMessage;
    end catch;
end;
go

-- Procedure to retrieve user details for forgot password
create procedure usp_forgot_password
    @Email varchar(100)
as
begin
    set nocount on;
    begin try
        if not exists (select 1 from users where email = @Email)
        begin
            throw 50002, 'User with this email does not exist.', 1;
        end

        select userId
        from users
        where email = @Email;
    end try
    begin catch
        select 
            error_number() as error_number,
            error_message() as error_message;
    end catch;
end;
go


-- Procedure to reset user password
create procedure usp_reset_password
    @Email varchar(100),
    @NewHashedPassword varchar(256)
as
begin
    --set nocount on;
    begin try
        begin transaction;

        -- Check if the email is valid
        if not exists (select 1 from users where email = @Email)
        begin
            throw 50002, 'User with this email does not exist.', 1;
        end

        -- Update the user's password
        update users
        set hashedPassword = @NewHashedPassword
        where email = @Email;

        -- Check if the password was successfully updated
        if @@rowcount = 0
        begin
            throw 50004, 'Password reset failed.', 1;
        end
        else
        begin
            select 'Password reset successful' as message;
            commit transaction;
        end
    end try
    begin catch
        -- Handle errors
        if @@trancount > 0
        begin
            rollback transaction;
        end
        
        -- Return the error information
        select 
            error_number() as errorNumber,
            error_message() as errorMessage;
    end catch;
end;
go














exec usp_create_user 'Mohammed Shoaib', '9100883881','smdshoaib137','smdshoaib137@gmail.com'
exec usp_create_user 'Santosh', 'santosh@gmail.com', '9110894393', 'santosh'

--drop table Users

--insert into Users(fullName,email,hashedPassword,isDeleted) values ('fullName','email','hashedPassword',0)
select * from Users
