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
        private const int DETAIL_LENGTH_FUTURE = 46;
        private const int DETAIL_LENGTH_WEIGHT = 47;

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
        public OrderHistoryFileReturn ParseMainframeFile(string filePath)
        {
            OrderHistoryFileReturn retVal = new OrderHistoryFileReturn();

            using (System.IO.TextReader txtFile = System.IO.File.OpenText(filePath)) {
                OrderHistoryFile currentFile = null;

                while (txtFile.Peek() != -1) {
                    string data = txtFile.ReadLine();

                    switch (data.Substring(RECORDTYPE_STARTPOS, RECORDTYPE_LENGTH)) {
                        case "H":
                            if (currentFile != null) { retVal.Files.Add(currentFile); }
                            currentFile = new OrderHistoryFile();

                            break;
                        case "D":
                            if (currentFile != null) {

                            }
                            break;
                        default:
                            break;
                    }

                } // end of while

                if (currentFile != null) { retVal.Files.Add(currentFile); }
            }

            return retVal;
        }
        #endregion
    }
}
