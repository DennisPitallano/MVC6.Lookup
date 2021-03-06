﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Options;
using NonFactors.Mvc.Lookup.Tests.Objects;
using NSubstitute;
using System;
using System.IO;
using System.Text.Encodings.Web;
using Xunit;

namespace NonFactors.Mvc.Lookup.Tests.Unit
{
    public class LookupExtensionsTests
    {
        private TestLookup<TestModel> lookup;
        private IHtmlHelper<TestModel> html;

        public LookupExtensionsTests()
        {
            html = MockHtmlHelper();
            lookup = new TestLookup<TestModel>();

            lookup.Filter.Page = 2;
            lookup.Filter.Rows = 11;
            lookup.Dialog = "Dialog";
            lookup.Filter.Sort = "First";
            lookup.Filter.Search = "Test";
            lookup.AdditionalFilters.Clear();
            lookup.AdditionalFilters.Add("Add1");
            lookup.AdditionalFilters.Add("Add2");
            lookup.Title = "Dialog lookup title";
            lookup.Url = "http://localhost/Lookup";
            lookup.Filter.Order = LookupSortOrder.Desc;
        }

        #region AutoComplete<TModel>(this IHtmlHelper<TModel> html, String name, MvcLookup model, Object value = null, Object htmlAttributes = null)

        [Fact]
        public void AutoComplete_FromModel()
        {
            String actual = ToString(html.AutoComplete("Test", lookup, "Value", new { @class = "classes", attribute = "attr" }));
            String expected =
                "<div class=\"mvc-lookup-browseless mvc-lookup-group\">" +
                    "<div class=\"mvc-lookup-values\" data-for=\"Test\">" +
                        "<input class=\"mvc-lookup-value\" id=\"Test\" name=\"Test\" type=\"hidden\" value=\"Value\" />" +
                    "</div>" +
                    "<div attribute=\"attr\" class=\"mvc-lookup-control\" data-dialog=\"Dialog\" " +
                        "data-filters=\"Add1,Add2\" data-for=\"Test\" data-multi=\"false\" data-order=\"Desc\" " +
                        "data-page=\"2\" data-rows=\"11\" data-search=\"Test\" data-sort=\"First\" " +
                        "data-title=\"Dialog lookup title\" data-url=\"http://localhost/Lookup\">" +
                        "<input class=\"mvc-lookup-input\" />" +
                        "<div class=\"mvc-lookup-control-loader\"></div>" +
                    "</div>" +
                "</div>";

            Assert.Equal(expected, actual);
        }

        #endregion

        #region AutoCompleteFor<TModel, TProperty>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, MvcLookup model, Object htmlAttributes = null)

        [Fact]
        public void AutoCompleteFor_FromModel()
        {
            String actual = ToString(html.AutoCompleteFor(model => model.ParentId, lookup, new { @class = "classes", attribute = "attr" }));
            String expected =
                "<div class=\"mvc-lookup-browseless mvc-lookup-group\">" +
                    "<div class=\"mvc-lookup-values\" data-for=\"ParentId\">" +
                        "<input class=\"mvc-lookup-value\" id=\"ParentId\" name=\"ParentId\" type=\"hidden\" value=\"Model&#x27;s parent ID\" />" +
                    "</div>" +
                    "<div attribute=\"attr\" class=\"mvc-lookup-control\" data-dialog=\"Dialog\" " +
                        "data-filters=\"Add1,Add2\" data-for=\"ParentId\" data-multi=\"false\" data-order=\"Desc\" " +
                        "data-page=\"2\" data-rows=\"11\" data-search=\"Test\" data-sort=\"First\" " +
                        "data-title=\"Dialog lookup title\" data-url=\"http://localhost/Lookup\">" +
                        "<input class=\"mvc-lookup-input\" />" +
                        "<div class=\"mvc-lookup-control-loader\"></div>" +
                    "</div>" +
                "</div>";

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Lookup<TModel>(this IHtmlHelper<TModel> html, String name, MvcLookup model, Object value = null, Object htmlAttributes = null)

        [Fact]
        public void Lookup_FromModel()
        {
            String actual = ToString(html.Lookup("Test", lookup, "Value", new { @class = "classes", attribute = "attr" }));
            String expected =
                "<div class=\"mvc-lookup-group\">" +
                    "<div class=\"mvc-lookup-values\" data-for=\"Test\">" +
                        "<input class=\"mvc-lookup-value\" id=\"Test\" name=\"Test\" type=\"hidden\" value=\"Value\" />" +
                    "</div>" +
                    "<div attribute=\"attr\" class=\"mvc-lookup-control\" data-dialog=\"Dialog\" " +
                        "data-filters=\"Add1,Add2\" data-for=\"Test\" data-multi=\"false\" data-order=\"Desc\" " +
                        "data-page=\"2\" data-rows=\"11\" data-search=\"Test\" data-sort=\"First\" " +
                        "data-title=\"Dialog lookup title\" data-url=\"http://localhost/Lookup\">" +
                        "<input class=\"mvc-lookup-input\" />" +
                        "<div class=\"mvc-lookup-control-loader\"></div>" +
                    "</div>" +
                    "<div class=\"mvc-lookup-browse\" data-for=\"Test\">" +
                    "</div>" +
                "</div>";

            Assert.Equal(expected, actual);
        }

        #endregion

        #region LookupFor<TModel, TProperty>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, MvcLookup model, Object htmlAttributes = null)

        [Fact]
        public void LookupFor_ForModel()
        {
            String actual = ToString(html.LookupFor(model => model.ParentId, lookup, new { @class = "classes", attribute = "attr" }));
            String expected =
                "<div class=\"mvc-lookup-group\">" +
                    "<div class=\"mvc-lookup-values\" data-for=\"ParentId\">" +
                        "<input class=\"mvc-lookup-value\" id=\"ParentId\" name=\"ParentId\" type=\"hidden\" value=\"Model&#x27;s parent ID\" />" +
                    "</div>" +
                    "<div attribute=\"attr\" class=\"mvc-lookup-control\" data-dialog=\"Dialog\" " +
                        "data-filters=\"Add1,Add2\" data-for=\"ParentId\" data-multi=\"false\" data-order=\"Desc\" " +
                        "data-page=\"2\" data-rows=\"11\" data-search=\"Test\" data-sort=\"First\" " +
                        "data-title=\"Dialog lookup title\" data-url=\"http://localhost/Lookup\">" +
                        "<input class=\"mvc-lookup-input\" />" +
                        "<div class=\"mvc-lookup-control-loader\"></div>" +
                    "</div>" +
                    "<div class=\"mvc-lookup-browse\" data-for=\"ParentId\">" +
                    "</div>" +
                "</div>";

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Test helpers

        private IHtmlHelper<TestModel> MockHtmlHelper()
        {
            IOptions<MvcViewOptions> options = Substitute.For<IOptions<MvcViewOptions>>();
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            options.Value.Returns(new MvcViewOptions());

            IHtmlGenerator generator = new DefaultHtmlGenerator(
                Substitute.For<IAntiforgery>(),
                options,
                provider,
                Substitute.For<IUrlHelperFactory>(),
                HtmlEncoder.Default,
                new ClientValidatorCache(),
                new DefaultValidationHtmlAttributeProvider(options, provider, new ClientValidatorCache()));

            HtmlHelper<TestModel> htmlHelper = new HtmlHelper<TestModel>(
                generator,
                Substitute.For<ICompositeViewEngine>(),
                provider,
                Substitute.For<IViewBufferScope>(),
                HtmlEncoder.Default,
                UrlEncoder.Default,
                new ExpressionTextCache());

            ViewContext context = new ViewContext();
            context.HttpContext = new DefaultHttpContext();
            TestModel model = new TestModel { ParentId = "Model's parent ID" };
            context.ViewData = new ViewDataDictionary<TestModel>(context.ViewData, model);

            htmlHelper.Contextualize(context);

            return htmlHelper;
        }

        private String ToString(IHtmlContent content)
        {
            using (StringWriter writer = new StringWriter())
            {
                content.WriteTo(writer, HtmlEncoder.Default);

                return writer.ToString();
            }
        }

        #endregion
    }
}
