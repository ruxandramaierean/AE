IF (DB_ID(N'Proiect') IS NULL)
	CREATE DATABASE Proiect
GO

USE Proiect
GO

IF OBJECT_ID ('Products') IS NULL
	CREATE TABLE Products
	(
	Id INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_Product PRIMARY KEY,
	[Name] NVARCHAR(256) NOT NULL,
	[Description] NVARCHAR(2000) NOT NULL, 
	Price NUMERIC(20, 2) NOT NULL,
	IsAvailable BIT NOT NULL,
	ImagePath NVARCHAR(1000) NULL
	)
GO

IF OBJECT_ID ('users') IS NULL
	create table users (
	Id INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_users PRIMARY KEY,
	username varchar(20) NOT NULL,
	passwrd varchar(20) not null,
	last_login date);
	-- pass = admin
	insert into users (username, passwrd) values ('admin', 'YWRtaW4=');
	-- pass = ruxi
	insert into users (username, passwrd) values ('ruxi', 'cnV4aQ==');
	-- pass = gabi
	insert into users (username, passwrd) values ('gabi', 'Z2FiaQ==');
GO