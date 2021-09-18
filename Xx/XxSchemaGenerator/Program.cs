using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Xx;

namespace xxsgen
{
    class Program
    {
        static int Main(string[] args)
        {
            if(args.Length < 1)
            {
                return -1;
            }

            var path = Path.GetFullPath(args[0]);
            var dir = Path.GetDirectoryName(path);
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);

            LoadReferencedTypes(assembly, dir);

            var assembliesParser = new AssemblyParser(assembly);
            if (!assembliesParser.Parse()) return -1;

            foreach (var info in assembliesParser.Infos)
            { 
                var generator = new SchemaGenerator(info.Namespace, assembliesParser.SimpleTypes, assembliesParser.ComplexTypes);
                var xmlDocument = generator.Generate();

                var outDir = args.Length < 2 ? dir : args[1].Trim('/', '\\');

                using (var stream = File.Open(Path.Combine(outDir, info.SchemaOutputFile), FileMode.Create))
                {
                    xmlDocument.Save(stream);
                    stream.Flush();
                }
            }
            return 0;
        }

        static void LoadReferencedTypes(Assembly assembly, string dir)
        {
            var referenced = assembly.GetReferencedAssemblies();

            foreach (var name in referenced)
            {
                try
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyName(name);
                }
                catch
                {
                    try
                    {
                        var path = Path.Combine(dir, name.Name + ".dll");
                        AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
