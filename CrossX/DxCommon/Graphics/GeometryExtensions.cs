// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.


using CrossX.Graphics;
using SharpDX.Direct3D;
using System;

using DxMatrix = SharpDX.Mathematics.Interop.RawMatrix;

namespace CrossX.DxCommon.Helpers
{
    public static class GeometryExtensions
    {
        public static DxMatrix ToRawMatrix(this Matrix matrix)
        {
            var mat = new DxMatrix();
            matrix = Matrix.Transpose(matrix);

            mat.M11 = matrix.M11;
            mat.M12 = matrix.M12;
            mat.M13 = matrix.M13;
            mat.M14 = matrix.M14;
            mat.M21 = matrix.M21;
            mat.M22 = matrix.M22;
            mat.M23 = matrix.M23;
            mat.M24 = matrix.M24;
            mat.M31 = matrix.M31;
            mat.M32 = matrix.M32;
            mat.M33 = matrix.M33;
            mat.M34 = matrix.M34;
            mat.M41 = matrix.M41;
            mat.M42 = matrix.M42;
            mat.M43 = matrix.M43;
            mat.M44 = matrix.M44;

            return mat;
        }

        public static PrimitiveTopology PrimitiveTopologyFromPrimitiveType(PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.TriangleList:
                    return PrimitiveTopology.TriangleList;

                case PrimitiveType.TriangleStrip:
                    return PrimitiveTopology.TriangleStrip;

                case PrimitiveType.LineList:
                    return PrimitiveTopology.LineList;

                case PrimitiveType.LineStrip:
                    return PrimitiveTopology.LineStrip;
            }
            throw new IndexOutOfRangeException($"There's no matching value for {primitiveType.ToString()}");
        }

        public static int StrideFromVertexContent(VertexContent vertexContent)
        {
            int stride = 0;

            if (vertexContent.HasFlag(VertexContent.Position)) stride += 4 * sizeof(float);
            if (vertexContent.HasFlag(VertexContent.Normal)) stride += 4 * sizeof(float);
            if (vertexContent.HasFlag(VertexContent.Color)) stride += 4 * sizeof(byte);
            if (vertexContent.HasFlag(VertexContent.TextureCoordinates)) stride += 2 * sizeof(float);

            return stride;
        }

        public static int CalculateVertexCount(int primitiveCount, PrimitiveType primitiveType)
        {
            switch(primitiveType)
            {
                case PrimitiveType.TriangleList:
                    return primitiveCount * 3;

                case PrimitiveType.TriangleStrip:
                    return primitiveCount + 2;

                case PrimitiveType.LineList:
                    return primitiveCount * 2;

                case PrimitiveType.LineStrip:
                    return primitiveCount + 1;
            }

            return 0;
        }
    }
}
