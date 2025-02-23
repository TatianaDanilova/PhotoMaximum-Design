create table Roles(
role_id int not null primary key, 
role_name nvarchar(100) not null)

create table Users(
[user_id] int not null primary key,
role_id int not null foreign key references Roles(role_id),
fio nvarchar(100) not null, 
phone nvarchar(12) not null,
[login] nvarchar(100) not null,
pass nvarchar(100) not null)

create table [Types](
[type_id] int not null primary key,
[type_name] nvarchar(100) not null)

create table Requests(
request_id int not null primary key,
[type_id] int not null foreign key references [Types]([type_id]),
client_id int not null foreign key references Users([user_id]),
master_id int null foreign key references Users([user_id]),
size nvarchar(100) not null,
photo nvarchar(500) not null,
price int not null,
comment nvarchar(500) null,
[status] nvarchar(50) not null,
date_start datetime null,
date_end datetime null)