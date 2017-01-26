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
            return Read<CatalogCampaignHeader>(new CommandDefinition(
                COMMAND_GET_ALL
            ));
        }
        #endregion

        #region save
        #endregion

        #region delete
        #endregion
    }
}
