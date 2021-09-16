namespace Xx
{
    public abstract class XxShemaInfo
    {
        public virtual string Namespace { get; }
        public virtual bool AllPropertiesBindable { get; }
        public virtual string SchemaOutputFile { get; }
    }
}
