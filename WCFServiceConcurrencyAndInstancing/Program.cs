using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace WCFServiceConcurrencyAndInstancing
{

    //public class NetDataContractFormatAttribute : Attribute, IOperationBehavior
    //{

    //    public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
    //    {

    //    }

    //    public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    //    {
    //        ReplaceDataContractSerializerOperationBehavior(operationDescription);
    //    }

    //    public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    //    {
    //        ReplaceDataContractSerializerOperationBehavior(operationDescription);
    //    }

    //    private static void ReplaceDataContractSerializerOperationBehavior(OperationDescription operationDescription)
    //    {

    //        DataContractSerializerOperationBehavior dcs = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>();
    //        if (dcs != null)
    //            operationDescription.Behaviors.Remove(dcs);

    //        operationDescription.Behaviors.Add(new NetDataContractSerializerOperationBehavior(operationDescription));
    //    }

    //    public class NetDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
    //    {
    //        private static NetDataContractSerializer serializer = new NetDataContractSerializer();

    //        public NetDataContractSerializerOperationBehavior(OperationDescription operationDescription)
    //            : base(operationDescription)
    //        {

    //        }

    //        public NetDataContractSerializerOperationBehavior(OperationDescription operationDescription, DataContractFormatAttribute dataContractFormatAttribute)
    //            : base(operationDescription, dataContractFormatAttribute)
    //        {

    //        }

    //        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
    //        {
    //            return NetDataContractSerializerOperationBehavior.serializer;
    //            //return new NetDataContractSerializer(name, ns);
    //        }

    //        public override XmlObjectSerializer CreateSerializer(Type type, System.Xml.XmlDictionaryString name, System.Xml.XmlDictionaryString ns, IList<Type> knownTypes)
    //        {
    //            return NetDataContractSerializerOperationBehavior.serializer;
    //            //return new NetDataContractSerializer(name, ns);
    //        }
    //    }

    //    public void Validate(OperationDescription operationDescription)
    //    {

    //    }
    //}

    public class MyEdnpointBehavior : IEndpointBehavior
    {

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new MyMessageInspector());
        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }
    }
    public class MyMessageInspector : IDispatchMessageInspector
    {

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            //  Console.WriteLine(request.ToString());
            using (StreamWriter fs = new StreamWriter("request1.xml"))
            {
                fs.Write(request.ToString());

            }
            return request;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            // Console.WriteLine(reply.ToString());
            using (StreamWriter fs = new StreamWriter("response1.xml"))
            {
                fs.Write(reply.ToString());

            }
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class MyOperationBehavior : Attribute, IOperationBehavior
    {

        public string pattern;
        public string message;

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {

        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.Add(new MyParameterInspector(this.pattern, this.message));
        }

        public void Validate(OperationDescription operationDescription)
        {

        }
    }
    public class MyParameterInspector : IParameterInspector
    {
        private string _pattern;
        private string _message;

        public MyParameterInspector(string pattern, string message)
        {
            // TODO: Complete member initialization
            this._pattern = pattern;
            this._message = message;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {

        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            foreach (var input in inputs)
            {
                if ((input != null) && (input.GetType() == typeof(string)))
                {
                    Regex regex = new Regex(_pattern);
                    if (regex.IsMatch((string)input))
                    {
                        throw new FaultException(string.Format("Parameter out of range: {0} , {1}", (string)input, _message));
                    }
                }
            }
            return null;
        }
    }

    // [DataContract]

    public class StockPrice
    {
        //    [DataMember]
        [XmlAttribute]
        public double price;

        //    [DataMember]
        [XmlAttribute]
        public int calls;

        [XmlAttribute]
        public string RequestedBy;
    }

    [XmlSerializerFormat]
    [ServiceContract(SessionMode = SessionMode.Required)]
    //    [ServiceContract]
    interface IStockService
    {
        [OperationContract]
        //[NetDataContractFormat]
        StockPrice GetPrice(string ticker);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
    ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    class StockService : IStockService
    {
        object lockthis = new object();
        private int n_Calls = 0;
        StockService()
        {
            Console.WriteLine("{0} : Created new isntance of StockService on thread", System.DateTime.Now);
        }


        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        [MyOperationBehavior(pattern = "[^a-zA-Z]", message = "Only alpha characters allowed")]
        public StockPrice GetPrice(string ticker)
        {
            StockPrice price = new StockPrice();

            Console.WriteLine("{0} : GetPrice called on thread {1}", System.DateTime.Now, Thread.CurrentThread.ManagedThreadId);
            price.price = 94.85;
            lock (lockthis)
            {
                price.calls = ++n_Calls;
            }
            price.RequestedBy = WindowsIdentity.GetCurrent().Name;
            Thread.Sleep(5000);
            return price;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost service = new ServiceHost(typeof(StockService));
            //ServicePointManager.MaxServicePointIdleTime = 20;
           // ServicePointManager.DefaultConnectionLimit = 1;
            
            
            foreach (ServiceEndpoint endpoint in service.Description.Endpoints)
            {
                endpoint.EndpointBehaviors.Add(new MyEdnpointBehavior());
            }
            
            service.Open();

            Console.WriteLine("Press <ENTER> to determine.");
            Console.ReadLine();

            service.Close();
        }
    }
}
