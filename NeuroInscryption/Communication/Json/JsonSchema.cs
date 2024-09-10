using System.Collections.Generic;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Json;

public sealed class JsonSchema
{
    public static JsonSchema NewResponseSchema() => new()
    {
        Type = JsonSchemaType.Object,
        Required = ["action", "data"],
        Properties = new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Type(JsonSchemaType.String),
            ["data"] = QJS.Type(JsonSchemaType.Object),
        }
    };

    #region Unimplemented keywords

    // $schema
    // $id
    // $comment
    // $dynamicAnchor
    // $dynamicRef
    // $vocabulary

    // then
    // if
    // else
    // patternProperties
    // propertyNames
    // dependentSchemas
    // contains

    // minProperties
    // maxContains
    // minContains

    // title
    // description
    // default
    // deprecated
    // examples
    // readOnly
    // writeOnly

    // contentMediaType
    // contentSchema
    // contentEncoding

    // unevaluatedItems
    // unevaluatedProperties

    #endregion

    #region Keywords

    [JsonProperty("$ref")]
    public string Ref { get; set; }
    [JsonProperty("$defs")]
    public Dictionary<string, JsonSchema> Defs { get; set; }
    [JsonProperty("$anchor")]
    public string Anchor { get; set; }

    [JsonProperty("anyOf")]
    public List<JsonSchema> AnyOf { get; set; }
    [JsonProperty("oneOf")]
    public List<JsonSchema> OneOf { get; set; }
    [JsonProperty("allOf")]
    public List<JsonSchema> AllOf { get; set; }
    [JsonProperty("not")]
    public JsonSchema Not { get; set; }
    [JsonProperty("properties")]
    public Dictionary<string, JsonSchema> Properties { get; set; }
    [JsonProperty("additionalProperties")]
    public bool? AdditionalProperties;
    [JsonProperty("prefixItems")]
    public List<JsonSchema> PrefixItems { get; set; }
    [JsonProperty("items")]
    public JsonSchema Items { get; set; }

    [JsonProperty("type")]
    private string _type;
    [JsonIgnore]
    public JsonSchemaType Type
    {
        get
        {
            return _type switch
            {
                "string" => JsonSchemaType.String,
                "number" => JsonSchemaType.Float,
                "integer" => JsonSchemaType.Integer,
                "boolean" => JsonSchemaType.Boolean,
                "object" => JsonSchemaType.Object,
                "array" => JsonSchemaType.Array,
                "null" => JsonSchemaType.Null,
                _ => JsonSchemaType.None
            };
        }
        set
        {
            _type = value switch
            {
                JsonSchemaType.String => "string",
                JsonSchemaType.Float => "number",
                JsonSchemaType.Integer => "integer",
                JsonSchemaType.Boolean => "boolean",
                JsonSchemaType.Object => "object",
                JsonSchemaType.Array => "array",
                JsonSchemaType.Null => "null",
                _ => null
            };
        }
    }
    [JsonProperty("enum")]
    public List<object> Enum { get; set; }
    [JsonProperty("const")]
    public object Const { get; set; }
    [JsonProperty("minLength")]
    public int? MinLength { get; set; }
    [JsonProperty("pattern")]
    public string Pattern { get; set; }
    [JsonProperty("maxLength")]
    public int? MaxLength { get; set; }
    [JsonProperty("maximum")]
    public int? Maximum { get; set; }
    [JsonProperty("exclusiveMinimum")]
    public int? ExclusiveMinimum { get; set; }
    [JsonProperty("exclusiveMaximum")]
    public int? ExclusiveMaximum { get; set; }
    [JsonProperty("minimum")]
    public int? Minimum { get; set; }
    [JsonProperty("multipleOf")]
    public int? MultipleOf { get; set; }
    [JsonProperty("dependentRequired")]
    public Dictionary<string, List<string>> DependentRequired { get; set; }
    [JsonProperty("required")]
    public List<string> Required { get; set; }
    [JsonProperty("minItems")]
    public int? MinItems { get; set; }
    [JsonProperty("maxItems")]
    public int? MaxItems { get; set; }
    [JsonProperty("uniqueItems")]
    public bool? UniqueItems { get; set; }

    [JsonProperty("format")]
    public string Format { get; set; }

    #endregion

    public void Apply(ISchemaModifier modifier)
    {
        modifier.Apply(this);
    }

    public void Apply<T>() where T : ISchemaModifier, new()
    {
        Apply(new T());
    }
}
