namespace CodingCode.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Model;
    using Common.ProcessExecution.Abstraction;
    using CodingCode.Abstraction;

    public class DalGenerator : IDalGenerator
    {
        private readonly string _initialDirectory;
        private string[] _tables;
        private ILogger _logger;
        private readonly IFinishingExecutorFactory _executorFactory;
        private readonly DnxOptions _options;

        public DalGenerator(IFinishingExecutorFactory executorFactory, ILogger<DalGenerator> logger, DnxOptions options)
        {
            _executorFactory = executorFactory;
            _initialDirectory = Directory.GetCurrentDirectory();
            _logger = logger;
            _options = options;
        }

        public DataAccessConfigurations DataAccessConfiguratios { get; set; }

        public void Dispose()
        {
            Directory.SetCurrentDirectory(_initialDirectory);
        }

        public void CreateDalDirectory()
        {
            ManageDirectory(DataAccessConfiguratios.ProjectDirectory);
            Directory.CreateDirectory(DataAccessConfiguratios.ProjectDirectory);
            Directory.SetCurrentDirectory(DataAccessConfiguratios.ProjectDirectory);
        }

        public void CopyProjectJson()
        {
            var templateFile = "project.json.template";
            var templatePath =
                Path.Combine(DataAccessConfiguratios.TemplateDirectory, templateFile);
            var destinationPath = Path.Combine(DataAccessConfiguratios.ProjectDirectory,
                "project.json");
            File.Copy(templatePath, destinationPath);
        }


        public async Task RestoreAsync()
        {
            await Task.Factory.StartNew(
                () => _executorFactory.Create(_options.Dnu, "restore", x => x.Contains("Error"))
                    .Execute());
        }

        
        public async Task ScaffoldAsync()
        {
            string arguments = $"ef dbcontext scaffold {GetConnectionString()} EntityFramework.MicrosoftSqlServer";
            await
                Task.Factory.StartNew(
                    () => _executorFactory.Create(_options.Dnx, arguments, (x) => x.Contains("error"))
                    .Execute());
                    
            var contextFileName = $"{DataAccessConfiguratios.DatabaseName}Context.cs";
            if (!File.Exists(Path.Combine(DataAccessConfiguratios.ProjectDirectory, contextFileName)))
                throw new Exception("Scaffold failed!");
            await WrapBug(contextFileName);
        }

        private async Task WrapBug(string contextFileName)
        {
            var contextFilePath = Path.Combine(Directory.GetCurrentDirectory(), contextFileName);
            await Task.Factory.StartNew(() =>
            {
                var lines = File.ReadAllLines(contextFilePath);
                for (var i = 0; i < lines.Length; i++)
                    if (ContainsBug(lines[i]))
                    {
                        _logger.LogInformation($"Buggy code found: {lines[i]}");
                        lines[i] = string.Empty;
                    }
                File.WriteAllLines(contextFilePath, lines);
            }
            );
        }

        private bool ContainsBug(string line)
        {
            return line.Contains("HasIndex") && line.Contains("new");
        }

        public void CodeContext()
        {
            var contextFile = Path.Combine(DataAccessConfiguratios.ProjectDirectory,
                $"{DataAccessConfiguratios.DatabaseName}Context.cs");
            var efScaffoldCode = File.ReadAllLines(contextFile);
            using (var streamWriter = new StreamWriter(contextFile))
            {
                streamWriter.WriteLine("using System;");
                streamWriter.WriteLine("using System.Linq;");

                var tableNames = new List<string>();
                var regex =
                    new Regex(
                        @"public virtual DbSet<(.*)> (\1) { get; set; }");
                for (var i = 0; i < efScaffoldCode.Length - 2; i++)
                {
                    streamWriter.WriteLine(efScaffoldCode[i]);
                    var match = regex.Match(efScaffoldCode[i]);
                    if (match.Success)
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
                        var entityFile = Path.Combine(DataAccessConfiguratios.ProjectDirectory,
                            $"{table}.cs");
                        var entityCode = File.ReadAllLines(entityFile);
                        using (
                            var streamWriter = new StreamWriter(entityFile)
                            )
                        {
                            IList<string> columns = new List<string>();
                            for (var i = 0; i < entityCode.Length - 2; i++)
                            {
                                if (IsRecognizedTableColumn(entityCode[i]))
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
            await Task.Factory.StartNew(
                    () => _executorFactory.Create(_options.Dnu, "build", x => !x.Contains("Build succeeded"))
                    .Execute());
        }

        public dynamic InstantiateDbContext()
        {
            var assemblyPath =
                Path.Combine(Directory.GetCurrentDirectory(),
                    "bin", "Debug", "dnx451",
                    $"{DataAccessConfiguratios.AssemblyName}.dll");
            var typeInfos =
                Assembly.LoadFrom(assemblyPath).DefinedTypes.ToArray();

            foreach (
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
                $"Server={DataAccessConfiguratios.ServerName};Database={DataAccessConfiguratios.DatabaseName};Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        private bool IsRecognizedTableColumn(string codeLine)
        {
            return codeLine.Contains("{ get; set; }") &&
                   NonNavTypesChecker.Check(codeLine);
        }

        private void WriteEntityCastomCode(StreamWriter streamWriter,
            IList<string> columns, string entityTypeName)
        {
            var templateFile = "Entity.template";
            var templatePath =
                Path.Combine(DataAccessConfiguratios.TemplateDirectory, templateFile);
            var customCode = File.ReadAllLines(templatePath);
            streamWriter.WriteLine(AddStaticTypeNameProperty(entityTypeName, customCode[0]));
            for (var i = 1; i < 5; i++)
            {
                streamWriter.WriteLine(customCode[i]);
            }
            foreach (var column in columns)
            {
                string dictionaryRecord =
                    $"rowValues[\"{column}\"] = {column};";
                streamWriter.WriteLine($"           {dictionaryRecord}");
            }
            for (var i = 5; i < customCode.Length; i++)
                streamWriter.WriteLine(customCode[i]);
        }

        private string AddStaticTypeNameProperty(string entityTypeName, string propertyTemplate)
        {
            if (
       propertyTemplate.Contains(
           "public static string TypeName = typeof"))
                return
                                        propertyTemplate.Replace("TableName", entityTypeName);
            throw new Exception("Expected template for TypeName property");
        }

        private void WriteContextCastomCode(StreamWriter streamWriter,
            string[] tableNames)
        {
            var templateFile = "Context.template";
            var templatePath =
                Path.Combine(DataAccessConfiguratios.TemplateDirectory, templateFile);
            var customCode = File.ReadAllLines(templatePath);
            customCode[0] = customCode[0].Replace("AssemblyName",
                $@"""{DataAccessConfiguratios.AssemblyName}""");
            streamWriter.WriteLine(customCode[0]);
            streamWriter.WriteLine(customCode[1]);
            streamWriter.WriteLine(customCode[2]);

            foreach (var tableName in tableNames)
            {
                string check =
                    $"           if (typeof(T).FullName.Equals({DataAccessConfiguratios.AssemblyName}.{tableName}.TypeName))";
                streamWriter.WriteLine(check);
                string returnTable =
                    $"                 return {tableName};";
                streamWriter.WriteLine(returnTable);
            }
            streamWriter.WriteLine(customCode[3]);
            streamWriter.WriteLine(customCode[4]);
            streamWriter.WriteLine(customCode[5]);
        }

        private void ManageDirectory(string dalDirectory)
        {
            if (Directory.Exists(DataAccessConfiguratios.ProjectDirectory))
                ClearFolder(dalDirectory);

            //to-do swap with ClearFolder
            //throw new Exception("Directory already exists");
        }

        private void ClearFolder(string folderName)
        {
            var dir = new DirectoryInfo(folderName);

            foreach (var fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (var di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }
    }
}