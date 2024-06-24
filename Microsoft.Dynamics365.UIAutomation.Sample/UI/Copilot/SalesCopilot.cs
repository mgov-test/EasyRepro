﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using System;
using System.Security;
using Microsoft.Dynamics365.UIAutomation.Api;

namespace Microsoft.Dynamics365.UIAutomation.Sample.Sales
{
    public static class SalesCopilot
    {
        public static string controlId = "AppSidePane_MscrmControls.CSIntelligence.AICopilotControl";
        public static string controlButtonId = "sidepane-tab-button-AppSidePane_MscrmControls.CSIntelligence.AICopilotControl";
        public static string userInputId = "webchat-sendbox-input";
        public static string Name = "name";
        public static string StartTime = "starttime";
        public static string EndTime = "endtime";
        public static string Duration = "duration";
        public static string Capacity = "msdyn_effort";
        public static string Resource = "resource";
        public static string BookingStatus = "bookingstatus";
        public static string ResourceRequirement = "msdyn_resourcerequirement";
        public static string BookingType = "bookingtype";
    }

    [TestClass]
    public class SalesCopilotTests : TestsBase
    {

        [TestCategory("Sales Hub")]
        [TestCategory("Copilot")]
        [TestMethod]
        public void CustomerServiceCopilot_AskAQuestion()
        {
            var client = new WebClient(TestSettings.Options);
            using (var xrmApp = new XrmApp(client))
            {
                xrmApp.OnlineLogin.Login(_xrmUri, _username, _password, _mfaSecretKey);

                xrmApp.Navigation.OpenApp(AppName.CustomerService);

                xrmApp.Navigation.OpenSubArea("Services", "Cases");

                xrmApp.Grid.SwitchView("Active Cases");

                xrmApp.Grid.OpenRecord(0);

                var copilotResponse = xrmApp.CustomerServiceCopilot.AskAQuestion("Help summarize this case");

                trace.Log("Suggest a call response: " + copilotResponse);
            }

        }

        [TestCategory("Customer Service Workspace")]
        [TestCategory("Copilot")]
        [TestMethod]
        public void CustomerServiceCopilot_WriteAnEmail()
        {
            var client = new WebClient(TestSettings.Options);
            using (var xrmApp = new XrmApp(client))
            {
                xrmApp.OnlineLogin.Login(_xrmUri, _username, _password, _mfaSecretKey);

                xrmApp.Navigation.OpenApp(AppName.CustomerService);

                xrmApp.Navigation.QuickCreate("email");

                var copilotResponse = xrmApp.CustomerServiceCopilot.WriteAnEmail("Suggest a call");
                trace.Log("Suggest a call response: " + copilotResponse);
            }

        }
    }
}