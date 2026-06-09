using System;


namespace EthioNegarit.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FilePathAttribute : Attribute
    {
        public string FilePath { get; set; }

        public FilePathAttribute(string filePath)
        {
            FilePath = filePath;
        }
    }
}
