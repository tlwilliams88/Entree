﻿using System;
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

    }
}
