using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Dynamics365.UIAutomation.Browser
{
    public static class PlaywrightExtensions
    {
        public static IElement ToElement(this ILocator element, IPage page, string selector)
        {
            IElement rtnObject = new PlaywrightElement(element);
            if (element == null) return null;
            try
            {
                rtnObject.Locator = selector;
            }
            catch (Playwright.PlaywrightException staleEx)
            {
                return null;
                //throw;
            }
            return rtnObject;
        }

    }
}
