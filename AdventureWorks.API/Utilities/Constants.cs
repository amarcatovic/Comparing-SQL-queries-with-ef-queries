namespace AdventureWorks.API.Utilities
{
    public static class Constants
    {
		public static string SP_TEST_SQL = @"SELECT 
													SOH.SalesOrderID,
													SOH.OrderDate,
													SOH.DueDate,
													SOH.ShipDate,
													SOH.SalesOrderNumber,
													SOD.rowguid AS Rowguid,
													SOD.UnitPrice,
													ST.Name
											FROM 
													Sales.SalesOrderDetail SOD
														LEFT JOIN Sales.SalesOrderHeader SOH
															ON SOD.SalesOrderID = SOH.SalesOrderID
														LEFT JOIN Sales.SalesTerritory ST
															ON SOH.TerritoryID = ST.TerritoryID
											WHERE
													YEAR(SOH.OrderDate) IN (2012, 2013, 2014)
											ORDER BY
													SOH.DueDate DESC";

		public static string SQL_ADDRESS_GET = @"SELECT * FROM Person.Address WHERE AddressLine1 LIKE '%street%'";	

		public static string SQL_ADDRESS_POST = @"INSERT INTO [Person].[Address]
																   ([AddressLine1]
																   ,[AddressLine2]
																   ,[City]
																   ,[StateProvinceID]
																   ,[PostalCode]
																   ,[SpatialLocation]
																   ,[rowguid]
																   ,[ModifiedDate])
															 VALUES
																   ('Fakultetska 2'
																   ,NULL
																   ,'Zenica'
																   ,79
																   ,72000
																   ,NULL
																   ,NEWID()
																   ,GETDATE())";

		public static string SQL_ADDRESS_PUT = @"update Person.Address
													SET AddressLine1 = 'Roßstr 7752'
													WHERE AddressLine2 = 'Kreditorenbuchhaltung' AND City = 'Münster'";

		public static string SQL_ADDRESS_JOIN_ADDRESS_TYPE = @"SELECT DISTINCT
																	   [AddressID]
																	  ,[AddressLine1]
																	  ,[AddressLine2]
																	  ,[City]
																	  ,[StateProvinceID]
																	  ,[PostalCode]
																	  ,A.[rowguid]
																	  ,A.[ModifiedDate]
																	  ,AT.AddressTypeID
																	  ,AT.Name
																  FROM [Person].[Address] A 
																	join [Person].[AddressType] AT 
																		ON at.ModifiedDate = AT.ModifiedDate"; 

		public static string SQL_PURCHASEORDERDETAILANDSALESORDERDETAIL = @"SELECT pod.OrderQty, pod.UnitPrice, sod.CarrierTrackingNumber  FROM Purchasing.PurchaseOrderDetail pod
																			JOIN Sales.SalesOrderDetail sod ON sod.ProductID = pod.ProductID";
	}
}
