﻿using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Profile;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class AvatarRepositoryImpl:IAvatarRepository {
        #region attributes
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        public AvatarRepositoryImpl(IEventLogRepository logRepo) {
            _log = logRepo;
        }
        #endregion

        #region methods
        public bool SaveAvatar(Guid userId, string fileName, string base64String) {
            bool returnValue = false;

            using(HttpClient client = new HttpClient()) {
                StringBuilder queryString = new StringBuilder("avatar");
                string endPoint = string.Concat(Configuration.MultiDocsUrl, queryString);

                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("UserId", userId.ToString());
                values.Add("FileName", fileName);
                values.Add("EncodedImageData", base64String);

                // TODO: Need to handle other status codes and encode the data to JSON before sending
                try {
                    System.Net.Http.HttpResponseMessage response = client.PostAsJsonAsync(endPoint, values).Result;

                    if(response.StatusCode.Equals(System.Net.HttpStatusCode.OK) || response.StatusCode.Equals(System.Net.HttpStatusCode.NoContent)) {
                        returnValue = true;
                    } else {
                        _log.WriteErrorLog(String.Format("Error uploading avatar for user: {0} - HttpResposne: {1}", userId, response.StatusCode));
                        throw new Exception("There was an error uploading this image");
                    }
                } catch(Exception ex) {
                    _log.WriteErrorLog("Error uploading avatar", ex);
                    throw new Exception("There was an error uploading this image");
                }
            }

            return returnValue;
        }

        /*
         * {
            "UserId": "{e21a937c-a125-47cb-824c-42fb466a0ac7}",
            "FileName": "MyFile.png",
            "EncodedImageData": "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"
            }
        */
        #endregion
    }
}
