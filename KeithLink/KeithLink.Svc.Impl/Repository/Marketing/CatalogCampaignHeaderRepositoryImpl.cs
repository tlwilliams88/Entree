using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Models.Marketing;

using KeithLink.Svc.Impl.Repository.DataConnection;

using Dapper;
using System.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Marketing
{
    public class CatalogCampaignHeaderRepositoryImpl : DapperDatabaseConnection, ICatalogCampaignHeaderRepository
    {
        #region attributes
        private const string COMMAND_GET_ONE = "Marketing.GetCatalogCampaignHeader";
        private const string COMMAND_GET_ALL = "Marketing.GetAllCatalogCampaignHeader";

        private const string COMMAND_GET_BY_URI = "Marketing.GetCatalogCampaignHeaderByUri";

        private const string COMMAND_ADD = "Marketing.AddCatalogCampaignHeader";
        private const string COMMAND_UPDATE = "Marketing.UpdateCatalogCampaignHeader";
        #endregion

        #region constructor
        public CatalogCampaignHeaderRepositoryImpl() : base(Configuration.BEKDBConnectionString)
        {
            
        }
        #endregion

        #region get
        public CatalogCampaignHeader GetHeader(int id)
        {
            return ReadOne<CatalogCampaignHeader>(new CommandDefinition(
                COMMAND_GET_ONE, 
                new { @id = id }, 
                commandType: CommandType.StoredProcedure
            ));
        }

        public CatalogCampaignHeader GetByUri(string uri)
        {
            return ReadOne<CatalogCampaignHeader>(new CommandDefinition(
                COMMAND_GET_BY_URI,
                new { @Uri = uri },
                commandType: CommandType.StoredProcedure
            ));
        }

        public List<CatalogCampaignHeader> GetAll()
        {
            var campaigns = Read<CatalogCampaignHeader>(new CommandDefinition(
                COMMAND_GET_ALL,
                commandType: CommandType.StoredProcedure
            ));
            return campaigns;
        }
        #endregion

        #region save
        public Int64 CreateOrUpdate(CatalogCampaignHeader header)
        {
            CatalogCampaignHeader oldheader = ReadOne<CatalogCampaignHeader>(new CommandDefinition(
                                                COMMAND_GET_BY_URI,
                                                new { @Uri = header.Uri },
                                                commandType: CommandType.StoredProcedure
                                              ));

            Int64 Id = 0;

            if (oldheader == null)
            {
                Id = ExecuteScalarCommand<int>(new CommandDefinition(
                    COMMAND_ADD,
                    new
                    {
                        @Description = header.Description,
                        @StartDate = header.StartDate,
                        @EndDate = header.EndDate,
                        @Uri = header.Uri
                    },
                    commandType: CommandType.StoredProcedure
                ));
            }
            else
            {
                Id = ExecuteScalarCommand<int>(new CommandDefinition(
                    COMMAND_UPDATE,
                    new
                    {
                        @Description = header.Description,
                        @Active = header.Active,
                        @StartDate = header.StartDate,
                        @EndDate = header.EndDate,
                        @Uri = header.Uri
                    },
                    commandType: CommandType.StoredProcedure
                ));
            }

            return Id;
        }
        #endregion

        #region delete
        #endregion
    }
}
