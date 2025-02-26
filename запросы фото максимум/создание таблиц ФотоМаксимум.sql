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

CREATE TABLE Notifications (
    notification_id INT IDENTITY(1,1) PRIMARY KEY, -- ”никальный ID уведомлени€
    request_id INT NOT NULL,                      -- ID заказа
    master_id INT NOT NULL,                       -- ID мастера (если применимо)
    recipient_id INT NOT NULL,                    -- ID получател€ уведомлени€
    message NVARCHAR(500) NOT NULL,               -- “екст уведомлени€
    is_read BIT NOT NULL DEFAULT 0,               -- ‘лаг прочтени€ (0 - не прочитано, 1 - прочитано)
    created_at DATETIME NOT NULL DEFAULT GETDATE(), -- ƒата создани€ уведомлени€
    FOREIGN KEY (request_id) REFERENCES Requests(request_id),
    FOREIGN KEY (master_id) REFERENCES Users(user_id),
    FOREIGN KEY (recipient_id) REFERENCES Users(user_id)
);