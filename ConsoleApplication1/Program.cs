using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace StockQuoteWCF
{
    //Request - Response pathern
    [ServiceContract]
    public interface IStockQuoteService
    {
        [OperationContract]
        double GetQuote(string symbol);
    }

    //Two one way Duplex pathern
    [ServiceContract(CallbackContract = typeof(IStockQuoteCallBack), SessionMode = SessionMode.Required)]
    public interface IStockQuoteDuplexService
    {
        [OperationContract(IsOneWay = true)]
        void SendQuoteRequest(string symbol);
    }

    //Two one way Duplex pathern
    public interface IStockQuoteCallBack
    {
        [OperationContract(IsOneWay = true)]
        void SendQuoteResponse(string symbol, double price);
    }

    //Duplex service
    public class StockQuoteDuplexService : IStockQuoteDuplexService
    {

        public void SendQuoteRequest(string symbol)
        {
            double value;

            if (symbol == "MSFT")
                value = 31.15;
            else if (symbol == "YHOO")
                value = 28.10;
            else if (symbol == "GOOG")
                value = 450.75;
            else
                value = double.NaN;

            OperationContext ctx = OperationContext.Current;
            IStockQuoteCallBack callBack = ctx.GetCallbackChannel<IStockQuoteCallBack>();
            callBack.SendQuoteResponse(symbol, value);
        }
    }

    //Normal service
    public class StockQuoteService : IStockQuoteService
    {

        public double GetQuote(string symbol)
        {
            double value;

            if (symbol == "MSFT")
                value = 31.15;
            else if (symbol == "YHOO")
                value = 28.10;
            else if (symbol == "GOOG")
                value = 450.75;
            else
                value = double.NaN;

            return value;
        }
    }

    //One-way MSMQ
    [ServiceContract]
    public interface IStockQuoteRequest
    {
        [OperationContract(IsOneWay = true)]
        void SendQuoteRequest(string symbol);
    }

    //One-way MSMQ
    [ServiceContract]
    public interface IStockQuoteResponse
    {
        [OperationContract(IsOneWay = true)]
        void SendQuoteResponse(string symbol, double price);
    }

    //netMsqmService
    public class StockQuoteRequestService : IStockQuoteRequest
    {

        public void SendQuoteRequest(string symbol)
        {
            double value;

            if (symbol == "MSFT")
                value = 31.15;
            else if (symbol == "YHOO")
                value = 28.10;
            else if (symbol == "GOOG")
                value = 450.75;
            else
                value = double.NaN;

            NetMsmqBinding msmqResponseBinding = new NetMsmqBinding();

            using (ChannelFactory<IStockQuoteResponse> cf = new ChannelFactory<IStockQuoteResponse>("NetMsmqResponseClient"))
            {
                IStockQuoteResponse client = cf.CreateChannel();

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    client.SendQuoteResponse(symbol, value);
                    scope.Complete();
                }
                cf.Close();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string queueName = ConfigurationManager.AppSettings["queueName"];
            if (!MessageQueue.Exists(queueName))
            {
                MessageQueue.Create(queueName, true);
            }

            ServiceHost service = new ServiceHost(typeof(StockQuoteService));
            ServiceHost serviceDuplex = new ServiceHost(typeof(StockQuoteDuplexService));
            ServiceHost serviceMsmq = new ServiceHost(typeof(StockQuoteRequestService));
            
            service.Open();
            serviceDuplex.Open();
            serviceMsmq.Open();

            Console.WriteLine("Press <ENTER> to determine.\n\n");
            Console.ReadLine();

            service.Close();
            serviceDuplex.Close();
            serviceMsmq.Close();
        }
    }
}
