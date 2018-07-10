using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microex.All.Swagger
{
    public class EnumFilter : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (context == null)
                throw new ArgumentNullException("context");

            if (context.SystemType.IsEnum)
                model.Extensions.Add("x-ms-enum", new
                {
                    name = context.SystemType.Name,
                    modelAsString = false,
                    values = context.SystemType.GetFields(BindingFlags.Static|BindingFlags.Public).Select(x=>new
                    {
                        name=x.Name,
                        value= x.GetCustomAttribute<DisplayNameAttribute>().DisplayName
                    })
                });
        }
    }

    public class EnumParamFilter : IParameterFilter
    {
        public void Apply(IParameter parameter, ParameterFilterContext context)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if (context == null)
                throw new ArgumentNullException("context");

            var paramType = context.ParameterInfo.ParameterType;
            if (paramType.IsEnum && paramType.GetCustomAttribute<DisplayAttribute>() != null)
            {
                var display = paramType.GetCustomAttribute<DisplayAttribute>();
                parameter.Extensions.Add("x-ms-enum", new
                {
                    name = paramType.Name,
                    modelAsString = false,
                    values = paramType.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => new
                    {
                        name = x.Name,
                        value = display.Name,
                        description = display.Description
                    })
                });
            }

            if ((paramType.IsGenericType &&
                 paramType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                 paramType.GetGenericArguments()[0].IsEnum) && paramType.GetGenericArguments()[0].GetCustomAttribute<DisplayAttribute>() != null)
            {
                var realType = paramType.GetGenericArguments()[0];
                var display = realType.GetCustomAttribute<DisplayAttribute>();
                parameter.Extensions.Add("x-ms-enum", new
                {
                    name = realType.Name,
                    modelAsString = false,
                    values = realType.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => new
                    {
                        name = x.Name,
                        value = display.Name,
                        description = display.Description
                    })
                });
            }
        }
    }
}
