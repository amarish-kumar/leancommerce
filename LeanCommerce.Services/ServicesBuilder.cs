﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace LeanCommerce.Services
{
    public static class ServicesBuilder
    {
        public static void RegisterBaseServices(ref IServiceCollection services)
        {
            services.AddSingleton<MongoSettings.Service.IMongoSettingsService, MongoSettings.Service.MongoSettingsService>();
            services.AddSingleton<EncryptionSettings.Service.IEncryptionSettingsService, EncryptionSettings.Service.EncryptionSettingsService>();
            services.AddSingleton<Menu.Service.IAdminMenuService, Menu.Service.AdminMenuService>();
            services.AddSingleton<Site.Service.ISiteSettingsService, Site.Service.SiteSettingsService>();
            services.AddSingleton<Catalog.Service.ICategoryService, Catalog.Service.CategoryService>();
            services.AddSingleton<Catalog.Service.IProductService, Catalog.Service.ProductService>();

            services.AddSingleton<Cms.Service.IMungeUrlService, Cms.Service.MungeUrlService>();
            services.AddSingleton<Cms.Service.ICmsWidgetContentService, Cms.Service.CmsWidgetContentService>();
            services.AddSingleton<Cms.Service.ICmsPageService, Cms.Service.CmsPageService>();
        }
    }
}
