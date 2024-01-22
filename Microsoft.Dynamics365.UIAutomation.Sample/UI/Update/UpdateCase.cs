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
    public class UpdateCase : TestsBase
    {

        [TestCategory("Entity")]
        [TestMethod]
        public void TestUpdateActiveCase()
        {
            var client = new WebClient(TestSettings.Options);
            using (var xrmApp = new XrmApp(client))
            {
                xrmApp.OnlineLogin.Login(_xrmUri, _username, _password, _mfaSecretKey);

                xrmApp.Navigation.OpenApp(AppName.CustomerService);

                xrmApp.Navigation.OpenSubArea("Service", "Cases");

                xrmApp.Grid.SwitchView("Active Cases");

                xrmApp.Grid.OpenRecord(0);

                xrmApp.Entity.SetValue("description", TestSettings.GetRandomString(10,15));

                xrmApp.Entity.Save();

            }
            
        }
        [TestCategory("Entity")]
        [TestCategory("RelatedGrid")]
        [TestMethod]
        public void TestUpdateActiveCaseSetHeaderValues()
        {
            var client = new WebClient(TestSettings.Options);
            using (var xrmApp = new XrmApp(client))
            {
                xrmApp.OnlineLogin.Login(_xrmUri, _username, _password, _mfaSecretKey);

                xrmApp.Navigation.OpenApp(AppName.CustomerService);

                xrmApp.Navigation.OpenSubArea("Service", "Cases");

                xrmApp.Grid.SwitchView("Active Cases");

                xrmApp.RelatedGrid.OpenGridRow(0);

                OptionSet priorityCode = new OptionSet() { Name = "prioritycode", Value="Low" };
                xrmApp.Entity.SetHeaderValue(priorityCode);

                xrmApp.Entity.Save();

                xrmApp.ThinkTime(2000);

            }
        }
    }
}