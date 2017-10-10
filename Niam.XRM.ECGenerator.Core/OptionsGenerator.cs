using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;

namespace Niam.XRM.ECGenerator.Core
{
    public class OptionsGenerator
    {
        private readonly EnumAttributeMetadata _metadata;
        private readonly string _template;
        private readonly int _space;

        public OptionsGenerator(EnumAttributeMetadata metadata, string template, int space = 16)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _template = template;
            _space = space;
        }

        public string Generate()
        {
            var builder = new StringBuilder(_template);
            builder.Replace("{{enum-name}}", _metadata.SchemaName);
            builder.Replace("{{enum-values}}", GenerateEnumValues());
            return builder.ToString();
        }

        private string GenerateEnumValues()
        {
            var builder = new StringBuilder();
            var valueNames = new HashSet<string>();
            var options = _metadata.OptionSet.Options;
            var count = options.Count;
            for (var i = 0; i < count; i++)
            {
                var lastOption = (i + 1) == options.Count;
                var option = options[i];
                var valueName = GetValueName(valueNames, option);
                if (String.IsNullOrWhiteSpace(valueName)) continue;
                
                var value = option.Value;
                var comma = lastOption ? "" : ",";
                var enumValueString = new String(' ', _space) + $"{valueName} = {value}{comma}";
                var last = (i + 1) == count;
                if (last)
                    builder.Append(enumValueString);
                else
                    builder.AppendLine(enumValueString);
            }
            return builder.ToString();
        }

        private string GetValueName(HashSet<string> names, OptionMetadata option)
        {
            var valueName = Helper.AdjustName(option.Label.UserLocalizedLabel.Label);
            if (String.IsNullOrWhiteSpace(valueName)) return null;

            if (names.Contains(valueName))
            {
                var i = 0;
                var newName = valueName + (++i);
                while (names.Contains(newName))
                    newName = valueName + (++i);

                valueName = newName;
            }

            names.Add(valueName);
            return valueName;
        }

        public static EnumAttributeMetadata[] Filter(AttributeMetadata[] attributes)
        {
            return attributes
                .Where(a => (a as EnumAttributeMetadata)?.OptionSet != null)
                .Where(a => a.AttributeOf == null)
                .Where(a => a.AttributeType != AttributeTypeCode.Virtual)
                .Cast<EnumAttributeMetadata>()
                .ToArray();
        }
    }
}
