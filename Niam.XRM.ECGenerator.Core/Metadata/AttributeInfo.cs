using System.Runtime.Serialization;

namespace Niam.XRM.ECGenerator.Core.Metadata
{
    [DataContract]
    public class AttributeInfo
    {
        [DataMember(Name = "a", EmitDefaultValue = false)]
        public int? StateCodeActiveValue { get; set; }
    }
}
