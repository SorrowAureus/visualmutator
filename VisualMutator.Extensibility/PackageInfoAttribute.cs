namespace VisualMutator.Extensibility
{
    using System;
    using System.ComponentModel.Composition;

    public class PackageInfoAttribute
    {
        public PackageInfoAttribute()
        {
        }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PackageExportAttribute : ExportAttribute
    {
        public PackageExportAttribute()
            : base(typeof(IOperatorsPackage))
        {
        }
    }
}