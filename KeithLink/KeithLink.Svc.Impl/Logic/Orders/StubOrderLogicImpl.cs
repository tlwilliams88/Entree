using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Exceptions.Orders;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Orders
{
    public class StubOrderLogicImpl : IOrderLogic
    {
        #region attributes
        private bool _fileRead;
        //private List<OrderDetail> _details;
        //private OrderHeader _header;
        private ISocketConnectionRepository _mfConnection;
        #endregion

        #region ctor
        public StubOrderLogicImpl(ISocketConnectionRepository mfCon)
        {
            //_fileRead = false;

            //_details = new List<OrderDetail>();
            //_header = new OrderHeader();

            _mfConnection = mfCon;
        }
        #endregion

        #region methods

        //public void ParseFile(string FileName)
        //{
        //    _header.Branch = "FDF";
        //    _header.ControlNumber = 1;
        //    _header.CustomerNumber = "001001";
        //    _header.DeliveryDate = DateTime.Now.AddDays(1);
        //    _header.OrderCreateDateTime = DateTime.Now;
        //    _header.OrderType = OrderType.NormalOrder;
        //    _header.OrderingSystem = OrderSource.KeithCom;
        //    _header.OrderSendDateTime = DateTime.Now;
        //    _header.PONumber = string.Empty;
        //    _header.Specialinstructions = string.Empty;
        //    _header.InvoiceNumber = string.Empty;
        //    _header.UserId = "KeithLink.Svc.Tests";


        //    _details.Add(new OrderDetail() { 
        //                                     LineNumber = 1,
        //                                     ItemNumber = "000001",
        //                                     UnitOfMeasure = UnitOfMeasure.Case,
        //                                     OrderedQuantity = 1,
        //                                     SellPrice = 1.50,
        //                                     Catchweight = false,
        //                                     ItemChange = LineType.Add,
        //                                     ReplacedOriginalItemNumber = string.Empty,
        //                                     SubOriginalItemNumber = string.Empty,
        //                                     ItemStatus = string.Empty
        //                                    });
        //    _details.Add(new OrderDetail()
        //                                    {
        //                                        LineNumber = 2,
        //                                        ItemNumber = "000002",
        //                                        UnitOfMeasure = UnitOfMeasure.Case,
        //                                        OrderedQuantity = 1,
        //                                        SellPrice = 2.37,
        //                                        Catchweight = false,
        //                                        ItemChange = LineType.Add,
        //                                        ReplacedOriginalItemNumber = string.Empty,
        //                                        SubOriginalItemNumber = string.Empty,
        //                                        ItemStatus = string.Empty
        //                                    });

        //    _fileRead = true;
        //}

        public void SendToHistory(OrderFile order)
        {
            throw new NotImplementedException();
        }

        public void SendToHost(OrderFile order)
        {
            //if (!_fileRead) { throw new ApplicationException("Cannot send file to the host because it has not been read."); }

            // open connection and call program
            _mfConnection.Connect();
            _mfConnection.StartTransaction(order.Header.ControlNumber.ToString().PadLeft(7, '0'));

            string startCode = _mfConnection.Receive();
            if (startCode.Length > 0 && startCode == Constants.MAINFRAME_RECEIVE_STATUS_GO)
            { }
            else
            {
                throw new ApplicationException("Invalid response received while starting the CICS transaction");
            }

            // start the transaction on the mainframe and send the header
            _mfConnection.Send("OTX");
            _mfConnection.Send(order.Header.ToString());

            // wait for a response from the mainframe
            bool waitingForHeaderResponse = true;
            do
            {
                string headerReturnCode = _mfConnection.Receive();

                if (headerReturnCode.Length == 0)
                {
                    throw new InvalidResponseException();
                }
                else
                {
                    switch (headerReturnCode)
                    {
                        case Constants.MAINFRAME_RECEIVE_STATUS_CANCELLED:
                            throw new CancelledTransactionException();
                        case Constants.MAINFRAME_RECEIVE_STATUS_GOOD_RETURN:
                            waitingForHeaderResponse = false;
                            break;
                        case Constants.MAINFRAME_RECEIVE_STATUS_WAITING:
                            waitingForHeaderResponse = true;
                            break;
                        default:
                            throw new CancelledTransactionException();
                    }
                }
            } while (waitingForHeaderResponse);


            // send the order details
            foreach (OrderDetail detail in order.Details)
            {
                _mfConnection.Send(detail.ToString());

                bool waitingForDetailResponse = true;

                do
                {
                    string detailReturnCode = _mfConnection.Receive();

                    if (detailReturnCode.Length == 0)
                    {
                        throw new InvalidResponseException();
                    }
                    else
                    {
                        switch (detailReturnCode)
                        {
                            case Constants.MAINFRAME_RECEIVE_STATUS_CANCELLED:
                                throw new CancelledTransactionException();
                            case Constants.MAINFRAME_RECEIVE_STATUS_GOOD_RETURN:
                                waitingForDetailResponse = false;
                                break;
                            case Constants.MAINFRAME_RECEIVE_STATUS_WAITING:
                                waitingForDetailResponse = true;
                                break;
                            default:
                                throw new InvalidResponseException();
                        }
                    }
                } while (waitingForDetailResponse);
            }

            // send end of records line
            _mfConnection.Send("*EOR");
            string eorReturnCode = _mfConnection.Receive();
            if (eorReturnCode.Length > 0 && eorReturnCode == Constants.MAINFRAME_RECEIVE_STATUS_GOOD_RETURN)
            { }
            else
            {
                throw new InvalidResponseException();
            }

            _mfConnection.Send("STOP");

            _mfConnection.Close();
        }

        #endregion
    }
}
