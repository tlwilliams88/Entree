using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Interface.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class OrderHistoryLogicImpl : IOrderHistoryLogic {
        #region attributes
        private const int RECORDTYPE_LENGTH = 1;
        private const int RECORDTYPE_STARTPOS = 0;

        private const int DETAIL_LENGTH_ITEMNUM = 6;
        private const int DETAIL_LENGTH_LINNUM = 5;
        private const int DETAIL_LENGTH_ORDQTY = 3;
        private const int DETAIL_LENGTH_SHIPQTY = 5;
        private const int DETAIL_LENGTH_UOM = 1;
        private const int DETAIL_LENGTH_SELLPRICE = 10;
        private const int DETAIL_LENGTH_CWT = 1;
        private const int DETAIL_LENGTH_ITEMDEL = 1;
        private const int DETAIL_LENGTH_SUBORG = 6;
        private const int DETAIL_LENGTH_REPLORG = 6;
        private const int DETAIL_LENGTH_ITEMSTS = 1;
        private const int DETAIL_LENGTH_FUTURE = 1;
        private const int DETAIL_LENGTH_WEIGHT = 8;

        private const int DETAIL_STARTPOS_ITEMNUM = 1;
        private const int DETAIL_STARTPOS_LINNUM = 7;
        private const int DETAIL_STARTPOS_ORDQTY = 12;
        private const int DETAIL_STARTPOS_SHIPQTY = 15;
        private const int DETAIL_STARTPOS_UOM = 20;
        private const int DETAIL_STARTPOS_SELLPRICE = 21;
        private const int DETAIL_STARTPOS_CWT = 31;
        private const int DETAIL_STARTPOS_ITEMDEL = 32;
        private const int DETAIL_STARTPOS_SUBORG = 33;
        private const int DETAIL_STARTPOS_REPLORG = 39;
        private const int DETAIL_STARTPOS_ITEMSTS = 45;
        private const int DETAIL_STARTPOS_FUTURE = 46;
        private const int DETAIL_STARTPOS_WEIGHT = 47;

        private const int HEADER_LENGTH_ORDSYS = 1; 
        private const int HEADER_LENGTH_BRANCH = 3; 
        private const int HEADER_LENGTH_CUSTNUM = 6; 
        private const int HEADER_LENGTH_DELVDATE = 8; 
        private const int HEADER_LENGTH_PONUM = 20; 
        private const int HEADER_LENGTH_CTRLNUM = 7; 
        private const int HEADER_LENGTH_INVNUM = 8; 
        private const int HEADER_LENGTH_ORDSTS = 1; 
        private const int HEADER_LENGTH_FUTURE = 1; 
        private const int HEADER_LENGTH_ERRSTS = 1;
        private const int HEADER_LENGTH_RTENUM = 3;
        private const int HEADER_LENGTH_STPNUM = 3;
        
        private const int HEADER_STARTPOS_ORDSYS = 1; 
        private const int HEADER_STARTPOS_BRANCH = 2; 
        private const int HEADER_STARTPOS_CUSTNUM = 5; 
        private const int HEADER_STARTPOS_DELVDATE = 11; 
        private const int HEADER_STARTPOS_PONUM = 19; 
        private const int HEADER_STARTPOS_CTRLNUM = 29; 
        private const int HEADER_STARTPOS_INVNUM = 36; 
        private const int HEADER_STARTPOS_ORDSTS = 44; 
        private const int HEADER_STARTPOS_FUTURE = 45; 
        private const int HEADER_STARTPOS_ERRSTS = 46;
        private const int HEADER_STARTPOS_RTENUM = 47;
        private const int HEADER_STARTPOS_STPNUM = 50;
        #endregion

        #region methods
        private OrderSource GetOrderSource(string orderSource) {
            switch (orderSource) {
                case "B":
                    return OrderSource.Entree;
                case "K":
                    return OrderSource.DSR;
                default:
                    return OrderSource.Other;
            }
        }

        private OrderHistoryDetail ParseDetail(string record) {
            OrderHistoryDetail retVal = new OrderHistoryDetail();

            if (record.Length > DETAIL_STARTPOS_ITEMNUM + DETAIL_LENGTH_ITEMNUM) {retVal.ItemNumber = record.Substring(DETAIL_STARTPOS_ITEMNUM, DETAIL_LENGTH_ITEMNUM);}

            if (record.Length > DETAIL_STARTPOS_LINNUM + DETAIL_LENGTH_LINNUM) {
                int lineNumber;
                int.TryParse(record.Substring(DETAIL_STARTPOS_LINNUM, DETAIL_LENGTH_LINNUM), out lineNumber);
                retVal.LineNumber = lineNumber;
            }

            if (record.Length > DETAIL_STARTPOS_ORDQTY + DETAIL_LENGTH_ORDQTY) {
                int ordQty;
                int.TryParse(record.Substring(DETAIL_STARTPOS_ORDQTY, DETAIL_LENGTH_ORDQTY), out ordQty);
                retVal.OrderQuantity = ordQty;
            }

            if (record.Length > DETAIL_STARTPOS_SHIPQTY + DETAIL_LENGTH_SHIPQTY) {
                int shippedQty;
                int.TryParse(record.Substring(DETAIL_STARTPOS_SHIPQTY, DETAIL_LENGTH_SHIPQTY), out shippedQty);
                retVal.ShippedQuantity = shippedQty;
            }

            if (record.Length > DETAIL_STARTPOS_UOM + DETAIL_LENGTH_UOM) {retVal.UnitOfMeasure = (record.Substring(DETAIL_STARTPOS_UOM, DETAIL_LENGTH_UOM) == "P" ? UnitOfMeasure.Package : UnitOfMeasure.Case);}

            if (record.Length > DETAIL_STARTPOS_SELLPRICE + DETAIL_LENGTH_SELLPRICE) {
                double sellPrice;
                double.TryParse(record.Substring(DETAIL_STARTPOS_SELLPRICE, DETAIL_LENGTH_SELLPRICE), out sellPrice);
                retVal.SellPrice = sellPrice;
            }

            if (record.Length > DETAIL_STARTPOS_CWT + DETAIL_LENGTH_CWT) {retVal.CatchWeight = (record.Substring(DETAIL_STARTPOS_CWT, DETAIL_LENGTH_CWT) == "Y");}
            if (record.Length > DETAIL_STARTPOS_ITEMDEL + DETAIL_LENGTH_ITEMDEL) {retVal.ItemDeleted = (record.Substring(DETAIL_STARTPOS_ITEMDEL, DETAIL_LENGTH_ITEMDEL) == "Y");}
            if (record.Length > DETAIL_STARTPOS_SUBORG + DETAIL_LENGTH_SUBORG) {retVal.SubbedOriginalItemNumber = record.Substring(DETAIL_STARTPOS_SUBORG, DETAIL_LENGTH_SUBORG);}
            if (record.Length > DETAIL_STARTPOS_REPLORG + DETAIL_LENGTH_REPLORG) {retVal.ReplacedOriginalItemNumber = record.Substring(DETAIL_STARTPOS_REPLORG, DETAIL_LENGTH_REPLORG);}
            if (record.Length > DETAIL_STARTPOS_ITEMSTS + DETAIL_LENGTH_ITEMSTS) {retVal.ItemStatus = record.Substring(DETAIL_STARTPOS_ITEMSTS, DETAIL_LENGTH_ITEMSTS);}
            if (record.Length > DETAIL_STARTPOS_FUTURE + DETAIL_LENGTH_FUTURE) {retVal.FutureItem = (record.Substring(DETAIL_STARTPOS_FUTURE, DETAIL_LENGTH_FUTURE) == "Y");}

            if (record.Length > DETAIL_STARTPOS_WEIGHT + DETAIL_LENGTH_WEIGHT) {
                double weight;
                double.TryParse(record.Substring(DETAIL_STARTPOS_WEIGHT, DETAIL_LENGTH_WEIGHT), out weight);
                retVal.TotalShippedWeight = weight;
            }

            return retVal;
        }

        private OrderHistoryHeader ParseHeader(string record) {
            OrderHistoryHeader retVal = new OrderHistoryHeader();

            if (record.Length > HEADER_STARTPOS_ORDSYS + HEADER_LENGTH_ORDSYS) { retVal.OrderSystem = GetOrderSource(record.Substring(HEADER_STARTPOS_ORDSYS, HEADER_LENGTH_ORDSYS));}
            if (record.Length > HEADER_STARTPOS_BRANCH + HEADER_LENGTH_BRANCH) {retVal.BranchId = record.Substring(HEADER_STARTPOS_BRANCH, HEADER_LENGTH_BRANCH);}
            if (record.Length > HEADER_STARTPOS_CUSTNUM + HEADER_LENGTH_CUSTNUM) {retVal.CustomerNumber = record.Substring(HEADER_STARTPOS_CUSTNUM, HEADER_LENGTH_CUSTNUM);}
            
            if (record.Length > HEADER_STARTPOS_DELVDATE + HEADER_LENGTH_DELVDATE) {
                string deliveryDate = record.Substring(HEADER_STARTPOS_DELVDATE, HEADER_LENGTH_DELVDATE);
                retVal.DeliveryDate = new DateTime(int.Parse(deliveryDate.Substring(0,4)),
                                                   int.Parse(deliveryDate.Substring(4,2)),
                                                   int.Parse(deliveryDate.Substring(6,2)));
            }

            if (record.Length > HEADER_STARTPOS_PONUM + HEADER_LENGTH_PONUM) {retVal.PONumber = record.Substring(HEADER_STARTPOS_PONUM, HEADER_LENGTH_PONUM);}
            if (record.Length > HEADER_STARTPOS_CTRLNUM + HEADER_LENGTH_CTRLNUM) {retVal.ControlNumber = record.Substring(HEADER_STARTPOS_CTRLNUM, HEADER_LENGTH_CTRLNUM);}
            if (record.Length > HEADER_STARTPOS_INVNUM + HEADER_LENGTH_INVNUM) {retVal.InvoiceNumber = record.Substring(HEADER_STARTPOS_INVNUM, HEADER_LENGTH_INVNUM);}
            if (record.Length > HEADER_STARTPOS_ORDSTS + HEADER_LENGTH_ORDSTS) {retVal.OrderStatus = record.Substring(HEADER_STARTPOS_ORDSTS, HEADER_LENGTH_ORDSTS);}
            
            // don't set Future Item flag here
            // don't set Error Status flag here

            if (record.Length > HEADER_STARTPOS_RTENUM + HEADER_LENGTH_RTENUM) {retVal.RouteNumber = record.Substring(HEADER_STARTPOS_RTENUM, HEADER_LENGTH_RTENUM);}
            if (record.Length > HEADER_STARTPOS_STPNUM + HEADER_LENGTH_STPNUM) {retVal.StopNumber = record.Substring(HEADER_STARTPOS_STPNUM, HEADER_LENGTH_STPNUM);}

            return retVal;
        }

        public OrderHistoryFileReturn ParseMainframeFile(string filePath)
        {
            OrderHistoryFileReturn retVal = new OrderHistoryFileReturn();

            using (System.IO.TextReader txtFile = System.IO.File.OpenText(filePath)) {
                OrderHistoryFile currentFile = null;

                while (txtFile.Peek() != -1) {
                    string data = txtFile.ReadLine();

                    switch (data.Substring(RECORDTYPE_STARTPOS, RECORDTYPE_LENGTH)) {
                        case "H":
                            if (currentFile != null) {
                                currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                                  where detail.ItemStatus != string.Empty
                                                                  select true).First();
                                currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                                  where detail.FutureItem
                                                                  select true).First();
                                retVal.Files.Add(currentFile); 
                            }
                            
                            currentFile = new OrderHistoryFile();

                            currentFile.Header = ParseHeader(data);
                            break;
                        case "D":
                            if (currentFile != null) {
                                currentFile.Details.Add(ParseDetail(data));
                            }
                            break;
                        default:
                            break;
                    }

                } // end of while

                if (currentFile != null) {
                    currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                      where detail.ItemStatus != string.Empty
                                                      select true).First();
                    currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                      where detail.FutureItem
                                                      select true).First();
                    retVal.Files.Add(currentFile);
                }
            }

            return retVal;
        }
        #endregion
    }
}
