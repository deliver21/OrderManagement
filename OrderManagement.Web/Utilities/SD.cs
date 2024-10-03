namespace OrderManagement.Web.Utilities
{
    public class SD
    {
        //get the Order Url for Order Api's with AppSetting
        public static string OrderAPIBase { get; set; }
        public const string BaseCurrency = "USD";
        public enum ApiType
        { 
            GET,
            POST,
            PUT,
            DELETE
        }

    }
}
