using System;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace NorthwindMvcClient.Views
{
    public static class HtmlExtensions
    {
        //public static HtmlString Image(this IHtmlHelper html, byte[] image, string alt = "image") => 
        //    new ($@"<img src=""data:image/jpg;base64,{Convert.ToBase64String(image is not null ? image[78..] : Array.Empty<byte>())}"" alt=""{alt}""/>");

        //public static HtmlString Image(this IHtmlHelper html, byte[] image, int width, int height, string alt = "image") =>
        //    new ($@"<img src=""data:image/jpg;base64,{Convert.ToBase64String(image is not null ? image[78..] : Array.Empty<byte>())}"" width=""{width}"" height=""{height}"" alt=""{alt}""/>");

        public static HtmlString Image(this IHtmlHelper html, byte[] image, object attributes = null)
        {
            var tag = new TagBuilder("img")
            {
                Attributes =
                {
                    ["src"] = image is null ? "/img/NoPhoto.jpg" : $"data:image/jpg;base64,{Convert.ToBase64String(image[78..])}"
                }
            };

            if (attributes is not null)
            {
                var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
                foreach (var attribute in htmlAttributes)
                {
                    tag.Attributes.Add(attribute.Key, attribute.Value.ToString());
                }
            }

            using var writer = new StringWriter();
            tag.WriteTo(writer, HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }

        public static HtmlString Image(this IHtmlHelper html, byte[] image, int width, int height, object attributes = null)
        {
            var tag = new TagBuilder("img")
            {
                Attributes =
                {
                    ["src"] = image is null ? "/img/NoPhoto.jpg" : $"data:image/jpg;base64,{Convert.ToBase64String(image[78..])}",
                    ["width"] = width.ToString(),
                    ["height"] = height.ToString(),
                }
            };

            if (attributes is not null)
            {
                var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
                foreach (var attribute in htmlAttributes)
                {
                    tag.Attributes.Add(attribute.Key, attribute.Value.ToString());
                }
            }

            using var writer = new StringWriter();
            tag.WriteTo(writer, HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }
    }
}
