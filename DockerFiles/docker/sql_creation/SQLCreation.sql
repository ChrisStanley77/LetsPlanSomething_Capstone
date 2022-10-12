
Create Database LetsPlanSomething
GO

use LetsPlanSomething
CREATE TABLE Accounts (
	Id int IDENTITY(1,1) PRIMARY KEY,
	FirstName varchar(50) NOT NULL,
	LastName varchar(50),
	Email varchar(50) NOT NULL,
	Username varchar(50) NOT NULL,
	Password varchar(50) NOT NULL
);
GO

-- Creates the login AbolrousHazem with password '340$Uuxwp7Mcxo7Khy'.  
CREATE LOGIN capstone_test   
    WITH PASSWORD = 'abc123!!@';  
GO  

-- Creates a database user for the login created above.  
CREATE USER sa FOR LOGIN capstone_test;  
GO 