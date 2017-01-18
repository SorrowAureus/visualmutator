namespace VisualMutator.Infrastructure
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Model.CoverageFinder;

    public class VisualStudioCodeElementsFormatter
    {//what abou tnonymous?
        public MethodIdentifier CreateIdentifier(string methodFullName, IList<string> parameters)
        {
            string converted = ConvertConstructorName(methodFullName);
            var result = new MethodIdentifier(AppendMethodParameters(converted, parameters));

            Trace.WriteLine(methodFullName + " converted to " + result);
            return result;
        }

        private string ConvertConstructorName(string methodFullName)
        {
            var id = new MethodIdentifier(methodFullName);
            if (id.ClassSimpleName == id.MethodNameWithoutParams)
            {
                int dotIndex = methodFullName.LastIndexOf('.');
                return methodFullName.Substring(0, dotIndex + 1) + ".ctor";
            }
            return methodFullName;
        }

        private string AppendMethodParameters(string methodName, IList<string> parameters)
        {
            if (parameters.Count == 0)
            {
                return methodName + "()";
            }
            return methodName + ('(' + parameters.Aggregate((a, b) => a + ", " + b) + ')');
        }
    }
}