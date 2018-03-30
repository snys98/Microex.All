using System;
using System.IO;
using Aliyun.OSS;
using Microex.AngularSpa.Extensions;
using Microsoft.AspNetCore.Http;

namespace Microex.AngularSpa.AliyunOss
{
    public class AliyunOssClient
    {
        private string _imageBulkName;
        private OssClient _ossClient;
        private AliyunOssOptions _options;

        public AliyunOssClient(AliyunOssOptions options)
        {
            this._imageBulkName = options.BulkName;
            this._ossClient = new OssClient(options.RemoteEndPoint, options.AccessKeyId, options.AccessKeySecret);
            this._options = options;
        }

        public string UploadBase64(string dataUrl)
        {
            var base64WithExt = dataUrl.DataUrlToBase64WithExt();
            var md5 = dataUrl.ComputeMd5();
            var fileName = $"{md5}.{base64WithExt.ext}";
            var result = this._ossClient.PutObject(this._imageBulkName, fileName, new MemoryStream(Convert.FromBase64String(base64WithExt.base64)), new ObjectMetadata()
            {
                ContentType = base64WithExt.ext
            });
            return fileName;
        }

        public string UploadPostFile(IFormFile formFile)
        {
            var stream = formFile.OpenReadStream();
            var fileMime = formFile.ContentType;
            //var fileExt = formFile.FileName.Split('.').LastOrDefault() ?? "";
            var result = this._ossClient.PutObject(this._imageBulkName, formFile.FileName, stream, new ObjectMetadata()
            {
                ContentType = fileMime
            });
            return $"{_options.ImageUrlPrefix}/{formFile.FileName}";
        }

    }
}