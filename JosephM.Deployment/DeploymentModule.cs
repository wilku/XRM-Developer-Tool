﻿using JosephM.Application.Modules;
using JosephM.Deployment.CreateDeploymentPackage;
using JosephM.Deployment.DeployPackage;
using JosephM.Deployment.ExportXml;
using JosephM.Deployment.ImportCsvs;
using JosephM.Deployment.ImportXml;

namespace JosephM.Deployment
{
    [DependantModule(typeof(ImportXmlModule))]
    [DependantModule(typeof(ImportCsvsModule))]
    [DependantModule(typeof(ExportXmlModule))]
    [DependantModule(typeof(CreateDeploymentPackageModule))]
    [DependantModule(typeof(DeployPackageModule))]
    public class DeploymentModule : ModuleBase
    {
        public override void InitialiseModule()
        {
        }

        public override void RegisterTypes()
        {
        }
    }
}