using CrossX.Abstractions.IoC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Xx.Definition;
using Xx.Toolkit;

namespace CrossX.Framework.XxTools
{
    public interface IXxFileParser
    {
        XxElement Parse(Assembly assembly, string path, bool cache = false);
    }

    internal class XxFileParserImpl : IXxFileParser
    {
        private readonly IObjectFactory objectFactory;

        private ConcurrentDictionary<Tuple<Assembly, string>, XxElement> cache = new ConcurrentDictionary<Tuple<Assembly, string>, XxElement>();

        public XxFileParserImpl(IObjectFactory objectFactory)
        {
            this.objectFactory = objectFactory;
        }
        public XxElement Parse(Assembly assembly, string path, bool cache = false)
        {
            var key = Tuple.Create(assembly, path);

            if (this.cache.TryGetValue(key, out var element)) return element;

            try
            {
                using (var stream = assembly.GetManifestResourceStream(path))
                {
                    var parser = objectFactory.Create<XxFileParser>();
                    element = parser.Parse(stream);
                }

                if(cache)
                {
                    this.cache.TryAdd(key, element);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return element;
        }
    }
}
