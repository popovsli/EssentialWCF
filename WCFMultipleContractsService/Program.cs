using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFMultipleContractsService
{
    [ServiceContract]
    public interface IGoodStockService
    {
        [OperationContract]
        double GetStockPrice(string ticker);
    }

    [ServiceContract]
    public interface IGreatStockService
    {
        [OperationContract]
        double GetStockPriceFast(string ticker);
    }

    public interface IAllStockService : IGoodStockService, IGreatStockService { };

    public class AllStockService : IAllStockService
    {

        public double GetStockPrice(string ticker)
        {
            Thread.Sleep(5000);
            return 94.85;
        }

        public double GetStockPriceFast(string ticker)
        {
            return 94.85;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost serviceHost = new ServiceHost(typeof(AllStockService));
            serviceHost.Open();

            Console.WriteLine("Press <ENTER> to determine.\n\n");
            Console.ReadLine();

            serviceHost.Close();

        }
    }
}
