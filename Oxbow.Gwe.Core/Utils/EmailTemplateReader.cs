using System;
using System.Collections.Generic;
using System.Linq;

namespace Oxbow.Gwe.Core.Utils
{
    public class EmailTemplateReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateReader"/> class.
        /// </summary>
        /// <param name="variableMappings">The variable mappings.</param>
        /// <param name="rawTemplateXML">The raw template XML.</param>
        public EmailTemplateReader(IDictionary<string, string> variableMappings, string rawTemplateXML)
        {
            VariableMappings = variableMappings;
            RawTemplateXML = rawTemplateXML;
            PopulateTemplate();
        }

        /// <summary>
        /// Gets or sets the variable mappings.
        /// </summary>
        /// <value>The variable mappings.</value>
        private IDictionary<string, string> VariableMappings { get; set; }

        /// <summary>
        /// Gets or sets the raw template XML.
        /// </summary>
        /// <value>The raw template XML.</value>
        private string RawTemplateXML { get; set; }

        /// <summary>
        /// Gets the target recipients.
        /// </summary>
        /// <value>The target recipients.</value>
        public IEnumerable<string> TargetRecipients
        {
            get { return GetTagContent("to").Split(new[] { ',', ' ' }).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()); }
        }

        /// <summary>
        /// Gets the target CC.
        /// </summary>
        /// <value>The target CC.</value>
        public IEnumerable<string> TargetCC
        {
            get { return GetTagContent("cc").Split(new[] { ',', ' ' }).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()); }
        }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get { return GetTagContent("subject"); }
        }

        /// <summary>
        /// Gets the from address.
        /// </summary>
        /// <value>From.</value>
        public string From
        {
            get { return GetTagContent("from"); }
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>The body.</value>
        public string Body
        {
            get { return GetTagContent("body"); }
        }

        /// <summary>
        /// Opens from template file name and path.
        /// </summary>
        /// <param name="filenameAndPath">The filename and path.</param>
        /// <returns></returns>
        public static EmailTemplateReader CreateTemplateFromResource(string resourceName, IDictionary<string, string> variableMappings)
        {
            var templateString = CommonCode.GetStringFromResource(resourceName);
            return new EmailTemplateReader(variableMappings, templateString);
        }

        /// <summary>
        /// Gets the content of the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns></returns>
        private string GetTagContent(string tagName)
        {
            string startTag = string.Format("<{0}>", tagName);
            string endTag = string.Format("</{0}>", tagName);

            if (RawTemplateXML.IndexOf(startTag) == -1)
                throw new Exception(string.Format("start tag not found:{0}", startTag));
            if (RawTemplateXML.IndexOf(endTag) == -1)
                throw new Exception(string.Format("end tag not found:{0}", endTag));
            int startIndex = RawTemplateXML.IndexOf(startTag) + startTag.Length;
            int endIndex = RawTemplateXML.IndexOf(endTag);

            return RawTemplateXML.Substring(startIndex, endIndex - startIndex).Trim();
        }

        /// <summary>
        /// Populates the template.
        /// </summary>
        private void PopulateTemplate()
        {
            foreach (var mapping in VariableMappings)
            {
                RawTemplateXML = RawTemplateXML.Replace(string.Format("${{{0}}}", mapping.Key), mapping.Value);
            }
        }
    }
}