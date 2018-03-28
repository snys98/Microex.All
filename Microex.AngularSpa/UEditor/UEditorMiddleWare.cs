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
            if (configPath.IsNullOrEmpty() && _resourceManager == null)
            {
                _resourceManager = new ResourceManager("Microex.AngularSpa.UEditor", typeof(Microex.AngularSpa.UEditor.UEditorMiddleware).GetTypeInfo().Assembly);
                this._config = _resourceManager.GetString("DefaultConfig.Json").ToObject<UEditorConfig>();
            }
            else
            {
                this._config = File.ReadAllText(Path.Combine(_env.WebRootPath, configPath)).ToObject<UEditorConfig>();
                
            }

            Handler.WebRootPath = _env.WebRootPath;
            Handler.Config = _config;
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
                            AllowExtensions = _config.ImageAllowFiles,
                            PathFormat = _config.ImagePathFormat,
                            SizeLimit = _config.ImageMaxSize,
                            UploadFieldName = _config.ImageFieldName
                        }).Process();
                        break;
                    }
                case "uploadscrawl":
                    {
                        new UploadHandler(context, new UploadConfig()
                        {
                            AllowExtensions = new string[] { ".png" },
                            PathFormat = _config.ScrawlPathFormat,
                            SizeLimit = _config.ScrawlMaxSize,
                            UploadFieldName = _config.ScrawlFieldName,
                            Base64 = true,
                            Base64Filename = "scrawl.png"
                        }).Process();
                        break;
                    }
                case "uploadvideo":
                    {
                        new UploadHandler(context, new UploadConfig()
                        {
                            AllowExtensions = _config.VideoAllowFiles,
                            PathFormat = _config.VideoPathFormat,
                            SizeLimit = _config.VideoMaxSize,
                            UploadFieldName = _config.VideoFieldName
                        }).Process();
                        break;
                    }
                case "uploadfile":
                    {
                        new UploadHandler(context, new UploadConfig()
                        {
                            AllowExtensions = _config.FileAllowFiles,
                            PathFormat = _config.FilePathFormat,
                            SizeLimit = _config.FileMaxSize,
                            UploadFieldName = _config.FileFieldName
                        }).Process();
                        break;
                    }
                case "listimage":
                    {
                        new ListFileManager(
                                context,
                                _config.ImageManagerListPath,
                                _config.ImageManagerAllowFiles)
                            .Process();
                        break;
                    }
                case "listfile":
                    {
                        new ListFileManager(
                                context,
                                _config.FileManagerListPath,
                                _config.FileManagerAllowFiles)
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
