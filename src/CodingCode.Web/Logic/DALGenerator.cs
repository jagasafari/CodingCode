namespace CodingCode.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using Contracts;
    using Share;

    public class DalGenerator : IDalGenerator
    {
        private readonly string _dnuPath;
        private readonly string _initialDirectory;
        private string[] _tables;

        public DalGenerator()
        {
            _initialDirectory = Directory.GetCurrentDirectory();
            _dnuPath = Path.Combine(DnxTool.GetDnxPath(), "dnu.cmd");
        }

        public string DatabaseName { get; set; }
        public string AssemblyName { get; set; }
        public string ConnectionString { get; set; }
        public string TemplateDirectory { get; set; }
        public string DalDirectory { get; set; }

        public void Dispose()
        {
            Directory.SetCurrentDirectory(_initialDirectory);
        }

        public void CreateDalDirectory()
        {
            ManageDirectory(DalDirectory);
            Directory.CreateDirectory(DalDirectory);
            Directory.SetCurrentDirectory(DalDirectory);
        }

        public void CopyProjectJson()
        {
            var templateFile = "project.json.template";
            var templatePath =
                Path.Combine(TemplateDirectory, templateFile);
            var destiantionPath = Path.Combine(DalDirectory,
                "project.json");
            File.Copy(templatePath, destiantionPath);
        }

        public void Restore()
        {
            ProcessExecutor.ExecuteAndWait(_dnuPath, "restore",
                x => x.Contains("Error"));
        }

        public void Scaffold()
        {
            var dnxProcessPath = Path.Combine(DnxTool.GetDnxPath(),
                "dnx.exe");
            var command =
                $"ef dbcontext scaffold {ConnectionString} EntityFramework.SqlServer";
            ProcessExecutor.ExecuteInShellAndWait(dnxProcessPath, command);
            var contextFileName = $"{DatabaseName}Context.cs";
            if(! File.Exists(Path.Combine(DalDirectory, contextFileName)))
                throw new Exception("Scaffold failed!");
        }

        public void CodeContext()
        {
            var contextFile = Path.Combine(DalDirectory,
                $"{DatabaseName}Context.cs");
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

        public void CodeEntities()
        {
            foreach(var table in _tables)
            {
                var entityFile = Path.Combine(DalDirectory,
                    $"{table}.cs");
                var entityCode = File.ReadAllLines(entityFile);
                using(var streamWriter = new StreamWriter(entityFile))
                {
                    IList<string> columns = new List<string>();
                    for(var i = 0; i < entityCode.Length - 2; i++)
                    {
                        if(IsRecognizedTableColumn(entityCode[i]))
                        {
                            var columnName = entityCode[i].Split(' ');
                            columns.Add(columnName[10]);
                        }
                        streamWriter.WriteLine(entityCode[i]);
                    }
                    WriteEntityCastomCode(streamWriter, columns, table);
                    streamWriter.WriteLine("    }");
                    streamWriter.WriteLine("}");
                }
            }
        }

        public void Build()
        {
            ProcessExecutor.ExecuteAndWait(_dnuPath, "build",
                x => ! x.Contains("Build succeeded"));
        }

        public dynamic InstantiateDbContext()
        {
            var assemblyPath =
                Path.Combine(Directory.GetCurrentDirectory(),
                    "bin", "Debug", "dnx451",
                    $"{AssemblyName}.dll");
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
                Path.Combine(TemplateDirectory, templateFile);
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
                Path.Combine(TemplateDirectory, templateFile);
            var customCode = File.ReadAllLines(templatePath);
            customCode[0] = customCode[0].Replace("AssemblyName",
                $@"""{AssemblyName}""");
            streamWriter.WriteLine(customCode[0]);
            streamWriter.WriteLine(customCode[1]);
            streamWriter.WriteLine(customCode[2]);

            foreach(var tableName in tableNames)
            {
                string check =
                    $"           if (tableType.Equals({AssemblyName}.{tableName}.TypeName))";
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
            if(Directory.Exists(DalDirectory))
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

        public static string GenerateAssemblyName(string connection)
        {
            var connectionParts = connection.Split(';', '=');
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(connectionParts[1].Replace(@"\", "_"));
            stringBuilder.Append("_");
            stringBuilder.Append(connectionParts[3]);

            return stringBuilder.ToString();
        }
    }
}