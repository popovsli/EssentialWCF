using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
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

    }

    class Client
    {
        static void Main(string[] args)
        {
            ChannelFactory<IStockService> myChanelFactory = new ChannelFactory<IStockService>(
                new BasicHttpBinding(), new EndpointAddress("http://localhost:8000/EssentialWCF"));

            IStockService wcfClient = myChanelFactory.CreateChannel();
            double dds = wcfClient.GetDDS(100);
            double priceWithDDS = wcfClient.GetPriceWithDDS(100);
            Console.WriteLine("DDS is:{0}", dds);
            Console.WriteLine("Price with DDS is :{0}", priceWithDDS);

            wcfClient.OneWay();
            Console.WriteLine("oneway");

            wcfClient.NotOneWay();
            Console.WriteLine("not");

            Console.ReadLine();
        }
    }
}
