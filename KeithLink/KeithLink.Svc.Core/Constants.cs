using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
{
    public class Constants
    {
        public static string ReturnSizeQueryStringParam { get { return "size"; } }
        public static string ReturnFromQueryStringParam { get { return "from"; } }

        //public const string AD_GUEST_CONTAINER = "_bek_guest";
        public const string AD_GUEST_FIRSTNAME = "guest";
        public const string AD_GUEST_LASTNAME = "account";

        public const string BRANCH_FAM = "FAM";
        public const string BRANCH_FAQ = "FAQ";
        public const string BRANCH_FAR = "FAR";
        public const string BRANCH_FDF = "FDF";
        public const string BRANCH_FHS = "FHS";
        public const string BRANCH_FLR = "FLR";
        public const string BRANCH_FOK = "FOK";
        public const string BRANCH_FSA = "FSA";
        public const string BRANCH_GOF = "GOF";

        public const string REGEX_AD_ILLEGALCHARACTERS = @"[/\\\[\]:;\|=,\+\*\?<>@']";
        public const string REGEX_BENEKEITHEMAILADDRESS = "@benekeith.com";
        public const string REGEX_PASSWORD_PATTERN = @"^.*(?=.*[a-z])(?=.*[A-Z])(?=.*[\d]).*$";

        public const string ROLE_EXTERNAL_ACCOUNTING = "Accounting";
        public const string ROLE_EXTERNAL_GUEST = "Guest";
        public const string ROLE_EXTERNAL_OWNER = "Owner";
        public const string ROLE_EXTERNAL_PURCHASINGAPPROVER = "Approver";
        public const string ROLE_EXTERNAL_PURCHASINGBUYER = "Buyer";

        public const string ROLE_CORPORATE_ADMIN = "CORP-LS-SYS-AC-Entree_Admins";
        public const string ROLE_CORPORATE_SECURITY = "CORP-DIS-Security";

        public const string ROLE_INTERNAL_CSR_FAQ = "fabq-ls-csv-all";
        public const string ROLE_INTERNAL_CSR_FAM = "fama-ls-csv-all";
        public const string ROLE_INTERNAL_CSR_FDF = "fdfw-ls-csv-all";
        public const string ROLE_INTERNAL_CSR_FHS = "fhst-ls-csv-all";
        public const string ROLE_INTERNAL_CSR_FLR = "flrk-ls-csv-all";
        public const string ROLE_INTERNAL_CSR_FSA = "fsan-ls-csv-all";
        public const string ROLE_INTERNAL_CSR_FOK = "fokc-ls-csv-all";

        public const string ROLE_INTERNAL_DSM_FAQ = "fabq-ls-sys-ac-dsms";
        public const string ROLE_INTERNAL_DSM_FAM = "fama-ls-sys-ac-dsms";
        public const string ROLE_INTERNAL_DSM_FDF = "fdfw-ls-sys-ac-dsms";
        public const string ROLE_INTERNAL_DSM_FHS = "fhst-ls-sys-ac-dsms";
        public const string ROLE_INTERNAL_DSM_FLR = "flrk-ls-sys-ac-dsms";
        public const string ROLE_INTERNAL_DSM_FSA = "fsan-ls-sys-ac-dsms";
        public const string ROLE_INTERNAL_DSM_FOK = "fokc-ls-sys-ac-dsms";

        public const string ROLE_INTERNAL_DSR_FAQ = "fabq-ls-sys-ac-dsrs";
        public const string ROLE_INTERNAL_DSR_FAM = "fama-ls-sys-ac-dsrs";
        public const string ROLE_INTERNAL_DSR_FDF = "fdfw-ls-sys-ac-dsrs";
        public const string ROLE_INTERNAL_DSR_FHS = "fhst-ls-sys-ac-dsrs";
        public const string ROLE_INTERNAL_DSR_FLR = "flrk-ls-sys-ac-dsrs";
        public const string ROLE_INTERNAL_DSR_FSA = "fsan-ls-sys-ac-dsrs";
        public const string ROLE_INTERNAL_DSR_FOK = "fokc-ls-sys-ac-dsrs";

        public const string ROLE_INTERNAL_MIS_FAQ = "FABQ-LS-SYS-AC-Entree_BranchIS";
        public const string ROLE_INTERNAL_MIS_FAM = "FAMA-LS-SYS-AC-Entree_BranchIS";
        public const string ROLE_INTERNAL_MIS_FDF = "FDFW-LS-SYS-AC-Entree_BranchIS";
        public const string ROLE_INTERNAL_MIS_FHS = "FHST-LS-SYS-AC-Entree_BranchIS";
        public const string ROLE_INTERNAL_MIS_FLR = "FLRK-LS-SYS-AC-Entree_BranchIS";
        public const string ROLE_INTERNAL_MIS_FAR = "FLAR-LS-SYS-AC-Entree_BranchIS";
        public const string ROLE_INTERNAL_MIS_FSA = "FSAN-LS-SYS-AC-Entree_BranchIS";
        public const string ROLE_INTERNAL_MIS_FOK = "FOKC-LS-SYS-AC-Entree_BranchIS";

        public const string ROLE_INTERNAL_POWERUSER_FAQ = "FABQ-LS-SYS-AC-Entree_PowerUsers";
        public const string ROLE_INTERNAL_POWERUSER_FAM = "FAMA-LS-SYS-AC-Entree_PowerUsers";
        public const string ROLE_INTERNAL_POWERUSER_FDF = "FDFW-LS-SYS-AC-Entree_PowerUsers";
        public const string ROLE_INTERNAL_POWERUSER_FHS = "FHST-LS-SYS-AC-Entree_PowerUsers";
        public const string ROLE_INTERNAL_POWERUSER_FLR = "FLRK-LS-SYS-AC-Entree_PowerUsers";
        public const string ROLE_INTERNAL_POWERUSER_FAR = "FLAR-LS-SYS-AC-Entree_PowerUsers";
        public const string ROLE_INTERNAL_POWERUSER_FSA = "FSAN-LS-SYS-AC-Entree_PowerUsers";
        public const string ROLE_INTERNAL_POWERUSER_FOK = "FOKC-LS-SYS-AC-Entree_PowerUsers";
        public const string ROLE_INTERNAL_POWERUSER_GOF = "FFGO-LS-SYS-AC-Entree_PowerUsers";

        public const string ROLE_NAME_BRANCHIS = "branchismanager";
        public const string ROLE_NAME_DSM = "dsm";
        public const string ROLE_NAME_DSR = "dsr";
        public const string ROLE_NAME_GUEST = "guest";
        public const string ROLE_NAME_KBITADMIN = "kbitadmin";
        public const string ROLE_NAME_POWERUSER = "poweruser";
        public const string ROLE_NAME_SYSADMIN = "beksysadmin";

        public static readonly List<string> INTERNAL_USER_ROLES = new List<string>() { 
            ROLE_CORPORATE_ADMIN, ROLE_CORPORATE_SECURITY, 
            ROLE_INTERNAL_CSR_FAQ, ROLE_INTERNAL_CSR_FAM, ROLE_INTERNAL_CSR_FDF, ROLE_INTERNAL_CSR_FHS, ROLE_INTERNAL_CSR_FLR, ROLE_INTERNAL_CSR_FSA, ROLE_INTERNAL_CSR_FOK,
            ROLE_INTERNAL_DSM_FAQ, ROLE_INTERNAL_DSM_FAM, ROLE_INTERNAL_DSM_FDF, ROLE_INTERNAL_DSM_FHS, ROLE_INTERNAL_DSM_FLR, ROLE_INTERNAL_DSM_FSA, ROLE_INTERNAL_DSM_FOK,
            ROLE_INTERNAL_DSR_FAQ, ROLE_INTERNAL_DSR_FAM, ROLE_INTERNAL_DSR_FDF, ROLE_INTERNAL_DSR_FHS, ROLE_INTERNAL_DSR_FLR, ROLE_INTERNAL_DSR_FSA, ROLE_INTERNAL_DSR_FOK,
            ROLE_INTERNAL_MIS_FAQ, ROLE_INTERNAL_MIS_FAM, ROLE_INTERNAL_MIS_FDF, ROLE_INTERNAL_MIS_FHS, ROLE_INTERNAL_MIS_FLR, ROLE_INTERNAL_MIS_FSA, ROLE_INTERNAL_MIS_FOK,
            ROLE_INTERNAL_MIS_FAR, ROLE_INTERNAL_POWERUSER_FAM, ROLE_INTERNAL_POWERUSER_FAQ, ROLE_INTERNAL_POWERUSER_FAR, ROLE_INTERNAL_POWERUSER_FDF, ROLE_INTERNAL_POWERUSER_FHS,
            ROLE_INTERNAL_POWERUSER_FLR, ROLE_INTERNAL_POWERUSER_FOK, ROLE_INTERNAL_POWERUSER_FSA, ROLE_INTERNAL_POWERUSER_GOF
        };

        public static readonly List<string> BEK_SYSADMIN_ROLES = new List<string>() {
           ROLE_CORPORATE_ADMIN, ROLE_CORPORATE_SECURITY 
        };

        public static readonly List<string> DSR_ROLES = new List<string>() {
            ROLE_INTERNAL_DSR_FAQ, ROLE_INTERNAL_DSR_FAM,
            ROLE_INTERNAL_DSR_FDF, ROLE_INTERNAL_DSR_FHS,
            ROLE_INTERNAL_DSR_FLR, ROLE_INTERNAL_DSR_FSA,
            ROLE_INTERNAL_DSR_FOK
        };

        public static readonly List<string> DSM_ROLES = new List<string>() {
            ROLE_INTERNAL_DSM_FAQ, ROLE_INTERNAL_DSM_FAM,
            ROLE_INTERNAL_DSM_FDF, ROLE_INTERNAL_DSM_FHS,
            ROLE_INTERNAL_DSM_FLR, ROLE_INTERNAL_DSM_FSA,
            ROLE_INTERNAL_DSM_FOK
        };

        public static readonly List<string> MIS_ROLES = new List<string>() {
            ROLE_INTERNAL_MIS_FAQ, ROLE_INTERNAL_MIS_FAM,
            ROLE_INTERNAL_MIS_FDF, ROLE_INTERNAL_MIS_FHS,
            ROLE_INTERNAL_MIS_FLR, ROLE_INTERNAL_MIS_FAR,
            ROLE_INTERNAL_MIS_FSA, ROLE_INTERNAL_MIS_FOK
        };

        public static readonly List<string> POWERUSER_ROLES = new List<string>(){
            ROLE_INTERNAL_POWERUSER_FAM, ROLE_INTERNAL_POWERUSER_FAQ,
            ROLE_INTERNAL_POWERUSER_FAR, ROLE_INTERNAL_POWERUSER_FDF,
            ROLE_INTERNAL_POWERUSER_FHS, ROLE_INTERNAL_POWERUSER_FLR,
            ROLE_INTERNAL_POWERUSER_FOK, ROLE_INTERNAL_POWERUSER_FSA,
            ROLE_INTERNAL_POWERUSER_GOF
        };

        // Elastic Search : Indexes
        public const string ES_INDEX_CATEGORIES = "categories";
        public const string ES_INDEX_BRANDS = "brands";

        // Elastic Search : Types
        public const string ES_TYPE_CATEGORY = "category";
        public const string ES_TYPE_BRAND = "brand";


        // Brand Assets
        public const string BRAND_IMAGE_URL_FORMAT = "http://{0}/{1}.jpg";

        // Mainframe Stuff (AKA Mainframe Things)
        public const int MAINFRAME_ORDER_RECORD_LENGTH = 250;
        public const string MAINFRAME_RECEIVE_STATUS_CANCELLED = "CC";
        public const string MAINFRAME_RECEIVE_STATUS_GO = "GO";
        public const string MAINFRAME_RECEIVE_STATUS_GOOD_RETURN = "YY";
        public const string MAINFRAME_RECEIVE_STATUS_WAITING = "WW";

        // Confirmation translations
        public const string CONFIRMATION_DETAIL_FILLED_CODE = "";
        public const string CONFIRMATION_DETAIL_FILLED_STATUS = "Filled";
        public const string CONFIRMATION_DETAIL_PARTIAL_SHIP_CODE = "P";
        public const string CONFIRMATION_DETAIL_PARTIAL_SHIP_STATUS = "Partially Shipped";
        public const string CONFIRMATION_DETAIL_OUT_OF_STOCK_CODE = "O";
        public const string CONFIRMATION_DETAIL_OUT_OF_STOCK_STATUS = "Out of Stock";
        public const string CONFIRMATION_DETAIL_ITEM_REPLACED_CODE = "R";
        public const string CONFIRMATION_DETAIL_ITEM_REPLACED_STATUS = "Item Replaced";
        public const string CONFIRMATION_DETAIL_ITEM_REPLACED_OUT_OF_STOCK_CODE = "Z";
        public const string CONFIRMATION_DETAIL_ITEM_REPLACED_OUT_OF_STOCK_STATUS = "Item Replaced, Out of Stock";
        public const string CONFIRMATION_DETAIL_PARTIAL_SHIP_REPLACED_CODE = "T";
        public const string CONFIRMATION_DETAIL_PARTIAL_SHIP_REPLACED_STATUS = "Partially Shipped, Item Replaced";
        public const string CONFIRMATION_DETAIL_ITEM_SUBBED_CODE = "S";
        public const string CONFIRMATION_DETAIL_ITEM_SUBBED_STATUS = "Item Subbed";
		public const string ITEM_DELETED_STATUS = "Deleted";

        public const string CONFIRMATION_HEADER_IN_PROCESS_CODE = "P";
        public const string CONFIRMATION_HEADER_IN_PROCESS_STATUS = "In Process";
        public const string CONFIRMATION_HEADER_INVOICED_CODE = "I";
        public const string CONFIRMATION_HEADER_INVOICED_STATUS = "Shipped";
        public const string CONFIRMATION_HEADER_DELETED_CODE = "D";
        public const string CONFIRMATION_HEADER_DELETED_STATUS = "Cancelled";
        public const string CONFIRMATION_HEADER_REJECTED_CODE = "R";
        public const string CONFIRMATION_HEADER_REJECTED_STATUS = "Rejected";
        public const string CONFIRMATION_HEADER_CONFIRMED_WITH_CHANGES_EXCEPTIONS_STATUS = "Confirmed With Changes And Exceptions";
        public const string CONFIRMATION_HEADER_CONFIRMED_WITH_EXCEPTIONS_STATUS = "Confirmed With Exceptions";
        public const string CONFIRMATION_HEADER_CONFIRMED_WITH_CHANGES_STATUS = "Confirmed With Changes";
        public const string CONFIRMATION_HEADER_CONFIRMED_CODE = "";
        public const string CONFIRMATION_HEADER_CONFIRMED_STATUS = "Confirmed";

        public const string CS_PURCHASE_ORDER_ORIGINAL_ORDER_NUMBER = "OriginalOrderNumber";
        public const string CS_PURCHASE_ORDER_MASTER_NUMBER = "MasterNumber";
        public const string CS_LINE_ITEM_MAIN_FRAME_STATUS = "MainFrameStatus";


        // ImageNow - Integration Services
        public const string IMAGING_HEADER_USERNAME = "X-IntegrationServer-Username";
        public const string IMAGING_HEADER_PASSWORD = "X-IntegrationServer-Password";
        public const string IMAGING_HEADER_SESSIONTOKEN = "X-IntegrationServer-Session-Hash";

        // content management
        public const string CONTENTMGMT_BRANCHNAME_FAM = "Amarillo";
        public const string CONTENTMGMT_BRANCHNAME_FAQ = "New Mexico";
        public const string CONTENTMGMT_BRANCHNAME_FAR = "Little Rock"; // not actually defined, but makes sense that FAR would see FLR's content
        public const string CONTENTMGMT_BRANCHNAME_FDF = "Dallas/Fort Worth";
        public const string CONTENTMGMT_BRANCHNAME_FHS = "Houston";
        public const string CONTENTMGMT_BRANCHNAME_FLR = "Little Rock";
        public const string CONTENTMGMT_BRANCHNAME_FOK = "Oklahoma";
        public const string CONTENTMGMT_BRANCHNAME_FSA = "San Antonio";
        public const string CONTENTMGMT_BRANCHNAME_GOF = "General Office";
    }
}
