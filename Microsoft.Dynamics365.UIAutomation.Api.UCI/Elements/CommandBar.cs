﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;

namespace Microsoft.Dynamics365.UIAutomation.Api.UCI
{
    public class CommandBar : Element
    {
        #region DTO
        public class CommandBarReference
        {
            public const string CommandBar = "CommandBar";
            #region private
            private string _Container = ".//ul[contains(@data-lp-id,\"commandbar-Form\")]";
            private string _ContainerGrid = "//ul[contains(@data-lp-id,\"commandbar-HomePageGrid\")]";
            private string _MoreCommandsMenu = "//*[@id=\"__flyoutRootNode\"]";
            private string _Button = "//*[contains(text(),'[NAME]')]";
            #endregion
            #region prop
            public string Container { get => _Container; set { _Container = value; } }
            public string ContainerGrid { get => _ContainerGrid; set { _ContainerGrid = value; } }
            public string MoreCommandsMenu { get => _MoreCommandsMenu; set { _MoreCommandsMenu = value; } }
            public string Button { get => _Button; set { _Button = value; } }
            #endregion
        }
        #endregion
        private readonly WebClient _client;
        private Entity.EntityReference _entityReference;
        #region ctor
        public CommandBar(WebClient client) : base()
        {
            _client = client;
            _entityReference = new Entity.EntityReference();
        }
        #endregion

        //public void ClickCommand(string name, string subname = null, string subSecondName = null)
        //{
        //    _client.ClickCommand(name, subname, subSecondName);
        //}



        /// <summary>
        /// Returns the values of CommandBar objects
        /// </summary>
        /// <param name="includeMoreCommandsValues">Flag to determine whether values should be returned from the more commands menu</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmApp.CommandBar.GetCommandValues(true);</example>
        //public BrowserCommandResult<List<string>> GetCommandValues(bool includeMoreCommandsValues = false, int thinkTime = Constants.DefaultThinkTime)
        //{
        //    List<string> commandValues = new List<string>();
        //    commandValues = _client.GetCommandValues(includeMoreCommandsValues, thinkTime);

        //    return commandValues;
        //}

        #region public Browser Commands
        #region CommandBar
        /// <summary>
        /// Clicks command on the command bar
        /// </summary>
        /// <param name="name">Name of button to click</param>
        /// <param name="subname">Name of button on submenu to click</param>
        /// <param name="subSecondName">Name of button on submenu (3rd level) to click</param>
        public BrowserCommandResult<bool> ClickCommand(string name, string subname = null, string subSecondName = null, int thinkTime = Constants.DefaultThinkTime)
        {
            return _client.Execute(_client.GetOptions($"Click Command"), driver =>
            {
                // Find the button in the CommandBar
                IWebElement ribbon;
                // Checking if any dialog is active
                if (driver.HasElement(By.XPath(string.Format(_client.ElementMapper.DialogsReference.DialogContext))))
                {
                    var dialogContainer = driver.FindElement(By.XPath(string.Format(_client.ElementMapper.DialogsReference.DialogContext)));
                    ribbon = dialogContainer.WaitUntilAvailable(By.XPath(string.Format(_client.ElementMapper.CommandBarReference.Container)));
                }
                else
                {
                    ribbon = driver.WaitUntilAvailable(By.XPath(_client.ElementMapper.CommandBarReference.Container));
                }


                if (ribbon == null)
                {
                    ribbon = driver.WaitUntilAvailable(By.XPath(_client.ElementMapper.CommandBarReference.ContainerGrid),
                        TimeSpan.FromSeconds(5),
                        "Unable to find the ribbon.");
                }

                //Is the button in the ribbon?
                if (ribbon.TryFindElement(By.XPath(_client.ElementMapper.SubGridReference.SubGridCommandLabel.Replace("[NAME]", name)), out var command))
                {
                    command.Click(true);
                    driver.WaitForTransaction();
                }
                else
                {
                    //Is the button in More Commands?
                    if (ribbon.TryFindElement(By.XPath(_client.ElementMapper.RelatedGridReference.CommandBarOverflowButton), out var moreCommands))
                    {
                        // Click More Commands
                        moreCommands.Click(true);
                        driver.WaitForTransaction();

                        //Click the button
                        var flyOutMenu = driver.WaitUntilAvailable(By.XPath(_client.ElementMapper.RelatedGridReference.CommandBarFlyoutButtonList)); ;
                        if (flyOutMenu.TryFindElement(By.XPath(_client.ElementMapper.SubGridReference.SubGridCommandLabel.Replace("[NAME]", name)), out var overflowCommand))
                        {
                            overflowCommand.Click(true);
                            driver.WaitForTransaction();
                        }
                        else
                            throw new InvalidOperationException($"No command with the name '{name}' exists inside of Commandbar or the flyout menu.");
                    }
                    else if (ribbon.TryFindElement(By.XPath(_client.ElementMapper.EntityReference.MoreCommands), out moreCommands))
                    {
                        moreCommands.Click(true);
                        driver.WaitForTransaction();

                        //Click the button
                        var flyOutMenu = driver.WaitUntilAvailable(By.XPath(_client.ElementMapper.RelatedGridReference.CommandBarFlyoutButtonList)); ;
                        if (flyOutMenu.TryFindElement(By.XPath(_client.ElementMapper.SubGridReference.SubGridCommandLabel.Replace("[NAME]", name)), out var overflowCommand))
                        {
                            overflowCommand.Click(true);
                            driver.WaitForTransaction();
                        }
                        else
                            throw new InvalidOperationException($"No command with the name '{name}' exists inside of Commandbar or the flyout menu.");
                    }
                    else
                        throw new InvalidOperationException($"No command with the name '{name}' exists inside of Commandbar.");
                }

                if (!string.IsNullOrEmpty(subname))
                {
                    var submenu = driver.WaitUntilAvailable(By.XPath(_client.ElementMapper.CommandBarReference.MoreCommandsMenu));

                    submenu.TryFindElement(By.XPath(_client.ElementMapper.SubGridReference.SubGridOverflowButton.Replace("[NAME]", subname)), out var subbutton);

                    if (subbutton != null)
                    {
                        subbutton.Click(true);
                    }
                    else
                        throw new InvalidOperationException($"No sub command with the name '{subname}' exists inside of Commandbar.");

                    if (!string.IsNullOrEmpty(subSecondName))
                    {
                        var subSecondmenu = driver.WaitUntilAvailable(By.XPath(_client.ElementMapper.CommandBarReference.MoreCommandsMenu));

                        subSecondmenu.TryFindElement(
                            By.XPath(_client.ElementMapper.SubGridReference.SubGridOverflowButton
                                .Replace("[NAME]", subSecondName)), out var subSecondbutton);

                        if (subSecondbutton != null)
                        {
                            subSecondbutton.Click(true);
                        }
                        else
                            throw new InvalidOperationException($"No sub command with the name '{subSecondName}' exists inside of Commandbar.");
                    }
                }

                driver.WaitForTransaction();

                return true;
            });
        }


        /// <summary>
        /// Returns the values of CommandBar objects
        /// </summary>
        /// <param name="includeMoreCommandsValues">Whether or not to check the more commands overflow list</param>
        /// <param name="thinkTime">Used to simulate a wait time between human interactions. The Default is 2 seconds.</param>
        /// <example>xrmApp.CommandBar.GetCommandValues();</example>
        public BrowserCommandResult<List<string>> GetCommandValues(bool includeMoreCommandsValues = false, int thinkTime = Constants.DefaultThinkTime)
        {
            _client.ThinkTime(thinkTime);

            return this._client.Execute(_client.GetOptions("Get CommandBar Command Count"), driver => TryGetCommandValues(includeMoreCommandsValues, driver));
        }

        private List<string> TryGetCommandValues(bool includeMoreCommandsValues, IWebDriver driver)
        {
            const string moreCommandsLabel = "more commands";

            //Find the button in the CommandBar
            IWebElement ribbon = GetRibbon(driver);

            //Get the CommandBar buttons
            Dictionary<string, IWebElement> commandBarItems = GetMenuItems(ribbon);
            bool hasMoreCommands = commandBarItems.TryGetValue(moreCommandsLabel, out var moreCommandsButton);
            if (includeMoreCommandsValues && hasMoreCommands)
            {
                moreCommandsButton.Click(true);

                driver.WaitUntilVisible(By.XPath(_client.ElementMapper.CommandBarReference.MoreCommandsMenu),
                    menu => AddMenuItems(menu, commandBarItems),
                    "Unable to locate the 'More Commands' menu"
                    );
            }

            var result = GetCommandNames(commandBarItems.Values);
            return result;
        }
        private static void AddMenuItems(IWebElement menu, Dictionary<string, IWebElement> dictionary)
        {
            var menuItems = menu.FindElements(By.TagName("li"));
            foreach (var item in menuItems)
            {
                string key = item.Text.ToLowerString();
                if (dictionary.ContainsKey(key))
                    continue;
                dictionary.Add(key, item);
            }
        }
        private static Dictionary<string, IWebElement> GetMenuItems(IWebElement menu)
        {
            var result = new Dictionary<string, IWebElement>();
            AddMenuItems(menu, result);
            return result;
        }

        private static List<string> GetCommandNames(IEnumerable<IWebElement> commandBarItems)
        {
            var result = new List<string>();
            foreach (var value in commandBarItems)
            {
                string commandText = value.Text.Trim();
                if (string.IsNullOrWhiteSpace(commandText))
                    continue;

                if (commandText.Contains("\r\n"))
                {
                    commandText = commandText.Substring(0, commandText.IndexOf("\r\n", StringComparison.Ordinal));
                }
                result.Add(commandText);
            }
            return result;
        }

        private IWebElement GetRibbon(IWebDriver driver)
        {
            var xpathCommandBarContainer = By.XPath(_client.ElementMapper.CommandBarReference.Container);
            var xpathCommandBarGrid = By.XPath(_client.ElementMapper.CommandBarReference.ContainerGrid);

            IWebElement ribbon =
                driver.WaitUntilAvailable(xpathCommandBarContainer, 5.Seconds()) ??
                driver.WaitUntilAvailable(xpathCommandBarGrid, 5.Seconds()) ??
                throw new InvalidOperationException("Unable to find the ribbon.");

            return ribbon;
        }

        internal BrowserCommandResult<bool> CloseOpportunity(bool closeAsWon, int thinkTime = Constants.DefaultThinkTime)
        {
            _client.ThinkTime(thinkTime);

            var xPathQuery = closeAsWon
                ? _entityReference.CloseOpportunityWin
                : _entityReference.CloseOpportunityLoss;

            return _client.Execute(_client.GetOptions($"Close Opportunity"), driver =>
            {
                var closeBtn = driver.WaitUntilAvailable(By.XPath(xPathQuery), "Opportunity Close Button is not available");

                closeBtn?.Click();
                driver.WaitUntilVisible(By.XPath(_client.ElementMapper.DialogsReference.CloseOpportunity.Ok));
                Dialogs dialogs = new Dialogs(_client);
                dialogs.CloseOpportunityDialog(true);

                return true;
            });
        }

        internal BrowserCommandResult<bool> CloseOpportunity(double revenue, DateTime closeDate, string description, int thinkTime = Constants.DefaultThinkTime)
        {
            _client.ThinkTime(thinkTime);

            return _client.Execute(_client.GetOptions($"Close Opportunity"), driver =>
            {
                //SetValue(Elements.ElementId[AppReference.Dialogs.CloseOpportunity.ActualRevenueId], revenue.ToString(CultureInfo.CurrentCulture));
                //SetValue(Elements.ElementId[AppReference.Dialogs.CloseOpportunity.CloseDateId], closeDate);
                //SetValue(Elements.ElementId[AppReference.Dialogs.CloseOpportunity.DescriptionId], description);

                driver.ClickWhenAvailable(By.XPath(_client.ElementMapper.DialogsReference.CloseOpportunity.Ok),
    TimeSpan.FromSeconds(5),
    "The Close Opportunity dialog is not available."
    );

                return true;
            });
        }

        internal BrowserCommandResult<bool> Delete(int thinkTime = Constants.DefaultThinkTime)
        {
            _client.ThinkTime(thinkTime);

            return _client.Execute(_client.GetOptions($"Delete Entity"), driver =>
            {
                var deleteBtn = driver.WaitUntilAvailable(By.XPath(_entityReference.Delete),
                    "Delete Button is not available");

                deleteBtn?.Click();
                Dialogs dialogs = new Dialogs(_client);
                dialogs.ConfirmationDialog(true);

                driver.WaitForTransaction();

                return true;
            });
        }

        /// <summary>
        /// Saves the entity
        /// </summary>
        /// <param name="thinkTime"></param>
        internal BrowserCommandResult<bool> Save(int thinkTime = Constants.DefaultThinkTime)
        {
            _client.ThinkTime(thinkTime);

            return _client.Execute(_client.GetOptions($"Save"), driver =>
            {
                Actions action = new Actions(driver);
                action.KeyDown(Keys.Control).SendKeys("S").Perform();

                return true;
            });
        }
        #endregion
        #endregion
    }
}
