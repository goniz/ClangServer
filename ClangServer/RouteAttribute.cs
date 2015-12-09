using System;

namespace ClangServer
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class RouteAttribute : Attribute
    {
        public string Path { get; set; }

        public RouteAttribute(string path)
        {
            this.Path = path;
        }
    }
}

