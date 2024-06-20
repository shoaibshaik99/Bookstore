create table Carts(
CartId int primary key identity,
UserId int foreign key references Users(UserId),
BookId int foreign key references Books(BookId),
Title nvarchar(max) not null,
Author nvarchar(max) not null,
Image nvarchar(max) not null,
Quantity int default 1 check(Quantity >= 1), 
OriginalBookPrice int not null,
FinalBookPrice int not null,
)