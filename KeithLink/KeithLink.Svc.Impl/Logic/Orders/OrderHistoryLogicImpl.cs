using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl.Repository.EF.Operational;
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

        private readonly IOrderHistoryDetailRepository _detailRepo;
        private readonly IOrderHistoryHeaderRepsitory _headerRepo;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region ctor
        public OrderHistoryLogicImpl(IOrderHistoryHeaderRepsitory headerRepo, IOrderHistoryDetailRepository detailRepo, IUnitOfWork unitOfWork) {
            _headerRepo = headerRepo;
            _detailRepo = detailRepo;
            _unitOfWork = unitOfWork;
        }
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
                            if (currentFile != null) {
                                currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                                  where detail.ItemStatus != string.Empty
                                                                  select true).FirstOrDefault();
                                currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                                  where detail.FutureItem == true
                                                                  select true).FirstOrDefault();
                                retVal.Files.Add(currentFile); 
                            }
                            
                            currentFile = new OrderHistoryFile();

                            currentFile.Header.Parse(data);
                            break;
                        case "D":
                            if (currentFile != null) {
                                OrderHistoryDetail orderDetail = new OrderHistoryDetail();
                                orderDetail.Parse(data);

                                currentFile.Details.Add(orderDetail);
                            }
                            break;
                        default:
                            break;
                    }

                } // end of while

                if (currentFile != null) {
                    currentFile.Header.ErrorStatus = (from OrderHistoryDetail detail in currentFile.Details
                                                      where detail.ItemStatus != string.Empty
                                                      select true).FirstOrDefault();
                    currentFile.Header.FutureItems = (from OrderHistoryDetail detail in currentFile.Details
                                                      where detail.FutureItem == true
                                                      select true).FirstOrDefault();
                    retVal.Files.Add(currentFile);
                }
            }

            return retVal;
        }

        public void Save(OrderHistoryFile currentFile) {
            EF.OrderHistoryHeader header = currentFile.Header.ToEntityFrameworkModel();

            header.OrderDetails = new List<EF.OrderHistoryDetail>();

            foreach (OrderHistoryDetail item in currentFile.Details) {
                header.OrderDetails.Add(item.ToEntityFrameworkModel());
            }

            _headerRepo.CreateOrUpdate(header);

            _unitOfWork.SaveChanges();
        }
        #endregion
    }
}
