// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelDescriptionGenerator.cs" company="PVWI Family">
//   Todos os direitos reservados.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PVWI.Areas.HelpPage.ModelDescriptions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Xml.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    /// Generates model descriptions for given types.
    /// </summary>
    public class ModelDescriptionGenerator
    {
        // Modify this to support more data annotation attributes.
        /// <summary>
        /// The annotation text generator.
        /// </summary>
        private readonly IDictionary<Type, Func<object, string>> AnnotationTextGenerator = new Dictionary<Type, Func<object, string>>
        {
            { typeof(RequiredAttribute), a => "Required" }, 
            { typeof(RangeAttribute), a =>
                {
                    RangeAttribute range = (RangeAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "Range: inclusive between {0} and {1}", range.Minimum, range.Maximum);
                }
            }, 
            { typeof(MaxLengthAttribute), a =>
                {
                    MaxLengthAttribute maxLength = (MaxLengthAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "Max length: {0}", maxLength.Length);
                }
            }, 
            { typeof(MinLengthAttribute), a =>
                {
                    MinLengthAttribute minLength = (MinLengthAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "Min length: {0}", minLength.Length);
                }
            }, 
            { typeof(StringLengthAttribute), a =>
                {
                    StringLengthAttribute strLength = (StringLengthAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "String length: inclusive between {0} and {1}", strLength.MinimumLength, strLength.MaximumLength);
                }
            }, 
            { typeof(DataTypeAttribute), a =>
                {
                    DataTypeAttribute dataType = (DataTypeAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "Data type: {0}", dataType.CustomDataType ?? dataType.DataType.ToString());
                }
            }, 
            { typeof(RegularExpressionAttribute), a =>
                {
                    RegularExpressionAttribute regularExpression = (RegularExpressionAttribute)a;
                    return string.Format(CultureInfo.CurrentCulture, "Matching regular expression pattern: {0}", regularExpression.Pattern);
                }
            }
        };

        // Modify this to add more default documentations.
        /// <summary>
        /// The default type documentation.
        /// </summary>
        private readonly IDictionary<Type, string> DefaultTypeDocumentation = new Dictionary<Type, string>
        {
            { typeof(Int16), "integer" }, 
            { typeof(Int32), "integer" }, 
            { typeof(Int64), "integer" }, 
            { typeof(UInt16), "unsigned integer" }, 
            { typeof(UInt32), "unsigned integer" }, 
            { typeof(UInt64), "unsigned integer" }, 
            { typeof(Byte), "byte" }, 
            { typeof(Char), "character" }, 
            { typeof(SByte), "signed byte" }, 
            { typeof(Uri), "URI" }, 
            { typeof(Single), "decimal number" }, 
            { typeof(Double), "decimal number" }, 
            { typeof(Decimal), "decimal number" }, 
            { typeof(String), "string" }, 
            { typeof(Guid), "globally unique identifier" }, 
            { typeof(TimeSpan), "time interval" }, 
            { typeof(DateTime), "date" }, 
            { typeof(DateTimeOffset), "date" }, 
            { typeof(Boolean), "boolean" }
        };

        /// <summary>
        /// The _documentation provider.
        /// </summary>
        private Lazy<IModelDocumentationProvider> _documentationProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDescriptionGenerator"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public ModelDescriptionGenerator(HttpConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            _documentationProvider = new Lazy<IModelDocumentationProvider>(() => config.Services.GetDocumentationProvider() as IModelDocumentationProvider);
            GeneratedModels = new Dictionary<string, ModelDescription>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the generated models.
        /// </summary>
        public Dictionary<string, ModelDescription> GeneratedModels { get; private set; }

        /// <summary>
        /// Gets the documentation provider.
        /// </summary>
        private IModelDocumentationProvider DocumentationProvider
        {
            get
            {
                return _documentationProvider.Value;
            }
        }

        /// <summary>
        /// The get or create model description.
        /// </summary>
        /// <param name="modelType">
        /// The model type.
        /// </param>
        /// <returns>
        /// The <see cref="ModelDescription"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public ModelDescription GetOrCreateModelDescription(Type modelType)
        {
            if (modelType == null)
            {
                throw new ArgumentNullException("modelType");
            }

            Type underlyingType = Nullable.GetUnderlyingType(modelType);
            if (underlyingType != null)
            {
                modelType = underlyingType;
            }

            ModelDescription modelDescription;
            string modelName = ModelNameHelper.GetModelName(modelType);
            if (GeneratedModels.TryGetValue(modelName, out modelDescription))
            {
                if (modelType != modelDescription.ModelType)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture, 
                            "A model description could not be created. Duplicate model name '{0}' was found for types '{1}' and '{2}'. " +
                            "Use the [ModelName] attribute to change the model name for at least one of the types so that it has a unique name.", 
                            modelName, 
                            modelDescription.ModelType.FullName, 
                            modelType.FullName));
                }

                return modelDescription;
            }

            if (DefaultTypeDocumentation.ContainsKey(modelType))
            {
                return GenerateSimpleTypeModelDescription(modelType);
            }

            if (modelType.IsEnum)
            {
                return GenerateEnumTypeModelDescription(modelType);
            }

            if (modelType.IsGenericType)
            {
                Type[] genericArguments = modelType.GetGenericArguments();

                if (genericArguments.Length == 1)
                {
                    Type enumerableType = typeof(IEnumerable<>).MakeGenericType(genericArguments);
                    if (enumerableType.IsAssignableFrom(modelType))
                    {
                        return GenerateCollectionModelDescription(modelType, genericArguments[0]);
                    }
                }

                if (genericArguments.Length == 2)
                {
                    Type dictionaryType = typeof(IDictionary<,>).MakeGenericType(genericArguments);
                    if (dictionaryType.IsAssignableFrom(modelType))
                    {
                        return GenerateDictionaryModelDescription(modelType, genericArguments[0], genericArguments[1]);
                    }

                    Type keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(genericArguments);
                    if (keyValuePairType.IsAssignableFrom(modelType))
                    {
                        return GenerateKeyValuePairModelDescription(modelType, genericArguments[0], genericArguments[1]);
                    }
                }
            }

            if (modelType.IsArray)
            {
                Type elementType = modelType.GetElementType();
                return GenerateCollectionModelDescription(modelType, elementType);
            }

            if (modelType == typeof(NameValueCollection))
            {
                return GenerateDictionaryModelDescription(modelType, typeof(string), typeof(string));
            }

            if (typeof(IDictionary).IsAssignableFrom(modelType))
            {
                return GenerateDictionaryModelDescription(modelType, typeof(object), typeof(object));
            }

            if (typeof(IEnumerable).IsAssignableFrom(modelType))
            {
                return GenerateCollectionModelDescription(modelType, typeof(object));
            }

            return GenerateComplexTypeModelDescription(modelType);
        }

        // Change this to provide different name for the member.
        /// <summary>
        /// The get member name.
        /// </summary>
        /// <param name="member">
        /// The member.
        /// </param>
        /// <param name="hasDataContractAttribute">
        /// The has data contract attribute.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetMemberName(MemberInfo member, bool hasDataContractAttribute)
        {
            JsonPropertyAttribute jsonProperty = member.GetCustomAttribute<JsonPropertyAttribute>();
            if (jsonProperty != null && !string.IsNullOrEmpty(jsonProperty.PropertyName))
            {
                return jsonProperty.PropertyName;
            }

            if (hasDataContractAttribute)
            {
                DataMemberAttribute dataMember = member.GetCustomAttribute<DataMemberAttribute>();
                if (dataMember != null && !string.IsNullOrEmpty(dataMember.Name))
                {
                    return dataMember.Name;
                }
            }

            return member.Name;
        }

        /// <summary>
        /// The should display member.
        /// </summary>
        /// <param name="member">
        /// The member.
        /// </param>
        /// <param name="hasDataContractAttribute">
        /// The has data contract attribute.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool ShouldDisplayMember(MemberInfo member, bool hasDataContractAttribute)
        {
            JsonIgnoreAttribute jsonIgnore = member.GetCustomAttribute<JsonIgnoreAttribute>();
            XmlIgnoreAttribute xmlIgnore = member.GetCustomAttribute<XmlIgnoreAttribute>();
            IgnoreDataMemberAttribute ignoreDataMember = member.GetCustomAttribute<IgnoreDataMemberAttribute>();
            NonSerializedAttribute nonSerialized = member.GetCustomAttribute<NonSerializedAttribute>();
            ApiExplorerSettingsAttribute apiExplorerSetting = member.GetCustomAttribute<ApiExplorerSettingsAttribute>();

            bool hasMemberAttribute = member.DeclaringType.IsEnum ?
                member.GetCustomAttribute<EnumMemberAttribute>() != null :
                member.GetCustomAttribute<DataMemberAttribute>() != null;

            // Display member only if all the followings are true:
            // no JsonIgnoreAttribute
            // no XmlIgnoreAttribute
            // no IgnoreDataMemberAttribute
            // no NonSerializedAttribute
            // no ApiExplorerSettingsAttribute with IgnoreApi set to true
            // no DataContractAttribute without DataMemberAttribute or EnumMemberAttribute
            return jsonIgnore == null &&
                xmlIgnore == null &&
                ignoreDataMember == null &&
                nonSerialized == null &&
                (apiExplorerSetting == null || !apiExplorerSetting.IgnoreApi) &&
                (!hasDataContractAttribute || hasMemberAttribute);
        }

        /// <summary>
        /// The create default documentation.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string CreateDefaultDocumentation(Type type)
        {
            string documentation;
            if (DefaultTypeDocumentation.TryGetValue(type, out documentation))
            {
                return documentation;
            }

            if (DocumentationProvider != null)
            {
                documentation = DocumentationProvider.GetDocumentation(type);
            }

            return documentation;
        }

        /// <summary>
        /// The generate annotations.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="propertyModel">
        /// The property model.
        /// </param>
        private void GenerateAnnotations(MemberInfo property, ParameterDescription propertyModel)
        {
            List<ParameterAnnotation> annotations = new List<ParameterAnnotation>();

            IEnumerable<Attribute> attributes = property.GetCustomAttributes();
            foreach (Attribute attribute in attributes)
            {
                Func<object, string> textGenerator;
                if (AnnotationTextGenerator.TryGetValue(attribute.GetType(), out textGenerator))
                {
                    annotations.Add(
                        new ParameterAnnotation
                        {
                            AnnotationAttribute = attribute, 
                            Documentation = textGenerator(attribute)
                        });
                }
            }

            // Rearrange the annotations
            annotations.Sort((x, y) =>
            {
                // Special-case RequiredAttribute so that it shows up on top
                if (x.AnnotationAttribute is RequiredAttribute)
                {
                    return -1;
                }

                if (y.AnnotationAttribute is RequiredAttribute)
                {
                    return 1;
                }

                // Sort the rest based on alphabetic order of the documentation
                return string.Compare(x.Documentation, y.Documentation, StringComparison.OrdinalIgnoreCase);
            });

            foreach (ParameterAnnotation annotation in annotations)
            {
                propertyModel.Annotations.Add(annotation);
            }
        }

        /// <summary>
        /// The generate collection model description.
        /// </summary>
        /// <param name="modelType">
        /// The model type.
        /// </param>
        /// <param name="elementType">
        /// The element type.
        /// </param>
        /// <returns>
        /// The <see cref="CollectionModelDescription"/>.
        /// </returns>
        private CollectionModelDescription GenerateCollectionModelDescription(Type modelType, Type elementType)
        {
            ModelDescription collectionModelDescription = GetOrCreateModelDescription(elementType);
            if (collectionModelDescription != null)
            {
                return new CollectionModelDescription
                {
                    Name = ModelNameHelper.GetModelName(modelType), 
                    ModelType = modelType, 
                    ElementDescription = collectionModelDescription
                };
            }

            return null;
        }

        /// <summary>
        /// The generate complex type model description.
        /// </summary>
        /// <param name="modelType">
        /// The model type.
        /// </param>
        /// <returns>
        /// The <see cref="ModelDescription"/>.
        /// </returns>
        private ModelDescription GenerateComplexTypeModelDescription(Type modelType)
        {
            ComplexTypeModelDescription complexModelDescription = new ComplexTypeModelDescription
            {
                Name = ModelNameHelper.GetModelName(modelType), 
                ModelType = modelType, 
                Documentation = CreateDefaultDocumentation(modelType)
            };

            GeneratedModels.Add(complexModelDescription.Name, complexModelDescription);
            bool hasDataContractAttribute = modelType.GetCustomAttribute<DataContractAttribute>() != null;
            PropertyInfo[] properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (ShouldDisplayMember(property, hasDataContractAttribute))
                {
                    ParameterDescription propertyModel = new ParameterDescription
                    {
                        Name = GetMemberName(property, hasDataContractAttribute)
                    };

                    if (DocumentationProvider != null)
                    {
                        propertyModel.Documentation = DocumentationProvider.GetDocumentation(property);
                    }

                    GenerateAnnotations(property, propertyModel);
                    complexModelDescription.Properties.Add(propertyModel);
                    propertyModel.TypeDescription = GetOrCreateModelDescription(property.PropertyType);
                }
            }

            FieldInfo[] fields = modelType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                if (ShouldDisplayMember(field, hasDataContractAttribute))
                {
                    ParameterDescription propertyModel = new ParameterDescription
                    {
                        Name = GetMemberName(field, hasDataContractAttribute)
                    };

                    if (DocumentationProvider != null)
                    {
                        propertyModel.Documentation = DocumentationProvider.GetDocumentation(field);
                    }

                    complexModelDescription.Properties.Add(propertyModel);
                    propertyModel.TypeDescription = GetOrCreateModelDescription(field.FieldType);
                }
            }

            return complexModelDescription;
        }

        /// <summary>
        /// The generate dictionary model description.
        /// </summary>
        /// <param name="modelType">
        /// The model type.
        /// </param>
        /// <param name="keyType">
        /// The key type.
        /// </param>
        /// <param name="valueType">
        /// The value type.
        /// </param>
        /// <returns>
        /// The <see cref="DictionaryModelDescription"/>.
        /// </returns>
        private DictionaryModelDescription GenerateDictionaryModelDescription(Type modelType, Type keyType, Type valueType)
        {
            ModelDescription keyModelDescription = GetOrCreateModelDescription(keyType);
            ModelDescription valueModelDescription = GetOrCreateModelDescription(valueType);

            return new DictionaryModelDescription
            {
                Name = ModelNameHelper.GetModelName(modelType), 
                ModelType = modelType, 
                KeyModelDescription = keyModelDescription, 
                ValueModelDescription = valueModelDescription
            };
        }

        /// <summary>
        /// The generate enum type model description.
        /// </summary>
        /// <param name="modelType">
        /// The model type.
        /// </param>
        /// <returns>
        /// The <see cref="EnumTypeModelDescription"/>.
        /// </returns>
        private EnumTypeModelDescription GenerateEnumTypeModelDescription(Type modelType)
        {
            EnumTypeModelDescription enumDescription = new EnumTypeModelDescription
            {
                Name = ModelNameHelper.GetModelName(modelType), 
                ModelType = modelType, 
                Documentation = CreateDefaultDocumentation(modelType)
            };
            bool hasDataContractAttribute = modelType.GetCustomAttribute<DataContractAttribute>() != null;
            foreach (FieldInfo field in modelType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (ShouldDisplayMember(field, hasDataContractAttribute))
                {
                    EnumValueDescription enumValue = new EnumValueDescription
                    {
                        Name = field.Name, 
                        Value = field.GetRawConstantValue().ToString()
                    };
                    if (DocumentationProvider != null)
                    {
                        enumValue.Documentation = DocumentationProvider.GetDocumentation(field);
                    }

                    enumDescription.Values.Add(enumValue);
                }
            }

            GeneratedModels.Add(enumDescription.Name, enumDescription);

            return enumDescription;
        }

        /// <summary>
        /// The generate key value pair model description.
        /// </summary>
        /// <param name="modelType">
        /// The model type.
        /// </param>
        /// <param name="keyType">
        /// The key type.
        /// </param>
        /// <param name="valueType">
        /// The value type.
        /// </param>
        /// <returns>
        /// The <see cref="KeyValuePairModelDescription"/>.
        /// </returns>
        private KeyValuePairModelDescription GenerateKeyValuePairModelDescription(Type modelType, Type keyType, Type valueType)
        {
            ModelDescription keyModelDescription = GetOrCreateModelDescription(keyType);
            ModelDescription valueModelDescription = GetOrCreateModelDescription(valueType);

            return new KeyValuePairModelDescription
            {
                Name = ModelNameHelper.GetModelName(modelType), 
                ModelType = modelType, 
                KeyModelDescription = keyModelDescription, 
                ValueModelDescription = valueModelDescription
            };
        }

        /// <summary>
        /// The generate simple type model description.
        /// </summary>
        /// <param name="modelType">
        /// The model type.
        /// </param>
        /// <returns>
        /// The <see cref="ModelDescription"/>.
        /// </returns>
        private ModelDescription GenerateSimpleTypeModelDescription(Type modelType)
        {
            SimpleTypeModelDescription simpleModelDescription = new SimpleTypeModelDescription
            {
                Name = ModelNameHelper.GetModelName(modelType), 
                ModelType = modelType, 
                Documentation = CreateDefaultDocumentation(modelType)
            };
            GeneratedModels.Add(simpleModelDescription.Name, simpleModelDescription);

            return simpleModelDescription;
        }
    }
}