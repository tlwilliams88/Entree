using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
{
    public class Constants
    {
        public const int LIMITEDCONCURRENCYTASK_ORDERUPDATES = 4;
        public const int LIMITEDCONCURRENCYTASK_SPECIALORDERUPDATES = 2;
        public const int LIMITEDCONCURRENCYTASK_CONFIRMATIONS = 4;
        public const int LIMITEDCONCURRENCYTASK_NOTIFICATIONS = 6;
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

        public const string CATALOG_BEK = "BEK";
        public const string CATALOG_UNFI = "UNFI";
        public const string CATALOG_CUSTOMINVENTORY = "CUSTOM";

        public const string REGEX_AD_ILLEGALCHARACTERS = @"[/\\\[\]:;\|=,\+\*\?<>@']";
        public const string REGEX_BENEKEITHEMAILADDRESS = "@benekeith.com";
        public const string REGEX_PASSWORD_PATTERN = @"^.*(?=.*[a-z])(?=.*[A-Z])(?=.*[\d]).*$";

        public const string ROLE_EXTERNAL_ACCOUNTING = "Accounting";
        public const string ROLE_EXTERNAL_GUEST = "Guest";
        public const string ROLE_EXTERNAL_OWNER = "Owner";
        public const string ROLE_EXTERNAL_PURCHASINGAPPROVER = "Approver";
        public const string ROLE_EXTERNAL_PURCHASINGBUYER = "Buyer";

        public const string PERMISSION_EXTERNAL_VIEWINVOICES = "ViewInvoices";

        //public const string ROLE_CORPORATE_ADMIN = "CORP-LS-SYS-AC-Entree_Admins";
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


        public const string ROLE_INTERNAL_MARKETING_FAQ = "FABQ-LS-SYS-AC-Entree_Marketing";
        public const string ROLE_INTERNAL_MARKETING_FAM = "FAMA-LS-SYS-AC-Entree_Marketing";
        public const string ROLE_INTERNAL_MARKETING_FDF = "FDFW-LS-SYS-AC-Entree_Marketing";
        public const string ROLE_INTERNAL_MARKETING_FHS = "FHST-LS-SYS-AC-Entree_Marketing";
        public const string ROLE_INTERNAL_MARKETING_FLR = "FLRK-LS-SYS-AC-Entree_Marketing";
        public const string ROLE_INTERNAL_MARKETING_FAR = "FLAR-LS-SYS-AC-Entree_Marketing";
        public const string ROLE_INTERNAL_MARKETING_FSA = "FSAN-LS-SYS-AC-Entree_Marketing";
        public const string ROLE_INTERNAL_MARKETING_FOK = "FOKC-LS-SYS-AC-Entree_Marketing";
        public const string ROLE_INTERNAL_MARKETING_GOF = "FFGO-LS-SYS-AC-Entree_Marketing";

        public const string ROLE_NAME_BRANCHIS = "branchismanager";
        public const string ROLE_NAME_DSM = "dsm";
        public const string ROLE_NAME_DSR = "dsr";
        public const string ROLE_NAME_GUEST = "guest";
        public const string ROLE_NAME_KBITADMIN = "kbitadmin";
        public const string ROLE_NAME_POWERUSER = "poweruser";
        public const string ROLE_NAME_SYSADMIN = "beksysadmin";
        public const string ROLE_NAME_MARKETING = "marketing";

        //public static readonly List<string> INTERNAL_USER_ROLES = new List<string>() { 
        //    ROLE_CORPORATE_ADMIN, ROLE_CORPORATE_SECURITY, 
        //    ROLE_INTERNAL_CSR_FAQ, ROLE_INTERNAL_CSR_FAM, ROLE_INTERNAL_CSR_FDF, ROLE_INTERNAL_CSR_FHS, ROLE_INTERNAL_CSR_FLR, ROLE_INTERNAL_CSR_FSA, ROLE_INTERNAL_CSR_FOK,
        //    ROLE_INTERNAL_DSM_FAQ, ROLE_INTERNAL_DSM_FAM, ROLE_INTERNAL_DSM_FDF, ROLE_INTERNAL_DSM_FHS, ROLE_INTERNAL_DSM_FLR, ROLE_INTERNAL_DSM_FSA, ROLE_INTERNAL_DSM_FOK,
        //    ROLE_INTERNAL_DSR_FAQ, ROLE_INTERNAL_DSR_FAM, ROLE_INTERNAL_DSR_FDF, ROLE_INTERNAL_DSR_FHS, ROLE_INTERNAL_DSR_FLR, ROLE_INTERNAL_DSR_FSA, ROLE_INTERNAL_DSR_FOK,
        //    ROLE_INTERNAL_MIS_FAQ, ROLE_INTERNAL_MIS_FAM, ROLE_INTERNAL_MIS_FDF, ROLE_INTERNAL_MIS_FHS, ROLE_INTERNAL_MIS_FLR, ROLE_INTERNAL_MIS_FSA, ROLE_INTERNAL_MIS_FOK,
        //    ROLE_INTERNAL_MIS_FAR, ROLE_INTERNAL_POWERUSER_FAM, ROLE_INTERNAL_POWERUSER_FAQ, ROLE_INTERNAL_POWERUSER_FAR, ROLE_INTERNAL_POWERUSER_FDF, ROLE_INTERNAL_POWERUSER_FHS,
        //    ROLE_INTERNAL_POWERUSER_FLR, ROLE_INTERNAL_POWERUSER_FOK, ROLE_INTERNAL_POWERUSER_FSA, ROLE_INTERNAL_POWERUSER_GOF, ROLE_INTERNAL_MARKETING_FAQ, ROLE_INTERNAL_MARKETING_FAM, ROLE_INTERNAL_MARKETING_FDF,
        //    ROLE_INTERNAL_MARKETING_FHS,ROLE_INTERNAL_MARKETING_FLR,ROLE_INTERNAL_MARKETING_FAR,ROLE_INTERNAL_MARKETING_FSA,ROLE_INTERNAL_MARKETING_FOK,ROLE_INTERNAL_MARKETING_GOF
        //};

        //public static readonly List<string> BEK_SYSADMIN_ROLES = new List<string>() {
        //   ROLE_CORPORATE_ADMIN, ROLE_CORPORATE_SECURITY 
        //};

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

        public static readonly List<string> MARKETING_ROLES = new List<string>(){
            ROLE_INTERNAL_MARKETING_FAQ, ROLE_INTERNAL_MARKETING_FAM, ROLE_INTERNAL_MARKETING_FDF,
			ROLE_INTERNAL_MARKETING_FHS,ROLE_INTERNAL_MARKETING_FLR,ROLE_INTERNAL_MARKETING_FAR,
			ROLE_INTERNAL_MARKETING_FSA,ROLE_INTERNAL_MARKETING_FOK,ROLE_INTERNAL_MARKETING_GOF
        };

        // Elastic Search : Indexes
        public const string ES_INDEX_CATEGORIES = "categories";
        public const string ES_UNFI_INDEX_CATEGORIES = "unfi_categories";
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

        // MIME TYPES
        public const string MIMETYPE_JPG = "image/jpeg";
        public const string MIMETYPE_PDF = "application/pdf";

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
        public const string CONFIRMATION_DETAIL_ITEM_STATUS_INVALID = "I";
        public const string CONFIRMATION_DETAIL_ITEM_STATUS_INVALID_DESCRIPTION = "Invalid";
        public const string CONFIRMATION_DETAIL_ITEM_STATUS_DELETE = "D";
        public const string CONFIRMATION_DETAIL_ITEM_STATUS_DELETE_DESCRIPTION = "Deleted";
        public const string CONFIRMATION_DETAIL_ITEM_STATUS_NOT_FOUND = "N";
        public const string CONFIRMATION_DETAIL_ITEM_STATUS_NOT_FOUND_DESCRIPTION = "Not Found";

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

        public const string SPECIALORDERITEM_NEW_STATUS_CODE = "00";
        public const string SPECIALORDERITEM_NEW_STATUS_TRANSLATED_CODE = "\u00C0";
        public const string SPECIALORDERITEM_NEW_STATUS = "New";
        public const string SPECIALORDERITEM_ERR_STATUS_CODE = "02";
        public const string SPECIALORDERITEM_ERR_STATUS_TRANSLATED_CODE = "\u00C1";
        public const string SPECIALORDERITEM_ERR_STATUS = "Err";
        public const string SPECIALORDERITEM_2MF_STATUS_CODE = "05";
        public const string SPECIALORDERITEM_2MF_STATUS_TRANSLATED_CODE = "\u00C2";
        public const string SPECIALORDERITEM_2MF_STATUS = "Deliver to MF";
        public const string SPECIALORDERITEM_REQ_STATUS_CODE = "10";
        public const string SPECIALORDERITEM_REQ_STATUS_TRANSLATED_CODE = "\u00C3";
        public const string SPECIALORDERITEM_REQ_STATUS = "Requested";
        public const string SPECIALORDERITEM_ACC_STATUS_CODE = "15";
        public const string SPECIALORDERITEM_ACC_STATUS_TRANSLATED_CODE = "\u00C4";
        public const string SPECIALORDERITEM_ACC_STATUS = "Accepted";
        public const string SPECIALORDERITEM_APP_STATUS_CODE = "20";
        public const string SPECIALORDERITEM_APP_STATUS_TRANSLATED_CODE = "\u00C5";
        public const string SPECIALORDERITEM_APP_STATUS = "Approved";
        public const string SPECIALORDERITEM_DEL_STATUS_CODE = "30";
        public const string SPECIALORDERITEM_DEL_STATUS_TRANSLATED_CODE = "\u00C6";
        public const string SPECIALORDERITEM_DEL_STATUS = "Deleted";
        public const string SPECIALORDERITEM_HLD_STATUS_CODE = "40";
        public const string SPECIALORDERITEM_HLD_STATUS_TRANSLATED_CODE = "\u00C7";
        public const string SPECIALORDERITEM_HLD_STATUS = "Held";
        public const string SPECIALORDERITEM_RCV_STATUS_CODE = "50";
        public const string SPECIALORDERITEM_RCV_STATUS_TRANSLATED_CODE = "\u00C8";
        public const string SPECIALORDERITEM_RCV_STATUS = "Received";
        public const string SPECIALORDERITEM_R_H_STATUS_CODE = "52";
        public const string SPECIALORDERITEM_R_H_STATUS_TRANSLATED_CODE = "\u00C9";
        public const string SPECIALORDERITEM_R_H_STATUS = "Received/Held";
        public const string SPECIALORDERITEM_ATT_STATUS_CODE = "55";
        public const string SPECIALORDERITEM_ATT_STATUS_TRANSLATED_CODE = "\u00CA";
        public const string SPECIALORDERITEM_ATT_STATUS = "Attached";
        public const string SPECIALORDERITEM_PTL_STATUS_CODE = "57";
        public const string SPECIALORDERITEM_PTL_STATUS_TRANSLATED_CODE = "\u00CB";
        public const string SPECIALORDERITEM_PTL_STATUS = "Partial Ship to BEK";
        public const string SPECIALORDERITEM_SHP_STATUS_CODE = "60";
        public const string SPECIALORDERITEM_SHP_STATUS_TRANSLATED_CODE = "\u00CC";
        public const string SPECIALORDERITEM_SHP_STATUS = "Shipped to BEK";
        public const string SPECIALORDERITEM_PUR_STATUS_CODE = "70";
        public const string SPECIALORDERITEM_PUR_STATUS_TRANSLATED_CODE = "\u00CD";
        public const string SPECIALORDERITEM_PUR_STATUS = "Purged";

        public const string CS_PURCHASE_ORDER_ORIGINAL_ORDER_NUMBER = "OriginalOrderNumber";
        public const string CS_PURCHASE_ORDER_MASTER_NUMBER = "MasterNumber";
        public const string CS_LINE_ITEM_MAIN_FRAME_STATUS = "MainFrameStatus";

        public const string TEMP_ZONE_FROZEN_CODE = "F";
        public const string TEMP_ZONE_FROZEN_DESCRIPTION = "Frozen";
        public const string TEMP_ZONE_REFRIGERATED_CODE = "C";
        public const string TEMP_ZONE_REFRIGERATED_DESCRIPTION = "Refrigerated";
        public const string TEMP_ZONE_DRY_CODE = "D";
        public const string TEMP_ZONE_DRY_DESCRIPTION = "Dry";

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

        public const int CONTENTMGMT_CONTRACTITEMS_THRESHOLD = 14;
        public const string CONTENTMGMT_CONTRACTITEMS_NEWADDED = "newly added";
        public const string CONTENTMGMT_CONTRACTITEMS_NEWDELETED = "newly deleted";
        public const string CONTENTMGMT_CONTRACTITEMS_ACTIVE = "active";

        // content management
        public const string INVOICETYPE_INITIALINVOICE = "IN ";

        // application defaults
        public const string APPDEFAULT_PAGELOADSIZE_KEY = "pageLoadSize";
        public const string APPDEFAULT_SORTPREFERENCES_KEY = "sortPreferences";

        // invoice transaction types and requests
        public const string INVOICETRANSACTIONTYPE_INITIALINVOICE = "IN ";
        public const string INVOICETRANSACTIONTYPE_CREDITMEMO = "CM ";
        public const string INVOICEREQUESTFILTER_CREDITMEMO_FIELDKEY = "hascreditmemos";
        public const string INVOICEREQUESTFILTER_CREDITMEMO_VALUECMONLY = "true";
        public const string INVOICEREQUESTFILTER_INVOICENUMBER_FIELDKEY = "invoicenumber";
        public const string INVOICEREQUESTFILTER_PONUMBER_FIELDKEY = "ponumber";
        public const string INVOICEREQUESTFILTER_TYPEDESCRIPTION_FIELDKEY = "typedescription";
        public const string INVOICEREQUESTFILTER_DATERANGE_YEARQTRKEY = "yearqtr";
        public const string INVOICEREQUESTFILTER_DATERANGE_STARTQ1 = "1/1";
        public const string INVOICEREQUESTFILTER_DATERANGE_STARTQ2 = "4/1";
        public const string INVOICEREQUESTFILTER_DATERANGE_STARTQ3 = "7/1";
        public const string INVOICEREQUESTFILTER_DATERANGE_STARTQ4 = "10/1";
        public const string INVOICEREQUESTFILTER_DATERANGE_YEARMONTHKEY = "yearmonth";
        public const string INVOICEREQUESTSORT_INVOICEAMOUNT = "invoiceamount";
        public const string INVOICEREQUESTSORT_INVOICEAMOUNT_ASCENDING = "asc";

        // item history unit of measure
        public const string ITEMHISTORY_AVERAGEUSE_CASE = "C";
        public const string ITEMHISTORY_AVERAGEUSE_PACKAGE = "P";

        // reportnames
        public const string SET_REPORT_SIZE_LANDSCAPE = "<DeviceInfo><PageHeight>8.5in</PageHeight><PageWidth>11in</PageWidth></DeviceInfo>";
        public const string REPORT_PRINTLIST = "KeithLink.Svc.Impl.Reports.ListReport.rdlc";
        public const string REPORT_PRINTLIST_YesParYesPriceYesNotes = "KeithLink.Svc.Impl.Reports.ListReport_YesParYesPriceYesNotes.rdlc";
        public const string REPORT_PRINTLIST_YesParYesPriceNoNotes = "KeithLink.Svc.Impl.Reports.ListReport_YesParYesPriceNoNotes.rdlc";
        public const string REPORT_PRINTLIST_YesParNoPriceYesNotes = "KeithLink.Svc.Impl.Reports.ListReport_YesParNoPriceYesNotes.rdlc";
        public const string REPORT_PRINTLIST_YesParNoPriceNoNotes = "KeithLink.Svc.Impl.Reports.ListReport_YesParNoPriceNoNotes.rdlc";
        public const string REPORT_PRINTLIST_NoParYesPriceYesNotes = "KeithLink.Svc.Impl.Reports.ListReport_NoParYesPriceYesNotes.rdlc";
        public const string REPORT_PRINTLIST_NoParYesPriceNoNotes = "KeithLink.Svc.Impl.Reports.ListReport_NoParYesPriceNoNotes.rdlc";
        public const string REPORT_PRINTLIST_NoParNoPriceYesNotes = "KeithLink.Svc.Impl.Reports.ListReport_NoParNoPriceYesNotes.rdlc";
        public const string REPORT_PRINTLIST_NoParNoPriceNoNotes = "KeithLink.Svc.Impl.Reports.ListReport_NoParNoPriceNoNotes.rdlc";
        public const string REPORT_NULL_Placeholder = "<BLANK>";

        // system alerts
        public const string EMAILMASK_ALLSYSTEMALERT = "ALERTALL";
        public const string EMAILMASK_BRANCHSYSTEMALERT = "ALERTBRANCH";

        // queue action retries
        public const int QUEUE_REPO_RETRY_COUNT = 5;
        public const int QUEUE_CHECKLOSTORDERS_RETRY_COUNT = 5;

        // RabbitMQ route keys
        public const string RABBITMQ_NOTIFICATION_ORDERCONFIRMATION_ROUTEKEY = "notificationtype=1";
        public const string RABBITMQ_NOTIFICATION_HASNEWS_ROUTEKEY = "notificationtype=8";
        public const string RABBITMQ_NOTIFICATION_ETA_ROUTEKEY = "notificationtype=32";
        public const string RABBITMQ_NOTIFICATION_PAYMENTNOTIFICATION_ROUTEKEY = "notificationtype=64";

        public const string IXONE_IMAGE_FILETYPE_STANDARDRES = "A";
        public const string IXONE_IMAGE_FILETYPE_HIGHRES = "C";
        public const string IXONE_IMAGE_FACING_FRONT = "1";
        public const string IXONE_IMAGE_FACING_LEFT = "2";
        public const string IXONE_IMAGE_FACING_TOP = "3";
        public const string IXONE_IMAGE_FACING_BACK = "7";
        public const string IXONE_IMAGE_FACING_RIGHT = "8";
        public const string IXONE_IMAGE_FACING_DISPLAY = "D";
        public const string IXONE_IMAGE_FACING_NUTRITION = "N";
        public const string IXONE_IMAGE_FACING_INGREDIENTS = "I";
        public const string IXONE_IMAGE_ANGLE_CENTER = "C";
        public const string IXONE_IMAGE_ANGLE_LEFT = "L";
        public const string IXONE_IMAGE_ANGLE_RIGHT = "R";
        public const string IXONE_IMAGE_ANGLE_NOPLUNGE = "N";
        public const string IXONE_IMAGE_PACK_IN = "1";
        public const string IXONE_IMAGE_PACK_OUT = "0";
        public const string IXONE_IMAGE_PACK_CASE = "A";
        public const string IXONE_IMAGE_PACK_INNER = "B";
        public const string IXONE_IMAGE_PACK_PREPARED = "D";
        public const string IXONE_PRODUCTINFO_GET_URL =
            "https://exchange.ix-one.net/services/Products/filtered";
        public const string IXONE_PRODUCTIMAGE_GET_URL =
            "https://exchange.ix-one.net/services/ImageHandler.aspx?FileName={0}&Type={1}&Size=MEDIUM";
        public const string IXONE_PRODUCTIMAGE_PREFERREDTYPE = "JPG";
        public const string IXONE_PRODUCTIMAGE_BACKUPTYPE = "PNG";

        // message templates
        public const string MESSAGE_TEMPLATE_PAYMENTCONFIRMATION = "PaymentConfirmation";
        public const string MESSAGE_TEMPLATE_PAYMENTDETAIL = "PaymentConfirmationDetail";
        public const string MESSAGE_TEMPLATE_MULTI_PAYMENTCONFIRMATION = "MultiPaymentConfirmation";
        public const string MESSAGE_TEMPLATE_MULTI_PAYMENTHEADER = "MultiPaymentConfirmationCustomerHeader";
        public const string MESSAGE_TEMPLATE_MULTI_PAYMENTDETAIL1 = "MultiPaymentConfirmationCustomerFirstRowDetail";
        public const string MESSAGE_TEMPLATE_MULTI_PAYMENTDETAIL2 = "MultiPaymentConfirmationCustomerNextRowsDetail";
        public const string MESSAGE_TEMPLATE_MULTI_PAYMENTDETAIL3 = "MultiPaymentConfirmationCustomerAltNextRowsDetail";
        public const string MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTERACCOUNT = "MultiPaymentConfirmationCustomerFooterAccount";
        public const string MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTERCUSTOMER = "MultiPaymentConfirmationCustomerFooterCustomer";
        public const string MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTERGRAND = "MultiPaymentConfirmationCustomerFooterGrand";
        public const string MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTEREND = "MultiPaymentConfirmationCustomerFooterEnd";

        // message templates
        public const string SPECIALFILTERS_FACET = "specialfilters";
        public const string SPECIALFILTERS_UNDETERMINEDCOUNT = "?";
        public const string SPECIALFILTER_DEVIATEDPRICES = "deviatedprices";
        public const string SPECIALFILTER_DEVIATEDPRICES_DESCRIPTION = "Deviated Prices";
        public const string SPECIALFILTER_PREVIOUSORDERED = "previousordered";
        public const string SPECIALFILTER_PREVIOUSORDERED_DESCRIPTION = "Previously Ordered";
    }
}
