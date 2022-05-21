using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NorthwindMvcClient.Views
{
    public static class HtmlExtensions
    {
        public static HtmlString Image(this IHtmlHelper html, byte[] image, string alt = "image") => 
            new ($@"<img src=""data:image/jpg;base64,{Convert.ToBase64String(image is not null ? image[78..] : Array.Empty<byte>())}"" alt=""{alt}""/>");

        public static HtmlString Image(this IHtmlHelper html, byte[] image, int width, int height, string alt = "image") =>
            new ($@"<img src=""data:image/jpg;base64,{Convert.ToBase64String(image is not null ? image[78..] : Array.Empty<byte>())}"" width=""{width}"" height=""{height}"" alt=""{alt}""/>");
    }
}
