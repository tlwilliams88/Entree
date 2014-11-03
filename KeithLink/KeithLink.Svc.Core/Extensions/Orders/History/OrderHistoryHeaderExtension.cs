﻿using KeithLink.Svc.Core.Extensions.Enumerations;
using KeithLink.Svc.Core.Models.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.Orders.History {
    public static class OrderHistoryHeaderExtension {
        #region attributes
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
        private const int HEADER_STARTPOS_CTRLNUM = 39;
        private const int HEADER_STARTPOS_INVNUM = 46;
        private const int HEADER_STARTPOS_ORDSTS = 54;
        private const int HEADER_STARTPOS_FUTURE = 55;
        private const int HEADER_STARTPOS_ERRSTS = 56;
        private const int HEADER_STARTPOS_RTENUM = 57;
        private const int HEADER_STARTPOS_STPNUM = 60;
        #endregion

        #region methods
        public static void Parse(this OrderHistoryHeader value, string record) {
            if (record.Length >= HEADER_STARTPOS_ORDSYS + HEADER_LENGTH_ORDSYS) { value.OrderSystem.Parse(record.Substring(HEADER_STARTPOS_ORDSYS, HEADER_LENGTH_ORDSYS)); }
            if (record.Length >= HEADER_STARTPOS_BRANCH + HEADER_LENGTH_BRANCH) { value.BranchId = record.Substring(HEADER_STARTPOS_BRANCH, HEADER_LENGTH_BRANCH); }
            if (record.Length >= HEADER_STARTPOS_CUSTNUM + HEADER_LENGTH_CUSTNUM) { value.CustomerNumber = record.Substring(HEADER_STARTPOS_CUSTNUM, HEADER_LENGTH_CUSTNUM); }

            if (record.Length >= HEADER_STARTPOS_DELVDATE + HEADER_LENGTH_DELVDATE) {
                string deliveryDate = record.Substring(HEADER_STARTPOS_DELVDATE, HEADER_LENGTH_DELVDATE);
                value.DeliveryDate = new DateTime(int.Parse(deliveryDate.Substring(0, 4)),
                                                   int.Parse(deliveryDate.Substring(4, 2)),
                                                   int.Parse(deliveryDate.Substring(6, 2)));
            }

            if (record.Length >= HEADER_STARTPOS_PONUM + HEADER_LENGTH_PONUM) { value.PONumber = record.Substring(HEADER_STARTPOS_PONUM, HEADER_LENGTH_PONUM).Trim(); }
            if (record.Length >= HEADER_STARTPOS_CTRLNUM + HEADER_LENGTH_CTRLNUM) { value.ControlNumber = record.Substring(HEADER_STARTPOS_CTRLNUM, HEADER_LENGTH_CTRLNUM); }
            if (record.Length >= HEADER_STARTPOS_INVNUM + HEADER_LENGTH_INVNUM) { value.InvoiceNumber = record.Substring(HEADER_STARTPOS_INVNUM, HEADER_LENGTH_INVNUM); }
            if (record.Length >= HEADER_STARTPOS_ORDSTS + HEADER_LENGTH_ORDSTS) { value.OrderStatus = record.Substring(HEADER_STARTPOS_ORDSTS, HEADER_LENGTH_ORDSTS); }

            // don't set Future Item flag here
            // don't set Error Status flag here

            if (record.Length >= HEADER_STARTPOS_RTENUM + HEADER_LENGTH_RTENUM) { value.RouteNumber = record.Substring(HEADER_STARTPOS_RTENUM, HEADER_LENGTH_RTENUM); }
            if (record.Length >= HEADER_STARTPOS_STPNUM + HEADER_LENGTH_STPNUM) { value.StopNumber = record.Substring(HEADER_STARTPOS_STPNUM, HEADER_LENGTH_STPNUM); }
        }

        public static void MergeWithEntity(this OrderHistoryHeader value, ref EF.OrderHistoryHeader entity) {
            entity.OrderSystem = value.OrderSystem.ToShortString();
            entity.BranchId = value.BranchId;
            entity.CustomerNumber = value.CustomerNumber;
            entity.InvoiceNumber = value.InvoiceNumber;
            entity.DeliveryDate = value.DeliveryDate;
            entity.PONumber = value.PONumber;
            entity.ControlNumber = value.ControlNumber;
            entity.OrderStatus = value.OrderStatus;
            entity.FutureItems = value.FutureItems;
            entity.ErrorStatus = value.ErrorStatus;
            entity.RouteNumber = value.RouteNumber;
            entity.StropNumber = value.StopNumber;
        }

        public static EF.OrderHistoryHeader ToEntityFrameworkModel(this OrderHistoryHeader value) {
            EF.OrderHistoryHeader retVal = new EF.OrderHistoryHeader();

            retVal.OrderSystem = value.OrderSystem.ToShortString();
            retVal.BranchId = value.BranchId;
            retVal.CustomerNumber = value.CustomerNumber;
            retVal.InvoiceNumber = value.InvoiceNumber;
            retVal.DeliveryDate = value.DeliveryDate;
            retVal.PONumber = value.PONumber;
            retVal.ControlNumber = value.ControlNumber;
            retVal.OrderStatus = value.OrderStatus;
            retVal.FutureItems = value.FutureItems;
            retVal.ErrorStatus = value.ErrorStatus;
            retVal.RouteNumber = value.RouteNumber;
            retVal.StropNumber = value.StopNumber;

            return retVal;
        }
        #endregion
    }
}