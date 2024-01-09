﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Dynamics365.UIAutomation.Api.UCI.DTO;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OtpNet;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;


namespace Microsoft.Dynamics365.UIAutomation.Api.UCI
{
    public class ElementMapper
    {
        public Lookup.AdvancedLookupReference AdvancedLookupReference;
        public Navigation.ApplicationReference ApplicationReference;
        public BusinessProcessFlow.BusinessProcessFlowReference BusinessProcessFlowReference;
        public CommandBar.CommandBarReference CommandBarReference;
        public Dashboard.DashboardReference DashboardReference;
        public Dialogs.DialogsReference DialogsReference;
        public Entity.EntityReference EntityReference;
        public GlobalSearch.GlobalSearchReference GlobalSearchReference;
        public Grid.GridReference GridReference;
        public OnlineLogin.LoginReference LoginReference;
        public Lookup.LookupReference LookupReference;
        public Navigation.NavigationReference NavigationReference;
        public PowerApp.PowerAppReference PowerAppReference;
        public QuickCreate.QuickCreateReference QuickCreateReference;
        public RelatedGrid.RelatedReference RelatedGridReference;
        public SubGrid.SubGridReference SubGridReference;
        public ElementMapper(IConfiguration config) {
            ApplicationReference = new Navigation.ApplicationReference();
            config.GetSection(Navigation.ApplicationReference.Application).Bind(ApplicationReference);
            AdvancedLookupReference = new Lookup.AdvancedLookupReference();
            config.GetSection(Lookup.AdvancedLookupReference.AdvancedLookup).Bind(AdvancedLookupReference);
            BusinessProcessFlowReference = new BusinessProcessFlow.BusinessProcessFlowReference();
            config.GetSection(BusinessProcessFlow.BusinessProcessFlowReference.BusinessProcessFlow).Bind(BusinessProcessFlowReference);
            CommandBarReference = new CommandBar.CommandBarReference();
            config.GetSection(CommandBar.CommandBarReference.CommandBar).Bind(CommandBarReference);
            DashboardReference = new Dashboard.DashboardReference();
            config.GetSection(Dashboard.DashboardReference.Dashboard).Bind(DashboardReference);
            DialogsReference = new Dialogs.DialogsReference();
            config.GetSection(Dialogs.DialogsReference.Dialogs).Bind(DialogsReference);
            EntityReference = new Entity.EntityReference();
            config.GetSection(Entity.EntityReference.Entity).Bind(EntityReference);
            GlobalSearchReference = new GlobalSearch.GlobalSearchReference();
            config.GetSection(GlobalSearch.GlobalSearchReference.GlobalSearch).Bind(GlobalSearchReference);
            GridReference = new Grid.GridReference();
            config.GetSection(Grid.GridReference.Grid).Bind(GridReference);
            LoginReference = new OnlineLogin.LoginReference();
            config.GetSection(OnlineLogin.LoginReference.Login).Bind(LoginReference);
            LookupReference = new Lookup.LookupReference();
            config.GetSection(Lookup.LookupReference.Lookup).Bind(LookupReference);
            NavigationReference = new Navigation.NavigationReference();
            config.GetSection(Navigation.NavigationReference.Navigation).Bind(NavigationReference);
            PowerAppReference = new PowerApp.PowerAppReference();
            config.GetSection(PowerApp.PowerAppReference.PowerApp).Bind(PowerAppReference);
            QuickCreateReference = new QuickCreate.QuickCreateReference();
            config.GetSection(QuickCreate.QuickCreateReference.QuickCreate).Bind(QuickCreateReference);
            RelatedGridReference = new RelatedGrid.RelatedReference();
            config.GetSection(RelatedGrid.RelatedReference.RelatedGrid).Bind(RelatedGridReference);
            SubGridReference = new SubGrid.SubGridReference();
            config.GetSection(SubGrid.SubGridReference.SubGrid).Bind(SubGridReference);
        }
    }
    public class WebClient : BrowserPage, IDisposable
    {
        public List<ICommandResult> CommandResults => Browser.CommandResults;
        public Guid ClientSessionId;
        internal ElementMapper ElementMapper;
        internal IConfiguration Configuration;
        private Entity.EntityReference _entityReference;
        public WebClient(BrowserOptions options)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(options.ConfigPath);
            Browser = new InteractiveBrowser(options);
            ClientSessionId = Guid.NewGuid();
            Configuration = builder.Build();
            ElementMapper = new ElementMapper(Configuration);
            _entityReference = new Entity.EntityReference();
        }



        internal BrowserCommandOptions GetOptions(string commandName)
        {
            return new BrowserCommandOptions(Constants.DefaultTraceSource,
                commandName,
                Constants.DefaultRetryAttempts,
                Constants.DefaultRetryDelay,
                null,
                true,
                typeof(NoSuchElementException), typeof(StaleElementReferenceException));
        }

        #region PageWaits
        internal bool WaitForMainPage(TimeSpan timeout, string errorMessage)
            => WaitForMainPage(timeout, null, () => throw new InvalidOperationException(errorMessage));

        internal bool WaitForMainPage(TimeSpan? timeout = null, Action<IWebElement> successCallback = null, Action failureCallback = null)
        {
            IWebDriver driver = Browser.Driver;
            timeout = timeout ?? Constants.DefaultTimeout;
            successCallback = successCallback ?? (
                                  _ =>
                                  {
                                      bool isUCI = driver.HasElement(By.XPath(this.ElementMapper.LoginReference.CrmUCIMainPage));
                                      if (isUCI)
                                          driver.WaitForTransaction();
                                  });

            var xpathToMainPage = By.XPath(this.ElementMapper.LoginReference.CrmMainPage);
            var element = driver.WaitUntilAvailable(xpathToMainPage, timeout, successCallback, failureCallback);
            return element != null;
        }
        #endregion

        internal void ClickIfVisible(string elementLocator)
        {

        }

        internal Element WaitUntilAvailable(string elementLocator, TimeSpan timeToWait)
        {
            if (this.Browser.Options.BrowserFramework == BrowserFramework.Selenium)
                return new Element();
            else if (this.Browser.Options.BrowserFramework == BrowserFramework.Playwright)
                return new Element();
            else return new Element();
        }

        #region FormContextType

        // Used by SetValue methods to determine the field context
        public IWebElement ValidateFormContext(IWebDriver driver, FormContextType formContextType, string field, IWebElement fieldContainer)
        {
            if (formContextType == FormContextType.QuickCreate)
            {
                // Initialize the quick create form context
                // If this is not done -- element input will go to the main form due to new flyout design
                var formContext = driver.WaitUntilAvailable(By.XPath(this.ElementMapper.QuickCreateReference.QuickCreateFormContext));
                fieldContainer = formContext.WaitUntilAvailable(By.XPath(_entityReference.TextFieldContainer.Replace("[NAME]", field)));
            }
            else if (formContextType == FormContextType.Entity)
            {
                // Initialize the entity form context
                var formContext = driver.WaitUntilAvailable(By.XPath(_entityReference.FormContext));
                fieldContainer = formContext.WaitUntilAvailable(By.XPath(_entityReference.TextFieldContainer.Replace("[NAME]", field)));
            }
            else if (formContextType == FormContextType.BusinessProcessFlow)
            {
                // Initialize the Business Process Flow context
                var formContext = driver.WaitUntilAvailable(By.XPath(this.ElementMapper.BusinessProcessFlowReference.BusinessProcessFlowFormContext));
                fieldContainer = formContext.WaitUntilAvailable(By.XPath(this.ElementMapper.BusinessProcessFlowReference.TextFieldContainer.Replace("[NAME]", field)));
            }
            else if (formContextType == FormContextType.Header)
            {
                // Initialize the Header context
                var formContext = driver.WaitUntilAvailable(By.XPath(_entityReference.HeaderContext));
                fieldContainer = formContext.WaitUntilAvailable(By.XPath(_entityReference.TextFieldContainer.Replace("[NAME]", field)));
            }
            else if (formContextType == FormContextType.Dialog)
            {
                // Initialize the Dialog context
                driver.WaitForTransaction();
                var formContext = driver
                    .FindElements(By.XPath(this.ElementMapper.DialogsReference.DialogContext))
                    .LastOrDefault() ?? throw new NotFoundException("Unable to find a dialog.");
                fieldContainer = formContext.WaitUntilAvailable(By.XPath(_entityReference.TextFieldContainer.Replace("[NAME]", field)));
            }

            return fieldContainer;
        }

        #endregion

        internal void ThinkTime(int milliseconds)
        {
            Browser.ThinkTime(milliseconds);
        }

        internal void ThinkTime(TimeSpan timespan)
        {
            ThinkTime((int)timespan.TotalMilliseconds);
        }

        public void Dispose()
        {
            Browser.Dispose();
        }
    }
}
