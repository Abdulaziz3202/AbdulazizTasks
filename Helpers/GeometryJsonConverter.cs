using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace KPMGTask.Helpers
{
    public class GeometryJsonConverter : JsonConverter<Geometry>
    {
        private readonly NetTopologySuite.IO.GeoJsonWriter _writer = new GeoJsonWriter();
        private readonly GeoJsonReader _reader = new GeoJsonReader();

        public override Geometry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonDocument.ParseValue(ref reader).RootElement.GetRawText();
            return _reader.Read<Geometry>(json);
        }

        public override void Write(Utf8JsonWriter writer, Geometry value, JsonSerializerOptions options)
        {
            var json = _writer.Write(value);
            writer.WriteRawValue(json);
        }
    }
}
