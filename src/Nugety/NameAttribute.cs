using System;

namespace Nugety
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class NameAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public NameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
