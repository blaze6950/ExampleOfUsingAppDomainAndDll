using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExampleOfUsingAppDomainAndDll;

namespace FifthClassLibrary
{
    public class Class1 : IExtension
    {
        public string GetExtensionName()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}
