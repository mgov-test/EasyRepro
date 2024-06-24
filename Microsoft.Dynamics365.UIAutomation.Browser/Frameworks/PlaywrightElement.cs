using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Dynamics365.UIAutomation.Browser
{
    internal class PlaywrightElement : IElement
    {
        private ILocator _element;
        public PlaywrightElement(ILocator element) {
            _element = element;

        }
        private string _tag;
        private string _locator;
        private string _id;
        private string _value;
        private bool _isAvaiable;
        private bool _isClickable;
        private bool _selected;
        private string _text;
        public string Tag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Locator { get => _locator; set => _locator = value; }
        public string Id { get => _element.GetAttributeAsync("id").Result; set => throw new NotImplementedException(); }
        public string Value { get => _element.InnerTextAsync().GetAwaiter().GetResult(); set => throw new NotImplementedException(); }
        public bool IsAvailable { get => _element.IsEnabledAsync().Result; set => throw new NotImplementedException(); }
        public bool IsClickable { get => _element.IsEditableAsync().Result; set => throw new NotImplementedException(); }
        public bool Selected { get => throw new NotImplementedException(); //_element.GetByRole(AriaRole.Textbox, new LocatorGetByRoleOptions() { });
                                                                           set => throw new NotImplementedException(); }
        public string Text { get => _element.InnerTextAsync().GetAwaiter().GetResult(); set => throw new NotImplementedException(); }
         
        public void Clear(BrowserPage page, string key)
        {
            Trace.TraceInformation("[Playwright Element] Clear inititated. XPath: " + key);
            this._element.ClearAsync().GetAwaiter().GetResult();
        }

        public void Click(BrowserPage page, bool? click = true)
        {
            Trace.TraceInformation("[Playwright Element] Click inititated.");
            this._element.ClickAsync().GetAwaiter().GetResult();
        }

        public bool ClickWhenAvailable(BrowserPage page, string key)
        {
            throw new NotImplementedException();
        }

        public bool ClickWhenAvailable(BrowserPage page, string key, TimeSpan timeToWait, string? exceptionMessage = null)
        {
            throw new NotImplementedException();
        }

        public void DoubleClick(BrowserPage page, string key)
        {
            throw new NotImplementedException();
        }

        public void Focus(BrowserPage page, string key)
        {
            throw new NotImplementedException();
        }

        public string GetAttribute(BrowserPage page, string attributeName)
        {
            return this._element.GetAttributeAsync(attributeName).GetAwaiter().GetResult();
        }

        public bool HasAttribute(BrowserPage page, string attributeName)
        {
            throw new NotImplementedException();
        }

        public void Hover(BrowserPage page, string key)
        {
            throw new NotImplementedException();
        }

        public void SendKeys(BrowserPage page, string[] keys)
        {
            throw new NotImplementedException();
        }

        public void SetValue(BrowserPage page, string value)
        {
            Trace.TraceInformation("[Playwright Element] Set Value inititated. XPath: " + _locator);
            _element.FillAsync(value).Wait();
        }

        public void Test(BrowserPage page, string value)
        {
            throw new NotImplementedException();
        }
    }
}
