using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.Playwright;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;

//using PlaywrightSharp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Dynamics365.UIAutomation.Browser
{
    internal class PlaywrightBrowser : IWebBrowser, IDisposable
    {
        private BrowserOptions _options;
        private IBrowser _browser;
        private IPage _page;
        private string _url = "";

        public PlaywrightBrowser(IBrowser browser, BrowserOptions options)
        {
            _options = options;
            _browser = browser;

            _page = _browser.NewPageAsync().GetAwaiter().GetResult();
        }

        public BrowserOptions Options { get { return _options; } set { _options = value; } }

        public string Url
        {
            get
            {
                try
                {
                   return _page.EvaluateAsync("document.location.href").Result.ToString();
                }
                catch (AggregateException ex)
                {

                    return _page.Url;
                }
            } set { _url = value; } }

        #region FindElement
        public IElement FindElement(string selector)
        {
            return GetElement(selector);
        }
        public List<IElement> FindElements(string selector)
        {
            return GetElements(selector);
        }
        #endregion

        #region ExecuteScript
        public object ExecuteScript(string script, params object[] args)
        {
            return _page.EvaluateAsync(script, args);
        }
        #endregion
        //private IElement? ConvertToElement(ILocator element, string selector)
        //{
        //    IElement rtnObject = new PlaywrightElement(element);
        //    if (element == null) return null;
        //    try
        //    {
        //        rtnObject.Text = element.InnerTextAsync().Result;
        //        //rtnObject.Tag = element.;
        //        //rtnObject.Selected = element.IsCheckedAsync().Result;
        //        rtnObject.Value = element.InnerTextAsync().Result;
        //        rtnObject.Id = element.GetAttributeAsync("id").Result;
        //        rtnObject.Locator = selector;
        //    }
        //    catch (StaleElementReferenceException staleEx)
        //    {
        //        return null;
        //        //throw;
        //    }



        //    return rtnObject;
        //}

        //private ICollection<IElement> ConvertToElements(IReadOnlyCollection<ILocator> elements, string selector)
        //{
        //    ICollection<IElement> rtnObject = new List<IElement>();
        //    foreach (var element in elements)
        //    {
        //        rtnObject.Add(ConvertToElement(element, selector));
        //    }
        //    return rtnObject;
        //}

        #region SendKeys

        public void SendKey(string selector, string key)
        {
            Trace.TraceInformation("[Playwright] Browser send key inititated. Key: " + key);
            _page.Keyboard.PressAsync(key);
        }

        public void SendKeys(string selector, string[] keys)
        {
            _page.Keyboard.TypeAsync(keys.ToString());
        }
        #endregion
        #region Navigate
        //https://playwright.dev/docs/navigations
        public void Navigate(string url)
        {
            Trace.TraceInformation("[Playwright] Browser navigate inititated. URL: " + url);
            _page.GotoAsync(url).GetAwaiter().GetResult();
            try
            {
                _page.WaitForLoadStateAsync(LoadState.NetworkIdle).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Trace.TraceError("[Playwright] Browser navigate error. URL: " + url + ". Error: " + ex.Message);
            }

        }
        public async void NavigateAsync(string url)
        {
            await _page.GotoAsync(url);
        }
        #endregion

        #region SwitchToFrame

        public void SwitchToFrame(string name, IElement? frameElement = null)
        {
            Trace.TraceInformation("[Playwright] Browser SwitchToFrame inititated. Name: " + name);
            try
            {
                if (frameElement != null)
                {
                    throw new NotImplementedException();
                }

                if (int.TryParse(name, out var frame))
                {
                    _page.Frames[frame].WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
                else
                {
                    _page.Frame(name).WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[Playwright] Browser SwitchToFrame error. Name: " + name + ". Error: " + ex.Message);
            }


        }


        #endregion

        #region TakeWindowScreenShot

        public void TakeWindowScreenShot(string filename, FileFormat fileFormat)
        {
            _page.ScreenshotAsync(new()
            {
                Path = filename,
                FullPage = true,
            });
        }
        #endregion

        #region HasElement
        public bool HasElement(string selector)
        {
            Trace.TraceInformation("[Playwright] Browser has element inititated. XPath: " + selector);
            if (_page.QuerySelectorAsync(selector).GetAwaiter().GetResult() == null)
            {
                return false;
            }
            else if (_page.QuerySelectorAllAsync(selector).GetAwaiter().GetResult().Count > 0)
            {
                return true;
            }
            else
            {
                return this.GetElement(selector).IsAvailable;
            }
        }
        #endregion

        #region Wait
        public void Wait(PageEvent pageEvent)
        {
            _page.WaitForLoadStateAsync(LoadState.Load).GetAwaiter().GetResult();
            _page.WaitForURLAsync(_page.Url).GetAwaiter().GetResult();
            _page.WaitForTimeoutAsync(1000).Wait();
        }
        public void Wait(TimeSpan? timeSpan = null)
        {
            ThinkTime((int)timeSpan.GetValueOrDefault(Constants.DefaultTimeout).TotalMilliseconds);
        }
        public void Wait(int milliseconds)
        {
            ThinkTime(milliseconds);
        }
        private void ThinkTime(int milliseconds)
        {
            Trace.TraceInformation("[Playwright] ThinkTime inititated. Milliseconds: " + milliseconds);
            Thread.Sleep(milliseconds);
        }
        #endregion

        #region WaitUntilAvailable
        internal void WaitUntilAvailable(string selector)
        {
            WaitForSelector(selector);
        }
        #endregion

        #region IsAvailable
        public bool IsAvailable(string selector)
        {
            var isAvailable = _page.WaitForSelectorAsync(selector).GetAwaiter().GetResult();
            return (isAvailable != null) ? true : false;
        }
        #endregion

        #region WaitUntilVisibile
        IElement IWebBrowser.WaitUntilAvailable(string selector)
        {
            WaitForSelector(selector);
            return GetElement(selector);
        }


        public IElement WaitUntilAvailable(string selector, TimeSpan timeToWait, string exceptionMessage)
        {
            try
            {
                PageWaitForSelectorOptions options = new PageWaitForSelectorOptions();
                options.Timeout = (float)timeToWait.TotalMilliseconds;
                WaitForSelector(selector, options);
            }
            catch (Exception e)
            {
                throw new Exception(exceptionMessage);
            }
            return GetElement(selector);
        }

        public IElement WaitUntilAvailable(string selector, string exceptionMessage)
        {
            try
            {
                WaitForSelector(selector);
            }
            catch (Exception e)
            {
                throw new Exception(exceptionMessage);
            }
            return GetElement(selector);
        }


        IElement IWebBrowser.WaitUntilAvailable(string selector, string exceptionMessage)
        {
            try
            {
                WaitForSelector(selector);
            }
            catch (Exception e)
            {
                throw new Exception(exceptionMessage);
            }
            return GetElement(selector);
        }
        #endregion


        #region WaitForSelector
        internal void WaitForSelector(string selector)
        {
            Trace.TraceInformation("[Playwright] Browser WaitForSelector inititated. XPath: " + selector);
            try
            {
                _page.WaitForLoadStateAsync(LoadState.NetworkIdle).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {

                Trace.TraceError("[Playwright] Error in WaitForSelector. XPath: " + selector + ". Error: " + ex.Message);
            }

            try
            {
                IElementHandle element = _page.QuerySelectorAsync(selector).GetAwaiter().GetResult();
                if (element == null)
                {
                    foreach(var frame in _page.Frames)
                    {
                        element = frame.QuerySelectorAsync(selector).GetAwaiter().GetResult();
                        if (element != null)
                        {
                            this.SwitchToFrame(frame.Name);
                            break;
                        }
                    }
                }
                else
                {
                    _page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions()
                    {
                        State = WaitForSelectorState.Visible
                    }).GetAwaiter().GetResult();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        internal void WaitForSelector(string selector, PageWaitForSelectorOptions options)
        {
            _page.WaitForLoadStateAsync(LoadState.NetworkIdle).GetAwaiter().GetResult();
            //_page.WaitForLoadStateAsync(LifecycleEvent.Networkidle).GetAwaiter().GetResult();
            _page.WaitForSelectorAsync(selector, options).GetAwaiter().GetResult();
        }

        internal async Task WaitForSelectorAsync(string selector)
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _page.WaitForSelectorAsync(selector);
        }
        #endregion


        #region GetElement
        private IElement GetElement(string selector)
        {
            Trace.TraceInformation("[Playwright] Browser get element inititated. XPath: " + selector);
            ILocator playWrightElement = _page.Locator(selector);
            if (playWrightElement == null) { throw new PlaywrightException(String.Format("Could not find element using selector '{0}'", selector)); }
            return playWrightElement.ToElement(_page,selector);
        }
        private List<IElement> GetElements(string selector)
        {
            Trace.TraceInformation("[Playwright] Browser get elements inititated. XPath: " + selector);
            //IReadOnlyList<IElementHandle> playWrightElements = (IReadOnlyList<IElementHandle>)_page.QuerySelectorAllAsync(selector).GetAwaiter().GetResult();

            IReadOnlyList<ILocator> locator = _page.Locator(selector).AllAsync().Result;

            if (locator == null) { throw new PlaywrightException(String.Format("Could not find element using selector '{0}'", selector)); }
            List<IElement> elements = new List<IElement>();
            foreach(var element in locator)
            {
                elements.Add(element.ToElement(_page, selector));
            }
            return elements;
        }
        #endregion

        #region Disposal / Finalization

        private readonly object syncRoot = new object();
        private readonly bool disposeOfDriver = true;
        private bool disposing = false;

        public void Dispose()
        {
            bool isDisposing;

            lock (this.syncRoot)
            {
                isDisposing = disposing;
            }

            if (!isDisposing)
            {
                lock (this.syncRoot)
                {
                    disposing = true;
                }
            }
        }

        public bool ClickWhenAvailable(string selector, TimeSpan timeToWait, string? exceptionMessage)
        {
            //throw new NotImplementedException();
            ILocator locator = _page.Locator(selector);
            IElementHandle element = _page.QuerySelectorAsync(selector).GetAwaiter().GetResult();
            if (element == null)
            {
                foreach (var frame in _page.Frames)
                {
                    element = frame.QuerySelectorAsync(selector).GetAwaiter().GetResult();
                    if (element != null)
                    {
                        this.SwitchToFrame(frame.Name);
                        locator = _page.Locator(selector);
                        element = frame.QuerySelectorAsync(selector).GetAwaiter().GetResult();
                        frame.ClickAsync(selector).GetAwaiter().GetResult();
                        break;
                    }
                }
            }
            else
            {
                locator.ClickAsync(new LocatorClickOptions()
                {

                }).GetAwaiter().GetResult();
            }

            return true;
        }

        public bool ClickWhenAvailable(string selector)
        {
            Trace.TraceInformation("[Playwright] Browser ClickWhenAvailable inititated. XPath: " + selector);
            ILocator locator = _page.Locator(selector);
            try
            {
                locator.ClickAsync(new LocatorClickOptions()
                {

                }).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }



        #endregion Disposal / Finalization




        //bool IWebBrowser.DoubleClick(string selector)
        //{
        //    _page.DblClickAsync(selector).GetAwaiter().GetResult();
        //    return true;
        //}


    }
}
