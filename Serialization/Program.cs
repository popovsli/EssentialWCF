using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Schema;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;

namespace Serialization
{
    [DataContract]
    public class Employee
    {
        private int employeeID;
        private string firsName;
        private string lastName;

        public Employee(int employeeID, string firstName, string lastName)
        {
            this.employeeID = employeeID;
            this.firsName = firstName;
            this.lastName = lastName;
        }

        [DataMember]
        public int EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; }
        }

        [DataMember]
        public string FirstName
        {
            get { return firsName; }
            set { firsName = value; }
        }

        [DataMember]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
    }
    
    public class Employee2
    {
        private int employeeID;
        private string firsName;
        private string lastName;

        public Employee2()
        {

        }

        public Employee2(int employeeID, string firstName, string lastName)
        {
            this.employeeID = employeeID;
            this.firsName = firstName;
            this.lastName = lastName;
        }

        public int EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; }
        }

        public string FirstName
        {
            get { return firsName; }
            set { firsName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            XsdDataContractExporter xsdexp = new XsdDataContractExporter();
            xsdexp.Options = new ExportOptions();
            xsdexp.Export(typeof(Employee));

            using (FileStream fs = new FileStream("sample.xsd", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                foreach (XmlSchema sch in xsdexp.Schemas.Schemas())
                {
                    sch.Write(fs);
                }
            }

            Employee e = new Employee(130, "Nikolai", "Popov");

            using (FileStream fs1 = new FileStream("DataConractSerializer.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(Employee));
                ser.WriteObject(fs1, e);
            }

            Employee2 e2 = new Employee2(144, "Nikolai", "Popov");
            List<Employee2> l = new List<Employee2>();
            l.Add(e2);
            l.Add(e2);
            l.Add(e2);
            l.Add(e2);
            l.Add(e2);

            using (FileStream fs2 = new FileStream("XmlSerializer.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Employee2));
                ser.Serialize(fs2, e2);
            }

            using (FileStream fs3 = new FileStream("JSON.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Employee2));
                ser.WriteObject(fs3, e2);
            }

            using (FileStream fs4 = new FileStream("NETDataContract.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                NetDataContractSerializer ser = new NetDataContractSerializer();
                ser.WriteObject(fs4, l);
            }

        }
    }
}
