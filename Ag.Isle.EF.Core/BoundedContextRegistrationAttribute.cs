using System.Diagnostics.CodeAnalysis;

namespace Ag.Isle.EF.Core
{
    /// <summary>
    /// This registration attribute allows EF entity configurations to register themselves
    /// with one or more bounded contexts. Each bounded context has a ContextName
    /// property which it uses to search for classes that register themselves with that
    /// context. Use a value of "*" to register with all bounded contexts. (Only recommended
    /// for truly common "summary" entities) Combine "*" With "!{ContextName}" to
    /// register with all contexts *except* the Not'd names.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class)]
    public class BoundedContextRegistrationAttribute : Attribute
    {
        private readonly string[] _contextNames;

        public BoundedContextRegistrationAttribute(params string[] contextNames)
        {
            _contextNames = contextNames.Select(n => n.ToLower()).ToArray();
        }

        public BoundedContextRegistrationAttribute(params Type[] contextTypes)
        {
            _contextNames = contextTypes.Select(x => x.Name.ToLower()).ToArray();
        }

        public bool IsRegisteredToContext(string contextName)
        {
            if (_contextNames == null || false == _contextNames.Any() || string.IsNullOrEmpty(contextName))
                return false;

            contextName = contextName.ToLower();
            bool inContextNames = _contextNames.Contains(contextName);
            bool inAllConditions = _contextNames.Contains("*") && (false == _contextNames.Contains("!" + contextName));

            return inContextNames | inAllConditions;
        }
    }
}
