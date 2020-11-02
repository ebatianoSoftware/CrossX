namespace CrossX
{
    public struct Aabb3
    {
        public Vector3 Min { get; }
        public Vector3 Max { get; }

        public float Width => Max.X - Min.X;
        public float Height => Max.Y - Min.Y;
        public float Depth => Max.Z - Min.Z;

        public Vector3 Center => (Min + Max) / 2;

        public Aabb3(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return $"min: ({Min.X}, {Min.Y}, {Min.Z}), max: ({Max.X}, {Max.Y}, {Max.Z}), width: {Width}, height: {Height}, depth: {Depth}";
        }
    }
}
