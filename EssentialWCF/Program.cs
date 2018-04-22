using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EssentialWCF
{

    [DataContract(Namespace = "http://EssentialWCF", Name = "Price")]
    //First
    //[KnownType(typeof(StockPrice))]
    //Third
    [KnownType("GetKnownTypes")]
    public class Price
    {
        public string Currency;

        //Third
        static Type[] GetKnownTypes()
        {
            return new Type[] { typeof(StockPrice) };
        }
    }

    [Serializable]
    public class PriceDetails
    {
        public string Ticker;
        public double Amount;
    }

    [MessageContract]
    public class StockPriceReq
    {
        [MessageBodyMember]
        public string Ticker;
    }

    [MessageContract]
    public class StockPriceSOAP
    {
        [MessageHeader]
        public DateTime CurrentTime;

        [MessageBodyMember]
        public PriceDetails Price;
    }

    [DataContract(Namespace = "http://EssentialWCF", Name = "StockPrice")]
    public class StockPrice : Price
    {
        [DataMember(Name = "CurrentPrice", Order = 0, IsRequired = true)]
        public double CurrentPriceNow;

        [DataMember(Name = "CurrentTime", Order = 1, IsRequired = true)]
        public DateTime CurrentTimeNow;
        
        [DataMember(Name = "Ticker", Order = 2, IsRequired = true)]
        public string TickerSymbol;

        [DataMember(Name = "DailyVolume", Order = 3, IsRequired = false)]
        public long DailyVolumeSoFar;

        [DataMember(Name = "DaylyChange", Order = 4, IsRequired = false)]
        public double DailyChangeSoFar;

        [DataMember(IsRequired = false)]
        public double test;

    }

    [ServiceContract]
    public interface IStockService
    {
        [OperationContract]
        double GetDDS(double price);

        [OperationContract]
        double GetPriceWithDDS(double price);

        [OperationContract(IsOneWay = true)]
        void OneWay();

        [OperationContract]
        void NotOneWay();

        [OperationContract]
        Price GetStockPrice(string ticker);

        [OperationContract]
        StockPriceSOAP GetStockPriceSOAP(StockPriceReq ticker);
    }

    //[ServiceBehavior(Namespace = "http://EssentialWCF/FinanceService/")]
    //[ServiceContract(Namespace = "http://EssentialWCF/FinanceService/")]
    public class StockService : IStockService
    {

        public double GetDDS(double price)
        {
            //Thread.Sleep(1000);
            return price * 0.2;
        }

        public double GetPriceWithDDS(double price)
        {
            //Thread.Sleep(1000);
            return price + GetDDS(price);
        }

        public void OneWay()
        {
            Thread.Sleep(10000);
        }

        public void NotOneWay()
        {
            Thread.Sleep(10000);
        }

        //Second
        //[OperationContract]
        //[ServiceKnownType(typeof(StockPrice))]
        public Price GetStockPrice(string ticker)
        {
            StockPrice s = new StockPrice();
            s.Currency = "USD";
            s.TickerSymbol = ticker;
            s.CurrentPriceNow = 100.00;
            s.CurrentTimeNow = DateTime.Now;
            s.DailyVolumeSoFar = 45000;
            //s.DailyChangeSoFar = .012345;
            s.test = 22;
            return s;
        }


        public StockPriceSOAP GetStockPriceSOAP(StockPriceReq ticker)
        {
            StockPriceSOAP s = new StockPriceSOAP();
            s.CurrentTime = DateTime.Now;
            s.Price = new PriceDetails();
            s.Price.Ticker = ticker.Ticker;
            s.Price.Amount = 99;
            return s;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            //ServiceHost serviceHost = new ServiceHost(typeof(StockService),
            //    new Uri("http://localhost:8000/EssentialWCF"));

            ServiceHost serviceHost = new ServiceHost(typeof(StockService));
            //ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            //behavior.HttpGetEnabled = true;
            // serviceHost.Description.Behaviors.Add(behavior);

            //serviceHost.AddServiceEndpoint(typeof(IStockService), new BasicHttpBinding(), "");
            //serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            serviceHost.Open();

            Console.WriteLine("Press <ENTER> to determine.\n\n");
            Console.ReadLine();

            serviceHost.Close();

        }
    }
}
