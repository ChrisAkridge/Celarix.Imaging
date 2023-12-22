using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Celarix.Imaging.Packing;

namespace Celarix.Imaging.JobRecovery
{
    public interface IBinaryJob
    {
        void Save(BinaryWriter writer);
        IBinaryJob Load(BinaryReader reader);

        static T Load<T>(BinaryReader reader) =>
            // there has got to be a better way
            typeof(T) == typeof(PackingJob)
                ? (T)new PackingJob().Load(reader)
                : throw new ArgumentOutOfRangeException();
    }
}
