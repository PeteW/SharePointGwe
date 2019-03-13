using System;
using Oxbow.GenericDashboard.Core.Contracts;
using Microsoft.SharePoint;

namespace Oxbow.GenericDashboard.Core.Configuration
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
        public ISpFieldRenderer OfSpFieldRenderer(SPFieldType fieldType)
        {
            if (fieldType == SPFieldType.Choice)
                return new ChoiceSpFieldRenderer();
            if (fieldType == SPFieldType.MultiChoice)
                return new ChoiceSpFieldRenderer();
            if (fieldType == SPFieldType.GridChoice)
                return new ChoiceSpFieldRenderer();
            if (fieldType == SPFieldType.DateTime)
                return new DateTimeSpFieldRenderer();
            if (fieldType == SPFieldType.User)
                return new UserSpFieldRenderer();
            return new DummySpFieldRenderer();
//            throw new Exception(string.Format("Unable to resolve type for {0}", typeof(T)));
        }
    }
}