
CREATE PROCEDURE [orders].[usp_GetLastFiveOrdersForItem]
       @BranchId            CHAR(3),
       @CustomerNumber      CHAR(6),
       @ItemNumber          CHAR(6)
AS
       SELECT 
              TOP 5
              *
       FROM
              ORDERS.ORDERHISTORYHEADER H
       INNER JOIN
              ORDERS.ORDERHISTORYDETAIL D
       ON
              (D.ORDERHISTORYHEADER_ID = H.ID)
       WHERE
              H.BRANCHID = @BranchId
       AND
              H.CUSTOMERNUMBER = @CustomerNumber
       AND
              D.ITEMNUMBER = @ItemNumber
       ORDER BY
              CAST(H.DELIVERYDATE AS DATETIME) DESC