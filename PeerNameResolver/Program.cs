using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.PeerToPeer;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace PeerNameResolverService
{
    [ServiceContract]
    public interface IPeerNetwork
    {
        [OperationContract(IsOneWay = true)]
        void GetName();
    }

    public class PeerNetwork : IPeerNetwork
    {
        public void GetName()
        {
            Console.WriteLine("Hello");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //PeerName peerName = new PeerName("peerchat", PeerNameType.Unsecured);
            //PeerNameRegistration pnReg = new PeerNameRegistration();
            //pnReg.PeerName = peerName;
            //pnReg.Port = 80;
            //pnReg.Comment = "My registration";
            //pnReg.Data = Encoding.UTF8.GetBytes("Some data!");

            //pnReg.Start();

            

            ServiceHost service = new ServiceHost(typeof(PeerNetwork));

            service.Open();

            Console.WriteLine("Press <ENTER> to determine.");
            Console.ReadLine();

            service.Close();
            //pnReg.Stop();
        }
    }
}
