using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MoreLinq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microex.All.Swagger
{
    public class EnumFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (context == null)
                throw new ArgumentNullException("context");
            var paramType = context.Type;
            if (paramType.IsEnum)
            {

                //model.Extensions.Add("x-ms-enum", new
                //{
                //    name = paramType.Name,
                //    modelAsString = false,
                //    values = paramType.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x =>
                //    {
                //        var enumMemberAttribute = x.GetCustomAttribute<EnumMemberAttribute>();
                //        return new
                //        {
                //            name = x.Name,
                //            value = enumMemberAttribute.Value,
                //            description = enumMemberAttribute.Value
                //        };
                //    })
                //});
            }

            if ((paramType.IsGenericType &&
                 paramType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                 paramType.GetGenericArguments()[0].IsEnum))
            {
                var realType = paramType.GetGenericArguments()[0];
                //model.Extensions.Add("x-ms-enum", new
                //{
                //    name = realType.Name,
                //    modelAsString = false,
                //    values = realType.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x =>
                //    {
                //        var enumMemberAttribute = x.GetCustomAttribute<EnumMemberAttribute>();
                //        return new
                //        {
                //            name = x.Name,
                //            value = enumMemberAttribute.Value,
                //            description = enumMemberAttribute.Value
                //        };
                //    })
                //});
            }
        }
    }

    public class EnumParamFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if (context == null)
                throw new ArgumentNullException("context");

            var paramType = context?.ParameterInfo?.ParameterType;
            if (paramType == default)
            {
                return;
            }
            if (paramType.IsEnum && paramType.GetCustomAttribute<EnumMemberAttribute>() != null)
            {
                //parameter.Extensions.Add("x-ms-enum", new
                //{
                //    name = paramType.Name,
                //    modelAsString = false,
                //    values = paramType.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x =>
                //    {
                //        var enumMemberAttribute = x.GetCustomAttribute<EnumMemberAttribute>();
                //        return new
                //        {
                //            name = x.Name,
                //            value = enumMemberAttribute.Value,
                //            description = enumMemberAttribute.Value
                //        };
                //    })
                //});
            }

            if ((paramType.IsGenericType &&
                 paramType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                 paramType.GetGenericArguments()[0].IsEnum))
            {
                var realType = paramType.GetGenericArguments()[0];
                //parameter.Extensions.Add("x-ms-enum", new
                //{
                //    name = realType.Name,
                //    modelAsString = false,
                //    values = realType.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x =>
                //    {
                //        var enumMemberAttribute = x.GetCustomAttribute<EnumMemberAttribute>();
                //        return new
                //        {
                //            name = x.Name,
                //            value = enumMemberAttribute.Value,
                //            description = enumMemberAttribute.Value
                //        };
                //    })
                //});
            }
        }
    }
}
