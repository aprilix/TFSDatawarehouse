using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarehouseAdapterTests
{
    [TestClass]
    public class RegexTests
    {
        [TestMethod]
        public void Can_Determine_Visual_Studio_Commercial_Name_By_Their_Version_Number()
        {
            var userAgents = new List<string>(){
                "Team Foundation (devenv.exe, 10.0.30319.1)",
                "Team Foundation (devenv.exe, 10.0.40219.1)",
                "Team Foundation (devenv.exe, 10.0.40219.383)",
                "Team Foundation (devenv.exe, 10.0.40219.445)",
                "Team Foundation (devenv.exe, 10.0.40219.457)",
                "Team Foundation (devenv.exe, 11.0.50727.1, Premium, SKU:7)",
                "Team Foundation (devenv.exe, 11.0.50727.1, Pro, SKU:6)",
                "Team Foundation (devenv.exe, 11.0.50727.1, TE, SKU:5)",
                "Team Foundation (devenv.exe, 11.0.50727.1, Ultimate, SKU:8)",
                "Team Foundation (devenv.exe, 11.0.60610.1, Premium, SKU:7)",
                "Team Foundation (devenv.exe, 11.0.60610.1, Pro, SKU:6)",
                "Team Foundation (devenv.exe, 11.0.60610.1, Ultimate, SKU:8)",
                "Team Foundation (devenv.exe, 11.0.61030.0, Premium, SKU:7)",
                "Team Foundation (devenv.exe, 11.0.61030.0, Pro, SKU:6)",
                "Team Foundation (devenv.exe, 11.0.61030.0, Ultimate, SKU:8)",
                "Team Foundation (devenv.exe, 12.0.21005.1, Premium, SKU:16)",
                "Team Foundation (devenv.exe, 12.0.21005.1, Ultimate, SKU:17)",
                "Team Foundation (devenv.exe, 9.0.30729.4413)",
                "Team Foundation (devenv.exe, 9.0.30729.5820)"
            };

            var tuplas = new List<Tuple<string, string>>();

            foreach (var agent in userAgents)
            {
                var Software = "";

                if (Regex.Match(agent.ToUpper(), @".+DEVENV.EXE, 8\.").Success)
                    Software = "Visual Studio 2005";
                else if (Regex.Match(agent.ToUpper(), @".+DEVENV.EXE, 9\.").Success)
                    Software = "Visual Studio 2008";
                else if (Regex.Match(agent.ToUpper(), @".+DEVENV.EXE, 10\.").Success)
                    Software = "Visual Studio 2010";
                else if (Regex.Match(agent.ToUpper(), @".+DEVENV.EXE, 11\.").Success)
                    Software = "Visual Studio 2012";
                else if (Regex.Match(agent.ToUpper(), @".+DEVENV.EXE, 12\.").Success)
                    Software = "Visual Studio 2013";

                tuplas.Add(new Tuple<string, string>(agent, Software));
            }
            
            tuplas.ForEach(x => Console.WriteLine(string.Format("{0} -> {1}", x.Item1, x.Item2)));

            tuplas.ForEach(x => Assert.IsFalse(string.IsNullOrEmpty(x.Item2)));
        }
    }
}
