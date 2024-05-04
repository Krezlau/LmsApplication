using LmsApplication.Core.Data;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.CreateContainers();

