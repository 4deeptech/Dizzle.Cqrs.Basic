using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dizzle.Cqrs.Portable.Storage
{
    public sealed class EventStorageStrategy : IDocumentSerializer
    {
        public void Serialize<TEntity>(TEntity entity, System.IO.Stream stream)
        {
            // ProtoBuf must have non-zero files
            stream.WriteByte(42);
            StreamWriter writer = new StreamWriter(stream);
            JsonTextWriter jsonWriter = new JsonTextWriter(writer);
            JsonSerializer ser = new JsonSerializer();
            ser.TypeNameHandling = TypeNameHandling.Auto;
            ser.Serialize(jsonWriter, entity);
            jsonWriter.Flush();
        }

        public TEntity Deserialize<TEntity>(System.IO.Stream stream)
        {
            var signature = stream.ReadByte();

            if (signature != 42)
                throw new InvalidOperationException("Unknown view format");

            StreamReader reader = new StreamReader(stream);
            JsonTextReader jsonReader = new JsonTextReader(reader);
            JsonSerializer ser = new JsonSerializer();
            ser.TypeNameHandling = TypeNameHandling.Auto;
            return ser.Deserialize<TEntity>(jsonReader);
        }
    }
}
