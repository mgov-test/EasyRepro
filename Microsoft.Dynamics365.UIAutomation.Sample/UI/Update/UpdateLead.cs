﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using System;
using System.Security;

namespace Microsoft.Dynamics365.UIAutomation.Sample
{
    [TestClass]
    public class UpdateLead : TestsBase
    {
        [TestCategory("Entity")]
        [TestMethod]
        public void TestUpdateActiveLead()
        {
            var client = new WebClient(TestSettings.Options);
            using (var xrmApp = new XrmApp(client))
            {
                xrmApp.OnlineLogin.Login(_xrmUri, _username, _password, _mfaSecretKey);

                xrmApp.Navigation.OpenApp(AppName.Sales);

                xrmApp.Navigation.OpenSubArea("Sales", "Leads");

                xrmApp.Grid.SwitchView("Open Leads");

                xrmApp.Grid.OpenRecord(0);

                xrmApp.ThinkTime(3000);

                xrmApp.Entity.SetValue("subject", TestSettings.GetRandomString(10,15));

                xrmApp.Entity.Save();

            }
            
        }

        [TestCategory("Entity")]
        [TestMethod]
        public void TestOpenActiveLeadBPF()
        {
            var client = new WebClient(TestSettings.Options);
            using (var xrmApp = new XrmApp(client))
            {
                xrmApp.OnlineLogin.Login(_xrmUri, _username, _password, _mfaSecretKey);

                xrmApp.Navigation.OpenApp(AppName.Sales);

                xrmApp.Navigation.OpenSubArea("Sales", "Leads");

                xrmApp.Grid.SwitchView("Open Leads");

                xrmApp.Grid.OpenRecord(0);

                xrmApp.BusinessProcessFlow.SelectStage("Qualify");

                xrmApp.BusinessProcessFlow.SetValue("budgetamount", "1000");

                xrmApp.ThinkTime(3000);
            }
        }
    }
}