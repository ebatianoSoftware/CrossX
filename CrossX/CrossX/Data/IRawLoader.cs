// MIT License - Copyright © Sebastian Sejud
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO;

namespace CrossX.Data
{
    /// <summary>
    /// Loader for specified raw format.
    /// </summary>
    public interface IRawLoader<T>
    {
        /// <summary>
        /// Loads raw media object from stream.
        /// </summary>
        /// <returns>Raw media object loaded from the <paramref name="stream"/>.</returns>
        /// <param name="stream">Raw media file stream.</param>
        T FromStream(Stream stream);
    }
}
