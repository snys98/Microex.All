using Microsoft.AspNetCore.Http;

namespace Microex.All.UEditor.Handlers
{
    internal class ConfigHandler : Handler
{
    public ConfigHandler(HttpContext context) : base(context) { }

    public override void Process()
    {
        WriteJson(Config);
    }
}
}