create database meta_apitude

CREATE TABLE accommodation (
    code varchar(5) NOT NULL,
    data varchar(255) NOT NULL,
    data_updateed varchar(255) NULL,
    PRIMARY KEY (code)
);

CREATE TABLE rateclass (
    code varchar(5) NOT NULL,
    data varchar(255) NOT NULL,
    data_updateed varchar(255) NULL,
    PRIMARY KEY (code)
);

CREATE TABLE boardclass (
    code varchar(5) NOT NULL,
    data varchar(255) NOT NULL,
    data_updateed varchar(255) NULL,
    PRIMARY KEY (code)
);

CREATE TABLE promotion (
    code varchar(5) NOT NULL,
    data varchar(255) NOT NULL,
    data_updateed varchar(255) NULL,
    PRIMARY KEY (code)
);

CREATE TABLE room (
    code varchar(100) NOT NULL,
    type varchar(255) NOT NULL,
    characteristic varchar(255) NULL,
	minPax int,
	maxPax int,
	maxAdults int,
	maxChildren int,
	minAdults int,
	description varchar(255) NULL,
	typeDescription varchar(255) NULL,
	characteristicDescription varchar(255) NULL,
    PRIMARY KEY (code)
);

CREATE TABLE country (
    code varchar(5) NOT NULL,
    name varchar(255) NOT NULL,
    PRIMARY KEY (code)
);

CREATE TABLE state (
    countrycode varchar(5) NOT NULL,
	code varchar(5) NOT NULL,
    name varchar(255) NOT NULL,
    PRIMARY KEY (countrycode,code)
);

CREATE TABLE destination (
    destinationCode varchar(5) NOT NULL,
	destinationName varchar(255) NOT NULL,
    countryCode varchar(5) NOT NULL,
	countryName varchar(255) NOT NULL,
	zoneCode INT,
	zoneName varchar(255) NOT NULL,
	displayName varchar(255) NOT NULL,
    PRIMARY KEY (destinationCode, countryCode, zoneCode)
);



CREATE TABLE Facilities (
    facilitycode int NOT NULL,
	facilityName varchar(255) NOT NULL,
    facilityGroupCode int NOT NULL,
	facilityGroupName varchar(255) NOT NULL,
	value  varchar(255) NULL
);


CREATE TABLE hotel (
    hotelCode varchar(5) NOT NULL,
    countryCode varchar(5) NOT NULL,
    destinationCode varchar(5) NOT NULL,
    hotelData LONGTEXT NULL,
    lastUpdate datetime not null,
    PRIMARY KEY (hotelCode,countryCode,destinationCode)
);

/*
docker run --name mysql_test -p 3306:3306 -e MYSQL_ROOT_PASSWORD=Admin!@12 -d mysql:latest

docker run --name phpmyadmin -d -e PMA_ARBITRARY=1 --link mysql_test:db -p 8080:80 phpmyadmin

docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo

docker run -d --rm --name mysql -p 3306:3306 -e MYSQL_ROOT_PASSWORD=Admin!@12 mysql
 
docker run -d --rm --name phpmyadmin -p 8080:80 --link mysql_db_server:mysql phpmyadmin

ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'Admin!@12'


dotnet new serverless.AspNetCoreWebAPI --name ValuesAPI
dotnet new -i Amazon.Lambda.Templates


docker run --name phpmyadmin_aws -d -e PMA_HOST=agents.cq94obqemon0.us-east-1.rds.amazonaws.com -p 8080:80 phpmyadmin

docker run -d --rm --name mysql_atoz -p 3306:3306 -e MYSQL_ROOT_PASSWORD=Admin!@12 mysql:8.0.30
docker run -d --rm --name phpmyadmin_atoz -p 8080:80 --link mysql_db_server:mysql_atoz phpmyadmin:latest

docker run --name atoz-redis -p 6379:6379 -d redis redis-server --save 60 1 --loglevel warning

*/

create database agency;
use agency;

DROP  table agents;
create table agents(
agentId INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
parentAgentId int,
agencyName nvarchar(200) null,
address nvarchar(200) null,
cityId int null,
countryId int null,
stateId int null,
postalCode varchar(10) null,
email varchar(100) null,
pnoneNumber varchar(20) NULL,
currency varchar(5) NULL,
dateFormate varchar(200) NULL,
isActive BOOLEAN default 1,
isDeleted BOOLEAN default 0,
createdDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
createdBy int,
updatedDate TIMESTAMP NULL,
updatedBy int NULL);

insert into agents
(parentAgentId, agencyName, address, cityId, countryId, stateId, postalCode, email, pnoneNumber, currency, 
dateFormate, createdBy)
VALUES (0, 'A to Z Travel PVT. LTD.', 'address', 1, 1, 1, '123456', 'a2ztravel@gmail.com','+27-1234567890', 'ZAR',
'YYYY-MM-DD', 0);

insert into agents
(parentAgentId, agencyName, address, cityId, countryId, stateId, postalCode, email, pnoneNumber, currency, 
dateFormate, createdBy)
VALUES (0, 'Vatsal Agency', 'Cape Town', 1, 1, 1, '234567', 'vatsal.atoz.sa@gmail.com','+27-2345678901', 'ZAR',
'YYYY-MM-DD', 0);

insert into agents
(parentAgentId, agencyName, address, cityId, countryId, stateId, postalCode, email, pnoneNumber, currency, 
dateFormate, createdBy)
VALUES (0, 'dummy Agency', 'Cape Town', 1, 1, 1, '345678', 'dummy.atoz.sa@gmail.com','+27-3456789012', 'ZAR',
'YYYY-MM-DD', 0);

DROP  table markup;

create table markup(
markupId INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
agentId INT ,
type varchar(200) default 'percentage',
value decimal,
isActive BOOLEAN default 1,
isDeleted BOOLEAN default 0,
createdDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
createdBy int,
updatedDate TIMESTAMP NULL,
updatedBy int NULL);

DROP  table markup;

create table markup(
markupId INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
agentId INT ,
type varchar(200) default 'percentage',
value decimal,
isActive BOOLEAN default 1,
isDeleted BOOLEAN default 0,
createdDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
createdBy int,
updatedDate TIMESTAMP NULL,
updatedBy int NULL);

TRUNCATE TABLE agents;

insert into agents
(parentAgentId, agencyName, address, cityId, countryId, stateId, postalCode, email, pnoneNumber, currency, 
dateFormate, createdBy)
VALUES (0, 'A to Z Travel PVT. LTD.', 'address', 1, 1, 1, '123456', 'a2ztravel@gmail.com','+27-1234567890', 'ZAR',
'YYYY-MM-DD', 0);

insert into agents
(parentAgentId, agencyName, address, cityId, countryId, stateId, postalCode, email, pnoneNumber, currency, 
dateFormate, createdBy)
VALUES (0, 'Vatsal Agency', 'Cape Town', 1, 1, 1, '234567', 'vatsal.atoz.sa@gmail.com','+27-2345678901', 'ZAR',
'YYYY-MM-DD', 0);

insert into agents
(parentAgentId, agencyName, address, cityId, countryId, stateId, postalCode, email, pnoneNumber, currency, 
dateFormate, createdBy)
VALUES (0, 'dummy Agency', 'Cape Town', 1, 1, 1, '345678', 'dummy.atoz.sa@gmail.com','+27-3456789012', 'ZAR',
'YYYY-MM-DD', 0);

TRUNCATE TABLE markup;
INSERT INTO `agency`.`markup` (`agentId`, `value`, `createdBy`) VALUES(1, 10, 1);
INSERT INTO `agency`.`markup` (`agentId`, `value`, `createdBy`) VALUES(2, 10, 1);
