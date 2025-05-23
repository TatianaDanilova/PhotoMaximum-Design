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
    notification_id INT IDENTITY(1,1) PRIMARY KEY, -- Уникальный ID уведомления
    request_id INT NOT NULL,                      -- ID заказа
    master_id INT NULL,                       -- ID мастера (если применимо)
    recipient_id INT NOT NULL,                    -- ID получателя уведомления
    message NVARCHAR(500) NOT NULL,               -- Текст уведомления
    is_read BIT NOT NULL DEFAULT 0,             
    created_at DATETIME NOT NULL DEFAULT GETDATE(), -- Дата создания уведомления
    FOREIGN KEY (request_id) REFERENCES Requests(request_id),
    FOREIGN KEY (master_id) REFERENCES Users(user_id),
    FOREIGN KEY (recipient_id) REFERENCES Users(user_id)
);

CREATE TABLE Reviews (
    review_id INT PRIMARY KEY IDENTITY,
    client_id INT NOT NULL,
    request_id INT NOT NULL,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    review_text NVARCHAR(500),
    review_date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (client_id) REFERENCES Users(user_id),
    FOREIGN KEY (request_id) REFERENCES Requests(request_id)
);

