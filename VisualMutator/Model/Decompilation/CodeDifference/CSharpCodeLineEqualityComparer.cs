using System;

namespace VisualMutator.Model.Decompilation.CodeDifference
{
    #region

    using System.Collections.Generic;

    #endregion

    public class CSharpCodeLineEqualityComparer : IEqualityComparer<string>
    {
        private readonly IEqualityComparer<string> baseComparer = EqualityComparer<string>.Default;

        public bool Equals(string x, string y)
        {
            return baseComparer.Equals(
                NormalizeLine(x),
                NormalizeLine(y)
                );
        }

        public int GetHashCode(string obj)
        {
            return baseComparer.GetHashCode(NormalizeLine(obj));
        }

        private string NormalizeLine(string line)
        {
            //IL_00ca:
            line = line.Trim();
            var index = line.IndexOf("//", StringComparison.Ordinal);
            if (index >= 0)
            {
                return line.Substring(0, index);
            }
            else if (line.StartsWith("#"))
            {
                return string.Empty;
            }
            else
            {
                return line;
            }
        }
    }
}