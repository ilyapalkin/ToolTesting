using System;

using Xunit;

namespace ToolTesting.EF.HowToManageCollections
{
    /// <summary>
    /// There is an issue with exposing read-only collections.
    /// See 
    /// http://stackoverflow.com/questions/25663810/domain-model-vs-entity-fw-is-this-a-case-when-splitting-in-a-persistence-model
    /// NHibernate approach http://stackoverflow.com/questions/645918/what-is-the-best-practice-for-readonly-lists-in-nhibernate
    /// </summary>
    public class ExposingReadonlyCollection
    {
        public ExposingReadonlyCollection()
        {
            new NotImplementedException("TODO:");
        }
    }
}
