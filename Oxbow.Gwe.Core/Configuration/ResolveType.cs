using System;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.TemplateEngine;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.Core.Configuration
{
    public class ResolveType
    {
        private static ResolveType instance;

        private ResolveType()
        {
        }

        public static ResolveType Instance
        {
            get
            {
                if (instance == null)
                    instance = new ResolveType();
                return instance;
            }
        }

        /// <summary>
        /// Get a concrete implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Of<T>() where T : class
        {
            if (typeof(T) == typeof(ILogger))
                return new EventLogLogger() as T;
            if (typeof(T) == typeof(ITemplateEngine))
                return new GweLangTemplateEngine() as T;
            if (typeof(T) == typeof(IUserIdentificationService))
                return new UserIdentificationService() as T;
            throw new Exception(string.Format("Unable to resolve type for {0}", typeof(T)));
        }

        public ISpListItemContainer OfSpListItemContainer(SPListItem spListItem, WorkflowConfiguration workflowConfiguration)
        {
            SPContentType formContentType = spListItem.Web.GetContentTypeByName("Form");
            if (!spListItem.ContentType.Id.IsChildOf(formContentType.Id))
                return new SpListItemContainer(spListItem, workflowConfiguration);
            else
                return new FormContainer(spListItem,workflowConfiguration);
        }
    }
}