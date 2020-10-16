using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Mirza.Web.Pages
{
    public static class ActivePageManager
    {
        public static string Index => "Index";



        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext ?? throw new ArgumentNullException(nameof(viewContext)), Index);

        public static string ActivatePage(ViewContext viewContext, string pageName) => PageNavClass(viewContext ?? throw new ArgumentNullException(nameof(viewContext)), pageName);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            if (string.Equals(viewContext.RouteData.Values["area"] as string, "Identity", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(page, "Identity", StringComparison.OrdinalIgnoreCase))
                {
                    return "active";
                }
                return null;
            }
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
