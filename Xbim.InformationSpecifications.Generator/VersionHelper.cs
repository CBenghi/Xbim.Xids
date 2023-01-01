using System;
using Xbim.Common;

namespace Xbim.InformationSpecifications.Generator
{
    public static class VersionHelper
    {
        public static string GetFileVersion(Type type)
        {
            var info = new XbimAssemblyInfo(type);
            return info.FileVersion;
        }
    }
}
