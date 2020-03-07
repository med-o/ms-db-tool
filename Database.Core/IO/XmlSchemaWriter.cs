using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Settings;

namespace Database.Core.IO
{
    public class XmlSchemaWriter : ISchemaWriter
    {
        private readonly ILogger _logger;
        private readonly IDatabaseSchemaSettingRepository _settingsRepository;

        public XmlSchemaWriter(
            ILogger logger,
            IDatabaseSchemaSettingRepository settingsRepository
        )
        {
            _logger = logger;
            _settingsRepository = settingsRepository;
        }

        public void Write(SchemaDefinition schema)
        {
            var shouldWriteSchema = _settingsRepository
                .Get()
                .FileLocations
                .GenerateSchemaDefinitionFiles;

            if (!shouldWriteSchema)
            {
                _logger.Log(LogLevel.Information, "Save schema ... skipped");
                return;
            }

            _logger.Log(LogLevel.Information, "Save schema ... start");
            _logger.Log(string.Empty);

            var filePath = _settingsRepository
                .Get()
                .FileLocations
                .SchemaDefinitionFilesLocation;

            // NOTE : XML is getting too big, needs to be split up..
            var groups = schema.GroupBy(
                pair => pair.Value.Type,
                pair => pair,
                (type, pairs) =>
                {
                    var partialSchema = new SchemaDefinition();
                    partialSchema.AddRange(pairs.ToDictionary(k => k.Key, v => v.Value));
                    return new KeyValuePair<SchemaObjectType, SchemaDefinition>(type, partialSchema);
                });

            foreach (var partial in groups)
            {
                using (var writer = new FileStream($"{filePath}\\{partial.Key}.xml", FileMode.Create))
                {
                    var serializer = new DataContractSerializer(typeof(SchemaDefinition));
                    serializer.WriteObject(writer, partial.Value);
                }
            }

            _logger.Log(LogLevel.Information, "Save schema ... end");
            _logger.Log(string.Empty);
        }
    }
}
