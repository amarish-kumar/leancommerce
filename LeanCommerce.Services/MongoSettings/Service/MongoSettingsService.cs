﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Newtonsoft.Json;
using MongoDB.Driver;

namespace LeanCommerce.Services.MongoSettings.Service
{
    public class MongoSettingsService : IMongoSettingsService
    {
        EncryptionSettings.Service.IEncryptionSettingsService _encryptionService;

        public event EventHandler SettingsChanged;

        public MongoSettingsService(EncryptionSettings.Service.IEncryptionSettingsService encryptionService)
        {
            _encryptionService = encryptionService;
            LoadSettings();
        }

        private MongoSettings.Model.MongoSettings MongoSettings { get; set; }
        public string MongoDBUrl
        {
            get
            {
                return MongoSettings.MongoDBUrl;
            }
            set
            {
                MongoSettings.MongoDBUrl = value;
            }
        }
        public string MongoDBName
        {
            get
            {
                return MongoSettings.MongoDBName;
            }
            set
            {
                MongoSettings.MongoDBName = value;
            }
        }
        public bool AdminCreated
        {
            get
            {
                return MongoSettings.AdminCreated;
            }
            set
            {
                MongoSettings.AdminCreated = value;
            }
        }
        public bool SiteSetup
        {
            get
            {
                return MongoSettings.SiteSetup;
            }
            set
            {
                MongoSettings.SiteSetup = value;
            }
        }

        public void SaveSettings()
        {
            string baseDirectory = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
            if (string.IsNullOrEmpty(baseDirectory))
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string settingsPath = baseDirectory + "/mongosettings.config";
            string data = JsonConvert.SerializeObject(MongoSettings);
            data = _encryptionService.EncryptValue(data);
            System.IO.File.WriteAllText(settingsPath, data);

            SettingsChanged(this, new EventArgs());

        }
        public async Task TestConnection()
        {
            var client = new MongoClient(MongoDBUrl);
            var database = client.GetDatabase(MongoDBName);

            await database.ListCollectionsAsync();

        }
        public void LoadSettings()
        {

            string baseDirectory = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
            if (string.IsNullOrEmpty(baseDirectory))
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string settingsPath = baseDirectory + "/mongosettings.config";
            if (System.IO.File.Exists(settingsPath) == true && _encryptionService!= null && _encryptionService.RequiresSetup() == false)
            {
                string data = System.IO.File.ReadAllText(settingsPath);
                data = _encryptionService.DecryptValue(data);
                MongoSettings = JsonConvert.DeserializeObject<Model.MongoSettings>(data);
            }
            if (MongoSettings == null)
                ResetSettings();

        }
        public void ResetSettings()
        {
            MongoSettings = new Model.MongoSettings();
        }
        public bool RequiresSetup()
        {
            if (string.IsNullOrEmpty(MongoSettings.MongoDBName) ||
                string.IsNullOrEmpty(MongoSettings.MongoDBUrl) ||
                MongoSettings.AdminCreated == false ||
                MongoSettings.SiteSetup == false ||
                _encryptionService.RequiresSetup()
                )
                return true;

            

            return false;
        }
        MongoClient _mongoClient;
        MongoClient GetMongoClient()
        {
            if (string.IsNullOrEmpty(MongoDBUrl) == false)
            {
                return new MongoClient(MongoDBUrl);

            }
            return null;
        }
        IMongoDatabase GetMongoDatabase()
        {
            MongoClient client = GetMongoClient();
            if (client != null && string.IsNullOrEmpty(MongoDBName) == false)
            {
                return client.GetDatabase(MongoDBName);
            }
            return null;
        }
        public IMongoCollection<T> GetMongoCollection<T> (string tableName)
        {
            IMongoDatabase mongoDatabase = GetMongoDatabase();
            if (mongoDatabase != null)
            {
                return mongoDatabase.GetCollection<T>(tableName);
            }
            return null;
        }
    }
}
