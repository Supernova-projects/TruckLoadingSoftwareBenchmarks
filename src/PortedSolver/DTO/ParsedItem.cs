using CsvHelper.Configuration.Attributes;

namespace PortedSolver.DTO
{
    public class ParsedItem
    {
        [Name("NUMBER OF ITEMS")]
        public int NumberOfItems { get; set; }

        [Name("LENGHT")]
        public double Length { get; set; }

        [Name("WIDTH")]
        public double Width { get; set; }

        [Name("HEIGHT")]
        public double Height { get; set; }

        [Name("WEIGHT")]
        public double Weight { get; set; }

        [Name("VOLUME")]
        public double Volume { get; set; }

        [Name("STACKABLE")]
        public int Stackable { get; set; }

        [Name("UNLOADING ORDER")]
        public int UnloadingOrder { get; set; }

        [Name("PRIORITY")]
        public int Priority { get; set; }

        [Name("TAXABILITY")]
        public double Taxability { get; set; }

        [Name("LOADED")]
        public int Loaded { get; set; }

        [Name("LOADING RULES")]
        public string LoadingRules { get; set; }

        [Name("COUNTRY RULES")]
        public string CountryRules { get; set; }

        [Name("CUSTOMER-PARTNER RULES")]
        public string CustomerPartnerRules { get; set; }

        [Name("SENDER RULES")]
        public string SenderRules { get; set; }

        [Name("CONSIGNEE RULES")]
        public string ConsigneeRules { get; set; }

        [Name("TRAFFIC LINE")]
        public string TrafficLine { get; set; }

        [Name("GOODS")]
        public string Goods { get; set; }

        [Name("YEAR")]
        public string Year { get; set; }

        [Name("BRANCH")]
        public int Branch { get; set; }
        
        [Name("SHIPMENT")]
        public int Shipment { get; set; }

        [Name("KIND OF PACKAGE")]
        public string KindOfPackage { get; set; }

        [Name("SENDER")]
        public string Sender { get; set; }

        [Name("CONSIGNEE")]
        public string Consignee { get; set; }

        [Name("ZIP")]
        public string Zip { get; set; }

        [Name("NATION")]
        public string Nation { get; set; }

        [Name("TRUCK YEAR")]
        public string TruckYear { get; set; }

        [Name("TRUCK BRANCH")]
        public int TruckBranch { get; set; }

        [Name("TRUCK NUMBER")]
        public string TruckNumber { get; set; }

        [Name("KIND OF TRUCK")]
        public string KindOfTruck { get; set; }
    }
}
