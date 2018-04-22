using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DuplexWCF
{
    [ServiceContract(CallbackContract = typeof(IClientCallBack))]
    public interface IServerStock
    {
        [OperationContract (IsOneWay = true)]
        void RegisterForUpdate(string ticker);
    }

    public interface IClientCallBack
    {
        [OperationContract(IsOneWay = true)]
        void PriceUpdate(string ticker, double price);
    }

    public class ServerStock : IServerStock
    {
        public class Worker
        {
            public string ticker;
            public Update workerProcess;
        }

        public static Hashtable workers = new Hashtable();

        public void RegisterForUpdate(string ticker)
        {
            Worker worker = null;
            if (!workers.ContainsKey(ticker))
            {
                worker = new Worker();
                worker.ticker = ticker;
                worker.workerProcess = new Update();
                worker.workerProcess.ticker = ticker;
                workers[ticker] = worker;
                Thread thread = new Thread(new ThreadStart(worker.workerProcess.SendUpdateToClient));
                thread.IsBackground = true;
                thread.Start();
            }
            worker = (Worker)workers[ticker];
            IClientCallBack clientCallBack = OperationContext.Current.GetCallbackChannel<IClientCallBack>();
            lock (worker.workerProcess.callBacks)
            {
                worker.workerProcess.callBacks.Add(clientCallBack);
            }
            
        }
    }

    public class Update
    {
        public string ticker;

        public List<IClientCallBack> callBacks = new List<IClientCallBack>();

        public void SendUpdateToClient()
        {
            Random w = new Random();
            Random p = new Random();
            while (true)
            {
                Thread.Sleep(w.Next(5000));
                lock (callBacks)
                {
                    foreach (IClientCallBack c in callBacks)
                    {
                        try
                        {
                            c.PriceUpdate(ticker, 100.0 + p.NextDouble()*10);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error sending cache to client: {0}", ex.Message);
                        }
                    }
                   
                }
            }
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost serviceHost = new ServiceHost(typeof(ServerStock));
            serviceHost.Open();

            Console.WriteLine("Press <ENTER> to determine.\n\n");
            Console.ReadLine();

            serviceHost.Close();
        }
    }
}
