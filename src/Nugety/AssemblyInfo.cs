using System.Reflection;

namespace Nugety
{
    public class AssemblyInfo
    {
        public AssemblyInfo(Assembly assembly)
        {
            this.Assembly = assembly;
            this.Location = Assembly.Location;
        }

        public Assembly Assembly { get; set; }
        public string Location { get; set; }
    }
}