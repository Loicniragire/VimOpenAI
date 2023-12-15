-- Start transaction
START TRANSACTION;

SET FOREIGN_KEY_CHECKS = 0;
	TRUNCATE TABLE `mydatabase`.`FinancingEntity`;
	TRUNCATE TABLE `mydatabase`.`Consumers`;
	TRUNCATE TABLE `mydatabase`.`RetailStore`;
	TRUNCATE TABLE `mydatabase`.`RetailStoreContract`;
	TRUNCATE TABLE `mydatabase`.`Merchandise`;
	TRUNCATE TABLE `mydatabase`.`LeaseStatus`;
	TRUNCATE TABLE `mydatabase`.`Leases`;
	TRUNCATE TABLE `mydatabase`.`RetailStoreMerchandise`;

	TRUNCATE TABLE `mydatabase`.`ConsumerAccounts`;
	TRUNCATE TABLE `mydatabase`.`LeaseTerms`;
	TRUNCATE TABLE `mydatabase`.`PaymentSchedules`;
	TRUNCATE TABLE `mydatabase`.`PaymentMethods`;
	TRUNCATE TABLE `mydatabase`.`CreditCards`;
	TRUNCATE TABLE `mydatabase`.`BankAccounts`;
	TRUNCATE TABLE `mydatabase`.`Ledgers`;
SET FOREIGN_KEY_CHECKS = 1;

INSERT INTO `mydatabase`.`FinancingEntity`(`Name`,`City`,`State`)
VALUES
	("FinSuccess","Philadelphia","PA"),
	("Tech","Houston","TX"),
	("Prog","Salt Lake","UT"),
	("Paypal","New York","NY"),
	("Amazon","Los Angeles","LA");

Insert INTO `mydatabase`.`Consumers`(`Name`, `ActiveLeaseCount`)
VALUES
	("Loic Niragire", 1),
    ("John Doe", 3),
    ("Jona Williams", 4),
	("Joe Smith", 1),
	("David Love", 2);
    
Insert INTO `mydatabase`.`RetailStore`(`Name`,`City`,`State`)
VALUES
	("BestBuy","Philadelphia","PA"),
    ("Lowes","Houston","TX"),
    ("John Deer","Cleveland","OH"),
	("Way Fair", "New York", "NY"),
	("Home Depot", "Harrisburg", "PA");
    
Insert INTO `mydatabase`.`RetailStoreContract`(`FinancingEntityID`,`RetailStoreID`)
VALUES
	(1,1),
    (2,1),
    (3,2),
	(4,2),
	(4,3),
	(4,4),
	(4,5),
	(5,5),
	(5,4),
	(5,2),
	(5,1);
    
Insert INTO `mydatabase`.`Merchandise`(`Name`,`Description`,`Price`)
VALUES
	("Laptop","Computer",1500.00),
    ("TV","Television",850.00),
    ("Tractor","Machine",2500.00),
	("Refrigirator","Appliance",1500.00),
	("Dish washer", "Appliance",1240.00);
    
Insert INTO `mydatabase`.`LeaseStatus`(`Status`,`Description`)
Values
	("Initated","Consumer initiated application process"),
    ("Accepted","Financing entity approved application request"),
    ("Denied","Financing entity declined application request"),
    ("Funded","Financing entity issued full payment to the retail store"),
	("Delinquent","Lease is behind payment"),
    ("Active","Lease in repayment"),
    ("Paid","Lease paid in full");
    
Insert INTO `mydatabase`.`Leases`(`ConsumerID`,`FinancingEntityID`,`RetailStoreID`,`MerchandiseID`,`OpenDate`,`StatusID`)
VALUES
	(1,1,1,2,"2023-05-15",4),
    (2,2,2,1,"2023-02-12",3),
    (3,3,2,2,"2023-07-01",5),
    (4,5,1,5,"2023-07-01",1),
    (5,4,4,2,"2023-07-01",2);
    
Insert INTO `mydatabase`.`RetailStoreMerchandise`(`MerchandiseID`,`RetailStoreContractID`)
VALUES
	(1,1),
    (1,1),
    (2,3),
    (4,6),
    (5,7);

-- TODO
Insert INTO `mydatabase`.`ConsumerAccounts`(`ConsumerID`,`LeaseID`)
VALUES
(1,1),
(2,2),
(3,3),
(4,4),
(5,5);

Insert INTO `mydatabase`.`LeaseTerms` (`LeaseID`,`DurationInMonths`,`CashPrice`,`InterestRate`,`MonthlyPayment`,`PaymentStartDateUTC`,`CreatedDateUTC`)
VALUES
(1,6,3000.00,0.01,200.00,"2023-09-01","2023-06-01"),
(2,6,2500.00,0.01,200.00,"2023-09-01","2023-06-01"),
(3,6,2500.00,0.01,200.00,"2023-09-01","2023-06-01"),
(4,6,3000.00,0.01,200.00,"2023-09-01","2023-06-01"),
(5,6,2500.00,0.01,200.00,"2023-09-01","2023-06-01");
Insert INTO `mydatabase`.`CreditCards`(`ConsumerAccountID`,`CardToken`,`ExpirationDate`,`BillingZipCode`)
VALUES
(1,"dfdnkgjnbfgjnbkf","2023-08-28","17022"),
(2,"fdbjhldfgbnjfgkb","2023-09-05","17022"),
(3,"gbnfkjgbnkfgjknj","2023-07-01","17022"),
(4,"fgbnfkgbjktgiero","2023-01-01","17022"),
(5,"fdbnkfgjbniuierk","2023-12-20","17022");

Insert INTO `mydatabase`.`BankAccounts`(`ConsumerAccountID`,`AccountToken`,`RountingNumber`)
VALUES
(1,"fdbnfgkbnijtrbni","123456789"),
(2,"fdbnfgkbnijtrbni","123456789"),
(3,"fdbnfgkbnijtrbni","123456789"),
(4,"fdbnfgkbnijtrbni","123456789"),
(5,"fdbnfgkbnijtrbni","123456789");


Insert INTO `mydatabase`.`PaymentMethods`(`DefaultPayment`,`SecondaryPayment`,`MethodName`)
VALUES
(1,0,"CreditCard"),
(0,1,"BankAccount"),
(0,1,"CreditCard"),
(1,0,"BankAccount");

Insert INTO `mydatabase`.`PaymentSchedules`(`ConsumerAccountID`,`PaymentDate`,`ProcessAmount`,`PaymentMethodID`,`CreditCardID`,`BankAccountID`,`CreatedDateUTC`)
VALUES
(1,"2023-10-1",200.00,1,1,null,"2023-12-01"),
(2,"2023-10-1",200.00,1,1,null,"2023-09-01"),
(3,"2023-10-1",200.00,1,2,null,"2023-01-01"),
(4,"2023-10-1",200.00,1,1,null,"2023-04-01"),
(5,"2023-10-1",200.00,1,2,null,"2023-06-01");


Insert INTO `mydatabase`.`Ledgers`(`ConsumerAccountID`,`DebitAmount`,`CreditAmount`,`Description`,`CreatedDateUTC`,`CreatedBy`)
VALUES
(1,null,125.00,"payment","2023-10-10","User"),
(2,30.00,null,"credit","2023-10-10","User"),
(3,345.00,null,"credit","2023-12-09","User"),
(4,450.00,null,"credit","2023-10-01","User"),
(5,null,650.00,"payment","2023-08-01","User");
	
-- Commit the transaction
COMMIT;
