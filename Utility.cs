﻿using System.Configuration;
using Tridion.ContentManager.CoreService.Client;

namespace AddMetadataInPageAndUpdateValue
{
    public static class Utility
    {
        private static CoreServiceClient _coreServiceSource;

        public static CoreServiceClient CoreServiceSource
        {
            get
            {
                if (_coreServiceSource != null) return _coreServiceSource;
                _coreServiceSource = new CoreServiceClient("sourceEndpoint");

                // _coreServiceSource.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["SourceUserName"];
                if (_coreServiceSource.ClientCredentials != null)
                {
                    _coreServiceSource.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["SourceUserPassword"];
                    string username = ConfigurationManager.AppSettings["SourceUserName"];
                    string domain = ConfigurationManager.AppSettings["SourceUserDomain"];
                    _coreServiceSource.ClientCredentials.UserName.UserName = string.Format(@"{0}\{1}", domain, username);
                    _coreServiceSource.ClientCredentials.Windows.ClientCredential.UserName = username;
                    _coreServiceSource.ClientCredentials.Windows.ClientCredential.Domain = domain;
                    _coreServiceSource.ClientCredentials.Windows.ClientCredential.Password = ConfigurationManager.AppSettings["SourceUserPassword"];
                }


                return _coreServiceSource;
            }
        }
    }
}