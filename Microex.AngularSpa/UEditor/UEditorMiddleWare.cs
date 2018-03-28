using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microex.AngularSpa.Extensions;
using Microex.AngularSpa.UEditor.Handlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Microex.AngularSpa.UEditor
{
    public class UEditorMiddleware : IMiddleware
    {
        private readonly IHostingEnvironment _env;

        private static ResourceManager _resourceManager;
        private UEditorConfig _config;

        public UEditorMiddleware(IHostingEnvironment env, string configPath)
        {
            _env = env;
            UEditorJsonConfig config = null;
            if (configPath.IsNullOrEmpty() && _resourceManager == null)
            {
                _resourceManager = new ResourceManager("Microex.AngularSpa.UEditor", typeof(Microex.AngularSpa.UEditor.UEditorMiddleware).GetTypeInfo().Assembly);
                config = _resourceManager.GetString("DefaultConfig.Json").ToObject<UEditorJsonConfig>();
            }
            else
            {
                config = File.ReadAllText(Path.Combine(_env.WebRootPath, configPath)).ToObject<UEditorJsonConfig>();
                
            }

            Handler.WebRootPath = _env.WebRootPath;
            Handler.Config = config;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var action = context.Request.Query["action"];
            switch (action)
            {
                case "config":
                    {
                        new ConfigHandler(context).Process();
                        break;
                    }
                case "uploadimage":
                    {
                        new UploadHandler(context, new UploadConfig()
                        {
                            AllowExtensions = _config.JsonConfig.ImageAllowFiles,
                            PathFormat = _config.JsonConfig.ImagePathFormat,
                            SizeLimit = _config.JsonConfig.ImageMaxSize,
                            UploadFieldName = _config.JsonConfig.ImageFieldName
                        }).Process();
                        break;
                    }
                case "uploadscrawl":
                    {
                        new UploadHandler(context, new UploadConfig()
                        {
                            AllowExtensions = new string[] { ".png" },
                            PathFormat = _config.JsonConfig.ScrawlPathFormat,
                            SizeLimit = _config.JsonConfig.ScrawlMaxSize,
                            UploadFieldName = _config.JsonConfig.ScrawlFieldName,
                            Base64 = true,
                            Base64Filename = "scrawl.png"
                        }).Process();
                        break;
                    }
                case "uploadvideo":
                    {
                        new UploadHandler(context, new UploadConfig()
                        {
                            AllowExtensions = _config.JsonConfig.VideoAllowFiles,
                            PathFormat = _config.JsonConfig.VideoPathFormat,
                            SizeLimit = _config.JsonConfig.VideoMaxSize,
                            UploadFieldName = _config.JsonConfig.VideoFieldName
                        }).Process();
                        break;
                    }
                case "uploadfile":
                    {
                        new UploadHandler(context, new UploadConfig()
                        {
                            AllowExtensions = _config.JsonConfig.FileAllowFiles,
                            PathFormat = _config.JsonConfig.FilePathFormat,
                            SizeLimit = _config.JsonConfig.FileMaxSize,
                            UploadFieldName = _config.JsonConfig.FileFieldName
                        }).Process();
                        break;
                    }
                case "listimage":
                    {
                        new ListFileManager(
                                context,
                                _config.JsonConfig.ImageManagerListPath,
                                _config.JsonConfig.ImageManagerAllowFiles)
                            .Process();
                        break;
                    }
                case "listfile":
                    {
                        new ListFileManager(
                                context,
                                _config.JsonConfig.FileManagerListPath,
                                _config.JsonConfig.FileManagerAllowFiles)
                            .Process();
                        break;
                    }
                case "catchimage":
                    {
                        new CrawlerHandler(context).Process();
                        break;
                    }

                default:
                    new NotSupportedHandler(context).Process();
                    break;
            }

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return context.Response.WriteAsync("不支持的ueditor action：" + action);
        }


    }
}
