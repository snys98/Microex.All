using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microex.All.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microex.All.Json
{
    /// <summary>
    /// Converts an <see cref="T:System.Enum" /> to and from its name string value.
    /// </summary>
    //public class StringXEnumConverter : StringEnumConverter
    //{
    //    /// <summary>Writes the JSON representation of the object.</summary>
    //    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
    //    /// <param name="value">The value.</param>
    //    /// <param name="serializer">The calling serializer.</param>
    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        var type = value.GetType();
    //        if (type.IsEnum)
    //        {
    //            writer.WriteValue(type.GetMember(value.ToString())[0].GetCustomAttribute<EnumMemberAttribute>().Value);
    //        }

    //        if (CanWrite)
    //        {
    //            type.GetCustomAttribute<StringEnumAttribute>() != null
    //        }
    //        base.WriteJson(writer,value,serializer);
    //    }

    //    /// <summary>Reads the JSON representation of the object.</summary>
    //    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    //    /// <param name="objectType">Type of the object.</param>
    //    /// <param name="existingValue">The existing value of object being read.</param>
    //    /// <param name="serializer">The calling serializer.</param>
    //    /// <returns>The object value.</returns>
    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        return base.ReadJson(reader,objectType,existingValue,serializer);
    //    }
    //}
}
