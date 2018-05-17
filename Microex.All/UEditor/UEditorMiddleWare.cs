using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microex.All.Extensions;
using Microex.All.UEditor.Handlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Microex.All.UEditor
{
    public class UEditorMiddleware : IMiddleware
    {
        private readonly IHostingEnvironment _env;
        private UEditorOptions _options;
        private readonly IMemoryCache _memoryCache;

        public UEditorMiddleware(IHostingEnvironment env, UEditorOptions options,IMemoryCache memoryCache)
        {
            _env = env;
            this._options = options;
            _memoryCache = memoryCache;
            UEditorJsonConfig jsonConfig = null;
            if (options.ConfigUrl.IsNullOrEmpty() && Handler.Config == null)
            {
                var resourceManager = new ResourceManager("Microex.AngularSpa.UEditor", typeof(UEditorMiddleware).GetTypeInfo().Assembly);
                jsonConfig = resourceManager.GetString("DefaultConfig.Json").ToObject<UEditorJsonConfig>();
            }
            else
            {
                jsonConfig = new HttpClient().GetStringAsync(options.ConfigUrl).Result.ToObject<UEditorJsonConfig>();
            }

            _options.JsonConfig = jsonConfig;
            Handler.WebRootPath = _env.WebRootPath;
            Handler.Config = jsonConfig;
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
                            AllowExtensions = Handler.Config.ImageAllowFiles,
                            PathFormat = Handler.Config.ImagePathFormat,
                            SizeLimit = Handler.Config.ImageMaxSize,
                            UploadFieldName = Handler.Config.ImageFieldName
                        }).Process();
                        break;
                    }
                case "uploadscrawl":
                    {
                        new UploadHandler(context, new UploadConfig()
                        {
                            AllowExtensions = new string[] { ".png" },
                            PathFormat = Handler.Config.ScrawlPathFormat,
                            SizeLimit = Handler.Config.ScrawlMaxSize,
                            UploadFieldName = Handler.Config.ScrawlFieldName,
                            Base64 = true,
                            Base64Filename = "scrawl.png"
                        }).Process();
                        break;
                    }
                case "uploadvideo":
                    {
                        new UploadHandler(context, new UploadConfig()
                        {
                            AllowExtensions = Handler.Config.VideoAllowFiles,
                            PathFormat = Handler.Config.VideoPathFormat,
                            SizeLimit = Handler.Config.VideoMaxSize,
                            UploadFieldName = Handler.Config.VideoFieldName
                        }).Process();
                        break;
                    }
                case "uploadfile":
                    {
                        new UploadHandler(context, new UploadConfig()
                        {
                            AllowExtensions = Handler.Config.FileAllowFiles,
                            PathFormat = Handler.Config.FilePathFormat,
                            SizeLimit = Handler.Config.FileMaxSize,
                            UploadFieldName = Handler.Config.FileFieldName
                        }).Process();
                        break;
                    }
                case "listimage":
                    {
                        new ListFileManager(
                                context,
                                Handler.Config.ImageManagerListPath,
                                Handler.Config.ImageManagerAllowFiles)
                            .Process();
                        break;
                    }
                case "listfile":
                    {
                        new ListFileManager(
                                context,
                                Handler.Config.FileManagerListPath,
                                Handler.Config.FileManagerAllowFiles)
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
