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



        // Elastic Search : Indexes
        public const string ES_INDEX_CATEGORIES = "categories";
        public const string ES_INDEX_BRANDS = "brands";

        // Elastic Search : Types
        public const string ES_TYPE_CATEGORY = "category";
        public const string ES_TYPE_BRAND = "brand";


        // Brand Assets
        public const string BRAND_IMAGE_URL_FORMAT = "http://{0}/{1}.jpg";


        // mainframe stuff
        public const int MAINFRAME_ORDER_RECORD_LENGTH = 250;
        public const string MAINFRAME_RECEIVE_STATUS_CANCELLED = "CC";
        public const string MAINFRAME_RECEIVE_STATUS_GO = "GO";
        public const string MAINFRAME_RECEIVE_STATUS_GOOD_RETURN = "YY";
        public const string MAINFRAME_RECEIVE_STATUS_WAITING = "WW";
    }
}
