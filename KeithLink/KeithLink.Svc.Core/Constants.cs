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

        public const string AD_GUEST_CONTAINER = "_bek_guest";
        public const string AD_GUEST_FIRSTNAME = "guest";
        public const string AD_GUEST_LASTNAME = "account";

        public const string REGEX_AD_ILLEGALCHARACTERS = @"[/\\\[\]:;\|=,\+\*\?<>@']";
        public const string REGEX_BENEKEITHEMAILADDRESS = "@benekeith.com";
        public const string REGEX_PASSWORD_PATTERN = @"^.*(?=.*[a-z])(?=.*[A-Z])(?=.*[\d]).*$";

        public const string ROLE_EXTERNAL_ACCOUNTING = "Accounting";
        public const string ROLE_EXTERNAL_GUEST = "Guest";
        public const string ROLE_EXTERNAL_OWNER = "Owner";
        public const string ROLE_EXTERNAL_PURCHASINGAPPROVER = "Approver";
        public const string ROLE_EXTERNAL_PURCHASINGBUYER = "Buyer";

        public const string ROLE_CORPORATE_ADMIN = "CORP-DIS-eBusiness";
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

        public const string ROLE_INTERNAL_MIS_FAQ = "fabq-ls-mis-all";
        public const string ROLE_INTERNAL_MIS_FAM = "fama-ls-mis-all";
        public const string ROLE_INTERNAL_MIS_FDF = "fdfw-ls-mis-all";
        public const string ROLE_INTERNAL_MIS_FHS = "fhst-ls-mis-all";
        public const string ROLE_INTERNAL_MIS_FLR = "flrk-ls-mis-all";
        public const string ROLE_INTERNAL_MIS_FSA = "fsan-ls-mis-all";
        public const string ROLE_INTERNAL_MIS_FOK = "fokc-ls-mis-all";

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

        // Confirmation Parsing Layout : Header
        public const int CONFIRMATION_HEADER_DATE_INDEX = 0;
        public const int CONFIRMATION_HEADER_DATE_LENGTH = 14;
        
        public const int CONFIFMATION_HEADER_COMPANY_INDEX = 14;
        public const int CONFIRMATION_HEADER_COMPANY_LENGTH = 3;
        
        public const int CONFIRMATION_HEADER_DIVISION_INDEX = 17;
        public const int CONFIRMATION_HEADER_DIVISION_LENGTH = 3;
        
        public const int CONFIRMATION_HEADER_DEPARTMENT_INDEX = 20;
        public const int CONFIRMATION_HEADER_DEPARTMENT_LENGTH = 3;
        
        public const int CONFIRMATION_HEADER_CUSTOMER_NUMBER_INDEX = 23;
        public const int CONFIRMATION_HEADER_CUSTOMER_NUMBER_LENGTH = 10;

        public const int CONFIRMATION_HEADER_USER_ID_INDEX = 33;
        public const int CONFIRMATION_HEADER_USER_ID_LENGTH = 10;

        public const int CONFIRMATION_HEADER_PN_ORDER_NUMBER_INDEX = 43;
        public const int CONFIRMATION_HEADER_PN_ORDER_NUMBER_LENGTH = 7;

        public const int CONFIRMATION_HEADER_CONFIRMATION_NUMBER_INDEX = 50;
        public const int CONFIRMATION_HEADER_CONFIRMATION_NUMBER_LENGTH = 12;

        public const int CONFIRMATION_HEADER_INVOICE_NUMBER_INDEX = 62;
        public const int CONFIRMATION_HEADER_INVOICE_NUMBER_LENGTH = 8;

        public const int CONFIRMATION_HEADER_SHIP_DATE_INDEX = 70;
        public const int CONFIRMATION_HEADER_SHIP_DATE_LENGTH = 8;

        public const int CONFIRMATION_HEADER_ROUTE_NUMBER_INDEX = 78;
        public const int CONFIRMATION_HEADER_ROUTE_NUMBER_LENGTH = 5;

        public const int CONFIRMATION_HEADER_STOP_NUMBER_INDEX = 83;
        public const int CONFIRMATION_HEADER_STOP_NUMBER_LENGTH = 3;

        public const int CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_INDEX = 86;
        public const int CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_LENGTH = 40;

        public const int CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_EXT_INDEX = 126;
        public const int CONFIRMATION_HEADER_SPECIAL_INSTRUCTIONS_EXT_LENGTH = 40;

        public const int CONFIRMATION_HEADER_TOTAL_QTY_ORDERED_INDEX = 166;
        public const int CONFIRMATION_HEADER_TOTAL_QTY_ORDERED_LENGTH = 7;

        public const int CONFIRMATION_HEADER_TOTAL_QTY_SHIPPED_INDEX = 173;
        public const int CONFIRMATION_HEADER_TOTAL_QTY_SHIPPED_LENGTH = 7;

        public const int CONFIRMATION_HEADER_TOTAL_INVOICE_INDEX = 180;
        public const int CONFIRMATION_HEADER_TOTAL_INVOICE_LENGTH = 16;

        public const int CONFIRMATION_HEADER_TOTAL_CUBE_INDEX = 196;
        public const int CONFIRMATION_HEADER_TOTAL_CUBE_LENGTH = 12;

        public const int CONFIRMATION_HEADER_TOTAL_WEIGHT_INDEX = 208;
        public const int CONFIRMATION_HEADER_TOTAL_WEIGHT_LENGTH = 12;

        public const int CONFIRMATION_HEADER_CONFIRMATION_MESSAGE_INDEX = 220;
        public const int CONFIRMATION_HEADER_CONFIRMATION_MESSAGE_LENGTH = 33;

        public const int CONFIRMATION_HEADER_CONFIRMATION_NUMBER_EXT_INDEX = 253;
        public const int CONFIRMATION_HEADER_CONFIRMATION_NUMBER_EXT_LENGTH = 7;

        public const int CONFIRMATION_HEADER_CONFIRMATION_STATUS_INDEX = 260;
        public const int CONFIRMATION_HEADER_CONFIRMATION_STATUS_LENGTH = 1;


        // Confirmation Parsing Layout : Detail

        public const int CONFIRMATION_DETAIL_RECORD_NUMBER_INDEX = 0;
        public const int CONFIRMATION_DETAIL_RECORD_NUMBER_LENGTH = 5;

        public const int CONFIRMATION_DETAIL_ITEM_NUMBER_INDEX = 5;
        public const int CONFIRMATION_DETAIL_ITEM_NUMBER_LENGTH = 10;

        public const int CONFIRMATION_DETAIL_QUANTITY_ORDERED_INDEX = 15;
        public const int CONFIRMATION_DETAIL_QUANTITY_ORDERED_LENGTH = 7;

        public const int CONFIRMATION_DETAIL_BROKEN_CASE_INDEX = 22;
        public const int CONFIRMATION_DETAIL_BROKEN_CASE_LENGTH = 1;

        public const int CONFIRMATION_DETAIL_QUANTITY_SHIPPED_INDEX = 23;
        public const int CONFIRMATION_DETAIL_QUANTITY_SHIPPED_LENGTH = 7;

        public const int CONFIRMATION_DETAIL_REASON_NOT_SHIPPED_INDEX = 30;
        public const int CONFIRMATION_DETAIL_REASON_NOT_SHIPPED_LENGTH = 3;

        public const int CONFIRMATION_DETAIL_SHIP_WEIGHT_INDEX = 33;
        public const int CONFIRMATION_DETAIL_SHIP_WEIGHT_LENGTH = 12;

        public const int CONFIRMATION_DETAIL_CASE_CUBE_INDEX = 45;
        public const int CONFIRMATION_DETAIL_CASE_CUBE_LENGTH = 12;

        public const int CONFIRMATION_DETAIL_CASE_WEIGHT_INDEX = 57;
        public const int CONFIRMATION_DETAIL_CASE_WEIGHT_LENGTH = 12;

        public const int CONFIRMATION_DETAIL_SALES_GROSS_INDEX = 69;
        public const int CONFIRMATION_DETAIL_SALES_GROSS_LENGTH = 16;

        public const int CONFIRMATION_DETAIL_SALES_NET_INDEX = 85;
        public const int CONFIRMATION_DETAIL_SALES_NET_LENGTH = 16;

        public const int CONFIRMATION_DETAIL_PRICE_NET_INDEX = 101;
        public const int CONFIRMATION_DETAIL_PRICE_NET_LENGTH = 10;

        public const int CONFIRMATION_DETAIL_SPLIT_PRICE_NET_INDEX = 111;
        public const int CONFIRMATION_DETAIL_SPLIT_PRICE_NET_LENGTH = 10;

        public const int CONFIRMATION_DETAIL_PRICE_GROSS_INDEX = 121;
        public const int CONFIRMATION_DETAIL_PRICE_GROSS_LENGTH = 10;

        public const int CONFIRMATION_DETAIL_SPLIT_PRICE_GROSS_INDEX = 131;
        public const int CONFIRMATION_DETAIL_SPLIT_PRICE_GROSS_LENGTH = 10;

        public const int CONFIRMATION_DETAIL_CONFIRMATION_MESSAGE_INDEX = 141;
        public const int CONFIRMATION_DETAIL_CONFIRMATION_MESSAGE_LENGTH = 40;


        // Confrimation Parsing Layout : Footer

    }
}
