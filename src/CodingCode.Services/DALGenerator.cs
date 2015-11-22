﻿namespace CodingCode.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.Logging;
    using Model;
    using ProcessExecution;
    using ProcessExecution.Model;

    public class DalGenerator : IDalGenerator
    {
        private readonly string _dnuPath;
        private readonly string _initialDirectory;
        private string[] _tables;
        private ILogger _logger;

        public DalGenerator(ILogger logger)
        {
            _logger = logger;
            _initialDirectory = Directory.GetCurrentDirectory();
            
            _dnuPath = DnxInformation.DnuPath;
            _logger.LogDebug(_dnuPath);
        }

        public ProcessProviderServices ProcessProviderServices { get; set; }
        public DataAccessSettings DataAccessSettings { get; set; }

        public void Dispose()
        {
            Directory.SetCurrentDirectory(_initialDirectory);
        }

        public void CreateDalDirectory()
        {
            ManageDirectory(DataAccessSettings.DalDirectory);
            Directory.CreateDirectory(DataAccessSettings.DalDirectory);
            Directory.SetCurrentDirectory(DataAccessSettings.DalDirectory);
        }

        public void CopyProjectJson()
        {
            var templateFile = "project.json.template";
            var templatePath =
                Path.Combine(DataAccessSettings.TemplateDirectory, templateFile);
            var destinationPath = Path.Combine(DataAccessSettings.DalDirectory,
                "project.json");
            File.Copy(templatePath, destinationPath);
        }

        public async Task RestoreAsync()
        {
            var instructions = new ProcessInstructions
            {
                Program = _dnuPath,
                Arguments = "restore"
            };
            var processExecutor = ProcessProviderServices
                .FinishingProcessExecutor(instructions, _logger);

            await Task.Factory.StartNew(
                () =>
                    processExecutor.ExecuteAndWait(x => x.Contains("Error")));
        }

        public async Task ScaffoldAsync()
        {
            var instructions = new ProcessInstructions
            {
                Program = DnxInformation.DnxPath,
                Arguments =
                    $"ef dbcontext scaffold {GetConnectionString()} EntityFramework.SqlServer"
            };

            await
                Task.Factory.StartNew(
                    () =>
                        InSellProcessExecutor.ExecuteAndWait(instructions));
            var contextFileName = $"{DataAccessSettings.Database}Context.cs";
            if(! File.Exists(Path.Combine(DataAccessSettings.DalDirectory, contextFileName)))
                throw new Exception("Scaffold faile d!");
        }

        public void CodeContext()
        {
            var contextFile = Path.Combine(DataAccessSettings.DalDirectory,
                $"{DataAccessSettings.Database}Context.cs");
            var efScaffoldCode = File.ReadAllLines(contextFile);
            using(var streamWriter = new StreamWriter(contextFile))
            {
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine("using System.Linq;");

                var tableNames = new List<string>();
                var regex =
                    new Regex(
                        @"public virtual DbSet<(.*)> (\1) { get; set; }");
                for(var i = 0; i < efScaffoldCode.Length - 2; i++)
                {
                    streamWriter.WriteLine(efScaffoldCode[i]);
                    var match = regex.Match(efScaffoldCode[i]);
                    if(match.Success)
                        tableNames.Add(match.Groups[1].Value);
                }
                _tables = tableNames.ToArray();
                WriteContextCastomCode(streamWriter, _tables);
                streamWriter.WriteLine("    }");
                streamWriter.WriteLine("}");
            }
        }

        public async Task CodeEntitiesAsync()
        {
            await
                Task.Factory.StartNew(
                    () => Parallel.ForEach(_tables, table =>
                    {
                        var entityFile = Path.Combine(DataAccessSettings.DalDirectory,
                            $"{table}.cs");
                        var entityCode = File.ReadAllLines(entityFile);
                        using(
                            var streamWriter = new StreamWriter(entityFile)
                            )
                        {
                            IList<string> columns = new List<string>();
                            for(var i = 0; i < entityCode.Length - 2; i++)
                            {
                                if(IsRecognizedTableColumn(entityCode[i]))
                                {
                                    var columnName =
                                        entityCode[i].Split(' ');
                                    columns.Add(columnName[10]);
                                }
                                streamWriter.WriteLine(entityCode[i]);
                            }
                            WriteEntityCastomCode(streamWriter, columns,
                                table);
                            streamWriter.WriteLine("    }");
                            streamWriter.WriteLine("}");
                        }
                    }));
        }

        public async Task BuildAsync()
        {
            var instructions = new ProcessInstructions
            {
                Program = _dnuPath,
                Arguments = "build"
            };
            var processExecutor =
                ProcessProviderServices.FinishingProcessExecutor(instructions, _logger);

            await
                Task.Factory.StartNew(
                    () => processExecutor.ExecuteAndWait(x => ! x.Contains("Build succeeded")));
        }

        public dynamic InstantiateDbContext()
        {
            var assemblyPath =
                Path.Combine(Directory.GetCurrentDirectory(),
                    "bin", "Debug", "dnx451",
                    $"{DataAccessSettings.AssemblyName}.dll");
            var typeInfos =
                Assembly.LoadFrom(assemblyPath).DefinedTypes.ToArray();

            foreach(
                var typeInfo in
                    typeInfos.Where(
                        typeInfo => typeInfo.Name.Contains("Context")))
            {
                return typeInfo.GetConstructors().Single().Invoke(null);
            }
            throw new Exception("Database context not created.");
        }

        private string GetConnectionString()
        {
            return
                $"Server={DataAccessSettings.Server};Database={DataAccessSettings.Database};Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        private bool IsRecognizedTableColumn(string codeLine)
        {
            return codeLine.Contains("{ get; set; }") &&
                   NonNavTypesChecker.Check(codeLine);
        }

        private void WriteEntityCastomCode(StreamWriter streamWriter,
            IList<string> columns, string table)
        {
            var templateFile = "Entity.template";
            var templatePath =
                Path.Combine(DataAccessSettings.TemplateDirectory, templateFile);
            var customCode = File.ReadAllLines(templatePath);
            for(var i = 0; i < 5; i++)
            {
                if(
                    customCode[i].Contains(
                        "public static string TypeName = typeof"))
                    customCode[i] =
                        customCode[i].Replace("TableName", table);
                streamWriter.WriteLine(customCode[i]);
            }
            foreach(var column in columns)
            {
                string dictionaryRecord =
                    $"Dictionary[\"{column}\"] = {column};";
                streamWriter.WriteLine($"           {dictionaryRecord}");
            }
            for(var i = 5; i < 8; i++)
                streamWriter.WriteLine(customCode[i]);
        }

        private void WriteContextCastomCode(StreamWriter streamWriter,
            string[] tableNames)
        {
            var templateFile = "Context.template";
            var templatePath =
                Path.Combine(DataAccessSettings.TemplateDirectory, templateFile);
            var customCode = File.ReadAllLines(templatePath);
            customCode[0] = customCode[0].Replace("AssemblyName",
                $@"""{DataAccessSettings.AssemblyName}""");
            streamWriter.WriteLine(customCode[0]);
            streamWriter.WriteLine(customCode[1]);
            streamWriter.WriteLine(customCode[2]);

            foreach(var tableName in tableNames)
            {
                string check =
                    $"           if (tableType.Equals({DataAccessSettings.AssemblyName}.{tableName}.TypeName))";
                streamWriter.WriteLine(check);
                string returnTable =
                    $"                 return {tableName}.ToArray();";
                streamWriter.WriteLine(returnTable);
            }
            streamWriter.WriteLine(customCode[3]);
            streamWriter.WriteLine(customCode[4]);
            streamWriter.WriteLine(customCode[5]);
        }

        private void ManageDirectory(string dalDirectory)
        {
            if(Directory.Exists(DataAccessSettings.DalDirectory))
                ClearFolder(dalDirectory);

            //to-do swap with ClearFolder
            //throw new Exception("Directory already exists");
        }

        private void ClearFolder(string folderName)
        {
            var dir = new DirectoryInfo(folderName);

            foreach(var fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach(var di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }
    }
}