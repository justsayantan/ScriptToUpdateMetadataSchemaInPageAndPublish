using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tridion.ContentManager.CoreService.Client;

namespace AddMetadataInPageAndUpdateValue
{
    class Script
    {
        public static CoreServiceClient client;
        public static string TemplateTcmUri = "tcm:22-6851-128";
        public static string TemplateTitle = "Page - Redirect";
        public static string SchemaTitle = "Page Details";
        public static string FieldValue = "AlternativeURL";
        public static string MetadataSchemaTcmUri = "tcm:11-33215-8";
        public static bool IsPublishingFromChild = false;
        public static string Purpose = "Staging";
        static void Main(string[] args)
        {
            client = Utility.CoreServiceSource;
            AddMetadataIntoPageAndUpdateValue();

        }
        private static void AddMetadataIntoPageAndUpdateValue()
        {
            string redirectUrl;
            XElement pages = GetAllPagesByPageTemplate(TemplateTcmUri);

            foreach (var item in pages.Elements())
            {
                string tcmID = item.Attribute("ID").Value;
                PageData pageData = client.Read(tcmID, null) as PageData;

                if (pageData.PageTemplate.Title == TemplateTitle)
                {
                    foreach (ComponentPresentationData cp in pageData.ComponentPresentations)
                    {
                        ComponentData component = (ComponentData)client.Read(cp.Component.IdRef, new ReadOptions());
                        if (component.Schema.Title == SchemaTitle)
                        {
                            var schemaFields = client.ReadSchemaFields(component.Schema.IdRef, false, null);
                            XNamespace ns = schemaFields.NamespaceUri;

                            //check if that AlternativeURL value exist
                            if ((XDocument.Parse(component.Content)).Root.Element(ns + FieldValue) != null)
                            {
                                redirectUrl =
                                    (XDocument.Parse(component.Content)).Root.Element(ns + FieldValue).Value;
                                if (redirectUrl.Contains("default.aspx"))
                                {
                                    redirectUrl = redirectUrl.Replace("default.aspx", "index.html");
                                }

                                //Pass the page data, target value and the metadata schema tcmuri  
                                UpdatePageData(pageData, redirectUrl, MetadataSchemaTcmUri);

                            }
                        }
                    }
                }

            }

        }
        private static void UpdatePageData(PageData pageData, string redirectUrl, string metadataSchemaUri)
        {
            pageData.MetadataSchema.IdRef = metadataSchemaUri;
            SchemaData schema = (SchemaData)client.Read(metadataSchemaUri, new ReadOptions());
            XNamespace mns = schema.NamespaceUri;
            XNamespace xlink = "http://www.w3.org/1999/xlink";
            var metaData = new XElement(mns + "Metadata");
            var redirect = new XElement(mns + "redirect");
            var externalLink = new XElement(mns + "externalLink");
            externalLink.SetAttributeValue(xlink + "type", "simple");
            externalLink.SetAttributeValue(xlink + "href", redirectUrl);
            redirect.Add(externalLink);
            metaData.Add(redirect);
            pageData.Metadata = metaData.ToString();
            try
            {
                client.Update(pageData, new ReadOptions());
                PublishPage(pageData);
                Console.WriteLine("----- Page Updated for -----" + pageData.Id + "-----" + pageData.Title);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: In Page Id " + pageData.Id);
            }
        }
        private static void PublishPage(PageData pageData)
        {
            Console.WriteLine("===============================================================================");
            Console.WriteLine($"Processing Component : {pageData.Title} (TcmId: {pageData.Id})");

            var pubData = new PublishInstructionData
            {
                ResolveInstruction = new ResolveInstructionData()
                {
                    IncludeChildPublications = IsPublishingFromChild,
                    Purpose = ResolvePurpose.RePublish
                },
                RenderInstruction = new RenderInstructionData()
            };
            try
            {
                client.Publish(new[] { pageData.Id }, pubData, new[] { Purpose }, PublishPriority.Low, new ReadOptions());
                Console.WriteLine($"Component {pageData.Title} has been sent for publishing to {Purpose}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }

            Console.WriteLine("===================================== End ======================================");
        }
        private static XElement GetAllPagesByPageTemplate(string templateTcmUri)
        {
            UsingItemsFilterData filter = new UsingItemsFilterData();
            filter.ItemTypes = new[] { ItemType.Page };
            filter.IncludedVersions = VersionCondition.OnlyLatestVersions;
            XElement pages = client.GetListXml(templateTcmUri, filter);
            return pages;
        }
    }
}
